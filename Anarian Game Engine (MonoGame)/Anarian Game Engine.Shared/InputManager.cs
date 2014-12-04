using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Anarian.Interfaces;
using Anarian.DataStructures;
using Anarian.DataStructures.Input;

namespace Anarian
{
    public class InputManager : IUpdatable
    {
        #region Singleton
        static InputManager m_instance;
        public static InputManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new InputManager();
                return m_instance;
            }
            set { }
        }
        #endregion


        #region Properties
        MouseManager m_mouse;
        KeyboardManager m_keyboard;

        Controller m_controller1;
        Controller m_controller2;
        Controller m_controller3;
        Controller m_controller4;

        TouchScreen m_touchScreen;


        public MouseManager Mouse { get { return m_mouse; } }
        public KeyboardManager Keyboard { get { return m_keyboard; } }
        public TouchScreen TouchScreen { get { return m_touchScreen; } }
        public Controller GetController(PlayerIndex index)
        {
            switch (index) {
                case PlayerIndex.Two:   return m_controller2;
                case PlayerIndex.Three: return m_controller3;
                case PlayerIndex.Four:  return m_controller4;
                default:
                case PlayerIndex.One:   return m_controller1;
            }
        }
        #endregion

        InputManager()
        {
            m_touchScreen = new TouchScreen();

//#if WINDOWS_APP
            m_mouse = new MouseManager();
            m_keyboard = new KeyboardManager();

            m_controller1 = new Controller(PlayerIndex.One);
            m_controller2 = new Controller(PlayerIndex.Two);
            m_controller3 = new Controller(PlayerIndex.Three);
            m_controller4 = new Controller(PlayerIndex.Four);
//#endif
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            // Update Universal Inputs
            m_touchScreen.Update(gameTime);
            
            // Update Windows Only Inputs
//#if WINDOWS_APP
            m_mouse.Update(gameTime);
            m_keyboard.Update(gameTime);

            m_controller1.Update(gameTime);
            m_controller2.Update(gameTime);
            m_controller3.Update(gameTime);
            m_controller4.Update(gameTime);
//#endif
        }
    }
}
