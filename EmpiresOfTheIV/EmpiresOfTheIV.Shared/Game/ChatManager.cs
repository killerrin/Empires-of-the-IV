using EmpiresOfTheIV.Data_Models;
using KillerrinStudiosToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EmpiresOfTheIV.Game
{
    public class ChatManager : IEnumerable<ChatMessage>, INotifyCollectionChanged
    {
        public static readonly Uri DefaultAvatar = new Uri("ms-appx:///Assets/Player/Default Avatar.png", UriKind.Absolute);

        public ObservableCollection<ChatMessage> ChatMessages;

        public int MessageCount { get { return ChatMessages.Count; } }

        public Uri CurrentAvatar;

        private object m_lockObject;

        public ChatManager()
        {
            ChatMessages = new ObservableCollection<ChatMessage>();
            CurrentAvatar = DefaultAvatar;

            m_lockObject = new object();
        }

        /// <summary>
        /// Adds a Message to the MessageLog
        /// </summary>
        /// <param name="name">The name of the User</param>
        /// <param name="message">The message to send</param>
        /// <returns>The generated ChatMessage</returns>
        public ChatMessage AddMessage(string name, string message) {
            ChatMessage chatMessage = new ChatMessage(CurrentAvatar, name, message, DateTime.UtcNow);
            AddMessage(chatMessage);
            return chatMessage;
        }
        public void AddMessage(ChatMessage message)
        {
            lock (m_lockObject)
            {
                //Debug.WriteLine("Adding Message");

                // Check for duplicates
                if (message.Equals(ChatMessages.Count - 1)) return;

                ChatMessages.Add(message);
                //ChatMessages.Sort();

                try
                {
                    if (CollectionChanged != null)
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
                }
                catch (Exception) { }
            }
        }

        public override string ToString()
        {
            string result = "";
            foreach (var message in ChatMessages)
            {
                result += message.ToString() + " \n";
            }
            return result;
        }


        #region Interface Implimentations
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
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
        public ChatMessage this[int i]
        {
            get { return ChatMessages[i]; }
            protected set { ChatMessages[i] = value; }
        }
    }
}
