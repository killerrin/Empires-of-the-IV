using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Anarian.Enumerators;

namespace Anarian.DataStructures
{
    public class Component
    {
        #region Fields/Properties
        protected string m_name;
        public string Name
        {
            get { return m_name; }
            protected set { m_name = value; }
        }

        protected ComponentTypes m_componentType;
        public ComponentTypes ComponentType
        {
            get { return m_componentType; }
            protected set { m_componentType = value; }
        }

        protected GameObject m_gameObject;
        public GameObject GameObject
        {
            get { return m_gameObject; }
            protected set { m_gameObject = value; }
        }
        #endregion

        public Component(GameObject gameObject)
        {
            m_name = "";
            m_componentType = ComponentTypes.None;

            m_gameObject = gameObject;
        }
        public Component(GameObject gameObject, ComponentTypes componentType)
        {
            m_name = componentType.ToString();
            m_componentType = componentType;

            m_gameObject = gameObject;
        }
    }
}
