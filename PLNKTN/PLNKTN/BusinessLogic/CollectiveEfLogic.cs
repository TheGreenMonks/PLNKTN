﻿using PLNKTN.Models;
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
                // Check if user has an EM for calculationDate and if not get all EMs from before this date.
                // The latest dated EM will then be used in the calculation.
                var emsInQuestion = user.EcologicalMeasurements.Where(em => em.Date_taken.Date == calculationDate.Date
                                                                        || em.Date_taken.Date < calculationDate.Date);

                if (emsInQuestion.Count() != 0)
                {
                    var latestEfEntryDate = emsInQuestion.Max(e => e.Date_taken.Date);
                    var userEf = user.EcologicalMeasurements.Single(x => x.Date_taken.Date == latestEfEntryDate.Date);
                    totalCollectiveEf += userEf.EcologicalFootprint;
                    count++;
                }
            }
            return (float)totalCollectiveEf / count;
        }
    }
}
