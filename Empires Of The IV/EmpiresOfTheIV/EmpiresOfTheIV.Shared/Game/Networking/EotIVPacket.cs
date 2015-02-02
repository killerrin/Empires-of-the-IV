using KillerrinStudiosToolkit.Datastructures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public class EotIVPacket : Packet
    {
        PacketType PacketType { get; set; }

        public EotIVPacket(bool requiresAck, PacketType packetType)
            :base(requiresAck)
        {
            PacketType = packetType;
        }

        public void SetFromOtherPacket(EotIVPacket o)
        {
            base.SetFromOtherPacket(o);
            PacketType = o.PacketType;
        }

        public override string ThisToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override void JsonToThis(string json)
        {
            JObject jObject = JObject.Parse(json);
            EotIVPacket packet = JsonConvert.DeserializeObject<EotIVPacket>(jObject.ToString());

            SetFromOtherPacket(packet);
        }
    }
}
