using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace PLNKTN.Models
{
    public class CollectiveEF
    {
        internal float collective_ef;

        public DateTime Date_taken { get; set; }
        public float Collective_EF { get; set; }
    }
}
