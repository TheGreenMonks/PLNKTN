using System;
using System.Collections.Generic;

namespace PLNKTN.Models
{
    public class Region
    {
        public string Region_name { set; get; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public string Tree_species { get; set; }
        public List<string> Images { get; set; }
    }
}
