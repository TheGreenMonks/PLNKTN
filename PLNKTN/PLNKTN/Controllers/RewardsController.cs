using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.Models;
using PLNKTN.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardRepository _rewardRepository;
        private readonly IUserRepository _userRepository;
        private readonly string strReward = "Reward";
        private readonly string strChallenge = "Challenge";

        public RewardsController(IRewardRepository rewardRepository, IUserRepository userRepository)
        {
            _rewardRepository = rewardRepository;
            _userRepository = userRepository;
        }

        #region Calculate Reward & Challenge Completion
        [AcceptVerbs("CalcRewards")]
        public async void CalculateUserRewardCompletion()
        {
            /* Algorithm
             * Foreach User check each Reward's Challenge status to see if the User has completed all Challenges.  If all Challenges in a
             * Reward are complete then set the status of the UserReward to complete and set the date of completion.
             * 
             * TODO - Efficiency Improvements -- 1. Only get the required fields from the db (i.e. eco measurments aren't needed)
             */

            // Get Users from DB

            /* TODO
             * This action will need to be paginated and parallelised as the max transfer size is 1MB, which will will be exceed quickly.
             * this functionality will enable multiple app threads to work on crunching the data via multiple 1MB requests
             * Reference -> https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_Scan.html
             * and -> https://stackoverflow.com/questions/48631715/retrieving-all-items-in-a-table-with-dynamodb
             * and -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html
             */

            // Get all users from the DB who have NOT NULL Reward arrays
            var dbUsers = await _userRepository.GetAllUsers();
            // New List to hold all email messages generated
            ICollection<string> emailMessages = new List<string>();

            // Iterate over each user that has rewards in their DB record.
            foreach (var _user in dbUsers.Where(u => u.UserRewards != null))
            {
                // Flag to indicate if changes to challenge status have been made
                var changesMade = false;

                // Iterate over all incomplete rewards.
                foreach (var _reward in _user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
                {
                    var incompleteChallenges = _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete).ToList();

                    // if all challenges are complete then set the reward status to complete
                    if (incompleteChallenges.Count() == 0)
                    {
                        _reward.DateCompleted = DateTime.UtcNow;
                        _reward.Status = UserRewardStatus.Complete;
                        _reward.NotificationStatus = NotificationStatus.Not_Notified;
                        changesMade = true;
                        emailMessages.Add(EmailHelper.EmailMessage(_user.Id, strReward, _reward.Id));
                    }
                }

                // Save changes to DB (removes eco emasurements as they don't all need to be sent back to the server.
                if (changesMade)
                {
                    _user.EcologicalMeasurements = null;
                    await _userRepository.UpdateUser(_user);
                }
            }

            // If there are no item completions this time then confirm this in the email with a message.
            if (emailMessages.Count == 0)
            {
                emailMessages.Add("No " + strReward + "s completed this time...\n");
            }

            EmailHelper.SendEmail(emailMessages, strReward);
        }

        [AcceptVerbs("CalcChallenges")]
        public async void CalculateUserChallengeCompletion()
        {
            /* Algorithm
             * Foreach User check each Reward and Challenge to see if User has already completed it.  If not then check if they have 
             * abstained OR only utilised CATEGORY x and SUBCATEGORY y for TIME z (where TIME z is from DATETIME.NOW backwards).
             * If they have then assign CHALLENGE as COMPLETE.  Do this for every CHALLENGE over every USER.
             * 
             * TODO - Efficiency Improvements -- 1. Only get the required amount of eco measurements based on what the
             * longest challenge is, i.e. if the longest challenge is 26 weeks then only get 26 weeks of measurements.
             * This will save data
             */

            // Get Users from DB

            /* TODO
             * This action will need to be paginated and parallelised as the max transfer size is 1MB, which will will exceed quickly.
             * this functionality will enable multiple app threads to work on crunching the data via multiple 1MB requests
             * Reference -> https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_Scan.html
             * and -> https://stackoverflow.com/questions/48631715/retrieving-all-items-in-a-table-with-dynamodb
             * and -> https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html
             */

            // Get all users from the DB who have NOT NULL Reward arrays
            var dbUsers = await _userRepository.GetAllUsers();

            // New List to hold all email messages generated
            ICollection<string> emailMessages = new List<string>();

            // Iterate over each user that has rewards in their DB record.
            foreach (var _user in dbUsers.Where(u => u.UserRewards != null))
            {
                // Flag to indicate if changes to challenge status have been made
                var changesMade = false;
                // Sort eco measurements by date for easier processing in all challenge rule checks.
                _user.EcologicalMeasurements = _user.EcologicalMeasurements.OrderBy(e => e.Date_taken.Date).ToList();

                // Iterate over all incomplete rewards.
                foreach (var _reward in _user.UserRewards.Where(ur => ur.Status != UserRewardStatus.Complete))
                {

                    // For each challenge in this reward that is not complete
                    foreach (var _challenge in _reward.Challenges.Where(c => c.Status != UserRewardChallengeStatus.Complete))
                    {
                        // Get rule information in local vars (because easier to user later)
                        var _category = _challenge.Rule.Category;
                        var _subCategory = _challenge.Rule.SubCategory;
                        var _time = _challenge.Rule.Time;
                        var _restrictionType = _challenge.Rule.RestrictionType;
                        // Flag to assess success of user against challenge rule.
                        var isSuccessful = false;

                        // Determine how many eco measurements will be required to successfully complete the challenge
                        // and an index start position for the for loop later.
                        var numOfEcoMeasurements = _user.EcologicalMeasurements.Count;
                        var offset = 0;

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
                        // Sets where the enumerator should start in the list of eco measurements
                        var indexStart = numOfEcoMeasurements - offset;

                        /*
                         * Check if there has been any activity in the specified category in the specified amount of time so that
                         * we can check early if the user is activly tracking this category while using PLNKTN.
                         */
                        var _activityDetected = false;
                        for (int i = indexStart; i < numOfEcoMeasurements; i++)
                        {
                            var ecoMeasureTemp = _user.EcologicalMeasurements.ElementAt(i);
                            // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                            // based on the text values held in the Challenge list entry.
                            var _itemsCategory = ecoMeasureTemp.GetType().GetProperty(_category).GetValue(ecoMeasureTemp);

                            // Create a dictionary to hold the list of property names and their values
                            //var properties = new Dictionary<string, int>();
                            // Iterate over the list of sub categories and add their property names and values to dict.
                            foreach (var _tempProps in _itemsCategory.GetType().GetProperties())
                            {
                                if ((int)_tempProps.GetValue(_itemsCategory) > 0)
                                {
                                    _activityDetected = true;
                                    break;
                                }
                                //properties.Add(_tempProps.Name, (int)_tempProps.GetValue(_itemsCategory));
                            }

                            if (_activityDetected)
                            {
                                break;
                            }
                        }

                        // If the rule restriction is SKIP, i.e. do not eat beef for 1 week AND
                        // If there are not enough eco measurements in dbUsers Db entry indexStart will be < 0 and it is impossible
                        // for them to successfully complete this task, therefore break.
                        if (_restrictionType == ChallengeType.Skip && indexStart >= 0 && _activityDetected)
                        {
                            // A counter to count the number of times an item has been skipped
                            var skippedEnoughTimes = 0;

                            for (int i = indexStart; i < numOfEcoMeasurements; i++)
                            {
                                var ecoMeasureTemp = _user.EcologicalMeasurements.ElementAt(i);

                                // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                // based on the text values held in the Challenge list entry.
                                var _skipItemsCategory = ecoMeasureTemp.GetType().GetProperty(_category).GetValue(ecoMeasureTemp);
                                var _skipItem = _skipItemsCategory.GetType().GetProperty(_subCategory).GetValue(_skipItemsCategory);

                                // Convert the retrieved value to an int.
                                var _skipItemAsInt = Convert.ToInt32(_skipItem);

                                // Check if the user has skipped the item in this EF and increment if they have.
                                if (_skipItemAsInt == 0)
                                {
                                    skippedEnoughTimes++;
                                }

                                // Checks if the challenge is to skip 1, 2 or 3 times in a week and if the user has skipped the item enough times
                                // to be successful yet.  Else if it is just a week or more check if they have skipped for the whole time.
                                if (_time <= 3 && skippedEnoughTimes == _time)
                                {
                                    isSuccessful = true;
                                    break;
                                }
                                else if (skippedEnoughTimes == _time)
                                {
                                    isSuccessful = true;
                                    break;
                                }

                            }
                        }
                        else if (_restrictionType == ChallengeType.Only_This && indexStart >= 0 && _activityDetected)
                        {
                            // A counter to count the number of times an item has been skipped
                            var skippedEnoughTimes = 0;

                            for (int i = indexStart; i < numOfEcoMeasurements; i++)
                            {
                                var ecoMeasureTemp = _user.EcologicalMeasurements.ElementAt(i);

                                // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                // based on the text values held in the Challenge list entry.
                                var _onlyThisItemsCategory = ecoMeasureTemp.GetType().GetProperty(_category).GetValue(ecoMeasureTemp);
                                var _onlyThisItem = _onlyThisItemsCategory.GetType().GetProperty(_subCategory).GetValue(_onlyThisItemsCategory);
                                // Count for other subcategories used and used in calculation of challenge success
                                var _countOtherSubCategoriesUsed = 0;

                                // Create a dictionary to hold the list of property names and their values
                                var properties = new Dictionary<string, int>();

                                // Iterate over the list of sub categories and add their property names and values to dict.
                                foreach (var _tempProps in _onlyThisItemsCategory.GetType().GetProperties())
                                {
                                    properties.Add(_tempProps.Name, (int)_tempProps.GetValue(_onlyThisItemsCategory));
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

                                // Convert the retrieved value to an int.
                                var _onlyThisItemAsInt = Convert.ToInt32(_onlyThisItem);

                                // Check if the user has only used the item in this EF and increment if they have.
                                if (_onlyThisItemAsInt > 0 && _countOtherSubCategoriesUsed == 0)
                                {
                                    skippedEnoughTimes++;
                                }

                                // Checks if the challenge is to only use a specific item for 1, 2 or 3 times in a week and if the user has only used the item enough times
                                // to be successful yet.  Else if it is just a week or more check if they have only used the item for the whole time.
                                if (_time <= 3 && skippedEnoughTimes == _time)
                                {
                                    isSuccessful = true;
                                    break;
                                }
                                else if (skippedEnoughTimes == _time)
                                {
                                    isSuccessful = true;
                                    break;
                                }
                            }
                        }

                        // Change the challenge status if user is successful.
                        if (isSuccessful)
                        {
                            _challenge.Status = UserRewardChallengeStatus.Complete;
                            _challenge.NotificationStatus = NotificationStatus.Not_Notified;
                            _challenge.DateCompleted = DateTime.UtcNow;
                            changesMade = true;
                            emailMessages.Add(EmailHelper.EmailMessage(_user.Id, strChallenge, _challenge.Id));
                        }

                    }
                }

                // Save changes to DB (removes eco emasurements as they don't all need to be sent back to the server.
                if (changesMade)
                {
                    _user.EcologicalMeasurements = null;
                    await _userRepository.UpdateUser(_user);
                }

            }

            // If there are no item completions this time then confirm this in the email with a message.
            if (emailMessages.Count == 0)
            {
                emailMessages.Add("No " + strChallenge + "s completed this time...\n");
            }

            EmailHelper.SendEmail(emailMessages, strChallenge);
        }
        #endregion


        // GET: api/Rewards
        [HttpGet]
        public string Get()
        {
            return "Not Implemented";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Not Implemented";
        }

        // POST api/Rewards
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Reward reward)
        {
            if (reward == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Reward information formatted incorrectly.");
            }

            var user = new Reward()
            {
                Id = reward.Id,
                Title = reward.Title,
                ImageURL = reward.ImageURL,
                Description = reward.Description,
                Link = reward.Link,
                GridPosition = reward.GridPosition,
                Text_When_Completed = reward.Text_When_Completed,
                Text_When_Not_Completed = reward.Text_When_Not_Completed,
                Source = reward.Source,
                Challenges = reward.Challenges,
                Country = reward.Country,
                Overview = reward.Overview,
                Impact = reward.Impact,
                Tree_species = reward.Tree_species
            };

            var result = await _rewardRepository.CreateReward(reward);

            // Generate a 'user reward' and add it to all users in the DB.
            var rewards = new List<Reward>();
            rewards.Add(reward);
            var userReward = UsersController.GenerateUserRewards(rewards);
            await _userRepository.AddUserRewardToAllUsers(userReward.First());

            if (result == 1)
            {
                // return HTTP 201 Created with reward object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = reward.Id }, reward);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as reward already exists in DB
                return Conflict("Reward with ID '" + reward.Id + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // PUT api/Rewards
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Reward reward)
        {
            if (reward == null)
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Reward information formatted incorrectly.");
            }

            var user = new Reward()
            {
                Id = reward.Id,
                Title = reward.Title,
                ImageURL = reward.ImageURL,
                Description = reward.Description,
                Link = reward.Link,
                GridPosition = reward.GridPosition,
                Text_When_Completed = reward.Text_When_Completed,
                Text_When_Not_Completed = reward.Text_When_Not_Completed,
                Source = reward.Source,
                Challenges = reward.Challenges,
                Country = reward.Country,
                Overview = reward.Overview,
                Impact = reward.Impact,
                Tree_species = reward.Tree_species
            };

            var result = await _rewardRepository.UpdateReward(reward);

            if (result == 1)
            {
                // return HTTP 201 Created with reward object in body of return and a 'location' header with URL of newly created object
                return CreatedAtAction("Get", new { id = reward.Id }, reward);
            }
            else if (result == -10)
            {
                // return HTTP 409 Conflict as reward already exists in DB
                return Conflict("Reward with ID '" + reward.Id + "' already exists.  Cannot create a duplicate.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // DELETE api/Rewards
        [HttpDelete]
        public void Delete()
        {
        }
    }
}
