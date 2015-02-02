using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public class SystemPacket : EotIVPacket
    {

        public SystemPacket(bool requiresAck)
            : base(requiresAck, PacketType.System)
        {
        }

        public override void SetFromOtherPacket(SystemPacket o)
        {
            base.SetFromOtherPacket(o);

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
