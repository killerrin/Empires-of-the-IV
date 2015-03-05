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

        #region Remove
        public void RemovePlayer(int index)
        {
            Players.RemoveAt(index);
        }
        public void RemovePlayer(uint playerID)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ID == playerID)
                {
                    Players.RemoveAt(i);
                    return;
                }
            }
        }
        public void RemovePlayer(string playerName)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Name == playerName)
                {
                    Players.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion

        public void Clear()
        {
            Players.Clear();
        }

        #region Exists
        public bool Exists(int index)
        {
            return index > 0 &&
                   index < PlayerCount;
        }
        public bool Exists(uint playerID)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ID == playerID)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Exists(string playerName)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Name == playerName)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetPlayer
        public Player GetPlayer(int index)
        {
            return Players[index];
        }

        /// <summary>
        /// Gets the player on the team at the specified index
        /// </summary>
        /// <param name="playerID">the ID of the player to get</param>
        /// <returns>The Player</returns>
        /// <exception cref="IndexOutOfRangeException" />
        public Player GetPlayer(uint playerID)
        {
            for (int i = 0; i < Players.Count; i++) {
                if (Players[i].ID == playerID) {
                    return Players[i];
                }
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the player on the team at the specified index
        /// </summary>
        /// <param name="playerID">the ID of the player to get</param>
        /// <returns>The Player</returns>
        /// <exception cref="IndexOutOfRangeException" />
        public Player GetPlayer(string playerName)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Name == playerName)
                {
                    return Players[i];
                }
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region GetIndex
        public int GetIndex(uint playerID)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ID == playerID)
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException();
        }
        public int GetIndex(string playerName)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Name == playerName)
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException();
        }
        #endregion

        #region Indexors
        public Player this[int i]
        {
            get { return Players[i]; }
            set { Players[i] = value; }
        }
        public Player this[uint i]
        {
            get { return GetPlayer(i); }
            set { Players[GetIndex(i)] = value; }
        }
        public Player this[string i]
        {
            get { return GetPlayer(i); }
            set { Players[GetIndex(i)] = value; }
        }
        #endregion
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
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public override string ToString()
        {
            string result = String.Format("Team: {0}, Count: {1} \n  ", TeamID.ToString(), PlayerCount.ToString());
            foreach(var i in Players)
            {
                result += i.ToString() + "\n  "; 
            }
            return result;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var p in Players)
            {
                p.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            foreach (var p in Players)
            {
                p.Draw(gameTime, spriteBatch, graphics, camera);
            }
        }
    }
}
