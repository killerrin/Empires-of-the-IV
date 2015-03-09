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
            ////m_game.SceneManager.CurrentScene.Camera.MoveVertical(30.0f);
            //
            //// Load the Assets
            //m_game.ResourceManager.LoadAsset(m_game.Content, typeof(AnimatedModel), "t-pose_3");
            //m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Model), "Factory Base");
            //
            //// Load the Terrain
            //Debug.WriteLine("Loading Terrain");
            //Texture2D heightMap = m_game.Content.Load<Texture2D>("Radient Flatlands HeightMap");
            //Texture2D grassTexture = m_game.Content.Load<Texture2D>("grassTexture");
            //Terrain m_terrain = new Terrain(graphics, heightMap, grassTexture);
            ////m_terrain.SetupGrid(true, AStarTerrainRule.ImpassableRule(RuleMeasureType.Absolute, Comparison.LessThanEqualTo, 10.0f));
            ////m_terrain.DebugPrintGrid();
            //
            //// Create the Game Objects
            Debug.WriteLine("Loading Game Objects");
            Building unanianFactory = new Building();
            //unanianFactory.Model3D = null;// m_game.ResourceManager.GetAsset(typeof(Model), "Factory Base") as Model;
            unanianFactory.Transform.Position = new Vector3(0.0f, -10.0f, 0.0f);
            unanianFactory.CullDraw = false;
            unanianFactory.RenderBounds = true;
            unanianFactory.Active = false;


            Unit armyGuy = new Unit(0);
            //armyGuy.Model3D = null;// m_game.ResourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
            armyGuy.Transform.Scale = new Vector3(0.007f);
            armyGuy.Transform.Position = new Vector3(0.2f, -0.5f, -5.50f);
            armyGuy.CullDraw = false;
            armyGuy.RenderBounds = true;
            armyGuy.Active = false;

            //// Load the models which contain animations
            //Debug.WriteLine("Loading Animations");
            //AnimatedModel walk = CustomContentLoader.LoadAnimatedModel(m_game.Content, "walk");
            //AnimationClip clip = walk.Clips[0];
            //
            //// Set our animations to the gameObjects
            //AnimationPlayer armyGuyPlayer = armyGuy.PlayClip(clip);
            //armyGuyPlayer.Looping = true;
            //
            //// PC Character Limit 250 - 125 each player
            //// Phone Character Limit ~60 - 30 Each Player
            //
            //List<Unit> armyUnits = new List<Unit>();
            //for (int i = 0; i < GameConsts.MaxUnitsOnWindowsPhone; i++)
            //{
            //    var unit = new Unit();
            //    unit.Model3D = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
            //    unit.Transform.Scale = new Vector3(0.007f);
            //    unit.Transform.Position = new Vector3((float)Consts.random.NextDouble(), -(float)Consts.random.NextDouble(), -5.50f + (float)Consts.random.NextDouble());
            //    unit.CullDraw = false;
            //    unit.RenderBounds = false;
            //
            //    AnimationPlayer animPlayer = unit.PlayClip(clip);
            //    animPlayer.Looping = true;
            //    armyUnits.Add(unit);
            //}

            

            // Add to the Scene
            Debug.WriteLine("Adding to Scene");
            //m_game.SceneManager.CurrentScene.SceneNode.AddChild(m_terrain.Transform);
            //m_game.SceneManager.CurrentScene.SceneNode.AddChild(armyGuy.Transform);
            //m_game.SceneManager.CurrentScene.SceneNode.AddChild(unanianFactory.Transform);

            //foreach (var i in armyUnits)
            //{
            //    m_game.SceneManager.CurrentScene.SceneNode.AddChild(i.Transform);
            //}
        }
    }
}
