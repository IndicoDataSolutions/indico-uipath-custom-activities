using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indico.RPAActivities.Models
{
    public class Extraction
    {
        public int start;
        public int end;
        public string label;
        public string text;
        public Dictionary<string, double> confidence;

        public Extraction(int start, int end, string label, string text, Dictionary<string, double> confidence)
        {
            this.start = start;
            this.end = end;
            this.label = label;
            this.text = text;
            this.confidence = confidence;
        }

        public override string ToString()
        {
            return "Prediction - ["+ label + "] : " + text + " (" + start + ", " + end + ")";
        }
    }
}
