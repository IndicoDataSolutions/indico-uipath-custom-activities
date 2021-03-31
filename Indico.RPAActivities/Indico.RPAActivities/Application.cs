using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Threading;
using IndicoV2;
using IndicoV2.DataSets.Models;
using IndicoV2.Workflows.Models;
using System.Linq;
using IndicoV2.Models.Models;
using IndicoV2.Ocr.Models;
using IndicoV2.Submissions.Models;
using SubmissionFilterV2 = IndicoV2.Submissions.Models.SubmissionFilter;

namespace Indico.RPAActivities
{
    public class Application
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(0.5);
        private readonly IndicoV2.IndicoClient _client;


        public Application(string token, string baseUrlString) => _client = new IndicoV2.IndicoClient(token, new Uri(baseUrlString));


        public async Task<IEnumerable<IDataSetFull>> ListDatasets(CancellationToken cancellationToken) =>
            await _client.DataSets().ListFullAsync(null, cancellationToken);

        public async Task<IEnumerable<IWorkflow>> ListWorkflows(int datasetId, CancellationToken cancellationToken = default) =>
            await _client.Workflows().ListAsync(datasetId, cancellationToken);

        public async Task<JObject> SubmitReview(int submissionId, JObject changes, bool rejected, bool? forceComplete,
            CancellationToken cancellationToken = default)
        {
            var jobId = await _client.Reviews()
                .SubmitReviewAsync(submissionId, changes, rejected, forceComplete, cancellationToken);
            var jobResult = await _client.JobAwaiter().WaitReadyAsync<JObject>(jobId, _checkInterval, cancellationToken);

            return jobResult;
        }

        public async Task<IModelGroup> GetModelGroup(int modelGroupId, CancellationToken cancellationToken) =>
            await _client.Models().GetGroup(modelGroupId, cancellationToken);

        public async Task<string> ExtractDocument(string filePath, DocumentExtractionPreset preset, CancellationToken cancellationToken = default)
        {
            if (preset == DocumentExtractionPreset.OnDocument)
            {
                throw new NotSupportedException(
                    $"{preset} is not supported in this version of the library.");
            }

            var ocrClient = _client.Ocr();
            var jobId = await ocrClient.ExtractDocumentAsync(filePath, preset, cancellationToken);
            var result = await _client.JobAwaiter().WaitReadyAsync<ExtractionJobResult>(jobId, _checkInterval, cancellationToken);
            var doc = await ocrClient.GetExtractionResultAsync(result.Url);

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
                result = await _client.Submissions().CreateAsync(workflowId, urls.Select(u => new Uri(u)).ToList(), cancellationToken);
            }

            return result;
        }

        public async Task<JObject> SubmissionResult(int submissionId, SubmissionStatus? checkStatus, CancellationToken cancellationToken = default)
            => checkStatus.HasValue
                ? await _client.GetSubmissionResultAwaiter().WaitReady(submissionId, checkStatus.Value, _checkInterval, cancellationToken)
                : await _client.GetSubmissionResultAwaiter().WaitReady(submissionId, _checkInterval, cancellationToken);

        public async Task<List<ISubmission>> ListSubmissions(List<int> submissionIds, List<int> workflowIds, SubmissionFilterV2 filters, int limit, CancellationToken cancellationToken = default)
            => (await _client.Submissions().ListAsync(submissionIds, workflowIds, filters, limit, cancellationToken)).ToList();

        public async Task<IPredictionJobResult> Classify(List<string> values, int modelGroupId, CancellationToken cancellationToken = default)
        {
            var models = _client.Models();
            var modelGroup = await models.GetGroup(modelGroupId, cancellationToken);

            var selectedModelId = modelGroup.SelectedModel.Id;
            _ = await models.LoadModel(selectedModelId, cancellationToken);
            var jobId = await models.Predict(selectedModelId, values, cancellationToken);
            var jobResult = await _client.JobAwaiter().WaitPredictionReadyAsync(jobId, _checkInterval, cancellationToken);
            
            return jobResult;
        }
    }
}