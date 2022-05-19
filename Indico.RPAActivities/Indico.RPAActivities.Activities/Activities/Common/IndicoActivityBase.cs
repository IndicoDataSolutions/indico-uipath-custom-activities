using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using Indico.UiPath.Shared.Activities;
using Indico.UiPath.Shared.Activities.Localization;
using Indico.UiPath.Shared.Activities.Utilities;

namespace Indico.RPAActivities.Activities.Activities
{
    public abstract class IndicoActivityBase<TInput, TOutput> : ContinuableAsyncCodeActivity
    {
        protected Application Application { get; private set; }
        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        protected IndicoActivityBase()
        {
            Constraints.Add(ActivityConstraints.HasParentType<IndicoActivityBase<TInput, TOutput>, IndicoScope>(string.Format(Resources.ValidationScope_Error, Resources.IndicoScope_DisplayName)));
        }

        protected abstract TInput GetInputs(AsyncCodeActivityContext ctx);

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            IObjectContainer objectContainer = context.GetFromContext<IObjectContainer>(IndicoScope.ParentContainerPropertyTag);
            Application = objectContainer.Get<Application>();
            return base.BeginExecute(context, callback, state);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            int timeout = TimeoutMS.Get(context);

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            // Set a timeout on the execution
            Task<TOutput> task = ExecuteWithTimeout(GetInputs(context), cts.Token);
            Task timer = Task.Delay(timeout, cts.Token);
            Task completedTask = await Task.WhenAny(task, timer);
            if (completedTask == task)
            {
                // Outputs
                return (ctx) =>
                {
                    SetResults(ctx, task.Result);
                };
            }

            else
            {
                throw new TimeoutException(Resources.Timeout_Error);
            }

        }
        protected abstract void SetResults(AsyncCodeActivityContext context, TOutput result);
        protected abstract Task<TOutput> ExecuteWithTimeout(TInput inputs, CancellationToken cancellationToken = default);
    }
}
