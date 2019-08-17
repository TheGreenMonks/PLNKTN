using PLNKTN.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.BusinessLogic
{
    public class ChallengeCheck
    {
        private readonly IUserRepository _userRepository;
        private int i = 0;

        //public ChallengeCheck(IUserRepository userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        public async void CheckChallengesCompleted(Object stateInfo)
        {
            /* Algorithm
             * Foreach User check each Challenege to see if User has already completed it.  If not then check if they have 
             * abstained from CATEGORY x and SUBCATEGORY y for TIME z (where TIME z is from DATETIME.NOW backwards).
             * If they have then assign CHALLENGE as COMPLETE.  Do this for every CHALLENGE over every USER.
             */

            // Get Users from DB

            /* TODO
             * This action will need to be paginated and parallelised as the max transfer size is 1MB, which will will exceed quickly.
             * this functionality will enable multiple app threads to work on crunching the data via multiple 1MB requests
             * Reference -> https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_Scan.html
             * and -> https://stackoverflow.com/questions/48631715/retrieving-all-items-in-a-table-with-dynamodb
             * and -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html
             */

            var _users = await _userRepository.GetAllUsers();

            Debug.WriteLine("CheckChallengesCompleted() called " + i++);
            //Debug.WriteLine(stateInfo.ToString());
        }
    }
}
