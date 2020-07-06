using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.Models.Dtos
{
    public class CreateUserDetailsDTO
    {
        [Required]
        public string First_name { get; set; }

        [Required]
        public string Last_name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public int LivingSpace { get; set; }

        [Required]
        public int NumPeopleHousehold { get; set; }

        public float? CarMPG { get; set; }

        [Required]
        public bool ShareData { get; set; }

        [Required]
        public string Country { get; set; }
    }
}