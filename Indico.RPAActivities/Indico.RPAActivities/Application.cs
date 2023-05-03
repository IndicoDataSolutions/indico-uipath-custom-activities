using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using IndicoV2;
using IndicoV2.DataSets.Models;
using IndicoV2.Ocr.Models;
using IndicoV2.Submissions.Models;
using IndicoV2.Workflows.Models;
using Newtonsoft.Json.Linq;
using System.IO;

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
            var doc = await ocrClient.GetExtractionResultAsync(result.Url, cancellationToken);

            return doc;
        }

        public async Task<IEnumerable<int>> WorkflowSubmission(int workflowId, IEnumerable<string> files, IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            IEnumerable<int> result = null;

            if (files != null)
            {

                result = await _client.Submissions().CreateAsyncLegacy(workflowId, files, cancellationToken);
            }
            else if (urls != null)
            {
                result = await _client.Submissions().CreateAsync(workflowId, urls.Select(u => new Uri(u)).ToList(), cancellationToken);
            }

            return result;
        }

        public async Task<ISubmission> MarkSubmissionAsRetrieved(int submissionId, bool retrieved, CancellationToken cancellationToken = default) => await _client.Submissions().MarkSubmissionAsRetrieved(submissionId, retrieved, cancellationToken);

        public async Task<JObject> SubmissionResult(int submissionId, SubmissionStatus? checkStatus, CancellationToken cancellationToken = default)
        {  
            if (checkStatus.HasValue)
            {
                //wait for the submission to enter a particular state.
                await _client.GetSubmissionResultAwaiter().WaitReady(submissionId, checkStatus.Value, _checkInterval, cancellationToken);
            }
            //fetch generate submission job result
            string jobId = await _client.Submissions().GenerateSubmissionResultAsync(submissionId, cancellationToken);
            JToken jobResult = await _client.Jobs().GetResultAsync<JToken>(jobId, cancellationToken);
            string jobResultUrl = jobResult.Value<string>("url");
            //fetch the storage result
            var storageResult = await _client.Storage().GetAsync(new Uri(jobResultUrl), default);
            using (var reader = new StreamReader(storageResult))
            {
                //return a jobject of the results.
               string resultAsString = reader.ReadToEnd();
               return JObject.Parse(resultAsString);
                
            }
        }
        /// <summary>
        /// Invoke generate submissions call.
        /// </summary>
        /// <param name="submissionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GenerateSubmissionResult(int submissionId, CancellationToken cancellationToken = default) =>
            await _client.Submissions().GenerateSubmissionResultAsync(submissionId, cancellationToken);
        public string InputFilename { get; set; }
        public SubmissionStatus? Status { get; set; }
        public bool? Retrieved { get; set; }

        public Task<IEnumerable<ISubmission>> ListSubmissions(List<int> submissionIds, List<int> workflowIds, string inputFilename, SubmissionStatus? status, bool? retrieved, int limit, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrWhiteSpace(inputFilename))
            {
                inputFilename = null;
            }

            var submissionFilter = new SubmissionFilter
            {
                InputFilename = inputFilename,
                Status = status,
                Retrieved = retrieved
            };

            return _client.Submissions().ListAsync(submissionIds, workflowIds, submissionFilter, limit, cancellationToken);
        }
    }
}