using Anarian.Events;
using KillerrinStudiosToolkit.Datastructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public delegate void EotIVPacketRecievedEventHandler(object sender, EotIVPacketRecievedEventArgs e);

    public class EotIVPacketRecievedEventArgs : AnarianEventArgs
    {
        public EotIVPacket Packet { get; protected set; }

        public EotIVPacketRecievedEventArgs()
            : base()
        {
            Packet = null;
        }
        public EotIVPacketRecievedEventArgs(EotIVPacket packet)
            : base(new GameTime())
        {
            Packet = packet;
        }
        public EotIVPacketRecievedEventArgs(EotIVPacket packet, Exception e, bool canceled, Object state)
            : base(new GameTime(), e, canceled, state)
        {
            Packet = packet;
        }
    }
}
