using System.Activities;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class SubmissionResultTests
    {
        private TestDataHelper _testData;

        [SetUp]
        public void SetUp() => _testData = new TestDataHelper();

        [Test]
        public void SubmissionResult_ShouldReturnResult()
        {
            var result = new SubmissionResult {SubmissionID = _testData.GetSubmissionId()}.Invoke();

            result.Value<int>("submission_id").Should().BeGreaterThan(0);
            result["results"].Should().BeOfType(typeof(JObject));
        }

        [Test]
        public void SubmissionResult_ShouldThrow_WhenInvalidSubmissionId() =>
            new SubmissionResult {SubmissionID = 0}.Invoking(act => act.Invoke())
                .Should()
                .ThrowAuthorizationException();

        [Test]
        public void SubmissionResult_ShouldThrow_WhenNoSubmissionId() =>
            new SubmissionResult { }.Invoking(act => act.Invoke())
                .Should().Throw<InvalidWorkflowException>()
                .WithMessage("*Value for a required activity argument 'SubmissionID' was not supplied.");
    }
}
