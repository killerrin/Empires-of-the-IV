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
            //m_game.SceneManager.CurrentScene.Camera.MoveVertical(30.0f);

            // Load the Assets
            m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Texture2D), "KillerrinStudiosLogo");
            m_game.ResourceManager.LoadAsset(m_game.Content, typeof(AnimatedModel), "t-pose_3");
            
            // Load the Terrain
            Texture2D heightMap = m_game.Content.Load<Texture2D>("heightmap");
            Texture2D grassTexture = m_game.Content.Load<Texture2D>("grassTexture");
            //Terrain m_terrain = new Terrain(graphics, heightMap, grassTexture);
           
            // Create the Game Objects
            Unit armyGuy = new Unit();
            armyGuy.Model3D = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
            armyGuy.Transform.Scale = new Vector3(0.007f);
            armyGuy.Transform.Position = new Vector3(0.2f, -0.5f, -5.50f);
            armyGuy.CullDraw = false;
            //armyGuy.RenderBounds = true;

            // Load the models which contain animations
            AnimatedModel walk = CustomContentLoader.LoadAnimatedModel(m_game.Content, "walk");
            AnimationClip clip = walk.Clips[0];
            
            // Set our animations to the gameObjects
            AnimationPlayer armyGuyPlayer = armyGuy.PlayClip(clip);
            armyGuyPlayer.Looping = true;

            // Create Planets
            Texture2D tyrilTexture = m_game.Content.Load<Texture2D>("Planet Textures\\Tyril Texture");
            Planet tyril = new Planet(graphics, tyrilTexture, MathHelper.ToRadians(-25.0f), Vector3.Zero, new Vector3(0.00020f, 0.0f, 0.0f), Vector3.Zero);
            tyril.Transform.Scale = new Vector3(10.0f);

            Texture2D hopeTexture = m_game.Content.Load<Texture2D>("Planet Textures\\Hope Texture");
            Planet hope = new Planet(graphics, hopeTexture, MathHelper.ToRadians(-20.0f), new Vector3(20.0f, 2.0f, 0.0f), new Vector3(0.00050f, 0.0f, 0.0f), new Vector3(0.00040f, 0.0f, 0.0f));
            hope.Transform.Scale = new Vector3(1.5f);

            Texture2D yolTexture = m_game.Content.Load<Texture2D>("Planet Textures\\Yol Texture");
            Planet yol = new Planet(graphics, yolTexture, MathHelper.ToRadians(-15.0f), new Vector3(2.0f, 20.0f, 0.0f), new Vector3(0.00040f, 0.0f, 0.0f), new Vector3(0.0f, 0.00040f, 0.0f));
            yol.Transform.Scale = new Vector3(1.2f);
            
            Texture2D luraTexture = m_game.Content.Load<Texture2D>("Planet Textures\\Lura Texture");
            Planet lura = new Planet(graphics, luraTexture, MathHelper.ToRadians(30.0f), new Vector3(-30.0f, 30.0f, 2.0f), new Vector3(0.00020f, 0.0f, 0.0f), new Vector3(0.00020f, 0.00020f, 0.0f));
            lura.Transform.Scale = new Vector3(2.0f);
            
            tyril.AddSatellite(hope);
            tyril.AddSatellite(yol);
            tyril.AddSatellite(lura);

            // Add to the Scene
            //m_game.SceneManager.CurrentScene.SceneNode.AddChild(m_terrain.Transform);
            //m_game.SceneManager.CurrentScene.SceneNode.AddChild(armyGuy.Transform);
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(tyril.Transform);
        }
    }
}
