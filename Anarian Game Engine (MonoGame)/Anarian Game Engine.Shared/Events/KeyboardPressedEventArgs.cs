using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void KeyboardClickedEventHandler(object sender, KeyboardClickedEventArgs e);
    public delegate void KeyboardDownEventHandler(object sender, KeyboardClickedEventArgs e);

    public class KeyboardClickedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public Keys KeyClicked { get; private set; }

        public KeyboardClickedEventArgs()
            : base(new Exception(), false, null)
        {
            KeyClicked = Keys.None;
        }
        public KeyboardClickedEventArgs(Keys keyboardKeyClicked)
            : base(new Exception(), false, null)
        {
            KeyClicked = keyboardKeyClicked;
        }
        public KeyboardClickedEventArgs(Keys keyboardKeyClicked, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            KeyClicked = keyboardKeyClicked;
        }
    }
}
