using System.Activities;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class WorkflowSubmissionTests
    {
        private int _workflowId;
        public List<string> _filepaths;
        public List<string> _uris;

        [OneTimeSetUp]
        public void SetUp()
        {
            var testDataHelper = new TestDataHelper();

            _workflowId = testDataHelper.GetWorkflowId();
            _filepaths = testDataHelper.GetWorkflowSubmissionFilePaths().ToList();
            _uris = testDataHelper.GetWorkflowSubmissionUris().ToList();
        }

        [Test]
        public void WorkflowSubmission_ShouldReturnListOfIds_WhenFilepathsPosted() => new WorkflowSubmission
                {WorkflowID = _workflowId, FilePaths = new InArgument<List<string>>(ctx => _filepaths)}.Invoke()
            .Should()
            .HaveSameCount(_filepaths);

        [Test]
        public void WorkflowSubmission_ShouldReturnListOfIds_WhenUrisPosted() =>
            new WorkflowSubmission { WorkflowID = _workflowId, Urls = new InArgument<List<string>>(ctx => _uris.ToList()) }
            .Invoke().Should().HaveSameCount(_uris);
    }
}
