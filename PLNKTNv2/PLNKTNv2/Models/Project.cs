using System.Collections.Generic;

namespace PLNKTNv2.Models
{
    public class Project
    {
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public string Impact { get; set; }
        public string Project_name { set; get; }
        public string Tree_species { get; set; }
        public string url { get; set; }
    }
}