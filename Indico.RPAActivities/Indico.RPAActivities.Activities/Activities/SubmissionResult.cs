using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Newtonsoft.Json.Linq;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;
using IndicoV2.Submissions.Models;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.SubmissionCategory))]
    [LocalizedDisplayName(nameof(Resources.SubmissionResult_DisplayName))]
    [LocalizedDescription(nameof(Resources.SubmissionResult_Description))]
    public class SubmissionResult : IndicoActivityBase<(int SubmissionId, SubmissionStatus? CheckStatus), JObject>
    {
        [LocalizedDisplayName(nameof(Resources.SubmissionResult_SubmissionID_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmissionResult_SubmissionID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<int> SubmissionID { get; set; }

        [LocalizedDisplayName(nameof(Resources.SubmissionResult_CheckStatus_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmissionResult_CheckStatus_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<SubmissionStatus?> CheckStatus { get; set; }

        [LocalizedDisplayName(nameof(Resources.SubmissionResult_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.SubmissionResult_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<JObject> Result { get; set; }
        
        protected override (int SubmissionId, SubmissionStatus? CheckStatus) GetInputs(AsyncCodeActivityContext ctx) =>
            (SubmissionID.Get(ctx), CheckStatus.Get(ctx));

        protected override Task<JObject> ExecuteAsync((int SubmissionId, SubmissionStatus? CheckStatus) input, CancellationToken cancellationToken) =>
            Application.SubmissionResult(input.SubmissionId, input.CheckStatus, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, JObject output) => Result.Set(ctx, output);
    }
}

