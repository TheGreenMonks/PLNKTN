using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.BusinessLogic
{
    public class UserRewards
    {
        //  Adds all reward and challenge data required by the user object to a 
        internal static ICollection<UserReward> GenerateUserRewards(ICollection<Reward> rewards)
        {
            ICollection<UserReward> generatedUserRewards = new List<UserReward>();

            foreach (var _reward in rewards)
            {
                var userRewardChallenges = new List<UserRewardChallenge>();

                foreach (var challenge in _reward.Challenges)
                {
                    userRewardChallenges.Add(new UserRewardChallenge
                    {
                        Id = challenge.Id,
                        DateCompleted = null,
                        Rule = new UserRewardChallengeRule
                        {
                            Category = challenge.Rule.Category,
                            RestrictionType = challenge.Rule.RestrictionType,
                            SubCategory = challenge.Rule.SubCategory,
                            Time = challenge.Rule.Time,
                            AmountToConsume = challenge.Rule.AmountToConsume
                        },
                        Status = UserRewardChallengeStatus.Incomplete,
                        NotificationStatus = NotificationStatus.Not_Complete
                    });
                }

                var userReward = new UserReward
                {
                    Id = _reward.Id,
                    Challenges = userRewardChallenges,
                    DateCompleted = null,
                    Status = UserRewardStatus.Incomplete,
                    NotificationStatus = NotificationStatus.Not_Complete,
                    IsRewardGranted = false
                };

                generatedUserRewards.Add(userReward);
            }

            return generatedUserRewards;
        }

        internal static UserReward GenerateUserReward(Reward reward)
        {
            var userRewardChallenges = new List<UserRewardChallenge>();

            foreach (var challenge in reward.Challenges)
            {
                userRewardChallenges.Add(new UserRewardChallenge
                {
                    Id = challenge.Id,
                    DateCompleted = null,
                    Rule = new UserRewardChallengeRule
                    {
                        Category = challenge.Rule.Category,
                        RestrictionType = challenge.Rule.RestrictionType,
                        SubCategory = challenge.Rule.SubCategory,
                        Time = challenge.Rule.Time,
                        AmountToConsume = challenge.Rule.AmountToConsume
                    },
                    Status = UserRewardChallengeStatus.Incomplete,
                    NotificationStatus = NotificationStatus.Not_Complete
                });
            }

            var userReward = new UserReward
            {
                Id = reward.Id,
                Challenges = userRewardChallenges,
                DateCompleted = null,
                Status = UserRewardStatus.Incomplete,
                NotificationStatus = NotificationStatus.Not_Complete,
                IsRewardGranted = false
            };

            return userReward;
        }

        //  Adds all reward and challenge data required by the user object to a 
        public static ICollection<UserReward> GenerateUpdateUserRewards(ICollection<Reward> rewards)
        {
            ICollection<UserReward> generatedUserRewards = new List<UserReward>();

            foreach (var _reward in rewards)
            {
                var userRewardChallenge = new List<UserRewardChallenge>();

                foreach (var challenge in _reward.Challenges)
                {
                    userRewardChallenge.Add(new UserRewardChallenge
                    {
                        Id = challenge.Id,
                        Rule = new UserRewardChallengeRule
                        {
                            Category = challenge.Rule.Category,
                            RestrictionType = challenge.Rule.RestrictionType,
                            SubCategory = challenge.Rule.SubCategory,
                            Time = challenge.Rule.Time,
                            AmountToConsume = challenge.Rule.AmountToConsume
                        },
                    });
                }

                var userReward = new UserReward
                {
                    Id = _reward.Id,
                    Challenges = userRewardChallenge,
                };

                generatedUserRewards.Add(userReward);
            }

            return generatedUserRewards;
        }

        //  Adds all reward and challenge data required by the user object to a 
        public static UserReward GenerateUpdatedUserReward(Reward reward)
        {

                var userRewardChallenge = new List<UserRewardChallenge>();

                foreach (var challenge in reward.Challenges)
                {
                    userRewardChallenge.Add(new UserRewardChallenge
                    {
                        Id = challenge.Id,
                        Rule = new UserRewardChallengeRule
                        {
                            Category = challenge.Rule.Category,
                            RestrictionType = challenge.Rule.RestrictionType,
                            SubCategory = challenge.Rule.SubCategory,
                            Time = challenge.Rule.Time,
                            AmountToConsume = challenge.Rule.AmountToConsume
                        },
                    });
                }

                var userReward = new UserReward
                {
                    Id = reward.Id,
                    Challenges = userRewardChallenge,
                };

            return userReward;
        }

        /* Add the 'userReward' to all 'users' in the DB.  This is used when a new 'Reward' is created.
         * 'Reward' - Refers to the information required by a user object in the DB in reference to
         * rewards and challenges.
         * TODO - Could use ref prefix for users param to prevent 2 user collections being held in memory.
         */
        internal static ICollection<User> AddUserRewardToAllUsers(UserReward reward, ICollection<User> users)
        {            
            foreach (var user in users)
            {
                UserReward dbReward = user.UserRewards.FirstOrDefault(r => r.Id == reward.Id);

                if (dbReward == null)
                {
                    user.UserRewards.Add(reward);
                }
            }

            return users;
        }

        internal static ICollection<User> UpdateUserRewardInAllUsers(UserReward reward, ICollection<User> users)
        {
            foreach (var user in users)
            {
                UserReward dbReward = user.UserRewards.FirstOrDefault(r => r.Id == reward.Id);

                if (dbReward != null)
                {
                    // Get each challenge that is currently stored in the DB so we can get some of its data
                    foreach (var challenge in dbReward.Challenges)
                    {
                        // Find the updated challenge that has been sent into this method
                        var newRewardChallenge = reward.Challenges.FirstOrDefault(c => c.Id == challenge.Id);
                        // Remove it from the updated reward, ready for changes to be made before re-insertion later
                        reward.Challenges.Remove(newRewardChallenge);

                        newRewardChallenge.DateCompleted = challenge.DateCompleted;
                        newRewardChallenge.NotificationStatus = challenge.NotificationStatus;
                        newRewardChallenge.Status = challenge.Status;
                        reward.Challenges.Add(newRewardChallenge);
                    }

                    // Update the reward information
                    reward.DateCompleted = dbReward.DateCompleted;
                    reward.NotificationStatus = dbReward.NotificationStatus;
                    reward.Status = dbReward.Status;
                    reward.IsRewardGranted = dbReward.IsRewardGranted;

                    // Remove the old reward entry and add the new one and save
                    user.UserRewards.Remove(dbReward);
                    user.UserRewards.Add(reward);
                }
                else
                {
                    user.UserRewards.Add(reward);
                }
            }

            return users;
        }

        /* Delete the specified 'userReward' in all 'users' in the DB.  This is used when a 'Reward' needs to be removed.
         * 'Reward' - Refers to the information required by a user object in the DB in reference to
         * rewards and challenges.
         * 
         */
        internal static ICollection<User> DeleteUserRewardFromAllUsers(string rewardId, ICollection<User> users)
        {
            foreach (var user in users)
            {
                UserReward dbUserReward = user.UserRewards.FirstOrDefault(r => r.Id == rewardId);

                if (dbUserReward != null)
                {
                    user.UserRewards.Remove(dbUserReward);
                }
            }
            return users;
        }
    }
}