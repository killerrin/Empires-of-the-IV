using Anarian.DataStructures;
using Anarian.DataStructures.Rendering;
using Anarian.Enumerators;
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
    public class GameInputManager : IDisposable
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties
        #endregion

        public GameInputManager (EmpiresOfTheIVGame game)
        {
            m_game = game;

            // Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown += InputManager_PointerDown;
            m_game.InputManager.PointerPressed += InputManager_PointerClicked;
            m_game.InputManager.PointerMoved += InputManager_PointerMoved;

            // Keybaord
            m_game.InputManager.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;
        }

        public void Dispose()
        {
            // De-Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown -= InputManager_PointerDown;
            m_game.InputManager.PointerPressed -= InputManager_PointerClicked;
            m_game.InputManager.PointerMoved -= InputManager_PointerMoved;

            // Keybaord
            m_game.InputManager.Keyboard.KeyboardDown -= Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed -= Keyboard_KeyboardPressed;

            // Surpress the Finalize
            GC.SuppressFinalize(this);
        }

        #region Interface Implimentations
        void IDisposable.Dispose() { Dispose(); }
        #endregion

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
                Camera camera = m_game.SceneManager.CurrentScene.Camera;
                Ray ray = camera.GetMouseRay(
                    e.Position,
                    m_game.Graphics.GraphicsDevice.Viewport
                    );

                bool intersects = m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).GameObject.CheckRayIntersection(ray);
                Debug.WriteLine("Hit: {0}, Ray: {1}", intersects, ray.ToString());

                currentRay = ray;

                // Get the point on the terrain
                rayPosOnTerrain = ((Terrain)m_game.SceneManager.CurrentScene.SceneNode.GetChild(0).GameObject).Intersects(ray);
            }
            if (e.Pointer == PointerPress.MiddleMouseButton) {
                Debug.WriteLine("Middle Mouse Pressed");
                //GamePage.PageFrame.Navigate(typeof(BlankPage));
                //GamePage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            if (e.Pointer == PointerPress.RightMouseButton) {
                Unit unit = m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).GameObject as Unit;
                unit.AnimationState.AnimationPlayer.Paused = !unit.AnimationState.AnimationPlayer.Paused;
            }
        }

        void InputManager_PointerMoved(object sender, Anarian.Events.PointerMovedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

        }

        void Keyboard_KeyboardDown(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            //Debug.WriteLine("Keyboard: {0}, Held Down", e.KeyClicked.ToString());
            Camera cam = m_game.SceneManager.CurrentScene.Camera;

            switch (e.KeyClicked) {
                case Keys.W:
                    cam.MoveForward(2.0f);
                    break;
                case Keys.S:
                    cam.MoveForward(-2.0f);
                    break;
                case Keys.A:
                    cam.MoveHorizontal(-2.0f);
                    break;
                case Keys.D:
                    cam.MoveHorizontal(2.0f);
                    break;
                case Keys.Q:
                    cam.MoveVertical(-2.0f);
                    break;
                case Keys.E:
                    cam.MoveVertical(2.0f);
                    break;

                case Keys.O:
                    cam.MoveDepth(-2.0f);
                    break;
                case Keys.L:
                    cam.MoveDepth(2.0f);
                    break;

                case Keys.Up:
                    cam.AddPitch(MathHelper.ToRadians(2));
                    break;
                case Keys.Down:
                    cam.AddPitch(MathHelper.ToRadians(-2));
                    break;
                case Keys.Left:
                    cam.AddYaw(MathHelper.ToRadians(2));
                    break;
                case Keys.Right:
                    cam.AddYaw(MathHelper.ToRadians(-2));
                    break;
            }
        }
        void Keyboard_KeyboardPressed(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.KeyClicked.ToString());
        }
        #endregion

    }
}
