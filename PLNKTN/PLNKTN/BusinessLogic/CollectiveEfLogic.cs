using PLNKTN.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PLNKTN.BusinessLogic
{
    public static class CollectiveEfLogic
    {
        public static CollectiveEF GenerateCollectiveEF(DateTime timeStamp, IList<User> userList)
        {
            float collectiveEfResult = ComputeCollectiveEFAsync(timeStamp, userList);
            CollectiveEF collectiveEf = new CollectiveEF()
            {
                Date_taken = timeStamp,
                Collective_EF = collectiveEfResult
            };

            return collectiveEf;
        }

        private static float ComputeCollectiveEFAsync(DateTime calculationDate, IList<User> userList)
        {
            float? totalCollectiveEf = 0;
            int count = 0;

            foreach (var user in userList.Where(u => u.EcologicalMeasurements.Count > 0))
            {
                var latestEfEntryDate = user.EcologicalMeasurements.Max(e => e.Date_taken.Date);

                if (DateTime.Equals(calculationDate, latestEfEntryDate))
                {
                    var userEf = user.EcologicalMeasurements.Single(x => x.Date_taken.Date == calculationDate.Date);
                    totalCollectiveEf += userEf.EcologicalFootprint;
                    count++;
                }
                else
                {
                    var userEf = user.EcologicalMeasurements.SingleOrDefault(x => x.Date_taken.Date == latestEfEntryDate.Date);
                    totalCollectiveEf += userEf.EcologicalFootprint;
                    count++;
                }

            }

            return (float)totalCollectiveEf / count;
        }
    }
}
