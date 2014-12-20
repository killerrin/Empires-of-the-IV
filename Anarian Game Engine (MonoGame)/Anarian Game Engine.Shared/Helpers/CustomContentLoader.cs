using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Anarian.DataStructures.Animation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Anarian.Helpers
{
    public static class CustomContentLoader
    {

        public static AnimatedModel LoadAnimatedModel(ContentManager content, string assetName)
        {
            AnimationAux.AnimatedModel oldModel = new AnimationAux.AnimatedModel(assetName);
            oldModel.LoadContent(content);

            AnimatedModel model = new AnimatedModel(oldModel);
            return model;
        }
    }
}
