using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void GamePadPressedEventHandler(object sender, GamePadPressedEventArgs e);
    public delegate void GamePadDownEventHandler(object sender, GamePadPressedEventArgs e);

    public class GamePadPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public Buttons ButtonPressed { get; private set; }

        public GamePadPressedEventArgs()
            : base(new Exception(), false, null)
        {
            ButtonPressed = Buttons.BigButton;
        }
        public GamePadPressedEventArgs(Buttons buttonPressed)
            : base(new Exception(), false, null)
        {
            ButtonPressed = buttonPressed;
        }
        public GamePadPressedEventArgs(Buttons buttonPressed, Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
            ButtonPressed = buttonPressed;
        }
    }
}
