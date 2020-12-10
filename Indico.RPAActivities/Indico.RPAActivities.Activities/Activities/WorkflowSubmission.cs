using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_DisplayName))]
    [LocalizedDescription(nameof(Resources.WorkflowSubmission_Description))]
    public class WorkflowSubmission : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_WorkflowID_DisplayName))]
        [LocalizedDescription(nameof(Resources.WorkflowSubmission_WorkflowID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> WorkflowID { get; set; }

        [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_FilePaths_DisplayName))]
        [LocalizedDescription(nameof(Resources.WorkflowSubmission_FilePaths_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<string>> FilePaths { get; set; }

        [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_Urls_DisplayName))]
        [LocalizedDescription(nameof(Resources.WorkflowSubmission_Urls_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<string>> Urls { get; set; }

        [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_SubmissionIDs_DisplayName))]
        [LocalizedDescription(nameof(Resources.WorkflowSubmission_SubmissionIDs_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<int>> SubmissionIDs { get; set; }

        #endregion


        #region Constructors

        public WorkflowSubmission()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (WorkflowID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(WorkflowID)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);
            var workflowid = WorkflowID.Get(context);
            var filepaths = FilePaths.Get(context);
            var urls = Urls.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            // Outputs
            return (ctx) => {
                SubmissionIDs.Set(ctx, task.Result);
            };
        }

        private async Task<List<int>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var workflowId = WorkflowID.Get(context);
            var files = FilePaths.Get(context);
            var urls = Urls.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            return await application.WorkflowSubmission(workflowId, files, urls);
        }

        #endregion
    }
}

