using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PLNKTN.BusinessLogic
{
    public class ChallengeTimer
    {
        private ChallengeCheck checker;
        private Timer timer;

        public ChallengeTimer()
        {
            checker = new ChallengeCheck();
            timer = new Timer(checker.CheckChallengesCompleted, null, 0, 5000);
        }


    }
}
