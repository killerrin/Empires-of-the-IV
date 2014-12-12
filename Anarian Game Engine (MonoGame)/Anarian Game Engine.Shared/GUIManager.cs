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
            InputManager.Instance.GetController(PlayerIndex.One).GamePadDown += GUIManager_GamePadOneDown;
            InputManager.Instance.GetController(PlayerIndex.One).GamePadClicked += GUIManager_GamePadOneClicked;

            InputManager.Instance.GetController(PlayerIndex.Two).GamePadDown += GUIManager_GamePadTwoDown;
            InputManager.Instance.GetController(PlayerIndex.Two).GamePadClicked += GUIManager_GamePadTwoClicked;

            InputManager.Instance.GetController(PlayerIndex.Three).GamePadDown += GUIManager_GamePadThreeDown;
            InputManager.Instance.GetController(PlayerIndex.Three).GamePadClicked += GUIManager_GamePadThreeClicked;

            InputManager.Instance.GetController(PlayerIndex.Four).GamePadDown += GUIManager_GamePadFourDown;
            InputManager.Instance.GetController(PlayerIndex.Four).GamePadClicked += GUIManager_GamePadFourClicked;
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
        #region Controller One
        private void GUIManager_GamePadOneClicked(object sender, GamePadPressedEventArgs e)
        {


        }

        void GUIManager_GamePadOneDown(object sender, GamePadPressedEventArgs e)
        {

        }
        #endregion

        #region Controller Two
        private void GUIManager_GamePadTwoClicked(object sender, GamePadPressedEventArgs e)
        {


        }

        void GUIManager_GamePadTwoDown(object sender, GamePadPressedEventArgs e)
        {

        }
        #endregion

        #region Controller Three
        private void GUIManager_GamePadThreeClicked(object sender, GamePadPressedEventArgs e)
        {


        }

        void GUIManager_GamePadThreeDown(object sender, GamePadPressedEventArgs e)
        {

        }
        #endregion

        #region Controller Four
        private void GUIManager_GamePadFourClicked(object sender, GamePadPressedEventArgs e)
        {


        }

        void GUIManager_GamePadFourDown(object sender, GamePadPressedEventArgs e)
        {

        }
        #endregion
        #endregion
    }
}
