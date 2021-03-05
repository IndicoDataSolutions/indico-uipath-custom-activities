using System;
using System.Activities;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UiPath.Shared.Activities.RuntimeSimple;
using FluentAssertions;

namespace UiPath.Shared.Activities.Tests.RuntimeSimple
{
    public class TaskActivityTests1 : TaskActivityTests<InvalidOperationException>
    {
    }

    public class TaskActivityTests2 : TaskActivityTests<NotImplementedException>
    {
    }

    public abstract class TaskActivityTests<TExpectedException>
        where TExpectedException : Exception, new()
    {
        private WorkflowInvoker _invoker;
        private FakeActivity _fakeActivity;

        [SetUp]
        public void SetUp() => _invoker = new WorkflowInvoker(_fakeActivity = new FakeActivity());

        [Test]
        public void Invoke_ShouldReturnObject()
        {
            // Arrange
            var inputs = new object();

            _fakeActivity.GetInputsHandler = _ => inputs;
            _fakeActivity.ExecuteAsyncHandler = (inParams, ct) => Task.FromResult(inParams);

            // Act
            var result = _invoker.Invoke();

            // Assert
            result.Single().Value.Should().Be(inputs);
        }

        [Theory]
        public void Invoke_ShouldThrow_WhenInitThrows_AndContinueOnErrorIsFalse(bool continueOnError)
        {
            _fakeActivity.InitHandler = _ => throw new TExpectedException();

            AssertThrowsWhenContinueOnErrorFalse(continueOnError);
        }

        [Theory]
        public void Invoke_ShouldThrow_WhenGetInputsThrows_AndContinueOnErrorIsFalse(bool continueOnError)
        {
            _fakeActivity.GetInputsHandler = _ => throw new TExpectedException();

            AssertThrowsWhenContinueOnErrorFalse(continueOnError);
        }


        [Theory]
        public void Invoke_ShouldThrow_WhenExecuteAsyncThrows_AndContinueOnErrorIsFalse(bool continueOnError)
        {
            _fakeActivity.ExecuteAsyncHandler = (i, ct) => throw new TExpectedException();

            AssertThrowsWhenContinueOnErrorFalse(continueOnError);
        }

        [Theory]
        public void Invoke_ShouldThrow_WhenExecuteAsyncReturnsTaskThrowing_AndContinueOnErrorIsFalse(bool continueOnError)
        {
            _fakeActivity.ExecuteAsyncHandler = (inputs, ct) => Task.FromException<object>(new TExpectedException());

            AssertThrowsWhenContinueOnErrorFalse(continueOnError);
        }

        [Theory]
        public void Invoke_ShouldThrow_WhenSetOutputThrows_AndContinueOnErrorIsFalse(bool continueOnError)
        {
            _fakeActivity.SetOutputsHandler = (context, o) => throw new TExpectedException();

            AssertThrowsWhenContinueOnErrorFalse(continueOnError);
        }

        private void AssertThrowsWhenContinueOnErrorFalse(bool continueOnError)
        {
            // Act, Assert
            var invokeAssertionBuilder = _invoker.Invoking(i =>
                   i.Invoke(new Dictionary<string, object> { { nameof(TaskActivity<int, int>.ContinueOnError), continueOnError } }));

            if (!continueOnError)
            {
                invokeAssertionBuilder
               .Should()
               .Throw<TExpectedException>();
            }
            else
            {
                invokeAssertionBuilder.Should().NotThrow();
            }
        }

        private class FakeActivity : TaskActivity<object, object>
        {
            public OutArgument<object> OutArg { get; set; }
            public Action<AsyncCodeActivityContext> InitHandler { get; set; } = _ => { };
            public Func<AsyncCodeActivityContext, object> GetInputsHandler { get; set; } = _ => new object();
            public Func<object, CancellationToken, Task<object>> ExecuteAsyncHandler { get; set; } = (inputs, _) => Task.FromResult(inputs);
            public Action<AsyncCodeActivityContext, object> SetOutputsHandler { get; set; } = (ctx, o) => { };

            protected override void Init(AsyncCodeActivityContext context)
            {
                base.Init(context);

                InitHandler(context);
            }

            protected override object GetInputs(AsyncCodeActivityContext ctx) => GetInputsHandler(ctx);

            protected override Task<object> ExecuteAsync(object input, CancellationToken cancellationToken) =>
                ExecuteAsyncHandler(input, cancellationToken);

            protected override void SetOutputs(AsyncCodeActivityContext ctx, object output)
            {
                OutArg.Set(ctx, output);
                SetOutputsHandler(ctx, output);
            }
        }
    }
}
