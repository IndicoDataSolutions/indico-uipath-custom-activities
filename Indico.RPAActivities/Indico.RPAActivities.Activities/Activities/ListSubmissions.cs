using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;
using Indico.Entity;
using Indico.RPAActivities.Activities.Activities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListSubmissions_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListSubmissions_Description))]
    public class ListSubmissions : IndicoActivityBase<(List<int> WorkflowIds, List<int> SubmissionIds, SubmissionFilter Filters, int Limit), List<Submission>>
    {
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

        protected override (List<int> WorkflowIds, List<int> SubmissionIds, SubmissionFilter Filters, int Limit) GetInputs(AsyncCodeActivityContext ctx) =>
            (WorkflowIDs.Get(ctx), SubmissionIDs.Get(ctx), Filters.Get(ctx), Limit.Get(ctx));

        protected override async Task<List<Submission>> ExecuteAsync((List<int> WorkflowIds, List<int> SubmissionIds, SubmissionFilter Filters, int Limit) p, CancellationToken cancellationToken) => 
            await Application.ListSubmissions(p.SubmissionIds, p.WorkflowIds, p.Filters, p.Limit, cancellationToken);

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<Submission> submissions) => Submissions.Set(ctx, submissions);
    }
}

