using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Indico.RPAActivities.Activities;
using IndicoV2.DataSets.Models;
using IndicoV2.Models.Models;
using IndicoV2.Submissions.Models;
using IndicoV2.Workflows.Models;
using Newtonsoft.Json.Linq;

namespace Indico.RPAActivities.IntegrationTests.Helpers
{
    internal static class TaskActivityExtensions
    {
        private static string BaseUrl => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string ApiToken => Environment.GetEnvironmentVariable("INDICO_TOKEN");

        public static List<IDataSetFull> Invoke(this ListDatasets listDataSetsActivity) =>
            listDataSetsActivity.Invoke<ListDatasets, List<IDataSetFull>>((lds, output) => lds.Datasets = output);

        public static List<IWorkflow> Invoke(this ListWorkflows listWorkflowsActivity) =>
            listWorkflowsActivity.Invoke<ListWorkflows, List<IWorkflow>>((a, output) => a.Workflows = output);

        public static List<int> Invoke(this WorkflowSubmission workflowSubmissionActivity) =>
            workflowSubmissionActivity
                .Invoke<WorkflowSubmission, List<int>>((a, output) => a.SubmissionIDs = output);

        public static List<ISubmission> Invoke(this ListSubmissions listSubmissions) =>
            listSubmissions.Invoke<ListSubmissions, List<ISubmission>>((a, outArg) => a.Submissions = outArg);

        public static JObject Invoke(this SubmissionResult submissionResultActivity) =>
            submissionResultActivity.Invoke<SubmissionResult, JObject>((a, outArg) => a.Result = outArg);

        public static JObject Invoke(this SubmitReview submitReviewActivity) =>
            submitReviewActivity.Invoke<SubmitReview, JObject>((a, outArg) => a.Result = outArg);

        public static IModelGroup Invoke(this GetModelGroup getModelGroupActivity) =>
            getModelGroupActivity.Invoke<GetModelGroup, IModelGroup>((a, outArg) => a.ModelGroupData = outArg);

        public static TOutput Invoke<TActivity, TOutput>(this TActivity activity, Action<TActivity, OutArgument<TOutput>> setOutput)
            where TActivity : Activity
        {
            var inToken = new InArgument<string>("inToken");
            var inBaseUrl = new InArgument<string>("inBaseUrl");

            var indicoScope = new IndicoScope
            {
                Host = new InArgument<string>(ctx => inBaseUrl.Get(ctx)),
                Token = new InArgument<string>(ctx => inToken.Get(ctx)),
                Body = { Handler = activity },
            };

            OutArgument<TOutput> outArg = new OutArgument<TOutput>();

            var root = new DynamicActivity
            {
                Properties =
                {
                    new DynamicActivityProperty
                    {
                        Name = nameof(IndicoScope.Token),
                        Value = inToken,
                        Type = inToken.GetType(),
                    },
                    new DynamicActivityProperty
                    {
                        Name = nameof(IndicoScope.Host),
                        Value = inBaseUrl,
                        Type = inBaseUrl.GetType(),
                    },
                    new DynamicActivityProperty
                    {
                        Name = "OutArg" ,
                        Type = typeof(OutArgument<TOutput>),
                        Value = outArg,
                    }
                },
                Implementation = () => indicoScope
            };
            setOutput(activity, new OutArgument<TOutput>(ctx => outArg.Get(ctx)));

            var resultDictionary = WorkflowInvoker.Invoke(root, GetScopeParams());

            var result = (TOutput)resultDictionary.Single().Value;

            return result;
        }

        public static IDictionary<string, object> GetScopeParams() => new Dictionary<string, object>
            {{nameof(IndicoScope.Host), BaseUrl}, {nameof(IndicoScope.Token), ApiToken}};
    }
}
