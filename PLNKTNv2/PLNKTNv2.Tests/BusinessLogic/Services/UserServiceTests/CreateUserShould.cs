using Newtonsoft.Json;
using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.BusinessLogic.Helpers.Implementation;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.Models;
using PLNKTNv2.Models.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PLNKTNv2.Tests.BusinessLogic.Services.UserServiceTests
{
    public class CreateUserShould
    {
        [Fact]
        public void CreateUserWithAllUserRewards()
        {
            // Arrange
            // Setup classes
            IMessenger iMessengerMock = new Email();
            IUserService sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            string userId = "2020-07-03T23-23-23";
            ICollection<Reward> rewards = new List<Reward>();
            string jsonTemp = File.ReadAllText("./Json/Models/Dtos/UserDetailsDto.json");
            UserDetailsDTO userDto = JsonConvert.DeserializeObject<UserDetailsDTO>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/Reward.json");
            Reward reward = JsonConvert.DeserializeObject<Reward>(jsonTemp);
            for (int i = 0; i < 3; i++)
            {
                reward.Id = "newRewardId" + rewards.Count();
                rewards.Add(reward);
            }

            // Act
            User user = sut.CreateUser(rewards, userDto, userId);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(user.UserRewards), GenerateAnswer(3));
            Assert.True(user.EcologicalMeasurements.Count() == 0);
            Assert.True(user.GrantedRewards.Count() == 0);
            Assert.True(user.UserRewards.Count() == 3);

        }

        // Create an example user reward and serialise to string to test against test cases.
        private string GenerateAnswer(int userRewardCount)
        {
            // Create base objects
            ICollection<UserReward> userRewards = new List<UserReward>();
            UserReward userReward = new UserReward()
            {
                Id = "newRewardId",
                Challenges = new List<UserRewardChallenge>(),
                DateCompleted = null,
                IsRewardGranted = false,
                NotificationStatus = NotificationStatus.Not_Complete,
                Status = UserRewardStatus.Incomplete
            };
            UserRewardChallenge urc = new UserRewardChallenge()
            {
                DateCompleted = null,
                Id = "string",
                NotificationStatus = NotificationStatus.Not_Complete,
                Rule = new UserRewardChallengeRule()
                {
                    AmountToConsume = 0,
                    Category = "string",
                    RestrictionType = ChallengeType.Skip,
                    SubCategory = "string",
                    Time = 0
                },
                Status = UserRewardChallengeStatus.Incomplete
            };

            // Add 6 challenges
            for (int i = 0; i < 6; i++)
            {
                userReward.Challenges.Add(urc);
            }

            for (int i = 0; i < userRewardCount; i++)
            {
                userReward.Id = "newRewardId" + userRewards.Count();
                userRewards.Add(userReward);
            }

            return JsonConvert.SerializeObject(userRewards);
        }
    }
}
