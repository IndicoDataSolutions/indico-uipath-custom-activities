using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Newtonsoft.Json.Linq;
using Indico.RPAActivities.Activities.Properties;
using Indico.UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    
    [LocalizedDisplayName(nameof(Resources.SubmitReview_DisplayName))]
    [LocalizedDescription(nameof(Resources.SubmitReview_Description))]
    public class SubmitReview : IndicoActivityBase<(int SubmissionId, JObject Changes, bool Rejected, bool? ForceComplete), JObject>
    {
        [LocalizedDisplayName(nameof(Resources.SubmitReview_SubmissionID_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmitReview_SubmissionID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
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

        protected override (int SubmissionId, JObject Changes, bool Rejected, bool? ForceComplete) GetInputs(AsyncCodeActivityContext ctx)
            => (SubmissionID.Get(ctx), Changes.Get(ctx), Rejected.Get(ctx), ForceComplete.Get(ctx));

        protected override Task<JObject> ExecuteWithTimeout((int SubmissionId, JObject Changes, bool Rejected, bool? ForceComplete) input, CancellationToken cancellationToken)
            => Application.SubmitReview(input.SubmissionId, input.Changes, input.Rejected, input.ForceComplete, cancellationToken);

        protected override void SetResults(AsyncCodeActivityContext ctx, JObject output) => Result.Set(ctx, output);
    }
}

