﻿using System;

namespace PLNKTNv2.Models
{
    [Flags]
    public enum NotificationStatus
    {
        Notified = 0,
        Not_Notified = 1,
        Not_Complete = 2,
        Info_Showed = 4
    }
}
