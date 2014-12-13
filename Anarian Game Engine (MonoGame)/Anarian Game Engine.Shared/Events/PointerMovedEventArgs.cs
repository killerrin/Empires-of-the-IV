using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Anarian.Enumerators;

namespace Anarian.Events
{
    public delegate void PointerMovedEventHandler(object sender, PointerMovedEventArgs e);

    public class PointerMovedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public PointerMovedEventArgs()
            : base(new Exception(), false, null)
        {
        }
        public PointerMovedEventArgs(Exception e, bool canceled, Object state)
            : base(e, canceled, state)
        {
        }
    }
}
