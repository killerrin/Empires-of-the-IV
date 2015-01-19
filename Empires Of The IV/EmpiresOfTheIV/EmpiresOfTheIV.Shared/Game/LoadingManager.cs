using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using KillerrinStudiosToolkit;

using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Input;
using Anarian.DataStructures.Rendering;
using Anarian.Enumerators;
using Anarian.Helpers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.GameObjects;

namespace EmpiresOfTheIV.Game
{
    public class LoadingManager
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties
        bool m_currentlyLoading;
        public bool CurrentlyLoading { get { return m_currentlyLoading; } protected set { m_currentlyLoading = value; } }

        float m_loadingPercentage;
        public float LoadingPercentage { get { return m_loadingPercentage; } protected set { m_loadingPercentage = value; } }

        private object m_loadingLock = new object();
        #endregion

        public LoadingManager(EmpiresOfTheIVGame game)
        {
            m_game = game;

            ResetLoad();
        }

        public void ResetLoad()
        {
            m_loadingPercentage = 0.0f;
            m_currentlyLoading = true;
        }

        public void LoadContent(GraphicsDevice graphics)
        {
            // Load the Assets
            m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Texture2D), "KillerrinStudiosLogo");
            m_game.ResourceManager.LoadAsset(m_game.Content, typeof(AnimatedModel), "t-pose_3");
            m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Model), "Tyril");
            
            // Load the Terrain
            //Texture2D heightMap = m_game.Content.Load<Texture2D>("heightmap");
            //Texture2D grassTexture = m_game.Content.Load<Texture2D>("grassTexture");
            //Terrain m_terrain = new Terrain(graphics, heightMap, grassTexture);
            //m_game.SceneManager.CurrentScene.SceneNode.AddChild(m_terrain.Transform);
            
            // Create the Game Objects
            Unit armyGuy = new Unit();
            armyGuy.Model3D = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
            armyGuy.Transform.Scale = new Vector3(0.007f);
            armyGuy.Transform.Position = new Vector3(0.2f, -0.5f, -5.50f);

            Planet tyril = new Planet();
            tyril.Model3D = m_game.ResourceManager.GetAsset(typeof(Model), "Tyril") as Model;
            tyril.Transform.Scale = new Vector3(100.0f);
            tyril.Transform.Position = new Vector3(0.0f, 0.0f, -10.0f);

            // Add to the Scene
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(armyGuy.Transform);
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(tyril.Transform);
            
            //// Load the models which contain animations
            //AnimatedModel walk = CustomContentLoader.LoadAnimatedModel(m_game.Content, "walk");
            //AnimationClip clip = walk.Clips[0];
            //
            //// Set our animations to the gameObjects
            //AnimationPlayer armyGuyPlayer = armyGuy.PlayClip(clip);
            //armyGuyPlayer.Looping = true;



            
            //m_game.SceneManager.CurrentScene.Camera.MoveVertical(30.0f);
        }
    }
}
