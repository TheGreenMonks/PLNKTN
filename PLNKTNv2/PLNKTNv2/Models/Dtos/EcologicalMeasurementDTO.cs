using PLNKTNv2.Models;
using System;

namespace PLNKTNv2.Models.Dtos
{
    public class EcologicalMeasurementDTO
    {
        public string UserId { get; set; }
        public DateTime Date_taken { get; set; }
        public float? EcologicalFootprint { get; set; }
        public Transport Transport { get; set; }
        public Diet Diet { get; set; }
        public Electronics Electronics { get; set; }
        public Clothing Clothing { get; set; }
        public Footwear Footwear { get; set; }
    }
}