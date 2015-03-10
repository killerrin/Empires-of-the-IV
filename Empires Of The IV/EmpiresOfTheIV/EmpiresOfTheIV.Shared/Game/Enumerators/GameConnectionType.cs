using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Enumerators
{
    public enum GameConnectionType
    {
        None,
        Singleplayer,
        BluetoothMultiplayer,
        LANMultiplayer,

        BluetoothHost,
        BluetoothClient,
        LANHost,
        LANClient
    }
}
