using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void MouseClickedEventHandler(object sender, MouseClickedEventArgs e);
    public delegate void MouseDownEventHandler(object sender, MouseClickedEventArgs e);

    public class MouseClickedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public MouseButtonClick ButtonClicked { get; private set; }
        public Vector2 Position { get; private set; }


        public MouseClickedEventArgs()
            : base(new Exception(), false, null)
        {
            ButtonClicked = MouseButtonClick.None;
            Position = new Vector2(-1.0f, -1.0f);
        }
        public MouseClickedEventArgs(MouseButtonClick mouseButtonClicked, Vector2 mousePosition)
            : base(new Exception(), false, null)
        {
            ButtonClicked = mouseButtonClicked;
            Position = mousePosition;
        }
        public MouseClickedEventArgs(MouseButtonClick mouseButtonClicked, Vector2 mousePosition, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ButtonClicked = mouseButtonClicked;
            Position = mousePosition;
        }
    }
}
