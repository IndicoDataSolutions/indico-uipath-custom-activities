using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Activities;
using Indico.RPAActivities.Activities.Properties;
using System.Linq;
using System;
using UiPath.Shared.Activities.Localization;

namespace Indico.RPAActivities.Activities
{
    [LocalizedCategory(nameof(Resources.WorkflowCategory))]
    [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_DisplayName))]
    [LocalizedDescription(nameof(Resources.WorkflowSubmission_Description))]
    public class WorkflowSubmission : IndicoActivityBase<(int WorkflowId, List<string> FilePaths, List<string> Urls), List<int>>
    {
        [LocalizedDisplayName(nameof(Resources.WorkflowSubmission_WorkflowID_DisplayName))]
        [LocalizedDescription(nameof(Resources.WorkflowSubmission_WorkflowID_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
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

        protected override (int WorkflowId, List<string> FilePaths, List<string> Urls) GetInputs(AsyncCodeActivityContext ctx)
            => (WorkflowID.Get(ctx), FilePaths.Get(ctx), Urls.Get(ctx));

        protected override async Task<List<int>> ExecuteAsync((int WorkflowId, List<string> FilePaths, List<string> Urls) input, CancellationToken cancellationToken)
        {
            var filePathsProvided = ValuesProvided(input.FilePaths);
            var urisProvided = ValuesProvided(input.Urls);

            if (filePathsProvided && urisProvided || !filePathsProvided && !urisProvided)
            {
                throw new ArgumentException(string.Format(Resources.ValidationExclusiveProperties_Error, nameof(FilePaths), nameof(Urls)));
            }

            return (await Application.WorkflowSubmission(input.WorkflowId, input.FilePaths, input.Urls, cancellationToken)).ToList();
        }

        private bool ValuesProvided<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null || !enumerable.Any())
            {
                return true;
            }

            return false;
        }

        protected override void SetOutputs(AsyncCodeActivityContext ctx, List<int> output) => SubmissionIDs.Set(ctx, output);
    }
}

