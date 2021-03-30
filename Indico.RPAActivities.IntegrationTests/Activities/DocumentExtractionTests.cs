using System;
using System.Activities;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using IndicoV2.Ocr.Models;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class DocumentExtractionTests
    {
        private TestDataHelper _testData;

        [SetUp]
        public void SetUp() => _testData = new TestDataHelper();


        [Theory]
        public void DocumentExtraction_ShouldReturnResult(DocumentExtractionPreset preset) =>
            new DocumentExtraction
                {
                    Document = _testData.GetFilePath(),
                    TimeoutMS = (int)TimeSpan.FromMinutes(10).TotalMilliseconds,
                    Preset = preset,
                }
                .Invoke()
                .Should()
                .StartWith("Our Google properties revenues");

        [Test]
        public void ShouldThrow_WhenNoDocument() => new DocumentExtraction()
            .Invoking(a => a.Invoke())
            .Should()
            .Throw<InvalidWorkflowException>()
            .WithMessage($"*Value for a required activity argument '{nameof(DocumentExtraction.Document)}' was not supplied.*");
    }
}
