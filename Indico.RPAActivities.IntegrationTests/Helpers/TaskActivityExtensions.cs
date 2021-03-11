using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using Indico.RPAActivities.Activities;
using IndicoV2.DataSets.Models;
using IndicoV2.Workflows.Models;
using UiPath.Shared.Activities.RuntimeSimple;

namespace Indico.RPAActivities.IntegrationTests.Helpers
{
    internal static class TaskActivityExtensions
    {
        private static string BaseUrl => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string ApiToken => Environment.GetEnvironmentVariable("INDICO_TOKEN");

        public static List<IDataSetFull> Invoke(this ListDatasets listDataSetsActivity) =>
            listDataSetsActivity.Invoke<ListDatasets, bool, List<IDataSetFull>>((lds, output) => lds.Datasets = output);

        public static List<IWorkflow> Invoke(this ListWorkflows listWorkflowsActivity) =>
            listWorkflowsActivity.Invoke<ListWorkflows, int, List<IWorkflow>>((a, output) => a.Workflows = output);
        
        public static List<int> Invoke(this WorkflowSubmission workflowSubmissionActivity) =>
            workflowSubmissionActivity
            .Invoke<WorkflowSubmission, (int WorkflowId, List<string> FilePaths, List<string> Urls), List<int>>((a, output) 
                => a.SubmissionIDs = output);

        public static TOutput Invoke<TActivity, TInput, TOutput>(this TActivity activity, Action<TActivity, OutArgument<TOutput>> setOutput)
            where TActivity : TaskActivity<TInput, TOutput>
        {
            var inToken = new InArgument<string>("inToken");
            var inBaseUrl = new InArgument<string>("inBaseUrl");
            var outVar = new Variable<TOutput>("outVar");
            const string outArgName = "OutArg";

            var indicoScope = new IndicoScope()
            {
                Host = new InArgument<string>(ctx => inBaseUrl.Get(ctx)),
                Token = new InArgument<string>(ctx => inToken.Get(ctx)),
            };
            indicoScope.Body.Handler = activity;
            setOutput(activity, new OutArgument<TOutput>(outVar));
            
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
                        Name = outArgName ,
                        Type = typeof(OutArgument<TOutput>),
                        Value = new OutArgument<TOutput>(),
                    }
                },
                Implementation = () => new Sequence
                {
                    Variables = { outVar },
                    Activities =
                    {
                        indicoScope,
                        new Assign<TOutput>()
                        {
                            Value = outVar,
                            To = new ArgumentReference<TOutput>(outArgName),
                        }
                    },
                },
            };

            var resultDictionary = WorkflowInvoker.Invoke(root, GetScopeParams());

            var result = (TOutput)resultDictionary.Single().Value;

            return result;
        }

        public static IDictionary<string, object> GetScopeParams() => new Dictionary<string, object>
            {{nameof(IndicoScope.Host), BaseUrl}, {nameof(IndicoScope.Token), ApiToken}};
    }
}
