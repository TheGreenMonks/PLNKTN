using PLNKTNv2.Models;
using System;

namespace PLNKTNv2.BusinessLogic.Services.Implementation
{
    public class EcologicalMeasurementService : IEcologicalMeasurementService
    {
        public EcologicalMeasurement GetEcologicalMeasurement(User user, DateTime date)
        {
            return user.EcologicalMeasurements.Find(
                delegate (EcologicalMeasurement em)
                    {
                        return DateTime.Equals(em.Date_taken.Date, date.Date);
                    }
                );
        }

        public Status InsertEcologicalMeasurement(User user, EcologicalMeasurement ecologicalMeasurement)
        {
            EcologicalMeasurement existingEcoMeasure = user.EcologicalMeasurements.Find(
                e => e.Date_taken.Date == ecologicalMeasurement.Date_taken.Date);

            /* Set large data lists to null as these aren't updated here, thus sending them
            back to the DB is a waste of BW and DyDB processing time.*/
            user.GrantedRewards = null;
            user.UserRewards = null;

            if (existingEcoMeasure == null)
            {
                user.EcologicalMeasurements.Add(ecologicalMeasurement);
                return Status.CREATED_AT;
            }

            return Status.CONFLICT;
        }

        public Status UpdateEcologicalMeasurement(User user, EcologicalMeasurement ecologicalMeasurement)
        {
            EcologicalMeasurement existingEcoMeasure = user.EcologicalMeasurements.Find(
                e => e.Date_taken.Date == ecologicalMeasurement.Date_taken.Date);

            /* Set large data lists to null as these aren't updated here, thus sending them
            back to the DB is a waste of BW and DyDB processing time.*/
            user.GrantedRewards = null;
            user.UserRewards = null;

            if (existingEcoMeasure != null)
            {
                // Remove old ecological measurement from DB User
                user.EcologicalMeasurements.Remove(existingEcoMeasure);

                // Update DB updatedEcoMeasure with new values
                existingEcoMeasure.EcologicalFootprint = ecologicalMeasurement.EcologicalFootprint;
                existingEcoMeasure.Diet = ecologicalMeasurement.Diet;
                existingEcoMeasure.Clothing = ecologicalMeasurement.Clothing;
                existingEcoMeasure.Electronics = ecologicalMeasurement.Electronics;
                existingEcoMeasure.Footwear = ecologicalMeasurement.Footwear;
                existingEcoMeasure.Transport = ecologicalMeasurement.Transport;

                user.EcologicalMeasurements.Add(existingEcoMeasure);
                return Status.OK;
            }

            return Status.NOT_FOUND;
        }
    }
}