using System.Collections.Generic;

namespace Indico.RPAActivities.Entity
{
    public class Extraction
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public Dictionary<string, double> Confidence { get; set; }
    }
}