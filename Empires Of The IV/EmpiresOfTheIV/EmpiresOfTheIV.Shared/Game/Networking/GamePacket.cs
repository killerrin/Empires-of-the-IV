using EmpiresOfTheIV.Game.Enumerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public class GamePacket : EotIVPacket
    {
        CommandType CommandType { get; set; }

        public GamePacket(bool requiresAck, CommandType commandType)
            : base(requiresAck, PacketType.GameData)
        {
            CommandType = commandType;
        }

        public void SetFromOtherPacket(GamePacket o)
        {
            base.SetFromOtherPacket(o);
            CommandType = o.CommandType;
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
