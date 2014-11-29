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
    public class TouchScreen : IUpdatable
    {
        TouchCollection m_touchCollection;
        TouchCollection m_prevTouchCollection;

        TouchPanelCapabilities m_touchPanelCapabilities;

        bool m_isConnected;
        bool m_isReadOnly;

        #region Properties
        public TouchCollection TouchCollection { get { return m_touchCollection; } }
        public TouchCollection PrevTouchCollection { get { return m_prevTouchCollection; } }
        
        public TouchPanelCapabilities TouchPanelCapabilities { get { return m_touchPanelCapabilities; } }

        public GestureSample ReadGesture { get { return TouchPanel.ReadGesture(); } }

        public GestureType EnabledGestures {
            get { return TouchPanel.EnabledGestures; }
            set { TouchPanel.EnabledGestures = value; }
        }

        public DisplayOrientation DislayOrientation { get { return TouchPanel.DisplayOrientation; } }

        public int DisplayWidth { get { return TouchPanel.DisplayWidth; } }
        public int DisplayHeight { get { return TouchPanel.DisplayHeight; } }

        public bool IsConnected { get { return m_isConnected; } }
        public bool IsReadOnly  { get { return m_isReadOnly; } }
        public bool IsGestureAvailable { get { return TouchPanel.IsGestureAvailable; } }

        public bool EnableMouseGestures {
            get { return TouchPanel.EnableMouseGestures; }
            set { TouchPanel.EnableMouseGestures = value; }
        }
        public bool EnableMouseTouchPoint {
            get { return TouchPanel.EnableMouseTouchPoint; }
            set { TouchPanel.EnableMouseTouchPoint = value; }
        }
        #endregion

        public TouchScreen()
        {
            m_touchCollection = TouchPanel.GetState();
            m_prevTouchCollection = m_touchCollection;

            m_isConnected = m_touchCollection.IsConnected;
            m_isReadOnly = m_touchCollection.IsReadOnly;
            m_touchPanelCapabilities = TouchPanel.GetCapabilities();
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public void Update(GameTime gameTime)
        {
            m_prevTouchCollection = m_touchCollection;
            m_touchCollection = TouchPanel.GetState();
        }

    }
}
