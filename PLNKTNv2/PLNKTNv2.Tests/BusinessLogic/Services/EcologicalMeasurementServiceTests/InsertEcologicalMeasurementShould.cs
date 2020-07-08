using Newtonsoft.Json;
using PLNKTNv2.BusinessLogic.Services;
using PLNKTNv2.BusinessLogic.Services.Implementation;
using PLNKTNv2.Models;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace PLNKTNv2.Tests.BusinessLogic.Services.EcologicalMeasurementServiceTests
{
    public class InsertEcologicalMeasurementShould
    {
        [Fact]
        public void InsertEcologicalMeasurementToUsersList()
        {
            // Arrange
            // Setup classes
            IEcologicalMeasurementService sut = new EcologicalMeasurementService();

            // Setup sut method parameters
            string jsonTemp = File.ReadAllText("./Json/Models/User.json");
            User user = JsonConvert.DeserializeObject<User>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/EcologicalMeasurement.json");
            EcologicalMeasurement em = JsonConvert.DeserializeObject<EcologicalMeasurement>(jsonTemp);

            // Setup expected answers
            DateTime dateCreated = new DateTime(2020, 07, 08, 00, 00, 00, 000);

            // Act
            Status status = sut.InsertEcologicalMeasurement(user, em);

            // Assert
            Assert.True(status == Status.CREATED_AT);
            Assert.True(user.EcologicalMeasurements.Count() == 2);
            Assert.True(user.EcologicalMeasurements.ElementAt(1).EcologicalFootprint == 10);
            Assert.Equal(user.EcologicalMeasurements.ElementAt(1).Date_taken, dateCreated);
        }

        [Fact]
        public void ReturnConflictIfEcologicalMeasurementAlreadyExists()
        {
            // Arrange
            // Setup classes
            IEcologicalMeasurementService sut = new EcologicalMeasurementService();

            // Setup sut method parameters
            string jsonTemp = File.ReadAllText("./Json/Models/User.json");
            User user = JsonConvert.DeserializeObject<User>(jsonTemp);
            jsonTemp = File.ReadAllText("./Json/Models/EcologicalMeasurement.json");
            EcologicalMeasurement em = JsonConvert.DeserializeObject<EcologicalMeasurement>(jsonTemp);

            // Add em to user
            user.EcologicalMeasurements.Add(em);

            // Act
            Status status = sut.InsertEcologicalMeasurement(user, em);

            // Assert
            Assert.True(status == Status.CONFLICT);
        }
    }
}