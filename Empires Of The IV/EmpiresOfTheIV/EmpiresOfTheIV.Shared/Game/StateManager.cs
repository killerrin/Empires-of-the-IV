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
using EmpiresOfTheIV.Game.Enumerators;
using Anarian.DataStructures.ScreenEffects;
using EmpiresOfTheIV.Game.Menus;

namespace EmpiresOfTheIV.Game
{
    public class StateManager : IUpdatable
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties
        bool m_goingBack;

        Stack<GameMenu> m_gameStates;
        GameMenu m_goingToMenu;
        Fade m_fadeEffect;

        public GameMenu CurrentMenu { get { return m_gameStates.Peek(); } set { Navigate(value); } }
        public GameState CurrentState { get { return m_gameStates.Peek().State; } set { Navigate(value); } }
        public Fade FadeEffect { get { return m_fadeEffect; } }

        public int BackStackDepth { get { return m_gameStates.Count; } }
        public bool CanGoBack { get { return m_gameStates.Count > 0; } }


        #region Flags
        bool m_exit;
        bool m_removePreviousOnCompleted;
        bool m_hideFrameBeforeTransition;

        public bool Exit { get { return m_exit; } set { m_exit = value; } }
        public bool RemovePreviousOnCompleted { get { return m_removePreviousOnCompleted; } set { m_removePreviousOnCompleted = value; } }
        public bool HideFrameBeforeTransition { get { return m_hideFrameBeforeTransition; } set { m_hideFrameBeforeTransition = value; } }
        #endregion
        #endregion

        public event EventHandler OnExit;

        public StateManager(EmpiresOfTheIVGame game, Color fadeColor)
        {
            m_game = game;
            m_exit = false;

            m_gameStates = new Stack<GameMenu>();
            m_goingToMenu = null;
            m_goingBack = false;

            ResetFlags();

            m_fadeEffect = new Fade(game.GraphicsDevice, fadeColor, 0.002f);
            m_fadeEffect.ChangeWithoutFade(FadeStatus.FadingIn);
            m_fadeEffect.Completed += FadeEffect_Completed;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        #region Helper Methods
        public void ExitGame()
        {
            // Hide the Frame to prepare for Navigation
            MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            m_exit = true;

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }
        public void ForceExitGame()
        {
            m_exit = true;
            if (OnExit != null)
                OnExit(this, null);
        }

        protected void ResetFlags()
        {
            m_removePreviousOnCompleted = false;
            m_hideFrameBeforeTransition = true;
        }
        #endregion

        #region Update/Draw
        public void Update(GameTime gameTime)
        {
            // Update the Fade
            m_fadeEffect.ApplyEffect(gameTime);

            if (m_exit) return;

            // Update the Current Menu
            if (CurrentMenu != null) CurrentMenu.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            // Draw the Current Menu
            if (!m_exit && CurrentMenu != null)
                CurrentMenu.Draw(gameTime, spriteBatch, graphics);

            // Draw the Fade
            m_fadeEffect.Draw(gameTime, spriteBatch);
        }
        #endregion

        #region Stack Management
        public void Navigate(GameState newState)
        {
            // Hide the Frame to prepare for Navigation
            if (m_hideFrameBeforeTransition)
                MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            GameMenu newMenu;
            switch (newState) {
                case GameState.SplashScreen:        newMenu = new SplashScreenMenu(m_game); break;
                case GameState.MainMenu:            newMenu = new MainMenu(m_game); break;

                case GameState.PlayGame:            newMenu = new PlayGameMenu(m_game); break;
                case GameState.Singleplayer:        newMenu = new SingleplayerMenu(m_game); break;
                case GameState.Multiplayer:         newMenu = new MultiplayerMenu(m_game); break;
                case GameState.EmpireSelection:     newMenu = new EmpireSelectionMenu(m_game); break;
                case GameState.InGame:              newMenu = new InGameMenu(m_game); break;
                case GameState.Paused:              newMenu = new PausedMenu(m_game); break;
                case GameState.GameOver:            newMenu = new GameOverMenu(m_game); break;

                case GameState.Options:             newMenu = new OptionsMenu(m_game); break;
                case GameState.Credits:             newMenu = new CreditsMenu(m_game); break;

                case GameState.None:                
                default:                            newMenu = new BlankMenu(m_game); break;
            }

            m_goingToMenu = newMenu;          
            m_goingBack = false;
            m_exit = false;

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }

        public void Navigate(GameMenu newMenu)
        {
            // Hide the Frame to prepare for Navigation
            if (m_hideFrameBeforeTransition)
                MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            m_goingToMenu = newMenu;
            m_goingBack = false;
            m_exit = false;

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }


        public void GoBack()
        {
            // Hide the Frame to prepare for Navigation
            try {
                if (m_hideFrameBeforeTransition)
                    MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (Exception) { }

            if (CanGoBack) {
                m_goingBack = true;
            }
            else {
                m_exit = true;
            }

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }

        /// <summary>
        /// Gets the Previous Menu
        /// </summary>
        /// <returns>The Previous GameMenu, or null if no previous GameMenu is available</returns>
        public GameMenu GetPreviousMenu()
        {
            if (m_gameStates.Count > 1) {
                GameMenu currentMenu = m_gameStates.Pop();
                GameMenu previousMenu = m_gameStates.Pop();

                m_gameStates.Push(previousMenu);
                m_gameStates.Push(currentMenu);

                return previousMenu;
            }
            return null;
        }

        /// <summary>
        /// Removes the last GameMenu from the Stack
        /// </summary>
        /// <returns>The removed GameMenu</returns>
        public GameMenu RemoveOneFromStack()
        {
            if (CanGoBack) {
                return m_gameStates.Pop();
            }
            else {
                m_exit = true;
                return new BlankMenu(m_game);
            }
        }

        /// <summary>
        /// Removes the previous GameMenu from the Stack
        /// </summary>
        /// <returns>The removed GameMenu</returns>
        public GameMenu RemovePreviousFromStack()
        {
            if (m_gameStates.Count > 1) {
                GameMenu temp = m_gameStates.Pop();
                GameMenu returningMenu = m_gameStates.Pop();

                m_gameStates.Push(temp);
                return returningMenu;
            }
            else if (m_gameStates.Count == 1) {
                return m_gameStates.Pop();
            }
            else {
                m_exit = true;
                return new BlankMenu(m_game);
            }
        }
        #endregion

        #region Events
        void FadeEffect_Completed(object sender, EventArgs e)
        {
            if (m_fadeEffect.FadeStatus == FadeStatus.None) return;

            // First, Check if we need to exit
            if (m_exit) {
                if (OnExit != null)
                    OnExit(this, null);
                return;
            }

            // We are fully blacked out, so we can change the Menu
            if (m_fadeEffect.FadeStatus == FadeStatus.FadingIn) {
                if (m_goingBack) {
                    try {
                        if (MainPage.PageFrame.CanGoBack)
                            MainPage.PageFrame.GoBack();
                    }
                    catch (Exception) { }

                    // Remove the Menu so that the Update/Draw will handle the correct Menu
                    m_gameStates.Pop();
                }
                else {
                    switch (m_goingToMenu.State) {
                        case GameState.SplashScreen:                MainPage.PageFrame.Navigate(typeof(SplashScreenPage)); break;
                        case GameState.MainMenu:                    MainPage.PageFrame.Navigate(typeof(MainMenuPage)); break;

                        case GameState.PlayGame:                    MainPage.PageFrame.Navigate(typeof(PlayGamePage)); break;
                        case GameState.Singleplayer:                MainPage.PageFrame.Navigate(typeof(SingleplayerPage)); break;
                        case GameState.Multiplayer:                 MainPage.PageFrame.Navigate(typeof(MultiplayerPage)); break;
                        case GameState.EmpireSelection:             MainPage.PageFrame.Navigate(typeof(EmpireSelectionPage));break;
                        case GameState.InGame:                      MainPage.PageFrame.Navigate(typeof(InGamePage)); break;
                        case GameState.Paused:                      MainPage.PageFrame.Navigate(typeof(PausedPage)); break;
                        case GameState.GameOver:                    MainPage.PageFrame.Navigate(typeof(GameOverPage)); break;

                        case GameState.Options:                     MainPage.PageFrame.Navigate(typeof(OptionsPage)); break;
                        case GameState.Credits:                     MainPage.PageFrame.Navigate(typeof(CreditsPage)); break;
                        default:
                        case GameState.None:                        MainPage.PageFrame.Navigate(typeof(BlankPage)); break;
                    }

                    if (m_removePreviousOnCompleted) {
                        m_gameStates.Pop();
                    }

                    // Add in the new menu
                    // This is done here so that the menu won't be pulled in during the Update/Draw while fading
                    m_gameStates.Push(m_goingToMenu);
                    m_goingToMenu = null;
                }

                // Reset Flags;
                ResetFlags();

                // Now that the menu is loaded, we will notify it
                m_gameStates.Peek().MenuLoaded();

                // Finally, fade back out
                m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingToContent);
                return;
            }
            //else if (m_fadeEffect.FadeStatus == FadeStatus.FadingToContent) {
                MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //}
        }
        #endregion
    }
}
