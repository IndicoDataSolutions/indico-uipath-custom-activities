using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;
using Indico.RPAActivities.Activities.Activities;
using IndicoV2.Submissions.Models;
using System;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.SubmissionCategory))]
    [LocalizedDisplayName(nameof(Resources.ListSubmissions_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListSubmissions_Description))]
    public class ListSubmissions : IndicoActivityBase<(List<int> WorkflowIds, List<int> SubmissionIds, string InputFilename, SubmissionStatus? Status, bool? Retrieved, int Limit), List<ISubmission>>
    {
        [LocalizedDisplayName(nameof(Resources.ListSubmissions_SubmissionIDs_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_SubmissionIDs_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<int>> SubmissionIDs { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_WorkflowIDs_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_WorkflowIDs_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<List<int>> WorkflowIDs { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_InputFilename_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_InputFilename_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> InputFilename { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Status_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<SubmissionStatus?> Status { get; set; }

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Retrieved_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Retrieved_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool?> Retrieved { get; set; }


        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Limit_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Limit_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> Limit { get; set; } = 1000;

        [LocalizedDisplayName(nameof(Resources.ListSubmissions_Submissions_DisplayName))]
        [LocalizedDescription(nameof(Resources.ListSubmissions_Submissions_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<List<ISubmission>> Submissions { get; set; }

        protected override (List<int> WorkflowIds, List<int> SubmissionIds, string InputFilename, SubmissionStatus? Status, bool? Retrieved, int Limit) GetInputs(AsyncCodeActivityContext ctx) =>
            (WorkflowIDs.Get(ctx), SubmissionIDs.Get(ctx), InputFilename.Get(ctx), Status.Get(ctx), Retrieved.Get(ctx), Limit.Get(ctx));

        protected override async Task<List<ISubmission>> ExecuteAsync((List<int> WorkflowIds, List<int> SubmissionIds, string InputFilename, SubmissionStatus? Status, bool? Retrieved, int Limit) p, CancellationToken cancellationToken) =>
            await Application.ListSubmissions(p.SubmissionIds, p.WorkflowIds, p.InputFilename, p.Status, p.Retrieved, p.Limit, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<ISubmission> submissions) => Submissions.Set(ctx, submissions);
    }
}

