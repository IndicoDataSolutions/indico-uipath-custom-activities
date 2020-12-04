using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.GetModelGroup_DisplayName))]
    [LocalizedDescription(nameof(Resources.GetModelGroup_Description))]
    public class GetModelGroup : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.GetModelGroup_ModelGroupID_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetModelGroup_ModelGroupID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> ModelGroupID { get; set; }

        [LocalizedDisplayName(nameof(Resources.GetModelGroup_ModelGroupData_DisplayName))]
        [LocalizedDescription(nameof(Resources.GetModelGroup_ModelGroupData_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<ModelGroup> ModelGroupData { get; set; }

        #endregion


        #region Constructors

        public GetModelGroup()
        {
            Constraints.Add(ActivityConstraints.HasParentType<GetModelGroup, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (ModelGroupID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(ModelGroupID)));

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
                ModelGroupData.Set(ctx, task.Result);
            };
        }

        private async Task<ModelGroup> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var modelGroupID = ModelGroupID.Get(context);
            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            return await application.GetModelGroup(modelGroupID);
        }

        #endregion
    }
}

