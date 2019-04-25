using System;
namespace PLNKTN.Models
{
    public class RewardClass
    {
        public int Id { get; set; }
        public int Challenge_id { get; set; }
        public string Country { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Sponsor { get; set; }

        public RewardClass()
        {
        }
    }
}
