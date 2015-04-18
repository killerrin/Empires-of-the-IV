using KillerrinStudiosToolkit.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Data_Models
{
    public class ChatMessage : IComparable<ChatMessage>, IEquatable<ChatMessage>
    {
        public Uri Image { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
       
        public DateTime TimeStamp { get; set; }
        public string TimeStampAsString { get { return RelativeDateTimeConverter.CalculateConversion(TimeStamp); } }

        public ChatMessage() 
        {
            Image = new Uri("http://www.killerrin.com", UriKind.Absolute);
            Name = "";
            Message = "";
            TimeStamp = DateTime.UtcNow;
        }
        public ChatMessage(Uri image, string name, string message, DateTime timeStamp)
        {
            Image = image;
            Name = name;
            Message = message;
            TimeStamp = timeStamp;
        }
        public ChatMessage(Uri image, string name, string message)
        {
            Image = image;
            Name = name;
            Message = message;
            TimeStamp = DateTime.UtcNow;
        }

        int IComparable<ChatMessage>.CompareTo(ChatMessage obj) { return CompareTo(obj); }
        public int CompareTo(ChatMessage obj) { return TimeStamp.CompareTo(obj.TimeStamp); }

        bool IEquatable<ChatMessage>.Equals(ChatMessage other) { return Equals(other); }
        public bool Equals(ChatMessage other) 
        {
            return (Name.Equals(other.Name) &&
                    Message.Equals(other.Message));
        }

        public override string ToString() { return Name + ", at " + TimeStamp.ToString() + " : " + Message; }

        #region Serialization Tools
        public void SetFromOtherMessage(ChatMessage o)
        {
            Image = o.Image;
            Name = o.Name;
            Message = o.Message;
            TimeStamp = o.TimeStamp;
        }

        public string ThisToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void JsonToThis(string json)
        {
            JObject jObject = JObject.Parse(json);
            ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(jObject.ToString());

            SetFromOtherMessage(message);
        }
        #endregion
    }
}
