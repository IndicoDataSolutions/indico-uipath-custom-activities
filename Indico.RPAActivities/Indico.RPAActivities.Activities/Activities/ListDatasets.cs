using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Models;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListDatasets_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListDatasets_Description))]
    public class ListDatasets : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.ListDatasets_Datasets_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListDatasets_Datasets_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<Dataset>> Datasets { get; set; }

        #endregion


        #region Constructors

        public ListDatasets()
        {
            Constraints.Add(ActivityConstraints.HasParentType<ListDatasets, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {

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
            return (ctx) => {
                Datasets.Set(ctx, task.Result);
            };
        }

        private async Task<List<Dataset>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();
            var datasets = application.ListDatasets();
            return datasets;
        }

        #endregion
    }
}

