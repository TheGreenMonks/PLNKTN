using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.BusinessLogic;
using PLNKTN.Models;
using PLNKTN.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RewardsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Calculate Reward Completion

        [AcceptVerbs("CalcRewards")]
        public async void CalculateUserRewardCompletion()
        {
            IEmailHelper emailHelper = new EmailHelper();
            ICollection<User> users = await _unitOfWork.Repository<User>().GetAllAsync();

            foreach (var _user in users)
            {
                User updatedUser = RewardCalculation.CalculateUserRewardCompletion(_user, ref emailHelper);

                if (updatedUser != null)
                {
                    users.
                }
            }
            // Send email with results of this method (Dexter has email originator address and recipients)
            emailHelper.SendEmail(strReward);
        }

        #endregion

        #region Calculate Challenge Completion

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

            /* TODO
             * As Diet is he only real category being tested all calculations are with floats bu tthis will need to be changed when
             * other categories are included as they have properties of different types in them...
             */
            
            var strChallenge = "Challenge";
            // Get all users from the DB who have NOT NULL Reward arrays
            var dbUsers = await _userRepository.GetAllUsers();

            // Email helper to manage results email
            IEmailHelper emailHelper = new EmailHelper();

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
                            var ecoMeasurementsOfInterest = _user.EcologicalMeasurements.Where(e => e.Date_taken.Date > challengeStart.Date).ToList();
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
                            emailHelper.AddEmailMessageLine(_user.Id, strChallenge, _challenge.Id);
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

            // Send email with results of this method (Dexter has email originator address and recipients)
            emailHelper.SendEmail(strChallenge);
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
            var result = await _unitOfWork.Repository<Reward>().GetAllAsync();

            if (result != null && result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("No Rewards are present in the DB.");
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            Reward result = await _unitOfWork.Repository<Reward>().GetByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("Reward does not exist.");
            }
        }

        // POST api/Rewards
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Reward reward)
        {
            if (reward == null)
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            var batchReward = _unitOfWork.Repository<Reward>().Insert(reward);
            var userReward = UserRewards.GenerateUserReward(reward);
            IList<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            ICollection<User> updatedUsers = UserRewards.AddUserRewardToAllUsers(userReward, users);
            BatchWrite<User> batchUsers = _unitOfWork.Repository<User>().Update(updatedUsers);

            await _unitOfWork.Commit(new BatchWrite[] { batchReward, batchUsers });

            return CreatedAtAction("Get", new { id = reward.Id }, reward);
        }

        // PUT api/Rewards
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Reward reward)
        {
            if (reward == null)
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            BatchWrite<Reward> batchReward = _unitOfWork.Repository<Reward>().Update(reward);
            var userReward = UserRewards.GenerateUpdatedUserReward(reward);
            IList<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            ICollection<User> updatedUsers = UserRewards.UpdateUserRewardInAllUsers(userReward, users);
            BatchWrite<User> batchUsers = _unitOfWork.Repository<User>().Update(updatedUsers);

            await _unitOfWork.Commit(new BatchWrite[] { batchReward, batchUsers });

            return Ok();
        }

        // DELETE api/Rewards/rewardId
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Reward information formatted incorrectly.");
            }

            BatchWrite<Reward> batchReward = _unitOfWork.Repository<Reward>().DeleteById(id);
            IList<User> users = await _unitOfWork.Repository<User>().GetAllAsync();
            ICollection<User> updatedUsers = UserRewards.DeleteUserRewardFromAllUsers(id, users);
            BatchWrite<User> batchUsers = _unitOfWork.Repository<User>().Update(updatedUsers);

            await _unitOfWork.Commit(new BatchWrite[] { batchReward, batchUsers });

            return Ok();
        }
    }
}
