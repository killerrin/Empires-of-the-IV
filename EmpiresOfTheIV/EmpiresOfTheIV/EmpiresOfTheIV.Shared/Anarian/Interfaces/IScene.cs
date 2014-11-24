using System;
using System.Collections.Generic;
using System.Text;
using Anarian.DataStructures;

namespace Anarian.Interfaces
{
    public interface IScene
    {
        Camera GetCamera();
        void SetCamera(Camera cam);

        GameObject GetSceneNode();
    }
}
