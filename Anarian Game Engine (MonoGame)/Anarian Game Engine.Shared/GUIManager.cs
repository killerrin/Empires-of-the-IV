using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

using Microsoft.Xna.Framework;

using Anarian.GUI;
using Anarian.DataStructures.Input;
using Anarian.Enumerators;
using Anarian.Events;

namespace Anarian
{
    public class GUIManager
    {
        #region Singleton
        static GUIManager m_instance;
        public static GUIManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new GUIManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        private GUIManager()
        {
            // Subscribe to Mouse Events
            InputManager.Instance.Mouse.MouseDown += Mouse_MouseDown;
            InputManager.Instance.Mouse.MouseClicked += Mouse_MouseClicked;
            InputManager.Instance.Mouse.MouseMoved += Mouse_MouseMoved;

            // Subscribe to Keyboard Events
            InputManager.Instance.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            InputManager.Instance.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;

            // Subscribe to Touchscreen Events


            // Subscribe to Controller Events
            Controller.GamePadDown += GUIManager_GamePadDown;
            Controller.GamePadClicked += GUIManager_GamePadClicked;
            Controller.GamePadMoved += GUIManager_GamePadMoved;
        }

        #region Mouse Events
        void Mouse_MouseMoved(object sender, MouseMovedEventArgs e)
        {
            
        }

        void Mouse_MouseClicked(object sender, MouseClickedEventArgs e)
        {
            
        }

        void Mouse_MouseDown(object sender, MouseClickedEventArgs e)
        {
            
        }
        #endregion

        #region Keyboard Events
        void Keyboard_KeyboardPressed(object sender, KeyboardPressedEventArgs e)
        {
            
        }

        void Keyboard_KeyboardDown(object sender, KeyboardPressedEventArgs e)
        {

        }
        #endregion

        #region Controller Events
        void GUIManager_GamePadDown(object sender, GamePadPressedEventArgs e)
        {

        }

        private void GUIManager_GamePadClicked(object sender, GamePadPressedEventArgs e)
        {

        }

        private void GUIManager_GamePadMoved(object sender, GamePadMovedEventsArgs e)
        {
        }
        #endregion
    }
}
