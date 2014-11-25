using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace Anarian
{
    public class ResourceManager
    {
        #region Singleton
        static ResourceManager m_instance;
        public static ResourceManager Instance
        {
            get {
                if (m_instance == null) m_instance = new ResourceManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        Dictionary<string, Texture2D> m_textures;
        Dictionary<string, Model> m_models;

        private ResourceManager()
        {
            m_textures = new Dictionary<string, Texture2D>();
            m_models = new Dictionary<string, Model>();
        }

        private string AssetName(string assetName)
        {
            char[] splitChars = {'/', '\\' };
            string[] assetNameSplit = assetName.Split(splitChars);
            return assetNameSplit[assetNameSplit.Length - 1];
        }

        public void LoadTexture(ContentManager Content, string assetName) {
            m_textures.Add(AssetName(assetName), Content.Load<Texture2D>(assetName)); 
        }
        public void LoadModel(ContentManager Content, string assetName) {
            m_models.Add(AssetName(assetName), Content.Load<Model>(assetName));
        }

        #region Add
        public void AddTexture(Texture2D texture, string assetName)
        {
            m_textures.Add(assetName, texture);
        }
        #endregion

        #region Get
        public Texture2D GetTexture(string key) { return m_textures[key]; }
        public Model GetModel(string key) { return m_models[key]; }
        #endregion
    }
}
