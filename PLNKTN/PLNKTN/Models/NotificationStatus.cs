using System;

namespace PLNKTN.Models
{
    [Flags]
    public enum NotificationStatus
    {
        Notified = 0,
        Not_Notified = 1,
        Not_Complete = 2
    }
}
