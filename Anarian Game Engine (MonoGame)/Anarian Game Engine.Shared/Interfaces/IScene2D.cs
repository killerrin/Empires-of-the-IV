using System;
using System.Collections.Generic;
using System.Text;

using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.GUI.Components;

namespace Anarian.Interfaces
{
    public interface IScene2D
    {
        Transform2D SceneNode
        {
            get;
            set;
        }
    }
}
