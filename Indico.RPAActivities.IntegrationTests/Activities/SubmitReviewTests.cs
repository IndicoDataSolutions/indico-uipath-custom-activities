using System.Activities;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class SubmitReviewTests
    {
        private TestDataHelper _testData;

        [SetUp]
        public void SetUp() => _testData = new TestDataHelper();

        [Test]
        public void SubmitReview_ShouldReturnResult()
        {
            var submissionId = _testData.GetSubmissionId();
            var submissionResult = new SubmissionResult { SubmissionID = submissionId }.Invoke();
            var changes = (JObject)submissionResult["results"]["document"]["results"];

            var result = new SubmitReview { SubmissionID = submissionId, Changes = new InArgument<JObject>(_ => changes), }.Invoke();

            result.Should().NotBeNull();
            result.Value<int>("review_id").Should().BeGreaterThan(0);
            result.Value<int>("submission_id").Should().BeGreaterThan(0);
        }

        [Test]
        public void SubmitReview_ShouldThrow_WhenInvalidSubmissionId() =>
            new SubmitReview {SubmissionID = 0, Rejected = true}
                .Invoking(a => a.Invoke())
                .Should()
                .ThrowAuthorizationException();

        [Test]
        public void SubmitReview_ShouldThrow_WhenNoSubmissionId() =>
            new SubmitReview()
                .Invoking(a => a.Invoke())
                .Should().Throw<InvalidWorkflowException>()
                .WithMessage("*Value for a required activity argument 'SubmissionID' was not supplied.");
    }
}
