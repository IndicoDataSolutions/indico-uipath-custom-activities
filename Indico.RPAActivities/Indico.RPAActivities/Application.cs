using System;
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
using IndicoV2;
using IndicoV2.DataSets.Models;
using IndicoV2.Workflows.Models;

namespace Indico.RPAActivities
{
    public class Application
    {
        #region Properties

        [Obsolete]
        private readonly IndicoClient _clientLegacy;

        private IndicoV2.IndicoClient _client;

        #endregion

        #region Constructors

        public Application(string token, string baseUrlString)
        {
            var baseUrl = new Uri(baseUrlString);
            var config = new IndicoConfig(host: baseUrl.Host, apiToken: token);
            _clientLegacy = new IndicoClient(config);
            _client = new IndicoV2.IndicoClient(token, baseUrl);
        }

        #endregion

        public async Task<IEnumerable<IDataSetFull>> ListDatasets(CancellationToken cancellationToken) =>
            await _client.DataSets().ListFullAsync(cancellationToken);

        public async Task<IEnumerable<IWorkflow>> ListWorkflows(int datasetId, CancellationToken cancellationToken = default) =>
            await _client.Workflows().ListAsync(datasetId, cancellationToken);

        public async Task<JObject> SubmitReview(int submissionId, JObject changes, bool rejected, bool? forceComplete, CancellationToken cancellationToken = default)
        {
            var submitReview = new SubmitReview(_clientLegacy)
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
            return await _clientLegacy.ModelGroupQuery(mgId).Exec(cancellationToken);
        }

        public async Task<Document> ExtractDocument(string document, string configType, CancellationToken cancellationToken = default)
        {
            var extractConfig = new JObject()
            {
                {"preset_config", configType}
            };

            var ocr = _clientLegacy.DocumentExtraction(extractConfig);
            var job = await ocr.Exec(document);
            var result = await job.Result();
            var resUrl = (string)result.GetValue("url");
            var blob = await _clientLegacy.RetrieveBlob(resUrl).Exec();
            var obj = blob.AsJSONObject();

            var doc = new Document
            {
                Text = (string)obj.GetValue("text")
            };

            return doc;
        }

        public async Task<IEnumerable<int>> WorkflowSubmission(int workflowId, IEnumerable<string> files, IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            IEnumerable<int> result = null;

            if (files != null)
            {
                result = await _client.Submissions().CreateAsync(workflowId, files, cancellationToken);
            }
            else if (urls != null)
            {
                result = await _client.Submissions().CreateAsync(workflowId, urls.Select(u => new Uri(u)).ToList());
            }

            return result;
        }

        public async Task<JObject> SubmissionResult(int submissionId, Types.SubmissionStatus? checkStatus, CancellationToken cancellationToken = default)
        {
            var submissionResult = new SubmissionResult(_clientLegacy)
            {
                SubmissionId = submissionId,
                CheckStatus = checkStatus
            };

            var job = await submissionResult.Exec(cancellationToken);
            var result = await job.Result();
            var resUrl = (string)result.GetValue("url");

            var retrieveBlob = new RetrieveBlob(_clientLegacy)
            {
                Url = resUrl
            };

            var blob = await retrieveBlob.Exec();
            return blob.AsJSONObject();
        }

        public async Task<List<Submission>> ListSubmissions(List<int> submissionIds, List<int> workflowIds, SubmissionFilter filters, int limit, CancellationToken cancellationToken = default)
        {
            var listSubmissions = new ListSubmissions(_clientLegacy)
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
            var mg = await _clientLegacy.ModelGroupQuery(modelGroup).Exec(cancellationToken);
            var status = await _clientLegacy.ModelGroupLoad(mg).Exec(cancellationToken);
            var job = await _clientLegacy.ModelGroupPredict(mg).Data(values).Exec(cancellationToken);
            var jobResult = await job.Results();

            return jobResult.ToObject<List<Dictionary<string, double>>>();
        }

        public async Task<List<List<Extraction>>> Extract(List<string> values, int modelGroup, CancellationToken cancellationToken = default)
        {
            var mg = await _clientLegacy.ModelGroupQuery(modelGroup).Exec(cancellationToken);
            var status = await _clientLegacy.ModelGroupLoad(mg).Exec(cancellationToken);
            var job = await _clientLegacy.ModelGroupPredict(mg).Data(values).Exec(cancellationToken);
            var jobResult = await job.Results();

            return jobResult.ToObject<List<List<Extraction>>>();
        }
    }
}