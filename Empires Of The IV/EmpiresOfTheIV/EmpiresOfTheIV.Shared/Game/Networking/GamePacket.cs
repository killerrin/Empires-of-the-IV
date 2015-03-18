using EmpiresOfTheIV.Game.Commands;
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
        public GamePacketID ID { get; set; }
        public Command Command { get; set; }


        public GamePacket(bool requiresAck, Command command, GamePacketID id)
            : base(requiresAck, PacketType.GameData)
        {
            ID = id;
            Command = command;
        }

        public void SetFromOtherPacket(GamePacket o)
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
            GamePacket packet = JsonConvert.DeserializeObject<GamePacket>(jObject.ToString());

            SetFromOtherPacket(packet);
        }
    }
}
