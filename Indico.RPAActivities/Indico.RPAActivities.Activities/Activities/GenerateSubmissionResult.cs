using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Activities;
using Newtonsoft.Json.Linq;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;
using IndicoV2.Submissions.Models;
using System;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.SubmissionCategory))]
    [LocalizedDisplayName(nameof(Resources.GenerateSubmissionResult_DisplayName))]
    [LocalizedDescription(nameof(Resources.GenerateSubmissionResult_Description))]
    public class GenerateSubmissionResult : IndicoActivityBase<int, string>
    {
        [LocalizedDisplayName(nameof(Resources.GenerateSubmissionResult_SubmissionID_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateSubmissionResult_SubmissionID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<int> SubmissionID { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateSubmissionResult_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateSubmissionResult_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Result { get; set; }

        protected override int GetInputs(AsyncCodeActivityContext ctx) =>
            SubmissionID.Get(ctx);

        protected override Task<string> ExecuteAsync(int input, CancellationToken cancellationToken) =>
            Application.GenerateSubmissionResult(input, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, string output) => Result.Set(ctx, output);
    }
}

