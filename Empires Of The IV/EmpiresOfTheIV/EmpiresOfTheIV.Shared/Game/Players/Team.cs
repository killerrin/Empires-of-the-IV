using Anarian.DataStructures;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class Team : IUpdatable, IRenderable, IEnumerable<APlayer>
    {
        public TeamID TeamID;
        public List<APlayer> Players { get; protected set; }
        public int PlayerCount { get { return Players.Count; } }

        public Team(TeamID teamID)
        {
            TeamID = teamID;
            Players = new List<APlayer>();
        }

        #region Collection Settings
        public void AddToTeam(PlayerType playerType, int playerID, string playerName)
        {
            switch (playerType)
            {
                case PlayerType.LocalPlayer:            Players.Add(new LocalPlayer(this, playerID, playerName));       break;
                case PlayerType.NetworkedPlayer:        Players.Add(new NetworkedPlayer(this, playerID, playerName));   break;
                case PlayerType.AI:                     Players.Add(new AIPlayer(this, playerID, playerName));          break;
            }
        }
        public void AddToTeam(APlayer player)
        {
            player.TeamID = this;
            Players.Add(player);
        }

        public void RemovePlayer(int playerID)
        {
            for (int i = 0; i < Players.Count; i++) {
                if (Players[i].PlayerID == playerID) {
                    Players.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the player on the team at the specified index
        /// </summary>
        /// <param name="playerID">the ID of the player to get</param>
        /// <returns>The Player, Null if no player is found</returns>
        public APlayer GetPlayer(int playerID)
        {
            for (int i = 0; i < Players.Count; i++) {
                if (Players[i].PlayerID == playerID) {
                    return Players[i];
                }
            }
            return null;
        }
        #endregion

        #region Interface Implimentations
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<APlayer> GetEnumerator()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                yield return Players[i];
            }
        }

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public void Update(GameTime gameTime)
        {
            foreach (var p in Players)
            {
                p.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {
            foreach (var p in Players)
            {
                p.Draw(gameTime, spriteBatch, graphics, camera);
            }
        }
    }
}
