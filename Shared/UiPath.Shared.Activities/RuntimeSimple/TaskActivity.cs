using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Indico.RPAActivities.Activities.Properties;
using UiPath.Shared.Activities.Localization;

namespace UiPath.Shared.Activities.RuntimeSimple
{
    public abstract class TaskActivity<TInput, TOutput> : AsyncCodeActivity
    {
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<TOutput>(state);
            var timeoutMs = TimeoutMS.Get(context);
            var cancellationTokenSource = new CancellationTokenSource(timeoutMs);

            context.UserState = new State(cancellationTokenSource);

            try
            {
                Init(context);
                var inputs = GetInputs(context);

                ExecuteAsync(inputs, cancellationTokenSource.Token)
                    .ContinueWith(
                        t =>
                        {
                            if (t.IsFaulted)
                            {
                                tcs.SetException(t.Exception?.InnerException ?? t.Exception ?? new Exception("Unexpected error"));
                            }
                            else if (t.IsCanceled)
                            {
                                tcs.SetCanceled();
                            }
                            else
                            {
                                tcs.SetResult(t.Result);
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

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result) => EndExecute(context, (Task<TOutput>)result);

        private void EndExecute(AsyncCodeActivityContext ctx, Task<TOutput> task)
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                SetOutputs(ctx, task.Result);
            }

            ((State)ctx.UserState).Dispose();

            if (task.IsFaulted)
            {
                throw task.Exception?.InnerException ?? task.Exception ?? new Exception("Unexpected error");
            }
        }

        protected virtual void Init(AsyncCodeActivityContext context)
        {
        }

        protected abstract TInput GetInputs(AsyncCodeActivityContext ctx);

        protected abstract Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken);

        protected abstract void SetOutputs(AsyncCodeActivityContext ctx, TOutput output);

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
