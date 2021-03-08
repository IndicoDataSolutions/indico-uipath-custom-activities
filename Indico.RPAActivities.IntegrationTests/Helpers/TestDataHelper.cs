using System.Linq;
using Indico.RPAActivities.Activities;

namespace Indico.RPAActivities.IntegrationTests.Helpers
{
    internal class TestDataHelper
    {
        public int GetDataSetId() => new ListDatasets().Invoke().First().Id;
    }
}
