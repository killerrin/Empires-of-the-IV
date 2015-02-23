using EmpiresOfTheIV.Data_Models;
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
    public class ChatManager : IEnumerable<ChatMessage> //, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public static readonly Uri DefaultAvatar = new Uri("http://www.killerrin.com/wikis/eot-iv/skins/logo.png", UriKind.Absolute);

        protected List<ChatMessage> m_chatMessages;
        public List<ChatMessage> ChatMessages { get { return m_chatMessages; } private set { m_chatMessages = value; } }

        public int MessageCount { get { return ChatMessages.Count; } }

        public Uri CurrentAvatar;

        private object m_lockObject;

        public ChatManager()
        {
            m_chatMessages = new List<ChatMessage>();
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
                m_chatMessages.Add(message);
                m_chatMessages.Sort((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));

                //try
                //{
                //    if (PropertyChanged != null)
                //        PropertyChanged(this, new PropertyChangedEventArgs("ChatMessages"));
                //}
                //catch (Exception) { }

                //try
                //{
                //    if (CollectionChanged != null)
                //        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
                //}
                //catch (Exception) { }
            }
        }


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
        public ChatMessage this[int i]
        {
            get { return ChatMessages[i]; }
            protected set { ChatMessages[i] = value; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
