using PLNKTNv2.Models;

namespace PLNKTNv2.BusinessLogic.Services
{
    public interface IBinService
    {
        Status InsertUserTreeToBin(UserGrantedRewardProject userTree, Bin bin);
    }
}