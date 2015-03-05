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
using Anarian.Pathfinding;
using System.Threading.Tasks;

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
            m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Model), "Factory Base");

            // Load the Terrain
            Debug.WriteLine("Loading Terrain");
            Texture2D heightMap = m_game.Content.Load<Texture2D>("Radient Flatlands HeightMap");
            Texture2D grassTexture = m_game.Content.Load<Texture2D>("grassTexture");
            Terrain m_terrain = new Terrain(graphics, heightMap, grassTexture);
            //m_terrain.SetupGrid(true, AStarTerrainRule.ImpassableRule(RuleMeasureType.Absolute, Comparison.LessThanEqualTo, 10.0f));
            //m_terrain.DebugPrintGrid();
           
            // Create the Game Objects
            Debug.WriteLine("Loading Game Objects");
            Building unanianFactory = new Building();
            unanianFactory.Model3D = m_game.ResourceManager.GetAsset(typeof(Model), "Factory Base") as Model;
            unanianFactory.Transform.Position = new Vector3(0.0f, -10.0f, 0.0f);
            unanianFactory.CullDraw = false;
            unanianFactory.RenderBounds = true;


            Unit armyGuy = new Unit();
            armyGuy.Model3D = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
            armyGuy.Transform.Scale = new Vector3(0.007f);
            armyGuy.Transform.Position = new Vector3(0.2f, -0.5f, -5.50f);
            armyGuy.CullDraw = false;
            armyGuy.RenderBounds = true;

            // Load the models which contain animations
            Debug.WriteLine("Loading Animations");
            AnimatedModel walk = CustomContentLoader.LoadAnimatedModel(m_game.Content, "walk");
            AnimationClip clip = walk.Clips[0];

            // Set our animations to the gameObjects
            AnimationPlayer armyGuyPlayer = armyGuy.PlayClip(clip);
            armyGuyPlayer.Looping = true;

            // PC Character Limit 250 - 125 each player
            // Phone Character Limit ~60 - 30 Each Player

            List<Unit> armyUnits = new List<Unit>();
            for (int i = 0; i < GameConsts.MaxUnitsOnWindowsPhone; i++)
            {
                var unit = new Unit();
                unit.Model3D = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
                unit.Transform.Scale = new Vector3(0.007f);
                unit.Transform.Position = new Vector3((float)Consts.random.NextDouble(), -(float)Consts.random.NextDouble(), -5.50f + (float)Consts.random.NextDouble());
                unit.CullDraw = false;
                unit.RenderBounds = false;

                AnimationPlayer animPlayer = unit.PlayClip(clip);
                animPlayer.Looping = true;
                armyUnits.Add(unit);
            }

            // Create Planets
            Debug.WriteLine("Loading Tyril");
            Texture2D tyrilTexture = m_game.Content.Load<Texture2D>("Tyril Texture");
            Planet tyril = new Planet(graphics, tyrilTexture, MathHelper.ToRadians(-25.0f), Vector3.Zero, new Vector3(0.00020f, 0.0f, 0.0f), Vector3.Zero);
            tyril.Transform.Scale = new Vector3(10.0f);

            Debug.WriteLine("Loading Hope");
            //Texture2D hopeTexture = m_game.Content.Load<Texture2D>("Hope Texture");
            //Planet hope = new Planet(graphics, hopeTexture, MathHelper.ToRadians(-20.0f), new Vector3(20.0f, 2.0f, 0.0f), new Vector3(0.00050f, 0.0f, 0.0f), new Vector3(0.00040f, 0.0f, 0.0f));
            //hope.Transform.Scale = new Vector3(1.5f);

            Debug.WriteLine("Loading Yol");
            //Texture2D yolTexture = m_game.Content.Load<Texture2D>("Planet Textures\\Yol Texture");
            //Planet yol = new Planet(graphics, yolTexture, MathHelper.ToRadians(-15.0f), new Vector3(2.0f, 20.0f, 0.0f), new Vector3(0.00040f, 0.0f, 0.0f), new Vector3(0.0f, 0.00040f, 0.0f));
            //yol.Transform.Scale = new Vector3(1.2f);

            Debug.WriteLine("Loading Lura");
            //Texture2D luraTexture = m_game.Content.Load<Texture2D>("Planet Textures\\Lura Texture");
            //Planet lura = new Planet(graphics, luraTexture, MathHelper.ToRadians(30.0f), new Vector3(-30.0f, 30.0f, 2.0f), new Vector3(0.00020f, 0.0f, 0.0f), new Vector3(0.00020f, 0.00020f, 0.0f));
            //lura.Transform.Scale = new Vector3(2.0f);

            Debug.WriteLine("Adding Satellites");
            //tyril.AddSatellite(hope);
            //tyril.AddSatellite(yol);
            //tyril.AddSatellite(lura);

            // Add to the Scene
            Debug.WriteLine("Adding to Scene");
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(m_terrain.Transform);
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(armyGuy.Transform);
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(unanianFactory.Transform);
            m_game.SceneManager.CurrentScene.SceneNode.AddChild(tyril.Transform);

            foreach (var i in armyUnits)
            {
                m_game.SceneManager.CurrentScene.SceneNode.AddChild(i.Transform);
            }
        }
    }
}
