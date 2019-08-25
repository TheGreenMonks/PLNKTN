﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLNKTN.Models;
using PLNKTN.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLNKTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public ChallengesController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/Challenges
        [HttpGet]
        public async void Get()
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

            // Iterate over each user that has rewards in their DB record.
            foreach (var _user in dbUsers.Where(u => u.UserRewards != null))
            {
                // Flag to indicate if changes to challenge status have been made
                var changesMade = false;
                // Sort eco measurements by date for easier processing in all challenge rule checks.
                _user.EcologicalMeasurements.OrderBy(e => e.Date_taken);

                // Iterate over all incomplete rewards.
                foreach (var _reward in _user.UserRewards.Where(ur => ur.Status != "COMPLETE"))
                {

                    // For each challenge in this reward that is not complete
                    foreach (var _challenge in _reward.Challenges.Where(c => c.Status != "COMPLETE"))
                    {
                        // Get rule information in local vars (because easier to user later)
                        var _category = _challenge.Rule.Category;
                        var _subCategory = _challenge.Rule.SubCategory;
                        var _time = _challenge.Rule.Time;
                        var _restrictionType = _challenge.Rule.RestrictionType;
                        // Flag to assess success of user against challenge rule.
                        var isSuccessful = true;

                        // Determine how many eco measurements will be required to successfully complete the challenge
                        // and an index start position for the for loop later.
                        var numOfEcoMeasurements = _user.EcologicalMeasurements.Count;
                        var offset = _time * 7;
                        var indexStart = numOfEcoMeasurements - offset;

                        // If there are not enough eco measurements in dbUsers Db entry indexStart will be < 0 and it is impossible
                        // for them to successfully complete this task, therefore break.
                        if (indexStart < 0)
                        {
                            break;
                        }

                        // If the rule restriction is SKIP, i.e. do not eat beef for 1 week.
                        if (String.Equals(_restrictionType, "SKIP"))
                        {
                            for (int i = indexStart; i <= offset; i++)
                            {
                                var ecoMeasureTemp = _user.EcologicalMeasurements.ElementAt(i);

                                // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                // based on the text values held in the Challenge list entry.
                                var _reflectedCategory = ecoMeasureTemp.GetType().GetProperty(_category).GetValue(ecoMeasureTemp);
                                var _reflectedSubCategory = _reflectedCategory.GetType().GetProperty(_subCategory).GetValue(_reflectedCategory);

                                // Convert the retrieved value to either an int.
                                var _reflectedRetrievedValue = Convert.ToInt32(_reflectedSubCategory);

                                if (_reflectedRetrievedValue > 0)
                                {
                                    isSuccessful = false;
                                    break;
                                }
                            }
                        }
                        else if (String.Equals(_restrictionType, "ONLY_THIS"))
                        {
                            for (int i = indexStart; i <= offset; i++)
                            {
                                var ecoMeasureTemp = _user.EcologicalMeasurements.ElementAt(i);

                                // Use reflection to dynamically get the correct 'category' and 'sub category' from the eco measurement
                                // based on the text values held in the Challenge list entry.
                                var _reflectedCategory = ecoMeasureTemp.GetType().GetProperty(_category).GetValue(ecoMeasureTemp);
                                var _reflectedSubCategoryValue = _reflectedCategory.GetType().GetProperty(_subCategory).GetValue(_reflectedCategory);
                                // Count for other subcategories used and used in calculation of challenge success
                                var _countOtherSubCategoriesUsed = 0;

                                // Create a dictionary to hold the list of property names and their values
                                var properties = new Dictionary<string, int>();

                                // Iterate over the list of sub categories and add their property names and values to dict.
                                foreach (var _tempProps in _reflectedCategory.GetType().GetProperties())
                                {
                                    properties.Add(_tempProps.Name, (int)_tempProps.GetValue(_reflectedCategory));
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
                                var _reflectedRetrievedValue = Convert.ToInt32(_reflectedSubCategoryValue);

                                // The use fails the challenge if any other sub category is used or if they have not logged 
                                // usage of the only sub category that is supposed to be used (prevents auto completion by not entering any info).
                                if (_reflectedRetrievedValue == 0 || _countOtherSubCategoriesUsed > 0)
                                {
                                    isSuccessful = false;
                                    break;
                                }
                            }
                        }

                        // Change the challenge status if user is successful.
                        if (isSuccessful)
                        {
                            _challenge.Status = "COMPLETE";
                            changesMade = true;
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
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
