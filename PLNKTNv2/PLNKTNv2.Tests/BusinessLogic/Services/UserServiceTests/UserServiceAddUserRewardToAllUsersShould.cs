using Newtonsoft.Json;
using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.BusinessLogic.Helpers.Implementation;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace PLNKTNv2.Tests.BusinessLogic.Services.UserServiceTests
{
    public class UserServiceAddUserRewardToAllUsersShould
    {
        [Fact]
        public void AddUserRewardToAllUsers()
        {
            // Arrange
            // Setup classes
            IMessenger iMessengerMock = new Email();
            IUserService sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            ICollection<User> users = new List<User>();
            string jsonTemp = File.ReadAllText("./Json/Models/Reward.json");
            Reward reward = JsonConvert.DeserializeObject<Reward>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/User.json");
            User user = JsonConvert.DeserializeObject<User>(jsonTemp);
            for (int i = 0; i < 3; i++)
            {
                user.Id = "string" + users.Count();
                users.Add(user);
            }

            // Act
            sut.AddUserRewardToAllUsers(reward, users);

            // Assert
            foreach (var _user in users)
            {
                Assert.True(_user.UserRewards.Count() == 2);
                Assert.True(_user.UserRewards[1].Id == "newRewardId");
            }
        }

        [Fact]
        public void ErrorWithNullReward()
        {
            // Arrange
            // Setup classes
            IMessenger iMessengerMock = new Email();
            IUserService sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            ICollection<User> users = new List<User>();
            Reward reward = null;
            string jsonTemp = File.ReadAllText("./Json/Models/User.json");
            User user = JsonConvert.DeserializeObject<User>(jsonTemp);
            for (int i = 0; i < 3; i++)
            {
                user.Id = "string" + users.Count();
                users.Add(user);
            }

            // Act Assert
            Assert.ThrowsAny<NullReferenceException>(() => sut.AddUserRewardToAllUsers(reward, users));
        }

        [Fact]
        public void ErrorWithNullUsers()
        {
            // Arrange
            // Setup classes
            IMessenger iMessengerMock = new Email();
            IUserService sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            ICollection<User> users = null;
            string jsonTemp = File.ReadAllText("./Json/Models/Reward.json");
            Reward reward = JsonConvert.DeserializeObject<Reward>(jsonTemp);

            // Act Assert
            Assert.ThrowsAny<NullReferenceException>(() => sut.AddUserRewardToAllUsers(reward, users));
        }
    }
}