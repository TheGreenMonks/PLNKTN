using PLNKTNv2.Models;

namespace PLNKTNv2.BusinessLogic.Services.Implementation
{
    public class BinService : IBinService
    {
        public Status InsertUserTreeToBin(UserGrantedRewardProject userTree, Bin bin)
        {
            bin.Projects.Add(userTree);
            return Status.OK;
        }
    }
}