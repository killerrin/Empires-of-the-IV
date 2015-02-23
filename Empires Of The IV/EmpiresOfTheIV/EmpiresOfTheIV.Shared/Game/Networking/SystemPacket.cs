using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public class SystemPacket : EotIVPacket
    {
        public SystemPacketID ID { get; set; }
        public string Command { get; set; }

        public SystemPacket(bool requiresAck, SystemPacketID id, string command)
            : base(requiresAck, PacketType.System)
        {
            ID = id;
            Command = command;
        }

        public void SetFromOtherPacket(SystemPacket o)
        {
            base.SetFromOtherPacket(o);
            ID = o.ID;
            Command = o.Command;
        }

        public override string ThisToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override void JsonToThis(string json)
        {
            JObject jObject = JObject.Parse(json);
            SystemPacket packet = JsonConvert.DeserializeObject<SystemPacket>(jObject.ToString());

            SetFromOtherPacket(packet);
        }
    }
}
