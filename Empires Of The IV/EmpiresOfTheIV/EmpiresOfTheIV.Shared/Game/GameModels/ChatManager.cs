using EmpiresOfTheIV.Data_Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmpiresOfTheIV.Game.GameModels
{
    public class ChatManager : IEnumerable<ChatMessage>
    {
        public static readonly Uri DefaultAvatar = new Uri("http://www.killerrin.com", UriKind.Absolute);

        public ObservableCollection<ChatMessage> ChatMessages { get; private set; }
        public int MessageCount { get { return ChatMessages.Count; } }

        public Uri CurrentAvatar;

        public ChatManager()
        {
            ChatMessages = new ObservableCollection<ChatMessage>();
            CurrentAvatar = DefaultAvatar;
        }

        public void AddMessage(string name, string message) { ChatMessages.Add(new ChatMessage(CurrentAvatar, name, message, DateTime.UtcNow)); }
        public void AddMessage(ChatMessage message) { ChatMessages.Add(message); }

        #region Interface Implimentations
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return GetEnumerator();
        }
        public IEnumerator<ChatMessage> GetEnumerator()
        {
            for (int i = 0; i < ChatMessages.Count; i++)
            {
                yield return ChatMessages[i];
            }
        }
        #endregion
    }
}
