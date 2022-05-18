using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indico.RPAActivities.Activities.Properties;
using Indico.RPAActivities.Activities.Activities;
using UiPath.Shared.Activities.Localization;
using IndicoV2.Submissions.Models;
using System;
using System.Linq;

namespace Indico.RPAActivities.Activities
{
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
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);

            var cts =  CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);
           
            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cts.Token);
            var timer = Task.Delay(timeout, cts.Token);
            var completedTask = await Task.WhenAny(task, timer);
            if (completedTask == task)
            {
                // Outputs
                return (ctx) =>
                {
                    Results(ctx, task.Result);
                };
            }

            else
            {
                throw new TimeoutException(Resources.Timeout_Error);
            }

         
        }

        protected void Results(AsyncCodeActivityContext context, IEnumerable<ISubmission> result)
        {
            Submissions.Set(context,result?.ToList());
        }
        protected async Task<IEnumerable<ISubmission>> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            ///////////////////////////
            // Add execution logic HERE

            ///////////////////////////
            ///
            var submissionIds = SubmissionIDs.Get(context);
            var workflowIds = WorkflowIDs.Get(context);
            var retrieved = Retrieved.Get(context);
            var limit = Limit.Get(context);
            var inputFilename = InputFilename.Get(context);
            var submissionStatus = Status.Get(context);

            return await Application.ListSubmissions(submissionIds, workflowIds, inputFilename, submissionStatus, retrieved, limit, cancellationToken);
        }

    }
}

