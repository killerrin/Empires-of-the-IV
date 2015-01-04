using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Anarian.GUI.Components;
using Anarian.Interfaces;
using Anarian.DataStructures;
using Anarian.Events;

namespace Anarian.GUI
{
    public class Menu : AnarianObject,
                           IScene2D, IUpdatable, Anarian.Interfaces.IDrawable
    {
        Transform2D m_sceneNode;
        public Transform2D SceneNode
        {
            get { return m_sceneNode; }
            protected set { m_sceneNode = value; }
        }

        public Menu()
            :base()
        {
            // When Creating the Base SceneNode, we will set
            // its Scale to Zero so that SceneNodes which get
            // added as children aren't screwed up
            GuiObject node = new GuiObject();
            node.Transform.Scale = Vector2.Zero;
            m_sceneNode = node.Transform;
        }

        #region Interface Implimentation
        #region IScene2D
        Transform2D IScene2D.SceneNode
        {
            get { return SceneNode; }
            set { }
        }
        void IScene2D.HandlePointerDown(object sender, PointerPressedEventArgs e) { HandlePointerDown(sender, e); }
        void IScene2D.HandlePointerPressed(object sender, PointerPressedEventArgs e) { HandlePointerPressed(sender, e); }
        void IScene2D.HandlePointerMoved(object sender, PointerMovedEventArgs e) { HandlePointerMoved(sender, e); }
        #endregion

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }

        void Anarian.Interfaces.IDrawable.Draw(GameTime gameTime, SpriteBatch spriteBatch) { Draw(gameTime, spriteBatch); }
        #endregion

        public void Update(GameTime gameTime)
        {
            m_sceneNode.GuiObject.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            m_sceneNode.GuiObject.Draw(gameTime, spriteBatch);
        }

        #region HandleEvents
        internal void HandlePointerDown(object sender, PointerPressedEventArgs e)
        {
        }

        internal void HandlePointerPressed(object sender, PointerPressedEventArgs e)
        {
        }

        internal void HandlePointerMoved(object sender, PointerMovedEventArgs e)
        {
        }
        #endregion
    }
}
