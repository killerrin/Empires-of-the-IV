using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Anarian.Interfaces;

namespace Anarian.DataStructures.Input
{
    public class KeyboardManager : IUpdatable
    {
        #region Fields and Properties
        KeyboardState m_keyboardState;
        KeyboardState m_prevKeyboardState;

        public KeyboardState KeyboardState { get { return m_keyboardState; } }
        public KeyboardState PrevKeyboardState { get { return m_prevKeyboardState; } }
        #endregion

        public KeyboardManager()
        {
            m_keyboardState = Keyboard.GetState();
            m_prevKeyboardState = m_keyboardState;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            m_prevKeyboardState = m_keyboardState;
            m_keyboardState = Keyboard.GetState();
        }

        #region Helper Methods

        #endregion

    }
}
