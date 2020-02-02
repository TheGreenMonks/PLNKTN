using PLNKTN.Models;
using System;
using System.Linq;

namespace PLNKTN.BusinessLogic
{
    public class RewardCalculation
    {
        internal static User CalculateUserRewardCompletion(User user, ref IEmailHelper emailHelper)
        {
            /* Algorithm
             * Foreach User check each Reward's Challenge status to see if the User has completed all Challenges.  If all Challenges in a
             * Reward are complete then set the status of the UserReward to complete and set the date of completion.
             * 
             * TODO - Efficiency Improvements -- 1. Only get the required fields from the db (i.e. eco measurments aren't needed)
             */

            /* TODO
             * This action will need to be paginated and parallelised as the max transfer size is 1MB, which will will be exceed quickly.
             * this functionality will enable multiple app threads to work on crunching the data via multiple 1MB requests
             * Reference -> https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_Scan.html
             * and -> https://stackoverflow.com/questions/48631715/retrieving-all-items-in-a-table-with-dynamodb
             * and -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html
             */

            //var strReward = "Reward";
            // Flag to indicate if changes to challenge status have been made
            var changesMade = false;

            // Iterate over all incomplete rewards.
            foreach (var _reward in user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
            {
                int incompleteChallenges = _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete).Count();

                // if all challenges are complete then set the reward status to complete
                if (incompleteChallenges == 0)
                {
                    _reward.DateCompleted = DateTime.UtcNow;
                    _reward.Status = UserRewardStatus.Complete;
                    _reward.NotificationStatus = NotificationStatus.Not_Notified;
                    changesMade = true;
                    emailHelper.AddEmailMessageLine(user.Id, "Reward", _reward.Id);
                }
            }

            if (changesMade)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
