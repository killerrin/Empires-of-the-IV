using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public enum SystemPacketID
    {
        None,

        GameModeChanged,
        MapChanged,
        GameStarted,

        RequestSetupData,

        JoinTeam1,
        JoinTeam2
    }
}
