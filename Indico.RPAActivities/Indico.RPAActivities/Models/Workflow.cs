using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Indico.RPAActivities.Models
{
    public class Workflow
    {
        public int id;
        public string name;

        public Workflow(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public override string ToString()
        {
            return $"{this.name} ({this.id})";
        }
    }
}
