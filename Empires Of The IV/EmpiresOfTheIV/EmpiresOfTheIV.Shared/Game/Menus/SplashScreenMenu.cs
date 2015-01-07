using Anarian.DataStructures;
using Anarian.GUI;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game.Menus
{
    public class SplashScreenMenu : GameMenu,
                                    IUpdatable, IRenderable
    {

        #region Fields/Properties
        Timer m_timer;
        #endregion

        public SplashScreenMenu(EmpiresOfTheIVGame game)
            : base(game, GameState.SplashScreen)
        {
            m_timer = new Timer(TimeSpan.FromSeconds(3.0));
            
            m_timer.Tick += Timer_Tick;
            m_timer.Completed += Timer_Completed;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            // Update the Timer
            m_timer.Update(gameTime);

            // Update the Menu
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_game.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime, spriteBatch, graphics);
        }

        #region Events
        void Timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Tick");
        }

        void Timer_Completed(object sender, EventArgs e)
        {
            Debug.WriteLine("SplashScreenTimer: Completed");

            // Set the flags
            m_game.GameManager.StateManager.HideFrameBeforeTransition = false;
            m_game.GameManager.StateManager.RemovePreviousOnCompleted = true;

            // Navigate to the MainMenu
            m_game.GameManager.StateManager.Navigate(GameState.MainMenu);
        }
        #endregion
    }
}
