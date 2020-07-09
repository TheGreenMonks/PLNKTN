using System.Collections.Generic;

namespace PLNKTNv2.Models
{
    public class UserGrantedReward
    {
        public int Count { get; set; } = 0;
        public List<UserGrantedRewardProject> Projects { set; get; }
        public string Region_name { set; get; }
    }
}