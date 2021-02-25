using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using Indico.Entity;
using UiPath.Shared.Activities.RuntimeSimple;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities
{
    [LocalizedDisplayName(nameof(Resources.ListSubmissions_DisplayName))]
    [LocalizedDescription(nameof(Resources.ListSubmissions_Description))]
    public class ListSubmissions : TaskActivity
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


        protected override async Task ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            var submissionids = SubmissionIDs.Get(context);
            var workflowids = WorkflowIDs.Get(context);
            var filters = Filters.Get(context);
            var limit = Limit.Get(context);

            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            var application = objectContainer.Get<Application>();

            var submissions = await application.ListSubmissions(submissionids, workflowids, filters, limit, cancellationToken);

            Submissions.Set(context, submissions.ToList());
        }
    }
}

