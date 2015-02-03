using KillerrinStudiosToolkit.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Data_Models
{
    public class ChatMessage
    {
        public Uri Image { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
       
        public DateTime TimeStamp { get; set; }
        public string TimeStampAsString { get { return RelativeDateTimeConverter.CalculateConversion(TimeStamp); } }

        public ChatMessage(Uri image, string name, string message, DateTime timeStamp)
        {
            Image = image;
            Name = name;
            Message = message;
        }
    }
}
