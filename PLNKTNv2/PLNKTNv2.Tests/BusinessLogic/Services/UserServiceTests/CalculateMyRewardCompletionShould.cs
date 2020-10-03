using Newtonsoft.Json;
using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.BusinessLogic.Helpers.Implementation;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.Models;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace PLNKTNv2.Tests.BusinessLogic.Services.UserServiceTests
{
    public class CalculateMyRewardCompletionShould : IDisposable
    {
        private bool disposedValue;
        private EcologicalMeasurement ecologicalMeasurement;
        private IMessenger iMessengerMock;
        private IUserService sut;
        private User user;

        [Fact]
        public void CompleteChallengeWhenSkipBeef1Day()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Beef = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            Assert.True(user.UserRewards.ElementAt(0).Challenges.Find(urc => urc.Id == "DIET_BEEF_0001").Status == UserRewardChallengeStatus.Complete);
        }

        [Fact]
        public void CompleteUserWhenSkipDairy1Day()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Dairy = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            Assert.True(user.UserRewards.ElementAt(0).Challenges.Find(urc => urc.Id == "DIET_DAIRY_0001").Status == UserRewardChallengeStatus.Complete);
        }

        [Fact]
        public void CompleteUserWhenSkipEgg1Day()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Egg = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            Assert.True(user.UserRewards.ElementAt(0).Challenges.Find(urc => urc.Id == "DIET_EGG_0001").Status == UserRewardChallengeStatus.Complete);
        }

        [Fact]
        public void CompleteUserWhenSkipPork1Day()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Pork = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            Assert.True(user.UserRewards.ElementAt(0).Challenges.Find(urc => urc.Id == "DIET_PORK_0001").Status == UserRewardChallengeStatus.Complete);
        }

        [Fact]
        public void CompleteUserWhenSkipPoultry1Day()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Poultry = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            Assert.True(user.UserRewards.ElementAt(0).Challenges.Find(urc => urc.Id == "DIET_POULTRY_0001").Status == UserRewardChallengeStatus.Complete);
        }

        [Fact]
        public void CompleteUserWhenSkipSeafood1Day()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Seafood = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            Assert.True(user.UserRewards.ElementAt(0).Challenges.Find(urc => urc.Id == "DIET_SEAFOOD_0001").Status == UserRewardChallengeStatus.Complete);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void User_RewardTree1_ChallengesNotCompleteWhenNoDietEntry()
        {
            // Arrange
            SetUpRewardTree0001Data();
            user.EcologicalMeasurements.ElementAt(0).Diet.Pork = 0;
            user.EcologicalMeasurements.ElementAt(0).Diet.Beef = 0;
            user.EcologicalMeasurements.ElementAt(0).Diet.Poultry = 0;
            user.EcologicalMeasurements.ElementAt(0).Diet.Seafood = 0;
            user.EcologicalMeasurements.ElementAt(0).Diet.Dairy = 0;
            user.EcologicalMeasurements.ElementAt(0).Diet.Egg = 0;
            user.EcologicalMeasurements.ElementAt(0).Diet.Plant_based = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenges = user.UserRewards[0].Challenges;

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenges[0].Status);
            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenges[1].Status);
            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenges[2].Status);
            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenges[3].Status);
            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenges[4].Status);
            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenges[5].Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete3DayOnlyThisPlantsChallengeWhenRequirementsMet()
        {
            // Arrange
            SetUpRewardTree0002Data();
            for (int i = 0; i < 3; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 0,
                        Dairy = 0,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 0,
                        Poultry = 0,
                        Seafood = 0
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.AddDays(-i),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_PLANTS_0003"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_3DayOnlyThisPlantsChallenge_WhenRequirementsMetNotSequential()
        {
            // Arrange
            SetUpRewardTree0002Data();
            for (int i = 0; i < 3; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 0,
                        Dairy = 0,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 0,
                        Poultry = 0,
                        Seafood = 0
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.AddDays(-i),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }
            // Make date taken not sequential
            user.EcologicalMeasurements[1].Date_taken = DateTime.UtcNow.AddDays(-2);
            user.EcologicalMeasurements[2].Date_taken = DateTime.UtcNow.AddDays(-7);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_PLANTS_0003"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_3DayOnlyThisPlantsChallenge_WhenRequirementsMetNotSequentialLongerThan7Days()
        {
            // Arrange
            SetUpRewardTree0002Data();
            for (int i = 0; i < 3; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 0,
                        Dairy = 0,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 0,
                        Poultry = 0,
                        Seafood = 0
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.AddDays(-i),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }
            // Make date taken not sequential
            user.EcologicalMeasurements[1].Date_taken = DateTime.UtcNow.AddDays(-2);
            user.EcologicalMeasurements[2].Date_taken = DateTime.UtcNow.AddDays(-8);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_PLANTS_0003");

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_3DayOnlyThisPlantsChallenge_WhenRequirementsNotMetNotSequential()
        {
            // Arrange
            SetUpRewardTree0002Data();
            for (int i = 0; i < 3; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 0,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 0,
                        Poultry = 0,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.AddDays(-i),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }
            // Make date taken not sequential
            user.EcologicalMeasurements[1].Date_taken = DateTime.UtcNow.AddDays(-2);
            user.EcologicalMeasurements[2].Date_taken = DateTime.UtcNow.AddDays(-7);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_PLANTS_0003");

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_1DayAnyWalk10Challenge_WhenRequirementsMet()
        {
            // Arrange
            SetUpRewardTree0002Data();
            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 0,
                    Dairy = 0,
                    Egg = 0,
                    Plant_based = 0,
                    Pork = 0,
                    Poultry = 0,
                    Seafood = 0
                },
                Transport = new Transport()
                {
                    Bicycle = 20,
                    Bus = 20,
                    Car = 20,
                    Flight = 20,
                    Subway = 20,
                    Walking = 10
                },
                Date_taken = DateTime.UtcNow,
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_010");

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_1DayAnyWalk10Challenge_WhenRequirementsNotMet()
        {
            // Arrange
            SetUpRewardTree0002Data();
            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 0,
                    Dairy = 0,
                    Egg = 0,
                    Plant_based = 0,
                    Pork = 0,
                    Poultry = 0,
                    Seafood = 0
                },
                Transport = new Transport()
                {
                    Bicycle = 20,
                    Bus = 20,
                    Car = 20,
                    Flight = 20,
                    Subway = 20,
                    Walking = 9
                },
                Date_taken = DateTime.UtcNow,
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_010");

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_1DayAnyBike5Challenge_WhenRequirementsMet()
        {
            // Arrange
            SetUpRewardTree0002Data();
            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 0,
                    Dairy = 0,
                    Egg = 0,
                    Plant_based = 0,
                    Pork = 0,
                    Poultry = 0,
                    Seafood = 0
                },
                Transport = new Transport()
                {
                    Bicycle = 6,
                    Bus = 20,
                    Car = 20,
                    Flight = 20,
                    Subway = 20,
                    Walking = 9
                },
                Date_taken = DateTime.UtcNow,
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_BIKE_005");

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_1DayAnyBike5Challenge_WhenRequirementsNotMet()
        {
            // Arrange
            SetUpRewardTree0002Data();
            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 0,
                    Dairy = 0,
                    Egg = 0,
                    Plant_based = 0,
                    Pork = 0,
                    Poultry = 0,
                    Seafood = 0
                },
                Transport = new Transport()
                {
                    Bicycle = 4,
                    Bus = 20,
                    Car = 20,
                    Flight = 20,
                    Subway = 20,
                    Walking = 9
                },
                Date_taken = DateTime.UtcNow,
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_BIKE_005");

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_3DaySkipPoultryChallenge_WhenRequirementsMetNotSequential()
        {
            // Arrange
            SetUpRewardTree0002Data();
            for (int i = 0; i < 3; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 1,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 0,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.AddDays(-i),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }
            // Make date taken not sequential
            user.EcologicalMeasurements[1].Date_taken = DateTime.UtcNow.AddDays(-2);
            user.EcologicalMeasurements[2].Date_taken = DateTime.UtcNow.AddDays(-7);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_POULTRY_0003"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_90DaySkipBeefChallenge_WhenRequirementsMetSequential()
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < 90; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 0,
                        Dairy = 1,
                        Egg = 1,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.Date,
                    EcologicalFootprint = (float?)0.9
                };
                _emTemp.Date_taken = _emTemp.Date_taken.AddDays(-(i + 1));
                user.EcologicalMeasurements.Add(_emTemp);
            }

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_BEEF_0090"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_60DaySkipEggsChallenge_WhenRequirementsMetSequential()
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < 60; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.Date.AddDays(-(i + 1)),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_EGG_0060"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_60DaySkipEggsChallenge_WhenRequirementsNotMet()
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < 60; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.Date.AddDays(-(i + 1)),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            user.EcologicalMeasurements[0].Diet.Egg = 1;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_EGG_0060"); ;

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Theory]
        [InlineData(5, UserRewardChallengeStatus.Incomplete)]
        [InlineData(59, UserRewardChallengeStatus.Incomplete)]
        [InlineData(60, UserRewardChallengeStatus.Complete)]
        public void HaveCorrectAmountCompleted_60DaySkipEggsChallenge(int daysSkipped, UserRewardChallengeStatus isChallengeComplete)
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < daysSkipped; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 1,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.Date.AddDays(-(i + 1)),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            for (int i = 0; i < daysSkipped; i++)
            {
                user.EcologicalMeasurements[i].Diet.Egg = 0;
            }

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_EGG_0060"); ;

            Assert.Equal(isChallengeComplete, challenge.Status);
            Assert.Equal(daysSkipped, challenge.AmountCompleted);
        }

        [Theory]
        [InlineData(10, UserRewardChallengeStatus.Incomplete)]
        [InlineData(89, UserRewardChallengeStatus.Incomplete)]
        [InlineData(90, UserRewardChallengeStatus.Complete)]
        public void HaveCorrectAmountCompleted_90DayOnlyThisPlantsChallenge(int daysEaten, UserRewardChallengeStatus isChallengeComplete)
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < daysEaten; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 1,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 0
                    },
                    Date_taken = DateTime.UtcNow.Date.AddDays(-(i + 1)),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            for (int i = 0; i < daysEaten; i++)
            {
                user.EcologicalMeasurements[i].Diet.Beef = 0;
                user.EcologicalMeasurements[i].Diet.Dairy = 0;
                user.EcologicalMeasurements[i].Diet.Egg = 0;
                user.EcologicalMeasurements[i].Diet.Plant_based = 1;
                user.EcologicalMeasurements[i].Diet.Pork = 0;
                user.EcologicalMeasurements[i].Diet.Poultry = 0;
                user.EcologicalMeasurements[i].Diet.Seafood = 0;
            }

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "DIET_PLANTS_0090"); ;

            Assert.Equal(isChallengeComplete, challenge.Status);
            Assert.Equal(daysEaten, challenge.AmountCompleted);
        }

        [Theory]
        [InlineData(40, UserRewardChallengeStatus.Incomplete)]
        [InlineData(79, UserRewardChallengeStatus.Incomplete)]
        [InlineData(80, UserRewardChallengeStatus.Complete)]
        public void HaveCorrectAmountCompleted_80MileWalkChallenge(int amountConsumed, UserRewardChallengeStatus isChallengeComplete)
        {
            // Arrange
            SetUpRewardTree0021Data();
            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 1,
                    Dairy = 1,
                    Egg = 1,
                    Plant_based = 1,
                    Pork = 1,
                    Poultry = 1,
                    Seafood = 1
                },
                Transport = new Transport()
                {
                    Bicycle = 0,
                    Bus = 0,
                    Car = 0,
                    Flight = 0,
                    Subway = 0,
                    Walking = amountConsumed
                },
                Date_taken = DateTime.UtcNow.Date.AddDays(-1),
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_80"); ;

            Assert.Equal(isChallengeComplete, challenge.Status);
            Assert.Equal(amountConsumed, challenge.AmountCompleted);
        }

        [Fact]
        public void Complete_80MileWalkChallenge_WhenRequirementsMetOver80Days()
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < 80; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 1
                    },
                    Date_taken = DateTime.UtcNow.Date.AddDays(-(i + 1)),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_80"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void Complete_80MileWalkChallenge_WhenRequirementsMetOver1Day()
        {
            // Arrange
            SetUpRewardTree0021Data();

            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 1,
                    Dairy = 1,
                    Egg = 0,
                    Plant_based = 1,
                    Pork = 1,
                    Poultry = 1,
                    Seafood = 1
                },
                Transport = new Transport()
                {
                    Bicycle = 0,
                    Bus = 0,
                    Car = 0,
                    Flight = 0,
                    Subway = 0,
                    Walking = 80
                },
                Date_taken = DateTime.UtcNow.Date.AddDays(-5),
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_80"); ;

            Assert.Equal(UserRewardChallengeStatus.Complete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_80MileWalkChallenge_WhenWalked79MilesIn1DayRecently()
        {
            // Arrange
            SetUpRewardTree0021Data();

            EcologicalMeasurement _emTemp = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 1,
                    Dairy = 1,
                    Egg = 0,
                    Plant_based = 1,
                    Pork = 1,
                    Poultry = 1,
                    Seafood = 1
                },
                Transport = new Transport()
                {
                    Bicycle = 0,
                    Bus = 0,
                    Car = 0,
                    Flight = 0,
                    Subway = 0,
                    Walking = 79
                },
                Date_taken = DateTime.UtcNow.Date.AddDays(-5),
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(_emTemp);

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_80"); ;

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        [Fact]
        public void NotComplete_80MileWalkChallenge_WhenRequirementsNotMet()
        {
            // Arrange
            SetUpRewardTree0021Data();
            for (int i = 0; i < 80; i++)
            {
                EcologicalMeasurement _emTemp = new EcologicalMeasurement()
                {
                    Diet = new Diet()
                    {
                        Beef = 1,
                        Dairy = 1,
                        Egg = 0,
                        Plant_based = 1,
                        Pork = 1,
                        Poultry = 1,
                        Seafood = 1
                    },
                    Transport = new Transport()
                    {
                        Bicycle = 0,
                        Bus = 0,
                        Car = 0,
                        Flight = 0,
                        Subway = 0,
                        Walking = 1
                    },
                    Date_taken = DateTime.UtcNow.Date.AddDays(-(i + 1)),
                    EcologicalFootprint = (float?)0.9
                };
                user.EcologicalMeasurements.Add(_emTemp);
            }

            user.EcologicalMeasurements[0].Transport.Walking = 0;

            // Act
            sut.CalculateMyRewardCompletion(user);

            // Assert
            var challenge = user.UserRewards[0].Challenges.Find(urc => urc.Id == "TRAN_WALK_80"); ;

            Assert.Equal(UserRewardChallengeStatus.Incomplete, challenge.Status);
            Assert.Equal(UserRewardStatus.Incomplete, user.UserRewards[0].Status);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    iMessengerMock = null;
                    sut = null;
                    user = null;
                    ecologicalMeasurement = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer (DON'T NEED)
                // TODO: set large fields to null (DON'T NEED)
                disposedValue = true;
            }
        }

        private void SetUpRewardTree0001Data()
        {
            // Setup classes
            iMessengerMock = new Email();
            sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            string jsonTemp = File.ReadAllText("./Json/Models/NewUser_RT1.json");
            user = JsonConvert.DeserializeObject<User>(jsonTemp);
            ecologicalMeasurement = new EcologicalMeasurement()
            {
                Diet = new Diet()
                {
                    Beef = 1,
                    Dairy = 1,
                    Egg = 1,
                    Plant_based = 1,
                    Pork = 1,
                    Poultry = 1,
                    Seafood = 1
                },
                Date_taken = DateTime.UtcNow,
                EcologicalFootprint = (float?)0.9
            };
            user.EcologicalMeasurements.Add(ecologicalMeasurement);
        }

        private void SetUpRewardTree0002Data()
        {
            // Setup classes
            iMessengerMock = new Email();
            sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            string jsonTemp = File.ReadAllText("./Json/Models/User_Blank_New.json");
            user = JsonConvert.DeserializeObject<User>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/UserRewards/UserReward_RT2.json");
            user.UserRewards.Add(JsonConvert.DeserializeObject<UserReward>(jsonTemp));
        }

        private void SetUpRewardTree0021Data()
        {
            // Setup classes
            iMessengerMock = new Email();
            sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            string jsonTemp = File.ReadAllText("./Json/Models/User_Blank_New.json");
            user = JsonConvert.DeserializeObject<User>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/UserRewards/UserReward_RT21.json");
            user.UserRewards.Add(JsonConvert.DeserializeObject<UserReward>(jsonTemp));
        }
    }
}