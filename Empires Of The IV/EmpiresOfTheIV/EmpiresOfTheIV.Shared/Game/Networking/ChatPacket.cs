using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public class ChatPacket : EotIVPacket
    {
        string Message { get; set; }

        public ChatPacket(bool requiresAck, string message)
            : base(requiresAck, PacketType.Chat)
        {
            Message = message;
        }

        public void SetFromOtherPacket(ChatPacket o)
        {
            base.SetFromOtherPacket(o);
            Message = o.Message;
        }

        public override string ThisToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override void JsonToThis(string json)
        {
            JObject jObject = JObject.Parse(json);
            ChatPacket packet = JsonConvert.DeserializeObject<ChatPacket>(jObject.ToString());

            SetFromOtherPacket(packet);
        }
    }
}
