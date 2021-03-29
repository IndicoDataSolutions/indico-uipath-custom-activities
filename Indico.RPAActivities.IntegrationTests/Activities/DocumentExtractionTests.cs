using System.Activities;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class DocumentExtractionTests
    {
        private TestDataHelper _testData;

        [SetUp]
        public void SetUp() => _testData = new TestDataHelper();


        [Test]
        public void Test() =>
            new DocumentExtraction { ConfigType = "standard", Document = _testData.GetFilePath() }
                .Invoke()
                .Should().NotBeNull();

        [Test]
        public void ShouldThrow_WhenNoDocument() => new DocumentExtraction()
            .Invoking(a => a.Invoke())
            .Should()
            .Throw<InvalidWorkflowException>()
            .WithMessage($"*Value for a required activity argument '{nameof(DocumentExtraction.Document)}' was not supplied.*");
    }
}
