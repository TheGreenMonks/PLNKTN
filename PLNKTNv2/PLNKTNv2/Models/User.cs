using System;
using System.Collections.Generic;

namespace PLNKTNv2.Models
{
    public class User : UsersTableEntry
    {
        public string First_name { get; set; }

        public string Last_name { get; set; }

        public DateTime? Created_at { get; set; }

        public string Email { get; set; }

        public string Level { get; set; }

        public List<EcologicalMeasurement> EcologicalMeasurements { get; set; }

        public int? LivingSpace { get; set; }

        public int? NumPeopleHousehold { get; set; }

        public float? CarMPG { get; set; }

        public bool? ShareData { get; set; }

        public string Country { get; set; } = null;

        public List<UserReward> UserRewards { get; set; }

        public List<UserGrantedReward> GrantedRewards { get; set; }

        public User()
        {
        }

    }
}
