using PLNKTNv2.Models;
using System;
using System.Collections.Generic;

namespace PLNKTNv2.BusinessLogic
{
    public interface ICollectiveEfService
    {
        CollectiveEF GenerateCollectiveEF(DateTime timeStamp, IEnumerable<User> userList);
    }
}