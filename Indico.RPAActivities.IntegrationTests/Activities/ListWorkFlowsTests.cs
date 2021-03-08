using System.Activities;
using FluentAssertions;
using Indico.Exception;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class ListWorkFlowsTests
    {
        private int _dataSetId;

        [OneTimeSetUp]
        public void SetUp()
        {
            _dataSetId = new TestDataHelper().GetDataSetId();
        }

        [Test]
        public void ListWorkflows_ShouldReturnWorkFlow() =>
            new ListWorkflows { DatasetID = _dataSetId }.Invoke().Should().NotBeEmpty();

        [Test]
        public void ListWorkflows_ShouldThrow_WhenNoDataSetId() =>
            new ListWorkflows()
                .Invoking(action => action.Invoke())
                .Should().Throw<InvalidWorkflowException>()
                .WithMessage("*Value for a required activity argument 'DatasetID' was not supplied.");

        [TestCase(0)]
        public void ListWorkflows_ShouldThrow_WhenInvalidDataSetId(int invalidDataSetId) =>
            new ListWorkflows {DatasetID = invalidDataSetId}
                .Invoking(action => action.Invoke())
                .Should().Throw<GraphQLException>()
                .WithMessage(
                    @"1 : http://elnino:5000/api/workflow?dataset_ids=0: {""code"": 403, ""content"": {}, ""error_class"": ""ServiceException"", ""error_type"": ""ForbiddenAccess"", ""message"": ""This user does not have access to this resource""}");
    }
}
