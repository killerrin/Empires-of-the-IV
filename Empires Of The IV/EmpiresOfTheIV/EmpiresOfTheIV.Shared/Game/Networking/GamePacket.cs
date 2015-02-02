using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public class GamePacket : EotIVPacket
    {

        public GamePacket(bool requiresAck)
            : base(requiresAck, PacketType.GameData)
        {
        }

        public override void SetFromOtherPacket(GamePacket o)
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
            GamePacket packet = JsonConvert.DeserializeObject<GamePacket>(jObject.ToString());

            SetFromOtherPacket(packet);
        }
    }
}
