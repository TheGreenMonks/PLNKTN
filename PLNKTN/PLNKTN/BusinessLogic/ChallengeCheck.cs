using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.BusinessLogic
{
    public class ChallengeCheck
    {
        private int i = 0;

        public void CheckChallengesCompleted(Object stateInfo)
        {
            Debug.WriteLine("CheckChallengesCompleted() called " + i++);
            //Debug.WriteLine(stateInfo.ToString());
        }
    }
}
