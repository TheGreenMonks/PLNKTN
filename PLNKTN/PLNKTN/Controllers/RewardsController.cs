﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.Models;
using PLNKTN.Repositories;

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardRepository _rewardRepository;
        private readonly IUserRepository _userRepository;

        public RewardsController(IRewardRepository rewardRepository, IUserRepository userRepository)
        {
            _rewardRepository = rewardRepository;
            _userRepository = userRepository;
        }

        #region Calculate Reward Completion

        [AcceptVerbs("CalcRewards")]
        public async Task<IActionResult> CalculateUserRewardCompletion()
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
            
            var strReward = "Reward";
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

            return Ok("All User Reward status' re-calculated.");
        }

        #endregion

        #region Calculate Challenge Completion

        [AcceptVerbs("CalcChallenges")]
        public async Task<IActionResult> CalculateUserChallengeCompletion()
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

            /* TODO
             * As Diet is he only real category being tested all calculations are with floats bu tthis will need to be changed when
             * other categories are included as they have properties of different types in them...
             */
            
            var strChallenge = "Challenge";
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
                        int amountToConsume = _challenge.Rule.AmountToConsume;
                        // Flag to assess success of user against challenge rule.
                        var isSuccessful = false;

                        if (String.Equals(_category, "Diet"))
                        {
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
                             * Check to see if the user has x (_time) days of Ecological Measurements from today back however many days are specified
                             * by variable 'offset' (this provides the date range) for the challenge to be assessed against.
                             * This works becasue the _user.EcologicalMeasurements list is ordered earlier in this method.
                             */
                            var _fullDateRange = false;
                            var challengeStart = DateTime.UtcNow.AddDays(-offset);
                            var ecoMeasurementsOfInterest = _user.EcologicalMeasurements.Where(e => e.Date_taken.Date >= challengeStart.Date).ToList();
                            if (ecoMeasurementsOfInterest.Count >= _time)
                                _fullDateRange = true;

                            // Check if the rule restriction is SKIP, i.e. do not eat beef for 1 week AND
                            // check there are enough eco measurements in the given date range to successfully complete the challenge
                            if (_restrictionType == ChallengeType.Skip && _fullDateRange)
                            {
                                // A counter to count the number of times an item has been skipped
                                var skippedEnoughTimes = 0;

                                foreach (var ecoMeasurement in ecoMeasurementsOfInterest)
                                {
                                    // Check if the user has logged activity for this EM in the specific category
                                    if (EcoMeasurementHasActivity(_category, ecoMeasurement))
                                    {
                                        // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                        // based on the text values held in the Challenge list entry.
                                        var _skipItemsCategory = ecoMeasurement.GetType().GetProperty(_category).GetValue(ecoMeasurement);
                                        var _skipItem = _skipItemsCategory.GetType().GetProperty(_subCategory).GetValue(_skipItemsCategory);

                                        // Convert the retrieved value to a float.
                                        var _skipItemAsFloat = Convert.ToSingle(_skipItem);

                                        // Check if the user has skipped the item in this EF and increment if they have.
                                        if (_skipItemAsFloat == 0)
                                        {
                                            skippedEnoughTimes++;
                                        }

                                        // Checks if challenge item has been skipped enough times to complete the challenge
                                        if (skippedEnoughTimes == _time)
                                        {
                                            isSuccessful = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (_restrictionType == ChallengeType.Only_This && _fullDateRange)
                            {
                                // A counter to count the number of times an item has been discretely used
                                var onlyUsedEnoughtTimes = 0;

                                foreach (var ecoMeasurement in ecoMeasurementsOfInterest)
                                {
                                    // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                    // based on the text values held in the Challenge list entry.
                                    var _onlyThisItemsCategory = ecoMeasurement.GetType().GetProperty(_category).GetValue(ecoMeasurement);
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
                                        onlyUsedEnoughtTimes++;
                                    }

                                    // Checks if challenge item has been discretely used enough times to complete the challenge
                                    if (onlyUsedEnoughtTimes == _time)
                                    {
                                        isSuccessful = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (String.Equals(_category, "Transport"))
                        {
                            if (_restrictionType == ChallengeType.Any)
                            {
                                int itemTotal = 0;

                                foreach (var ecoMeasure in _user.EcologicalMeasurements)
                                {
                                    // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                    // based on the text values held in the Challenge list entry.
                                    var itemCategory = ecoMeasure.GetType().GetProperty(_category).GetValue(ecoMeasure);
                                    var item = itemCategory.GetType().GetProperty(_subCategory).GetValue(itemCategory);

                                    itemTotal += (int)item;

                                    if (itemTotal >= amountToConsume)
                                    {
                                        isSuccessful = true;
                                        break;
                                    }
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

            return Ok("All User Challenge status' re-calculated.");
        }

        /*
        * Check if there has been any activity in the specified category in the specified amount of time so that
        * we can check early if the user is activly tracking this category while using PLNKTN.
        */
        private bool EcoMeasurementHasActivity(string _category, EcologicalMeasurement ecoMeasurement)
        {

            var _activityDetected = false;

            // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
            // based on the text values held in the Challenge list entry.
            var _itemsCategory = ecoMeasurement.GetType().GetProperty(_category).GetValue(ecoMeasurement);

            // Create a dictionary to hold the list of property names and their values
            //var properties = new Dictionary<string, int>();
            // Iterate over the list of sub categories and add their property names and values to dict.
            foreach (var _tempProps in _itemsCategory.GetType().GetProperties())
            {
                if ((float)_tempProps.GetValue(_itemsCategory) > 0)
                {
                    _activityDetected = true;
                    break;
                }
            }

            return _activityDetected;
        }

        #endregion


        // GET: api/Rewards
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _rewardRepository.GetAllRewards();

            if (result != null && result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                // return HTTP 404 as rewards cannot be found in DB
                return NotFound("No Rewards are present in the DB.");
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Reward information formatted incorrectly.");
            }

            var result = await _rewardRepository.GetReward(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                // return HTTP 404 as rewards cannot be found in DB
                return NotFound("Reward with ID '" + id + "' does not exist.");
            }
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

            var result = await _rewardRepository.CreateReward(reward);

            if (result == 1)
            {
                // Generate a 'user reward' and add it to all users in the DB.
                var rewards = new List<Reward>
                {
                    reward
                };
                var userReward = UsersController.GenerateUserRewards(rewards);
                await _userRepository.AddUserRewardToAllUsers(userReward.First());

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

            var result = await _rewardRepository.UpdateReward(reward);

            if (result == 1)
            {
                // Generate a 'user reward' and update it in all users in the DB.
                var rewards = new List<Reward>
                {
                    reward
                };
                // Make the user reward information ready for sending to the repository
                var userReward = UsersController.GenerateUpdateUserRewards(rewards);
                await _userRepository.UpdateUserRewardInAllUsers(userReward.First());

                // return HTTP 200 Ok reward was updated.  PUT does not require object to be returned in HTTP body.
                return Ok();
            }
            else if (result == -9)
            {
                // return HTTP 404 as object cannot be found in DB
                return NotFound("Reward with ID '" + reward.Id + "' does not exist.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }

        // DELETE api/Rewards/rewardId
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("Reward information formatted incorrectly.");
            }

            var result = await _rewardRepository.DeleteReward(id);

            if (result == 1)
            {
                await _userRepository.DeleteUserRewardFromAllUsers(id);
                return Ok();
            }
            else if (result == -9)
            {
                // return HTTP 404 as rewards cannot be found in DB
                return NotFound("Reward with ID '" + id + "' does not exist.");
            }
            else
            {
                // return HTTP 400 badrequest as something is wrong
                return BadRequest("An internal error occurred.  Please contact the system administrator.");
            }
        }
    }
}
