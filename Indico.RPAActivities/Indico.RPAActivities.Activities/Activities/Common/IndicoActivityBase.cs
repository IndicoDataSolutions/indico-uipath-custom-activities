using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using IndicoV2.Submissions.Models;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities.Activities
{
    public abstract class IndicoActivityBase<TInput, TOutput> : ContinuableAsyncCodeActivity
    {
        protected Application Application { get; private set; }


        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        protected IndicoActivityBase()
        {
            Constraints.Add(ActivityConstraints.HasParentType<IndicoActivityBase<TInput, TOutput>, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));

        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            Application = objectContainer.Get<Application>();

            return base.BeginExecute(context, callback, state);

        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
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

        protected abstract void Results(AsyncCodeActivityContext context, TOutput result);

        protected abstract Task<TOutput> ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default);
    }
}


