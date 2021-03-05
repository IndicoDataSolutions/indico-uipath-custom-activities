using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Indico.RPAActivities.Activities;
using UiPath.Shared.Activities.RuntimeSimple;

namespace Indico.RPAActivities.IntegrationTests.Helpers
{
    internal static class TaskActivityExtensions
    {
        private static string BaseUrl => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string ApiToken => Environment.GetEnvironmentVariable("INDICO_TOKEN");

        public static IDictionary<string, object> Invoke<TInput, TOutput>(this TaskActivity<TInput, TOutput> activity,
            IDictionary<string, object> args = null)
            => activity.WrapWithIndicoScope().Invoke(args);

        private static IndicoScope WrapWithIndicoScope<TInput, TOutput>(this TaskActivity<TInput, TOutput> activity)
        {
            var scope = new IndicoScope();
            scope.Body.Handler = activity;

            return scope;
        }

        private static IDictionary<string, object> Invoke(this IndicoScope scope,
            IDictionary<string, object> args = null)
            => WorkflowInvoker.Invoke(
                scope,
                GetInvokeParams(args));

        private static IDictionary<string, object> GetInvokeParams(IDictionary<string, object> args) =>
            new Dictionary<string, object>
                    {{nameof(IndicoScope.BaseUrl), BaseUrl}, {nameof(IndicoScope.Token), ApiToken}}
                .Union(args ?? Enumerable.Empty<KeyValuePair<string, object>>())
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
