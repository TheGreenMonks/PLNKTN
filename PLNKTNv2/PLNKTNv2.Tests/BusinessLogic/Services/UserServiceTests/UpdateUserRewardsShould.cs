using Newtonsoft.Json;
using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.BusinessLogic.Helpers.Implementation;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PLNKTNv2.Tests.BusinessLogic.Services.UserServiceTests
{
    public class UpdateUserRewardsShould : IDisposable
    {
        private bool disposedValue;
        private IMessenger iMessengerMock;
        private IUserService sut;
        private User user;
        private List<UserRewardDto> userRewardDtos;

        [Fact]
        public void UpdateRewardIsRewardCompletedAndNotificationStatus_WithSingleReward()
        {
            // Arrange
            SetUpUser();
            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Id = "REWARD_TREE_0005",
                IsRewardGranted = true,
                NotificationStatus = NotificationStatus.Not_Notified
            };

            userRewardDtos = new List<UserRewardDto>();
            userRewardDtos.Add(userRewardDto);

            // Act
            sut.UpdateUserRewards(user, userRewardDtos);

            // Assert
            var _userRewardTemp = user.UserRewards.Find(ur => ur.Id == "REWARD_TREE_0005");

            Assert.True(_userRewardTemp.IsRewardGranted);
            Assert.Equal(NotificationStatus.Not_Notified, _userRewardTemp.NotificationStatus);
        }

        [Fact]
        public void UpdateRewardNotificationStatusOnly_WithSingleReward()
        {
            // Arrange
            SetUpUser();
            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Id = "REWARD_TREE_0005",
                NotificationStatus = NotificationStatus.Notified
            };

            userRewardDtos = new List<UserRewardDto>();
            userRewardDtos.Add(userRewardDto);

            // Act
            sut.UpdateUserRewards(user, userRewardDtos);

            // Assert
            var _userRewardTemp = user.UserRewards.Find(ur => ur.Id == "REWARD_TREE_0005");

            Assert.True(!_userRewardTemp.IsRewardGranted);
            Assert.Equal(NotificationStatus.Notified, _userRewardTemp.NotificationStatus);
        }

        [Fact]
        public void UpdateRewardChallengeSingle_WithSingleReward()
        {
            // Arrange
            SetUpUser();
            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Challenges = new List<UserRewardChallengeDto>()
                {
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_BIKE_030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    }
                },
                Id = "REWARD_TREE_0005",
            };

            userRewardDtos = new List<UserRewardDto>();
            userRewardDtos.Add(userRewardDto);

            // Act
            sut.UpdateUserRewards(user, userRewardDtos);

            // Assert
            var _userRewardChallengeTemp = user.UserRewards.Find(ur => ur.Id == "REWARD_TREE_0005")
                .Challenges.Find(urc => urc.Id == "TRAN_BIKE_030");

            Assert.Equal(NotificationStatus.Info_Showed, _userRewardChallengeTemp.NotificationStatus);
        }

        [Fact]
        public void UpdateAllRewardChallengesToInfoShowed_WithSingleReward()
        {
            // Arrange
            SetUpUser();
            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Challenges = new List<UserRewardChallengeDto>()
                {
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_BIKE_030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_WALK_010",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_PLANTS_0003",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_WALK_005",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_SEAFOOD_0030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_PORK_0030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    }
                },
                Id = "REWARD_TREE_0005",
            };

            userRewardDtos = new List<UserRewardDto>();
            userRewardDtos.Add(userRewardDto);

            // Act
            sut.UpdateUserRewards(user, userRewardDtos);

            // Assert
            var _userRewardTemp = user.UserRewards.Find(ur => ur.Id == "REWARD_TREE_0005");

            foreach (var challenge in _userRewardTemp.Challenges)
            {
                Assert.Equal(NotificationStatus.Info_Showed, challenge.NotificationStatus);
            }
        }

        [Fact]
        public void UpdateAllRewardsAndChallenges_WithDifferentValues()
        {
            // Arrange
            SetUpUser();
            userRewardDtos = new List<UserRewardDto>();

            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Challenges = new List<UserRewardChallengeDto>()
                {
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_BIKE_030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_WALK_010",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_PLANTS_0003",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_WALK_005",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_SEAFOOD_0030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_PORK_0030",
                        NotificationStatus = NotificationStatus.Info_Showed
                    }
                },
                Id = "REWARD_TREE_0005"
            };

            userRewardDtos.Add(userRewardDto);

            userRewardDto = new UserRewardDto()
            {
                Challenges = new List<UserRewardChallengeDto>()
                {
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_DAIRY_0001",
                        NotificationStatus = NotificationStatus.Notified
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_SEAFOOD_0001",
                        NotificationStatus = NotificationStatus.Notified
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "TRAN_BIKE_015",
                        NotificationStatus = NotificationStatus.Notified
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_PLANTS_0002",
                        NotificationStatus = NotificationStatus.Notified
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_EGG_0003",
                        NotificationStatus = NotificationStatus.Notified
                    },
                    new UserRewardChallengeDto()
                    {
                        Id = "DIET_PORK_0003",
                        NotificationStatus = NotificationStatus.Notified
                    }
                },
                Id = "REWARD_TREE_0004",
                IsRewardGranted = true,
                NotificationStatus = NotificationStatus.Notified
            };

            userRewardDtos.Add(userRewardDto);

            // Act
            sut.UpdateUserRewards(user, userRewardDtos);

            // Assert
            var _userRewardTemp = user.UserRewards.Find(ur => ur.Id == "REWARD_TREE_0005");
            Assert.Equal(NotificationStatus.Not_Complete, _userRewardTemp.NotificationStatus);
            Assert.True(!_userRewardTemp.IsRewardGranted);
            foreach (var challenge in _userRewardTemp.Challenges)
            {
                Assert.Equal(NotificationStatus.Info_Showed, challenge.NotificationStatus);
            }

            _userRewardTemp = user.UserRewards.Find(ur => ur.Id == "REWARD_TREE_0004");
            Assert.Equal(NotificationStatus.Notified, _userRewardTemp.NotificationStatus);
            Assert.True(_userRewardTemp.IsRewardGranted);
            foreach (var challenge in _userRewardTemp.Challenges)
            {
                Assert.Equal(NotificationStatus.Notified, challenge.NotificationStatus);
            }
        }

        [Fact]
        public void ErrorWithBadRewardId()
        {
            // Arrange
            SetUpUser();
            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Id = "Bad-Reward-Id",
                IsRewardGranted = true,
                NotificationStatus = NotificationStatus.Not_Notified
            };

            userRewardDtos = new List<UserRewardDto>();
            userRewardDtos.Add(userRewardDto);

            // Act & Assert
            Assert.ThrowsAny<NullReferenceException>(() => sut.UpdateUserRewards(user, userRewardDtos));
        }

        [Fact]
        public void ErrorWithBadRewardChallengeId()
        {
            // Arrange
            SetUpUser();
            UserRewardDto userRewardDto = new UserRewardDto()
            {
                Challenges = new List<UserRewardChallengeDto>()
                {
                    new UserRewardChallengeDto()
                    {
                        Id = "Bad-Challenge-Id",
                        NotificationStatus = NotificationStatus.Info_Showed
                    }
                },
                Id = "REWARD_TREE_0005",
            };

            userRewardDtos = new List<UserRewardDto>();
            userRewardDtos.Add(userRewardDto);

            // Act & Assert
            Assert.ThrowsAny<NullReferenceException>(() => sut.UpdateUserRewards(user, userRewardDtos));
        }

        private void SetUpUser()
        {
            // Setup classes
            iMessengerMock = new Email();
            sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            string jsonTemp = File.ReadAllText("./Json/Models/User_Blank_New.json");
            user = JsonConvert.DeserializeObject<User>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/UserRewards/UserRewards_FullSet.json");
            user.UserRewards = JsonConvert.DeserializeObject<List<UserReward>>(jsonTemp);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    iMessengerMock = null;
                    sut = null;
                    user = null;
                    userRewardDtos = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}