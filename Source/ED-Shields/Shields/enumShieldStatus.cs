﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enhanced_Development.Shields
{
    public enum enumShieldStatus
    {
        //Online and gathering power
        ActiveCharging,
        //Charged and sustaining
        ActiveSustaining,
        //Online but low power
        ActiveDischarging,
        //Online and gathering power
        Initilising,
        //Disabled and offline
        Offline
    }
}
