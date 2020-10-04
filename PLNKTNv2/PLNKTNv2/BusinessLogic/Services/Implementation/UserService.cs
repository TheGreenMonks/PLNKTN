using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PLNKTNv2.BusinessLogic.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IMessenger _messenger;

        public UserService(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void AddUserRewardToAllUsers(Reward reward, IEnumerable<User> users)
        {
            UserReward userReward = GenerateUserReward(reward);

            foreach (var user in users)
            {
                // TODO: This should be a dictionary as the kvp will allow for more efficient processing
                bool userRewardExists = user.UserRewards.Any(r => r.Id == userReward.Id);

                if (!userRewardExists)
                {
                    user.UserRewards.Add(userReward);
                    /* Set large data lists to null as these aren't updated here, thus sending them
                    back to the DB is a waste of BW and DyDB processing time.*/
                    user.EcologicalMeasurements = null;
                    user.GrantedRewards = null;
                }
            }
        }

        public User CreateUser(ICollection<Reward> rewards, CreateUserDetailsDTO userDto, string id)
        {
            // Validation
            _ = String.IsNullOrEmpty(id) ? throw new ArgumentException("The UserId is null or empty", "id") : false;
            ICollection<UserReward> userRewards = GenerateUserRewards(rewards);

            var user = new User()
            {
                Id = id,
                First_name = userDto.First_name,
                Last_name = userDto.Last_name,
                Created_at = DateTime.UtcNow,
                Email = userDto.Email,
                Level = userDto.Level,
                EcologicalMeasurements = new List<EcologicalMeasurement>(),
                LivingSpace = userDto.LivingSpace,
                NumPeopleHousehold = userDto.NumPeopleHousehold,
                CarMPG = userDto.CarMPG,
                ShareData = userDto.ShareData,
                Country = userDto.Country,
                UserRewards = (List<UserReward>)userRewards,
                GrantedRewards = new List<UserGrantedReward>()
            };

            return user;
        }

        public void DeleteUserRewardFromAllUsers(string rewardId, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                user.UserRewards.RemoveAll(ur => ur.Id == rewardId);
            }
        }

        public void UpdateUserRewards(User user, ICollection<UserRewardDto> userRewardDtos)
        {
            foreach (var userRewardDto in userRewardDtos)
            {
                UserReward _currentUserReward = user.UserRewards.Find(ur => ur.Id == userRewardDto.Id);

                if (userRewardDto.Challenges != null)
                {
                    foreach (var challenge in userRewardDto.Challenges)
                    {
                        UserRewardChallenge _currentUserRewardChallenge = _currentUserReward.Challenges.Find(urc => urc.Id == challenge.Id);
                        _currentUserRewardChallenge.NotificationStatus = (NotificationStatus)challenge.NotificationStatus;
                    }
                }

                if (userRewardDto.NotificationStatus != null)
                {
                    _currentUserReward.NotificationStatus = (NotificationStatus) userRewardDto.NotificationStatus;
                }
                if (userRewardDto.IsRewardGranted != null)
                {
                    _currentUserReward.IsRewardGranted = (bool) userRewardDto.IsRewardGranted;
                }
            }
        }

        public void UpdateUserRewardInAllUsers(Reward reward, IEnumerable<User> users)
        {
            // Make the user reward information ready for sending to the repository
            var userReward = GenerateUpdatedUserReward(reward);

            foreach (var user in users)
            {
                // TODO: This should be a dictionary as the kvp will allow for more efficient processing
                UserReward dbReward = user.UserRewards.First(r => r.Id == userReward.Id);

                /* We have to check each challenge in user reward to ensure that user unique data
                * (such as dates completed and challenge completion status) are transferred over correctly
                * once all of the generic user reward data has been moved over.*/
                foreach (var challenge in dbReward.Challenges)
                {
                    // Find the updated challenge that has been sent into this method
                    var newRewardChallenge = userReward.Challenges.First(c => c.Id == challenge.Id);
                    // Remove it from the updated reward, ready for changes to be made before re-insertion later
                    userReward.Challenges.Remove(newRewardChallenge);

                    newRewardChallenge.DateCompleted = challenge.DateCompleted;
                    newRewardChallenge.NotificationStatus = challenge.NotificationStatus;
                    newRewardChallenge.Status = challenge.Status;
                    userReward.Challenges.Add(newRewardChallenge);
                }

                // The same of the reward overall must be done as for the challenges above.
                userReward.DateCompleted = dbReward.DateCompleted;
                userReward.NotificationStatus = dbReward.NotificationStatus;
                userReward.Status = dbReward.Status;
                userReward.IsRewardGranted = dbReward.IsRewardGranted;

                user.UserRewards.Remove(dbReward);
                user.UserRewards.Add(userReward);
            }
        }

        public void CalculateUserRewardCompletion(List<User> users)
        {
            users.RemoveAll(u => u.Id == "UserCount" || u.Id == "AppTotalTreesPlanted");

            foreach (var user in users)
            {
                CalculateUserChallengeCompletion(user, _messenger);
                CalculateUserRewardCompletion(user, _messenger);
            }

            _messenger.Send("Both Reward and Challenge Completion Calculation Controller Methods");
        }

        public void CalculateMyRewardCompletion(User user)
        {
            CalculateUserChallengeCompletion(user, _messenger);
            CalculateUserRewardCompletion(user, _messenger);

            _messenger.Send("Both Reward and Challenge Completion Calculation Controller Methods");
        }

        private void CalculateUserRewardCompletion(User user, IMessenger messenger)
        {
            /* Algorithm
             * For the given User check each Reward's Challenge status to see if the User has completed all Challenges.  If all Challenges in a
             * Reward are complete then set the status of the UserReward to complete and set the date of completion.
             */

            foreach (var _reward in user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
            {
                int incompleteChallenges = _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete).Count();

                // if all challenges are complete then set the reward status to complete
                if (incompleteChallenges == 0)
                {
                    _reward.DateCompleted = DateTime.UtcNow;
                    _reward.Status = UserRewardStatus.Complete;
                    _reward.NotificationStatus = NotificationStatus.Not_Notified;
                    messenger.AddLine(user.Id, "Reward", _reward.Id);
                }
            }
        }

        private bool EcoMeasurementDietHasActivity(Diet dietCategory)
        {
            if (dietCategory.Beef > 0)
                return true;
            else if (dietCategory.Dairy > 0)
                return true;
            else if (dietCategory.Egg > 0)
                return true;
            else if (dietCategory.Plant_based > 0)
                return true;
            else if (dietCategory.Pork > 0)
                return true;
            else if (dietCategory.Poultry > 0)
                return true;
            else if (dietCategory.Seafood > 0)
                return true;
            else
                return false;
        }

        private void TransportChallengeComplete(UserRewardChallenge challenge, IList<EcologicalMeasurement> ecologicalMeasurements, string userId, IMessenger messenger)
        {
            var _restrictionType = challenge.Rule.RestrictionType;
            int? amountToConsume = challenge.Rule.AmountToConsume;
            var _subCategory = challenge.Rule.SubCategory;

            ChallengeProgress challengeProgress = ChallengeProgress.No_Data;
            int amountComplete = 0;

            if (_restrictionType == ChallengeType.Any)
            {
                int itemTotal = 0;

                foreach (var ecoMeasurement in ecologicalMeasurements)
                {
                    // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                    // based on the text values held in the Challenge list entry.
                    var itemCategory = ecoMeasurement.Transport;
                    var item = itemCategory.GetType().GetProperty(_subCategory).GetValue(itemCategory);

                    itemTotal += (int)item;

                    challenge.AmountCompleted = itemTotal;
                }

                if (itemTotal >= amountToConsume)
                {
                    challengeProgress = ChallengeProgress.Complete;
                    amountComplete = itemTotal;
                }
                else
                {
                    challengeProgress = ChallengeProgress.Partial;
                    amountComplete = itemTotal;
                }
            }

            // Change the challenge status if user is successful.
            if (challengeProgress == ChallengeProgress.Complete)
            {
                challenge.Status = UserRewardChallengeStatus.Complete;
                challenge.NotificationStatus = NotificationStatus.Not_Notified;
                challenge.DateCompleted = DateTime.UtcNow;
                challenge.AmountCompleted = amountComplete;
                messenger.AddLine(userId, "Challenge", challenge.Id);
            }
            else
            {
                challenge.AmountCompleted = amountComplete;
            }
        }

        private void DietChallengeComplete(UserRewardChallenge challenge, IList<EcologicalMeasurement> ecologicalMeasurement, string userId, IMessenger messenger)
        {
            // Determine how many eco measurements will be required to successfully complete the challenge
            // and an index start position for the for loop later.
            var numOfEcoMeasurements = ecologicalMeasurement.Count;
            var offset = 0;

            var _category = challenge.Rule.Category;
            var _subCategory = challenge.Rule.SubCategory;
            var _time = challenge.Rule.Time;
            var _restrictionType = challenge.Rule.RestrictionType;
            int? amountToConsume = challenge.Rule.AmountToConsume;

            ChallengeProgress challengeProgress = ChallengeProgress.No_Data;
            int amountComplete = 0;

            // This sets the offset (the number of eco measurements to look at) based on how long the user
            // is to go without/only use a specific item.
            if (_time <= 7)
            {
                offset = 7;
            }
            else if (_time > 7 && _time <= 14)
            {
                offset = 14;
            }
            else if (_time > 14 && _time <= 30)
            {
                offset = 30;
            }
            else if (_time > 30 && _time <= 60)
            {
                offset = 60;
            }
            else if (_time > 60 && _time <= 90)
            {
                offset = 90;
            }
            // Sets where the enumerator should start in the list of eco measurements
            var indexStart = numOfEcoMeasurements - offset;

            /*
             * Check to see if the user has x (_time) days of Ecological Measurements from today back however many days are specified
             * by variable 'offset' (this provides the date range) for the challenge to be assessed against.
             * This works becasue the _user.EcologicalMeasurements list is ordered earlier in this method.
             */
            var challengeStart = DateTime.UtcNow.AddDays(-offset).Date;
            var ecoMeasurementsOfInterest = ecologicalMeasurement.Where(e => e.Date_taken.Date >= challengeStart.Date &&
                                                                                e.Date_taken.Date <= DateTime.UtcNow.Date).ToList();

            // Check if the rule restriction is SKIP, i.e. do not eat beef for 1 week AND
            // check there are enough eco measurements in the given date range to successfully complete the challenge
            if (_restrictionType == ChallengeType.Skip)
            {
                // A counter to count the number of times an item has been skipped
                int skippedEnoughTimes = 0;

                foreach (var ecoMeasurement in ecoMeasurementsOfInterest)
                {
                    // Check if the user has logged activity for this EM in the specific category
                    if (EcoMeasurementDietHasActivity(ecoMeasurement.Diet))
                    {
                        // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                        // based on the text values held in the Challenge list entry.
                        var _skipItemsCategory = ecoMeasurement.Diet;
                        var _skipItem = _skipItemsCategory.GetType().GetProperty(_subCategory).GetValue(_skipItemsCategory);

                        // Convert the retrieved value to a float.
                        var _skipItemAsFloat = Convert.ToSingle(_skipItem);

                        // Check if the user has skipped the item in this EF and increment if they have.
                        if (_skipItemAsFloat == 0)
                        {
                            skippedEnoughTimes++;
                        }
                    }
                }

                // Checks if challenge item has been skipped enough times to complete the challenge
                if (skippedEnoughTimes >= _time)
                {
                    challengeProgress = ChallengeProgress.Complete;
                    amountComplete = skippedEnoughTimes;
                }
                else
                {
                    challengeProgress = ChallengeProgress.Partial;
                    amountComplete = skippedEnoughTimes;
                }
            }
            else if (_restrictionType == ChallengeType.Only_This)
            {
                // A counter to count the number of times an item has been discretely used
                var onlyUsedEnoughTimes = 0;

                foreach (var ecoMeasurement in ecoMeasurementsOfInterest)
                {
                    // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                    // based on the text values held in the Challenge list entry.
                    var _onlyThisItemsCategory = ecoMeasurement.Diet;
                    var _onlyThisItem = _onlyThisItemsCategory.GetType().GetProperty(_subCategory).GetValue(_onlyThisItemsCategory);
                    // Count for other subcategories used and utilsied in calculation of challenge success
                    var _countOtherSubCategoriesUsed = 0;

                    // Create a dictionary to hold the list of property names and their values
                    var properties = new Dictionary<string, float>();

                    // Iterate over the list of sub categories and add their property names and values to dict.
                    foreach (var _tempProps in _onlyThisItemsCategory.GetType().GetProperties())
                    {
                        properties.Add(_tempProps.Name, (float)_tempProps.GetValue(_onlyThisItemsCategory));
                    }

                    // Remove the subcategory that is only to be used in this challenge so it doesn't false flag and fail the challenge.
                    properties.Remove(_subCategory);

                    // Iterate over the properties and see if they have been used in this eco measurement.
                    foreach (var item in properties)
                    {
                        if (item.Value > 0)
                        {
                            _countOtherSubCategoriesUsed++;
                        }
                    }

                    // Convert the retrieved value to a float.
                    var _onlyThisItemAsFloat = Convert.ToSingle(_onlyThisItem);

                    // Check if the user has only used the 'only_use' item in this EF and increment if they have.
                    if (_onlyThisItemAsFloat > 0 && _countOtherSubCategoriesUsed == 0)
                    {
                        onlyUsedEnoughTimes++;
                    }
                }

                // Checks if challenge item has been discretely used enough times to complete the challenge
                if (onlyUsedEnoughTimes >= _time)
                {
                    challengeProgress = ChallengeProgress.Complete;
                    amountComplete = onlyUsedEnoughTimes;
                }
                else
                {
                    challengeProgress = ChallengeProgress.Partial;
                    amountComplete = onlyUsedEnoughTimes;
                }
            }

            // Change the challenge status if user is successful.
            if (challengeProgress == ChallengeProgress.Complete)
            {
                challenge.Status = UserRewardChallengeStatus.Complete;
                challenge.NotificationStatus = NotificationStatus.Not_Notified;
                challenge.DateCompleted = DateTime.UtcNow;
                challenge.AmountCompleted = amountComplete;
                messenger.AddLine(userId, "Challenge", challenge.Id);
            }
            else
            {
                challenge.AmountCompleted = amountComplete;
            }
        }

        private void CalculateUserChallengeCompletion(User user, IMessenger messenger)
        {
            /* Algorithm
             * Foreach User check each Reward and Challenge to see if User has already completed it.  If not then check if they have
             * abstained OR only utilised CATEGORY x and SUBCATEGORY y for TIME z (where TIME z is from DATETIME.NOW backwards).
             * If they have then assign CHALLENGE as COMPLETE.  Do this for every CHALLENGE over every USER.
             */

            /* TODO
             * As Diet is the only real category being tested all calculations are with floats but this will need to be changed when
             * other categories are included as they have properties of different types in them...
             */

            // Sort eco measurements by date for easier processing in all challenge rule checks.
            user.EcologicalMeasurements = user.EcologicalMeasurements.OrderBy(e => e.Date_taken.Date).ToList();

            // Iterate over all incomplete rewards.
            foreach (var _reward in user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
            {
                // For each challenge in this reward that is not complete
                foreach (var _challenge in _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete))
                {
                    if (String.Equals(_challenge.Rule.Category, "Diet"))
                    {
                        DietChallengeComplete(_challenge, user.EcologicalMeasurements, user.Id, messenger);
                    }
                    else if (String.Equals(_challenge.Rule.Category, "Transport"))
                    {
                        TransportChallengeComplete(_challenge, user.EcologicalMeasurements, user.Id, messenger);
                    }
                }
            }
        }

        //  Adds all reward and challenge data required by the user object to a
        private ICollection<UserReward> GenerateUpdateUserRewards(ICollection<Reward> rewards)
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

        private UserReward GenerateUpdatedUserReward(Reward reward)
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

        //  Adds all reward and challenge data required by the user object to a
        private ICollection<UserReward> GenerateUserRewards(ICollection<Reward> rewards)
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
                    Challenges = userRewardChallenge,
                    DateCompleted = null,
                    Status = UserRewardStatus.Incomplete,
                    NotificationStatus = NotificationStatus.Not_Complete,
                    IsRewardGranted = false
                };

                generatedUserRewards.Add(userReward);
            }

            return generatedUserRewards;
        }

        private UserReward GenerateUserReward(Reward reward)
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
    }
}