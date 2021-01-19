using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListWorkflows_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListWorkflows_Description))]
    public class ListWorkflows : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.ListWorkflows_DatasetID_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListWorkflows_DatasetID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> DatasetID { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListWorkflows_Workflows_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListWorkflows_Workflows_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<object> Workflows { get; set; }

        #endregion


        #region Constructors

        public ListWorkflows()
        {
            Constraints.Add(ActivityConstraints.HasParentType<ListWorkflows, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (DatasetID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DatasetID)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);
            var datasetid = DatasetID.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            // Outputs
            return (ctx) => {
                Workflows.Set(ctx, task.Result);
            };
        }

        private async Task<List<Workflow>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var datasetId = DatasetID.Get(context);
            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            return await application.ListWorkflows(datasetId, cancellationToken);
        }

        #endregion
    }
}

