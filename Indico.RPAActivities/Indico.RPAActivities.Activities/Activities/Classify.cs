using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.Classify_DisplayName))]
    [LocalizedDescription(nameof(Resources.Classify_Description))]
    public class Classify : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedDisplayName(nameof(Resources.Classify_Text_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_Text_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<string>> Text { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_ModelGroup_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_ModelGroup_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> ModelGroup { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_TopResults_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_TopResults_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<String>> TopResults { get; set; }

        [LocalizedDisplayName(nameof(Resources.Classify_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.Classify_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<Dictionary<string, double>>> Results { get; set; }

        #endregion


        #region Constructors

        public Classify()
        {
            Constraints.Add(ActivityConstraints.HasParentType<Classify, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Text == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Text)));
            if (ModelGroup == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ModelGroup)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            // Find the top class for each result set
            List<String> topResults = new List<String>();
            foreach (Dictionary<String, Double> resultSet in task.Result)
            {
                KeyValuePair<String, Double> topClass = new KeyValuePair<String, Double>();
                foreach (KeyValuePair<String, Double> result in resultSet)
                {
                    if(result.Value > topClass.Value)
                    {
                        topClass = result;
                    }
                }
                topResults.Add(topClass.Key);
            }

            // Outputs
            return (ctx) => {
                TopResults.Set(ctx, topResults);
                Results.Set(ctx, task.Result);
            };
        }

        private async Task<List<Dictionary<string, double>>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var text = Text.Get(context);
            var modelgroup = ModelGroup.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();
            var result = await application.Classify(text, modelgroup, cancellationToken);
            return result;
        }

        #endregion
    }
}

