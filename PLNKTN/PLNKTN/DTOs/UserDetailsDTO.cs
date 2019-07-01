using System;

namespace PLNKTN.DTOs
{
    public class UserDetailsDTO
    {
        public string Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public DateTime? Created_at { get; set; }
        public string Email { get; set; }
        public string Level { get; set; }
        public int? LivingSpace { get; set; }
        public int? NumPeopleHousehold { get; set; }
        public float? CarMPG { get; set; }
        public bool? ShareData { get; set; }
        //public float? EcologicalFootprint { get; set; }
        public float Collective_EF { get; set; }
        public string Country { get; set; }
    }
}
