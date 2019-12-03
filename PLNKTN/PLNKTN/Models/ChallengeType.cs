using System;

namespace PLNKTN.Models
{
    [Flags]
    public enum ChallengeType
    {
        // The user must skip consuming this item
        Skip = 0,
        // The user can only use this item and no others
        Only_This = 1,
        // The user can use this time and any other in the category
        Any = 2
    }
}
