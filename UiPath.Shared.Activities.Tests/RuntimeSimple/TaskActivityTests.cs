﻿using System;
using System.Activities;
using System.Collections.Generic;
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
        public void SetUp() => _invoker = new WorkflowInvoker((_fakeActivity = new FakeActivity()));

        [Test]
        public void Invoke_ShouldReturnObject()
        {
            // Arrange
            var inputs = new object();
            object outputs = null;

            _fakeActivity.GetInputsHandler = _ => inputs;
            _fakeActivity.ExecuteAsyncHandler = (inParams, ct) => Task.FromResult(inParams);
            _fakeActivity.SetOutputsHandler = (context, o) => outputs = o;

            // Act
            var result = _invoker.Invoke();

            // Assert
            outputs.Should().Be(inputs);
        }

        [Test]
        public void Invoke_ShouldThrow_WhenInitThrows()
        {
            _fakeActivity.InitHandler = _ => throw new TExpectedException();

            AssertThrows();
        }

        [Test]
        public void Invoke_ShouldThrow_WhenGetInputsThrows()
        {
            _fakeActivity.GetInputsHandler = _ => throw new TExpectedException();

            AssertThrows();
        }


        [Test]
        public void Invoke_ShouldThrow_WhenExecuteAsyncThrows()
        {
            _fakeActivity.ExecuteAsyncHandler = (i, ct) => throw new TExpectedException();

            AssertThrows();
        }

        [Test]
        public void Invoke_ShouldThrow_WhenExecuteAsyncReturnsTaskThrowing()
        {
            _fakeActivity.ExecuteAsyncHandler = (inputs, ct) => Task.FromException<object>(new TExpectedException());

            AssertThrows();
        }

        [Test]
        public void Invoke_ShouldThrow_WhenSetOutputThrows()
        {
            _fakeActivity.SetOutputsHandler = (context, o) => throw new TExpectedException();

            AssertThrows();
        }

        private void AssertThrows()
        {
            // Act, Assert
            var invokeAssertionBuilder = _invoker.Invoking(i =>
                   i.Invoke(new Dictionary<string, object>()));

            invokeAssertionBuilder
           .Should()
           .Throw<TExpectedException>();
        }

        private class FakeActivity : TaskActivity<object, object>
        {
            public Action<AsyncCodeActivityContext> InitHandler { get; set; } = _ => { };
            public Func<AsyncCodeActivityContext, object> GetInputsHandler { get; set; } = _ => new object();
            public Func<object, CancellationToken, Task<object>> ExecuteAsyncHandler { get; set; } = (inputs, _) => Task.FromResult(inputs);
            public Action<AsyncCodeActivityContext, object> SetOutputsHandler { get; set; } = (_, o) => { };

            protected override void Init(AsyncCodeActivityContext context)
            {
                base.Init(context);

                InitHandler(context);
            }

            protected override object GetInputs(AsyncCodeActivityContext ctx) => GetInputsHandler(ctx);

            protected override Task<object> ExecuteAsync(object input, CancellationToken cancellationToken) =>
                ExecuteAsyncHandler(input, cancellationToken);

            protected override void SetOutputs(AsyncCodeActivityContext ctx, object output) =>
                SetOutputsHandler(ctx, output);
        }
    }
}
