using FluentAssertions;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class ListDatasetsTests
    {
        [Test]
        public void ListDataSets_ShouldReturnListOfDataSets() => new ListDatasets().Invoke().Should().NotBeEmpty();
    }
}
