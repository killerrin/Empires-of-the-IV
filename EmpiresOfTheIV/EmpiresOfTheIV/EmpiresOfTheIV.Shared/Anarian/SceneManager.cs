using System;
using System.Collections.Generic;
using System.Text;
using Anarian.Interfaces;

namespace Anarian
{
    public class SceneManager
    {
        #region Singleton
        static SceneManager m_instance;
        public static SceneManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new SceneManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        private SceneManager()
        {
            CurrentScene = null;
        }

        public IScene CurrentScene { get; set; }
    }
}
