using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using Indico.Entity;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListSubmissions_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListSubmissions_Description))]
    public class ListSubmissions : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_SubmissionIDs_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_SubmissionIDs_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<int>> SubmissionIDs { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_WorkflowIDs_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_WorkflowIDs_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<int>> WorkflowIDs { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Filters_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Filters_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<SubmissionFilter> Filters { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Limit_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Limit_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> Limit { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Submissions_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Submissions_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<Submission>> Submissions { get; set; }

        #endregion


        #region Constructors

        public ListSubmissions()
        {
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
            return async (ctx) => {
                Submissions.Set(ctx, await task);
            };
        }

        private async Task<List<Submission>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var submissionids = SubmissionIDs.Get(context);
            var workflowids = WorkflowIDs.Get(context);
            var filters = Filters.Get(context);
            var limit = Limit.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            return await application.ListSubmissions(submissionids, workflowids, filters, limit, cancellationToken);
        }

        #endregion
    }
}

