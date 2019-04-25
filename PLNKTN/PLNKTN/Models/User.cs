using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Models
{
    public class User
    {
        public int Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public DateTime Created_at { get; set; }
        public string Level { get; set; }

        public User()
        {
        }

    }
}
