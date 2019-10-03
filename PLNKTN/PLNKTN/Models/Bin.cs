using System;
using System.Collections.Generic;
namespace PLNKTN.Models
{
    public class Bin
    {
        public string Region_name { set; get; }
        public List<Rgn> Projects { set; get; }
        public int Count { get; set; } = 0;
    }
}
