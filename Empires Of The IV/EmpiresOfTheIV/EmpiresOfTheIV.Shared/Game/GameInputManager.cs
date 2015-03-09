using Anarian.DataStructures;
using Anarian.DataStructures.Rendering;
using Anarian.DataStructures.ScreenEffects;
using Anarian.Enumerators;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game
{
    public class GameInputManager : IDisposable, IUpdatable, IRenderable
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties

        #endregion

        #region Constructors
        public GameInputManager (EmpiresOfTheIVGame game)
        {
            m_game = game;

            // Subscribe to our Events
            // Pointer
            //m_game.InputManager.PointerDown += InputManager_PointerDown;
            //m_game.InputManager.PointerPressed += InputManager_PointerClicked;
            //m_game.InputManager.PointerMoved += InputManager_PointerMoved;
        }

        public void Dispose()
        {
            // De-Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown -= InputManager_PointerDown;
            m_game.InputManager.PointerPressed -= InputManager_PointerClicked;
            m_game.InputManager.PointerMoved -= InputManager_PointerMoved;

            // Surpress the Finalize
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Interface Implimentations
        void IDisposable.Dispose() { Dispose(); }
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {

        }

        #region Input Events
        void InputManager_PointerDown(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());
        }

        public Ray? currentRay;
        public Vector3? rayPosOnTerrain;
        void InputManager_PointerClicked(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

            if (e.Pointer == PointerPress.LeftMouseButton ||
                e.Pointer == PointerPress.Touch) {
                UniversalCamera camera = m_game.SceneManager.CurrentScene.Camera as UniversalCamera;
                Ray ray = camera.GetMouseRay(
                    e.Position,
                    m_game.Graphics.GraphicsDevice.Viewport
                    );

                bool intersects = m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).GameObject.CheckRayIntersection(ray);
                Debug.WriteLine("Hit: {0}, Ray: {1}", intersects, ray.ToString());

                currentRay = ray;

                // Get the point on the terrain
                rayPosOnTerrain = ((Terrain)m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).GameObject).Intersects(ray);
            }
            if (e.Pointer == PointerPress.MiddleMouseButton) {
                Debug.WriteLine("Middle Mouse Pressed");
            }
            if (e.Pointer == PointerPress.RightMouseButton) {
                Unit unit = m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).GameObject as Unit;
                unit.AnimationState.AnimationPlayer.Paused = !unit.AnimationState.AnimationPlayer.Paused;
            }
        }

        void InputManager_PointerMoved(object sender, Anarian.Events.PointerMovedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

        }
        #endregion

    }
}
