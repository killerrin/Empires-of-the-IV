using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void MouseMovedEventHandler(object sender, MouseMovedEventArgs e);

    public class MouseMovedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public Vector2 Position { get; private set; }
        public Vector2 DeltaPosition { get; private set;}

        public MouseMovedEventArgs()
            : base(new Exception(), false, null)
        {
            Position = new Vector2(-1.0f, -1.0f);
            DeltaPosition = Vector2.Zero;
        }
        public MouseMovedEventArgs(Vector2 mousePosition, Vector2 deltaMousePosition)
            : base(new Exception(), false, null)
        {
            Position = mousePosition;
            DeltaPosition = deltaMousePosition;
        }
        public MouseMovedEventArgs(Vector2 mousePosition, Vector2 deltaMousePosition, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            Position = mousePosition;
            DeltaPosition = deltaMousePosition;
        }
    }
}
