using Indico.Entity;

namespace Indico.RPAActivities.Entity
{
    public class Dataset
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int NumModelGroups { get; set; }
        public int RowCount { get; set; }
        public string Status { get; set; }
        public ModelGroup[] ModelGroups { get; set; }
    }
}