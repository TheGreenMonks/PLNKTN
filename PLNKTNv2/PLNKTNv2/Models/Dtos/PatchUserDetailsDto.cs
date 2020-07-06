using System;

namespace PLNKTNv2.Models.Dtos
{
    public class PatchUserDetailsDto
    {
        public string First_name { get; set; }

        public string Last_name { get; set; }

        public string Email { get; set; }

        public string Level { get; set; }

        public int LivingSpace { get; set; }

        public int NumPeopleHousehold { get; set; }

        public float? CarMPG { get; set; }

        public bool ShareData { get; set; }

        public string Country { get; set; }
    }
}