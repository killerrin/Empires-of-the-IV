using Anarian.DataStructures;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class Team : IUpdatable, IRenderable//, IEnumerable<Player>
    {
        public TeamID TeamID { get; protected set; }
        public List<Player> Players { get; protected set; }

        public int PlayerCount { get { return Players.Count; } }
        public int Count { get { return PlayerCount; } }

        public Team(TeamID teamID)
        {
            TeamID = teamID;
            Players = new List<Player>();
        }

        #region Collection Settings
        public void AddToTeam(PlayerType playerType, uint playerID, string playerName)
        {
            switch (playerType)
            {
                case PlayerType.Human:      Players.Add(Player.HumanPlayer(playerID, playerName));       break;
                case PlayerType.AI:         Players.Add(Player.AIPlayer(playerID,    playerName));          break;
            }
        }
        public void AddToTeam(Player player)
        {
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

        public void Clear()
        {
            Players.Clear();
        }

        /// <summary>
        /// Gets the player on the team at the specified index
        /// </summary>
        /// <param name="playerID">the ID of the player to get</param>
        /// <returns>The Player, Null if no player is found</returns>
        public Player GetPlayer(int playerID)
        {
            for (int i = 0; i < Players.Count; i++) {
                if (Players[i].PlayerID == playerID) {
                    return Players[i];
                }
            }
            return null;
        }
        #endregion

        #region Serialization
        public string ThisToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void JsonToThis(string json)
        {
            JObject jObject = JObject.Parse(json);
            Team team = JsonConvert.DeserializeObject<Team>(jObject.ToString());

            SetFromOtherTeam(team);
        }

        private void SetFromOtherTeam (Team o)
        {
            TeamID = o.TeamID;
            Players = o.Players;
        }

        #endregion

        #region Interface Implimentations
        //IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        //public IEnumerator<Player> GetEnumerator()
        //{
        //    for (int i = 0; i < Players.Count; i++)
        //    {
        //        yield return Players[i];
        //    }
        //}
         
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
