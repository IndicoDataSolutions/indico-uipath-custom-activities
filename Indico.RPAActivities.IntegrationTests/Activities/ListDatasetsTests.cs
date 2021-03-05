using System;
using Indico.RPAActivities.Activities;
using Indico.RPAActivities.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Indico.RPAActivities.IntegrationTests.Activities
{
    public class ListDatasetsTests
    {
        [Test]
        public void ListDataSets_ShouldReturnListOfDataSets()
        {
            var activity = new ListDatasets();
            var result = activity.Invoke();
            
            throw new NotImplementedException();
        }
    }
}
