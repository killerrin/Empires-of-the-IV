using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void PointerDownEventHandler(object sender, PointerPressedEventArgs e);
    public delegate void PointerPressedEventHandler(object sender, PointerPressedEventArgs e);

    public class PointerPressedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        public PointerPressedEventArgs()
            : base(new Exception(), false, null)
        {

        }
        public PointerPressedEventArgs(Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {

        }
    }
}
