using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Anarian.Interfaces;
using Anarian.Events;

namespace Anarian.DataStructures.Input
{
    public class Controller : IUpdatable
    {
        PlayerIndex m_playerIndex;
        GamePadType m_gamePadType;

        GamePadCapabilities m_gamePadCapabilities;

        GamePadState m_gamePadState;
        GamePadState m_prevGamePadState;

        bool m_isConnected;

        public PlayerIndex PlayerIndex { get { return m_playerIndex; } }
        public GamePadType GamePadType { get { return m_gamePadType; } }
        public GamePadCapabilities GamePadCapabilities { get { return m_gamePadCapabilities; } }
        public GamePadState GamePadState { get { return m_gamePadState; } }
        public GamePadState PrevGamePadState { get { return m_prevGamePadState; } }
        public bool IsConnected { get { return m_isConnected; } }

        public Controller(PlayerIndex playerIndex)
        {
            m_playerIndex = playerIndex;
            Reset();
        }

        public void Reset()
        {
            m_gamePadCapabilities = GamePad.GetCapabilities(m_playerIndex);
            m_gamePadType = m_gamePadCapabilities.GamePadType;

            m_gamePadState = GamePad.GetState(m_playerIndex);
            m_prevGamePadState = m_gamePadState;

            m_isConnected = m_gamePadState.IsConnected;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            //if (!m_isConnected) return;

            // Get the States
            m_prevGamePadState = m_gamePadState;
            m_gamePadState = GamePad.GetState(m_playerIndex);

            // Update if it is connected or not
            m_isConnected = m_gamePadState.IsConnected;
            
            // Preform Events
        }

        #region Helper Methods
        public bool ButtonPressed(Buttons button)
        {
            if (m_prevGamePadState.IsButtonDown(button) == true &&
                m_gamePadState.IsButtonUp(button) == true)
                return true;
            return false;
        }

        public bool IsButtonDown(Buttons button)
        {
            return m_gamePadState.IsButtonDown(button);
        }
        public bool IsButtonUp(Buttons button)
        {
            return m_gamePadState.IsButtonUp(button);
        }

        public void SetVibration(float leftMotor, float rightMotor)
        {
            GamePad.SetVibration(m_playerIndex, leftMotor, rightMotor);
        }
        #endregion

        #region Events
        public event GamePadDownEventHandler GamePadDown;
        public event GamePadPressedEventHandler GamePadClicked;
        #endregion
    }
}
