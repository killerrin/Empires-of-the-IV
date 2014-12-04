using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Anarian.Interfaces;
using Anarian.Enumerators;
using Anarian.Events;

namespace Anarian.DataStructures.Input
{
    public class MouseManager : IUpdatable
    {
        #region Fields and Properties
        MouseState m_mouseState;
        MouseState m_prevMouseState;

        Point m_mouseFixedPos;
        bool m_isMouseFixed;


        public MouseState MouseState { get { return m_mouseState; } }
        public MouseState PrevMouseState { get { return m_prevMouseState; } }

        /// <summary>
        /// Gets or Sets the Fixed Mouse Position of the Mouse. 
        /// Set to null to unfix mouse
        /// Set to value to Fix Mouse to that position
        /// </summary>
        public Point MousedFixedPosition { 
            get { return m_mouseFixedPos; }
            set
            {
                if (value == null) {
                    m_isMouseFixed = false;
                }
                else {
                    m_isMouseFixed = true;
                    m_mouseFixedPos = value;
                }

            }
        }
        public bool IsMouseFixed { get { return m_isMouseFixed; } }
        #endregion 

        public MouseManager()
        {
            m_mouseState = Mouse.GetState();
            m_prevMouseState = m_mouseState;

            m_isMouseFixed = false;
            m_mouseFixedPos = new Point(-1, -1);
        }


        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            m_prevMouseState = m_mouseState;
            m_mouseState = Mouse.GetState();

            // If we have subscribers to the events, call them now
            if (MouseClicked != null) {
                if (LeftMouseClicked()) 
                    MouseClicked(this, new MouseClickedEventArgs(MouseButtonClick.LeftMouseButton, GetMousePosition()));
                if (MiddleMouseClicked())
                    MouseClicked(this, new MouseClickedEventArgs(MouseButtonClick.MiddleMouseButton, GetMousePosition()));
                if (RightMouseClicked()) 
                    MouseClicked(this, new MouseClickedEventArgs(MouseButtonClick.RightMouseButton, GetMousePosition()));
            }

            if (MouseMoved != null) {
                if (m_mouseState.Position != m_prevMouseState.Position) 
                    MouseMoved(this, new MouseMovedEventArgs(GetMousePosition(), GetMouseDelta()));
            }

            // If Mouse is Fixed, Place it back to its fixed location
            if (m_isMouseFixed) {
                SetMousePosition(m_mouseFixedPos.X, m_mouseFixedPos.Y);
            }
        }

        #region Helper Methods
        public bool LeftMouseClicked()
        {
            if (m_prevMouseState.LeftButton == ButtonState.Pressed &&
                m_mouseState.LeftButton == ButtonState.Released)
                return true;
            return false;
        }
        public bool RightMouseClicked()
        {
            if (m_prevMouseState.RightButton == ButtonState.Pressed &&
                m_mouseState.RightButton == ButtonState.Released)
                return true;
            return false;
        }
        public bool MiddleMouseClicked()
        {
            if (m_prevMouseState.MiddleButton == ButtonState.Pressed &&
                m_mouseState.MiddleButton == ButtonState.Released)
                return true;
            return false;
        }

        public Vector2 GetMousePosition() { return m_mouseState.Position.ToVector2(); }
        public Vector2 GetMouseDelta() { return m_mouseState.Position.ToVector2() - m_prevMouseState.Position.ToVector2(); }

        public void SetMousePosition(int x, int y) { Mouse.SetPosition(x, y); }
        #endregion

        #region Events
        public event MouseClickedEventHandler MouseClicked;
        public event MouseMovedEventHandler MouseMoved;
        #endregion

    }
}
