using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Indico.Entity;
using Indico.Mutation;
using Indico.Query;
using Indico.RPAActivities.Entity;
using Indico.Types;
using Indico.Storage;
using System.Threading;

namespace Indico.RPAActivities
{
    public class Application
    {
        #region Properties

        private readonly IndicoClient _client;

        #endregion

        #region Constructors

        public Application(string token, string host)
        {
            var config = new IndicoConfig(host: host, apiToken: token);
            _client = new IndicoClient(config);
        }

        #endregion

        public async Task<List<Dataset>> ListDatasets(CancellationToken cancellationToken)
        {
            string query = @"
              query GetDatasets {
                datasets {
                  id
                  name
                  status
                  rowCount
                  numModelGroups
                  modelGroups {
                    id
                  }
                }
              }
            ";

            var request = _client.GraphQLRequest(query, "GetDatasets");
            var result = await request.Call();
            var datasets = (JArray)result.GetValue("datasets");

            return datasets.ToObject<List<Dataset>>();
        }

        public async Task<List<Workflow>> ListWorkflows(int datasetId, CancellationToken cancellationToken = default)
        {
            var listWorkflows = new ListWorkflows(_client)
            {
                DatasetIds = new List<int> { datasetId }
            };

            return await listWorkflows.Exec(cancellationToken);
        }

        public async Task<JObject> SubmitReview(int submissionId, JObject changes, bool rejected, bool? forceComplete, CancellationToken cancellationToken = default)
        {
            var submitReview = new SubmitReview(_client)
            {
                SubmissionId = submissionId,
                Changes = changes,
                Rejected = rejected,
                ForceComplete = forceComplete
            };
            var job = await submitReview.Exec(cancellationToken);
            return await job.Result();
        }

        public async Task<ModelGroup> GetModelGroup(int mgId, CancellationToken cancellationToken)
        {
            return await _client.ModelGroupQuery(mgId).Exec(cancellationToken);
        }

        public async Task<Document> ExtractDocument(string document, string configType, CancellationToken cancellationToken = default)
        {
            var extractConfig = new JObject()
            {
                {"preset_config", configType}
            };

            var ocr = _client.DocumentExtraction(extractConfig);
            var job = await ocr.Exec(document);
            var result = await job.Result();
            var resUrl = (string)result.GetValue("url");
            var blob = await _client.RetrieveBlob(resUrl).Exec();
            var obj = blob.AsJSONObject();

            var doc = new Document
            {
                Text = (string)obj.GetValue("text")
            };

            return doc;
        }

        public async Task<List<int>> WorkflowSubmission(int workflowId, List<string> files, List<string> urls, CancellationToken cancellationToken = default)
        {
            var workflowSubmission = new WorkflowSubmission(_client)
            {
                WorkflowId = workflowId,
                Files = files,
                Urls = urls
            };

            return await workflowSubmission.Exec(cancellationToken);
        }

        public async Task<JObject> SubmissionResult(int submissionId, SubmissionStatus? checkStatus, CancellationToken cancellationToken = default)
        {
            var submissionResult = new SubmissionResult(_client)
            {
                SubmissionId = submissionId,
                CheckStatus = checkStatus
            };

            var job = await submissionResult.Exec(cancellationToken);
            var result = await job.Result();
            var resUrl = (string)result.GetValue("url");

            var retrieveBlob = new RetrieveBlob(_client)
            {
                Url = resUrl
            };

            var blob = await retrieveBlob.Exec();
            return blob.AsJSONObject();
        }

        public async Task<List<Submission>> ListSubmissions(List<int> submissionIds, List<int> workflowIds, SubmissionFilter filters, int limit, CancellationToken cancellationToken = default)
        {
            var listSubmissions = new ListSubmissions(_client)
            {
                SubmissionIds = submissionIds,
                WorkflowIds = workflowIds,
            };

            if (filters != null)
                listSubmissions.Filters = filters;
            if (limit > 0)
                listSubmissions.Limit = limit;

            return await listSubmissions.Exec();
        }

        public async Task<List<Dictionary<string, double>>> Classify(List<string> values, int modelGroup, CancellationToken cancellationToken = default)
        {
            var mg = await _client.ModelGroupQuery(modelGroup).Exec(cancellationToken);
            var status = await _client.ModelGroupLoad(mg).Exec(cancellationToken);
            var job = await _client.ModelGroupPredict(mg).Data(values).Exec(cancellationToken);
            var jobResult = await job.Results();

            return jobResult.ToObject<List<Dictionary<string, double>>>();
        }

        public async Task<List<List<Extraction>>> Extract(List<string> values, int modelGroup, CancellationToken cancellationToken = default)
        {
            var mg = await _client.ModelGroupQuery(modelGroup).Exec(cancellationToken);
            var status = await _client.ModelGroupLoad(mg).Exec(cancellationToken);
            var job = await _client.ModelGroupPredict(mg).Data(values).Exec(cancellationToken);
            var jobResult = await job.Results();

            return jobResult.ToObject<List<List<Extraction>>>();
        }
    }
}