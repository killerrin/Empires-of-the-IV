using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameModels;
using EmpiresOfTheIV.Game.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Menus.PageParameters
{
    public struct InGamePageParameter
    {
        public GameMode GameType;
        public MapName MapName;
        public double maxUnitsPerPlayer;

        public string myUserName;
        public uint myPlayerID;

        public Team team1;
        public Team team2;

        public ChatManager chatManager;
    }
}
