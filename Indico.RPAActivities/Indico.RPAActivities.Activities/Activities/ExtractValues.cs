using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Entity;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ExtractValues_DisplayName))]
    [LocalizedDescription(nameof(Resources.ExtractValues_Description))]
    public class ExtractValues : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.ExtractValues_Text_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractValues_Text_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<string>> Text { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractValues_ModelGroup_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractValues_ModelGroup_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> ModelGroup { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractValues_Results_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractValues_Results_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<List<Extraction>>> Results { get; set; }

        #endregion


        #region Constructors

        public ExtractValues()
        {
            Constraints.Add(ActivityConstraints.HasParentType<ExtractValues, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
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

            // Outputs
            return async (ctx) => {
                Results.Set(ctx, await task);
            };
        }

        private async Task<List<List<Extraction>>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var text = Text.Get(context);
            var modelgroup = ModelGroup.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();
            var result = await application.Extract(text, modelgroup, cancellationToken);
            return result;
        }

        #endregion
    }
}
