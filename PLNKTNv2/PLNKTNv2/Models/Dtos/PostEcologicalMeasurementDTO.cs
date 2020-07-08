using System;
using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models.Dtos
{
    public class PostEcologicalMeasurementDTO
    {
        [Required]
        public DateTime Date_taken { get; set; }

        [Required]
        public float? EcologicalFootprint { get; set; }

        [Required]
        public Transport Transport { get; set; }

        [Required]
        public Diet Diet { get; set; }

        [Required]
        public Electronics Electronics { get; set; }

        [Required]
        public Clothing Clothing { get; set; }

        [Required]
        public Footwear Footwear { get; set; }
    }
}