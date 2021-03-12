using System.Activities;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using IndicoV2.Submissions.Models;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class ListSubmissionsTests
    {
        [Test]
        public void ListSubmissions_ReturnsSubmissions()
        {
            var action = new ListSubmissions
            {
                Limit = 1
            };

            var result = action.Invoke<ListSubmissions, (List<int> WorkflowIds, List<int> SubmissionIds, SubmissionFilter Filters, int Limit), List<ISubmission>>(
                (a, outArg) => a.Submissions = outArg);

            result.Count.Should().Be(1);
            var submission = result.Single();
            submission.DatasetId.Should().BeGreaterThan(0);
            submission.WorkflowId.Should().BeGreaterThan(0);
            submission.Id.Should().BeGreaterThan(0);
            submission.InputFile.Should().NotBeEmpty();
        }
    }
}
