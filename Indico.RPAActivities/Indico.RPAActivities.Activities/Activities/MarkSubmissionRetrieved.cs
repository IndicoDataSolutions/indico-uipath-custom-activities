using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using Indico.UiPath.Shared.Activities.Localization;
using IndicoV2.Submissions.Models;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.MarkSubmissionRetrieved_DisplayName))]
    [LocalizedDescription(nameof(Resources.MarkSubmissionRetrieved_Description))]
    public class MarkSubmissionRetrieved : IndicoActivityBase<int, ISubmission>
    {
        [LocalizedDisplayName(nameof(Resources.MarkSubmissionRetrieved_SubmissionID_DisplayName))]
        [LocalizedDescription(nameof(Resources.MarkSubmissionRetrieved_SubmissionID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<int> SubmissionID { get; set; }

        [LocalizedDisplayName(nameof(Resources.MarkSubmissionRetrieved_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.MarkSubmissionRetrieved_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<ISubmission> Result { get; set; }

        protected override int GetInputs(AsyncCodeActivityContext ctx)
        {
            return (SubmissionID.Get(ctx));
        }

        protected async override Task<ISubmission> ExecuteWithTimeout(int input, CancellationToken cancellationToken) =>
            await Application.MarkSubmissionAsRetrieved(input, true, cancellationToken);

        protected override void SetResults(AsyncCodeActivityContext ctx, ISubmission output) => Result.Set(ctx, output);

    }
}
