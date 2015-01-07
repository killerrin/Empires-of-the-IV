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
        bool m_exit;

        Stack<GameMenu> m_gameStates;
        Fade m_fadeEffect;

        public GameMenu CurrentMenu { get { return m_gameStates.Peek(); } set { Navigate(value); } }
        public GameState CurrentState { get { return m_gameStates.Peek().State; } set { Navigate(value); } }
        public Fade FadeEffect { get { return m_fadeEffect; } }

        public int BackStackDepth { get { return m_gameStates.Count; } }
        public bool Exit { get { return m_exit; } set { m_exit = value; } }
        #endregion

        public EventHandler OnExit;

        public StateManager(EmpiresOfTheIVGame game, Color fadeColor)
        {
            m_game = game;
            m_exit = false;

            m_gameStates = new Stack<GameMenu>();
            m_goingBack = false;

            m_fadeEffect = new Fade(game.GraphicsDevice, fadeColor, 0.003f);
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
        #endregion

        #region Update/Draw
        public void Update(GameTime gameTime)
        {
            // Update the Fade
            m_fadeEffect.ApplyEffect(gameTime);

            // Update the Current Menu
            if (CurrentMenu != null) CurrentMenu.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            // Draw the Current Menu
            if (CurrentMenu != null) CurrentMenu.Draw(gameTime, spriteBatch, graphics);

            // Draw the Fade
            m_fadeEffect.Draw(gameTime, spriteBatch);
        }
        #endregion

        #region Stack Management
        public void Navigate(GameState newState)
        {
            // Hide the Frame to prepare for Navigation
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

            m_gameStates.Push(newMenu);           
            m_goingBack = false;
            m_exit = false;

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }

        public void Navigate(GameMenu newMenu)
        {
            // Hide the Frame to prepare for Navigation
            MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            m_gameStates.Push(newMenu);
            m_goingBack = false;
            m_exit = false;

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }


        public void GoBack()
        {
            // Hide the Frame to prepare for Navigation
            MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (m_gameStates.Count > 0) {
                m_gameStates.Pop();
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
        /// Removes the last GameState from the Stack
        /// </summary>
        /// <returns>The removed GameState</returns>
        public GameMenu RemoveOneFromStack()
        {
            if (m_gameStates.Count > 0) {
                return m_gameStates.Pop();
            }
            else {
                m_exit = true;
                return new BlankMenu(m_game);
            }
        }

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
                if (m_goingBack) MainPage.PageFrame.GoBack();
                else {
                    switch (m_gameStates.Peek().State) {
                        case GameState.SplashScreen:        MainPage.PageFrame.Navigate(typeof(SplashScreenPage)); break;
                        case GameState.MainMenu:            MainPage.PageFrame.Navigate(typeof(MainMenuPage)); break;

                        case GameState.PlayGame:            break;
                          case GameState.Singleplayer:        break;
                          case GameState.Multiplayer:         break;
                            case GameState.EmpireSelection:     break;
                              case GameState.InGame:              break;
                              case GameState.Paused:              break;
                                case GameState.GameOver:            break;

                        case GameState.Options:             break;
                        case GameState.Credits:             break;
                        default:
                        case GameState.None:                MainPage.PageFrame.Navigate(typeof(BlankPage)); break;
                    }
                }

                m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingToContent);
                return;
            }
            // else if (m_fadeEffect.FadeStatus == FadeStatus.FadingToContent)
            MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
        #endregion
    }
}
