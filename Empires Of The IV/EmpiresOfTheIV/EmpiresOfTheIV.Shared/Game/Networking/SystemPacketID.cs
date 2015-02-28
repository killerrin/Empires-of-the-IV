using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public enum SystemPacketID
    {
        None,

        RequestSetupData,
        
        GameModeChanged,
        UnitMaxChanged,
        MapChanged,

        JoinTeam1,
        JoinTeam2,

        GameStart,
        GameBegin,

        GameLoaded,
        WaitingForData,

        GameSync,

        Pause,
        Resume,

        Quit
    }
}
