using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Menus.PageParameters
{
    public struct GameOverPageParameter
    {
        public GameConnectionType GameConnectionType;

        public GameMode GameType;
        public MapName MapName;
        public double maxUnitsPerPlayer;

        public Player me;

        public Team team1;
        public Team team2;

        public override string ToString()
        {
            return "GameConnectionType: " + GameConnectionType.ToString() + " | " +
                   "GameType: " + GameType.ToString() + " | " +
                   "MapName: " + MapName.ToString() + " | " +
                   "MaxUnitsPerPlayer: " + maxUnitsPerPlayer.ToString() + " | " +
                   "Me: " + me.ToString() + " | " +
                   "Team1: " + team1.ToString() + " | " +
                   "Team2: " + team2.ToString();
        }
    }
}
