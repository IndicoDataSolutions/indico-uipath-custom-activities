using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;

namespace UiPath.Shared.Activities.RuntimeSimple
{
    public abstract class TaskActivity : AsyncCodeActivity
    {
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;
        
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<bool>(state);
            var timeoutMs = TimeoutMS.Get(context);
            var cancellationTokenSource = new CancellationTokenSource(timeoutMs);

            context.UserState = new State(cancellationTokenSource);

            try
            {
                ExecuteAsync(context, cancellationTokenSource.Token)
                    .ContinueWith(
                        t =>
                        {
                            if (t.IsFaulted)
                            {
                                tcs.SetException(t.Exception?.InnerException ?? t.Exception ?? new Exception("Unknown error"));
                            }
                            else if (t.IsCanceled)
                            {
                                tcs.SetCanceled();
                            }
                            else
                            {
                                tcs.SetResult(true);
                            }

                            callback?.Invoke(tcs.Task);
                        },
                        cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
                callback?.Invoke(tcs.Task);
            }

            return tcs.Task;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result) =>
            EndExecute((State)context.UserState, (Task<bool>)result, ContinueOnError.Get(context));

        private void EndExecute(State state, Task<bool> task, bool continueOnError)
        {
            state.Dispose();

            if (task.IsFaulted && !continueOnError)
            {
                throw task.Exception?.InnerException ?? task.Exception ?? new Exception("Unexpected exception");
            }
        }

        protected abstract Task ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken);

        private class State : IDisposable
        {
            private readonly IDisposable[] _disposables;

            public State(params IDisposable[] disposables)
            {
                _disposables = disposables;
            }
            public void Dispose()
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
