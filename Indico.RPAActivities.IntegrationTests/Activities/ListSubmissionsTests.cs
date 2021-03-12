using System.Linq;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class ListSubmissionsTests
    {
        [Test]
        public void ListSubmissions_ReturnsSubmissions()
        {
            var action = new ListSubmissions { Limit = 1 };

            var result = action.Invoke();

            result.Count.Should().Be(1);
            var submission = result.Single();
            submission.DatasetId.Should().BeGreaterThan(0);
            submission.WorkflowId.Should().BeGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.InputFile.Should().NotBeEmpty();
        }
    }
}
