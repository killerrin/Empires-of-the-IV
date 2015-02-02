using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Enumerators
{
    public enum NetworkStage
    {
        None,
        HandshakingConnection,
        InLobby,
        InGame,
        GameOver
    }
}
