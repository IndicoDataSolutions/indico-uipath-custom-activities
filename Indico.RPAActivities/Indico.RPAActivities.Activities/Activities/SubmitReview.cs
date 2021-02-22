using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.SubmitReview_DisplayName))]
    [LocalizedDescription(nameof(Resources.SubmitReview_Description))]
    public class SubmitReview : ContinuableAsyncCodeActivity
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

        [LocalizedDisplayName(nameof(Resources.SubmitReview_SubmissionID_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmitReview_SubmissionID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> SubmissionID { get; set; }

        [LocalizedDisplayName(nameof(Resources.SubmitReview_Changes_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmitReview_Changes_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<JObject> Changes { get; set; }

        [LocalizedDisplayName(nameof(Resources.SubmitReview_Rejected_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmitReview_Rejected_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> Rejected { get; set; } = false;

        [LocalizedDisplayName(nameof(Resources.SubmitReview_ForceComplete_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmitReview_ForceComplete_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool?> ForceComplete { get; set; }

        [LocalizedDisplayName(nameof(Resources.SubmitReview_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmitReview_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<JObject> Result { get; set; }

        #endregion


        #region Constructors

        public SubmitReview()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (SubmissionID == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(SubmissionID)));

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
                Result.Set(ctx, await task);
            };
        }

        private async Task<JObject> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            var submissionid = SubmissionID.Get(context);
            var changes = Changes.Get(context);
            var rejected = Rejected.Get(context);
            var forcecomplete = ForceComplete.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            return await application.SubmitReview(submissionid, changes, rejected, forcecomplete, cancellationToken);
        }

        #endregion
    }
}

