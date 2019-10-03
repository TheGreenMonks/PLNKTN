using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class EcologicalMeasurement
    {
        public DateTime Date_taken { get; set; }
        public float? EcologicalFootprint { get; set; }
        public Transport Transport { get; set; }
        public Diet Diet { get; set; }
        public Electronics Electronics { get; set; }
        public Clothing Clothing { get; set; }
        public Footwear Footwear { get; set; }

    }
}
