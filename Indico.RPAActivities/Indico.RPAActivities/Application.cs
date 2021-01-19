using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Indico.Entity;
using Indico.Request;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using Indico.RPAActivities.Entity;
using System.Linq;
using Indico.Types;
using Indico.Storage;

namespace Indico.RPAActivities
{
    public class Application
    {
        #region Properties

        IndicoClient _client;

        #endregion

        #region Constructors

        public Application(string token, string host)
        {
            IndicoConfig config = new IndicoConfig(host: host, apiToken: token);
            _client = new IndicoClient(config);
        }

        #endregion

        public async Task<List<Dataset>> ListDatasets()
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

            GraphQLRequest request = _client.GraphQLRequest(query, "GetDatasets");
            JObject result = await request.Call();
            JArray datasets = (JArray) result.GetValue("datasets");
            return datasets.ToObject<List<Dataset>>();
        }

        public async Task<List<Workflow>> ListWorkflows(int datasetId)
        {
            ListWorkflows listWorkflows = new ListWorkflows(_client)
            {
                DatasetIds = new List<int> { datasetId }
            };
            List<Workflow> workflows = await listWorkflows.Exec();
            return workflows;
        }

        public async Task<JObject> SubmitReview(int submissionId, JObject changes, bool rejected, bool? forceComplete)
        {
            var submitReview = new SubmitReview(_client)
            {
                SubmissionId = submissionId,
                Changes = changes,
                Rejected = rejected,
                ForceComplete = forceComplete
            };
            var job = await submitReview.Exec();
            return await job.Result();
        }

        public async Task<ModelGroup> GetModelGroup(int mgId)
        {
            ModelGroup mg = await _client.ModelGroupQuery(mgId).Exec();
            return mg;
        }

        public async Task<Document> ExtractDocument(string document, string configType = "standard")
        {
            JObject extractConfig = new JObject()
            {
                {"preset_config", configType}
            };

            DocumentExtraction ocr = _client.DocumentExtraction(extractConfig);
            Job job = await ocr.Exec(document);
            JObject result = await job.Result();
            string resUrl = (string) result.GetValue("url");
            Storage.Blob blob = await _client.RetrieveBlob(resUrl).Exec();
            JObject obj = blob.AsJSONObject();

            var doc = new Document();
            doc.Text = (string) obj.GetValue("text");

            return doc;
        }

        public async Task<List<int>> WorkflowSubmission(int workflowId, List<string> files, List<string> urls)
        {
            var workflowSubmission = new WorkflowSubmission(_client)
            {
                WorkflowId = workflowId,
                Files = files,
                Urls = urls
            };

            return await workflowSubmission.Exec();
        }

        public async Task<JObject> SubmissionResult(int submissionId, SubmissionStatus? checkStatus)
        {
            var submissionResult = new SubmissionResult(_client)
            {
                SubmissionId = submissionId,
                CheckStatus = checkStatus
            };

            Job job = await submissionResult.Exec();
            JObject result = await job.Result();
            string resUrl = (string)result.GetValue("url");

            var retrieveBlob = new RetrieveBlob(_client) 
            {
                Url = resUrl
            };

            Blob blob = await retrieveBlob.Exec();
            return blob.AsJSONObject();
        }

        public async Task<List<int>> ListSubmissions(List<int> submissionIds, List<int> workflowIds, SubmissionFilter filters, int limit)
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

            var submissions = await listSubmissions.Exec();
            return submissions.Any() ? submissions.Select(s => s.Id).ToList() : new List<int>();
        }

        public async Task<List<Dictionary<string, double>>> Classify(List<string> values, int modelGroup)
        {
            ModelGroup mg = await _client.ModelGroupQuery(modelGroup).Exec();
            var status = await _client.ModelGroupLoad(mg).Exec();
            Job job = await _client.ModelGroupPredict(mg).Data(values).Exec();
            JArray jobResult = await job.Results();
            return jobResult.ToObject<List<Dictionary<string, double>>>();
        }

        public async Task<List<List<Extraction>>> Extract(List<string> values, int modelGroup)
        {
            ModelGroup mg = await _client.ModelGroupQuery(modelGroup).Exec();
            string status = await _client.ModelGroupLoad(mg).Exec();
            Job job = await _client.ModelGroupPredict(mg).Data(values).Exec();
            JArray jobResult = await job.Results();
            return jobResult.ToObject<List<List<Extraction>>>();
        }
    }
}