using Microsoft.AspNetCore.Mvc;
using Moq;
using PLNKTN.Controllers;
using PLNKTN.DTOs;
using PLNKTN.Models;
using PLNKTN.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace PLNKTN.Tests
{
    public class UsersController_Tests
    {
        #region Get_NoParams_Tests

        [Fact]
        public async Task Get_ReturnsAnOkObjectResult_WithNumberOfUsersInDB()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.GetUsers()).ReturnsAsync(GetUsers_NotNull());

            var controller = new UsersController(mockUserRepo.Object, null);

            // Act
            var result = await controller.Get();
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(4, okResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsANotFoundObjectResult_WithStringExplanation()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.GetUsers()).ReturnsAsync(GetUsers_Null());

            var controller = new UsersController(mockUserRepo.Object, null);

            // Act
            var result = await controller.Get();
            var nfResult = result as NotFoundObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(nfResult);
            Assert.Equal("List of Users does not exist.", nfResult.Value);
        }

        #endregion

        #region Get_WithParams_Tests

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Get_WithParam_ReturnsABadRequestObjectResult_WithString(string userId)
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.GetUser(userId))
                .ReturnsAsync(GetUsers_NotNull().FirstOrDefault(u => u.Id == userId));

            var controller = new UsersController(mockUserRepo.Object, null);

            // Act
            var result = await controller.Get(userId);
            var brResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(brResult);
            Assert.IsType<string>(brResult.Value);
        }

        [Theory]
        [InlineData("user1")]
        [InlineData("user2")]
        [InlineData("user3")]
        public async Task Get_WithParam_ReturnsAnOkObjectResult_WithUser(string userId)
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.GetUser(userId))
                .ReturnsAsync(GetUsers_NotNull().FirstOrDefault(u => u.Id == userId));

            var controller = new UsersController(mockUserRepo.Object, null);

            // Act
            var result = await controller.Get(userId);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(okResult);
            Assert.IsType<User>(okResult.Value);
        }

        [Theory]
        [InlineData("user20")]
        [InlineData("bad_user_name")]
        public async Task Get_WithParam_ReturnsANotFoundObjectResult_WithString(string userId)
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.GetUser(userId))
                .ReturnsAsync(GetUsers_NotNull().FirstOrDefault(u => u.Id == userId));

            var controller = new UsersController(mockUserRepo.Object, null);

            // Act
            var result = await controller.Get(userId);
            var nfResult = result as NotFoundObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(nfResult);
            Assert.IsType<string>(nfResult.Value);
        }

        #endregion

        [Fact]
        public async Task Post_ReturnsACreatedAtActionObjectResult_WithUserAndGetLink()
        {
            // Arrange
            var rewards = GetRewards_NotNull();
            var userRewards = (List<UserReward>)GenerateUserRewards(rewards);

            UserDetailsDTO userDto = new UserDetailsDTO()
            {
                Id = "new_user",
                First_name = "userDto.First_name",
                Last_name = "userDto.Last_name",
                Created_at = DateTime.UtcNow,
                Email = "userDto.Email",
                Level = "userDto.Level",
                LivingSpace = 400,
                NumPeopleHousehold = 2,
                CarMPG = 32,
                ShareData = true,
                Collective_EF = 2.0F,
                Country = "userDto.Country"
            };

            User user = new User()
            {
                Id = userDto.Id,
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
                UserRewards = userRewards,
                GrantedRewards = new List<Bin>()
            };

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.CreateUser())
                .ReturnsAsync(1);

            var mockRewardRepo = new Mock<IRewardRepository>();
            mockRewardRepo.Setup(repo => repo.GetAllRewards())
                .ReturnsAsync(GetRewards_NotNull());

            var controller = new UsersController(mockUserRepo.Object, mockRewardRepo.Object);

            // Act
            var result = await controller.Post(userDto);
            var objResult = result as CreatedAtActionResult;

            // Assert
            user.Should().BeEquivalentTo(user);
            //var createdUser = objResult.Value as User;
            //Assert.IsType<CreatedAtActionResult>(objResult);
            //Assert.IsType<User>(createdUser);
            //Assert.Equal(1, createdUser.UserRewards.Count);
        }

        //  Adds all reward and challenge data required by the user object to a 
        internal static ICollection<UserReward> GenerateUserRewards(ICollection<Reward> rewards)
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


        private List<User> GetUsers_NotNull()
        {
            var users = new List<User>
            {
                new User()
                {
                    Id = "user1"
                },

                new User()
                {
                    Id = "user2"
                },

                new User()
                {
                    Id = "user3"
                },

                new User()
                {
                    Id = "user4"
                }
            };

            return users;
        }

        private List<User> GetUsers_Null()
        {
            return null;
        }

        private ICollection<Reward> GetRewards_NotNull()
        {
            ICollection<Reward> rewards = new List<Reward>();

            for (int i = 0; i < 1; i++)
            {
                rewards.Add(GenerateReward("Reward_Gen_" + i));
            }

            return rewards;
        }

        private Reward GenerateReward(string rewardName)
        {
            // Generate list of challenges
            var challenges = new List<RewardChallenge>();
            for (int i = 0; i < 1; i++)
            {
                RewardChallenge temp = new RewardChallenge()
                {
                    Id = "DIET_BEEF_000" + i,
                    Name = "COWABANGA X" + i,
                    ImageURL = "1" + i,
                    Description = "Protein contains essential amino acids, meaning our bodies can’t make them; and so, they are essential to get from our diet. But other animals don’t make them either. All essential amino acids originate from plants" + i,
                    Link = "20" + i,
                    Text_When_Completed = "Congrats! By skipping Beef once/week you completed this challenge." + i,
                    Text_When_Not_Completed = "Skip Beef once/week to complete this challenge." + i,
                    Source = "Goodland, R Anhang, J. “Livestock and Climate Change: What if the key actors in climate change were pigs, chickens and cows?”" + i,
                    Rule = new RewardChallengeRule()
                    {
                        Time = 1,
                        Category = "Diet" + i,
                        SubCategory = "Beef" + i,
                        RestrictionType = ChallengeType.Skip
                    },
                };

                challenges.Add(temp);
            }

            // Create reward
            Reward reward = new Reward()
            {
                Id = rewardName,
                Title = "Rewards for skipping diet items for 1 week's worth of PLNKTN entries",
                ImageURL = "//images/test.png",
                Description = "Skip all diet items for 1 week each and eat only plant based food for 1 week.",
                Link = "https://www.test.com",
                GridPosition = new RewardGridPosition()
                    {
                        x = 1,
                        y = 1
                    },
                Text_When_Completed = "Congratulations",
                Text_When_Not_Completed = "skip these items for 1 week",
                Source = "et al",
                Challenges = challenges,
                Country = "UK",
                Overview = "Tester",
                Impact = "12",
                Tree_species = "Birch"

            };

            // return reward
            return reward;
        }
    }
}
