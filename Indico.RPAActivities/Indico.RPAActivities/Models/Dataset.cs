using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indico.RPAActivities.Models
{
    public class Dataset
    {
        public string id;
        public string name;
        public int numModelGroups;
        public int rowCount;
        public string status;
        public ModelGroup[] modelGroups;

        public Dataset()
        {
        }
    }
}
