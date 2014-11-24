using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Anarian.Interfaces
{
    public interface IMoveable
    {
        void Move(GameTime gameTime, Vector3 movement);


        void MoveUp(GameTime gameTime, float amount);
        void MoveDown(GameTime gameTime, float amount);

        void MoveLeft(GameTime gameTime, float amount);
        void MoveRight(GameTime gameTime, float amount);

        void MoveForward(GameTime gameTime, float amount);
        void MoveBack(GameTime gameTime, float amount);
    }
}
