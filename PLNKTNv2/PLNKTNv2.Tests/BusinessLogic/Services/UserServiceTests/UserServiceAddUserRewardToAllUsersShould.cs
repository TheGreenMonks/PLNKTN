using Newtonsoft.Json;
using PLNKTNv2.BusinessLogic.Helpers;
using PLNKTNv2.BusinessLogic.Helpers.Implementation;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace PLNKTNv2.Tests.BusinessLogic.Services.UserServiceTests
{
    public class UserServiceAddUserRewardToAllUsersShould
    {
        [Fact]
        public void UpdateAllUsersWithUserReward()
        {
            // Arrange
            // Setup classes
            IMessenger iMessengerMock = new Email();
            IUserService sut = new UserService(iMessengerMock);

            // Setup sut method parameters
            Reward reward;
            ICollection<User> users = new List<User>();
            string jsonTemp = File.ReadAllText("./Json/Models/Reward.json");
            reward = JsonConvert.DeserializeObject<Reward>(jsonTemp);
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
    }
}