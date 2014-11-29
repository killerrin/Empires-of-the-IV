using System;
using System.Collections.Generic;
using System.Text;
using Anarian.DataStructures;

namespace Anarian.Interfaces
{
    public interface IScene
    {
        //Camera GetCamera();
        //void SetCamera(Camera cam);

        Camera Camera
        {
            get;
            set;
        }

        //GameObject GetSceneNode();
        GameObject SceneNode
        {
            get;
            set;
        }
    }
}
