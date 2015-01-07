using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.DataStructures
{
    public class Timer : AnarianObject, IGameComponent, IUpdatable
    {
        #region Fields/Properties
        bool m_completed;
        public bool IsCompleted { get { return m_completed; } protected set { m_completed = value; } }

        TimeSpan m_lastTick;
        public TimeSpan LastTick { get { return m_lastTick; } protected set { m_lastTick = value; } }

        TimeSpan m_interval;
        public TimeSpan Interval { get { return m_interval; } set { m_interval = value; } }

        public TimeSpan TimeRemaining { get { return m_interval - m_lastTick; } }
        #endregion

        public Timer(TimeSpan interval)
            :base()
        {
            m_interval = interval;
            Reset();
        }

        public virtual void Reset()
        {
            m_completed = false;
            m_lastTick = TimeSpan.Zero; // new TimeSpan();
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IGameComponent.Initialize() { Reset(); }
        #endregion

        #region Helper Methods
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            if (m_completed) return;

            m_lastTick += gameTime.ElapsedGameTime;

            if (m_lastTick < m_interval) {
                if (Tick != null)
                    Tick(this, null);
            }
            else {
                m_completed = true;

                if (Completed != null)
                    Completed(this, null);
            }
        }

        public event EventHandler Tick;
        public event EventHandler Completed;
    }
}
