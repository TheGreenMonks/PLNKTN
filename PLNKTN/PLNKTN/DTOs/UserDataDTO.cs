using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.DTOs
{
    public class UserDataDTO
    {
        public TransportDTO Transport { get; set; }
        public DietDTO Diet { get; set; }
        public ElectronicsDTO Electronics { get; set; }
        public ClothingDTO Clothing { get; set; }
        public FootwearDTO Footwear { get; set; }
        public int LivingSpace { get; set; }
        public int NumPeopleHousehold { get; set; }
        public float CarMPG { get; set; }
        public bool ShareData { get; set; }
        public float EcologicalFootprint { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "This email address is not the correct format.")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }

    }
}
