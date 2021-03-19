using System.Activities;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class ClassifyTests
    {
        private int _modelGroupId;

        [SetUp]
        public void SetUp() => _modelGroupId = new TestDataHelper().GetModelGroupId();

        [TestCase("test")]
        [TestCase("Invoice date: 2010-11-12 InvoiceNumber: test", "Invoice date: 2012-11-12")]
        public void Classify_ShouldReturnResult(params string[] data)
        {
            var predictionResults = new Classify
            {
                Text = new InArgument<List<string>>(_ => data.ToList()),
                ModelGroup = _modelGroupId,
            }.Invoke();

            predictionResults.Count.Should().Be(data.Length);

            var firstPrediction = predictionResults.First().First();
            firstPrediction.Label.Should().NotBeNullOrEmpty();
            firstPrediction.Text.Should().NotBeNullOrEmpty();
            
            firstPrediction.Start.Should().BeGreaterThan(0);
            firstPrediction.End.Should().BeGreaterThan(0);

            firstPrediction.Confidence.Should().NotBeEmpty();
            firstPrediction.Confidence.First().Value.Should().BeGreaterThan(0);
        }
    }
}
