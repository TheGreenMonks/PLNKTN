using PLNKTNv2.Models;
using System;

namespace PLNKTNv2.BusinessLogic.Services
{
    public interface IEcologicalMeasurementService
    {
        EcologicalMeasurement GetEcologicalMeasurement(User user, DateTime date);
        Status InsertEcologicalMeasurement(User user, EcologicalMeasurement ecologicalMeasurement);
        Status UpdateEcologicalMeasurement(User user, EcologicalMeasurement ecologicalMeasurement);
    }
}