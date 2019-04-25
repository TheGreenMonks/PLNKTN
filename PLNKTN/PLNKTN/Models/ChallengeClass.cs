using System;
namespace PLNKTN.Models
{
    public class ChallengeClass
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Reward { get; set; }
        public string Status { get; set; }
       
        public ChallengeClass()
        {
        }
    }
}
