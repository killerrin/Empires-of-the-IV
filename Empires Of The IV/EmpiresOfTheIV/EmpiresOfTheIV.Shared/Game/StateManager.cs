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

namespace EmpiresOfTheIV.Game
{
    public class StateManager : IUpdatable
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties
        bool m_goingBack;
        bool m_exit;

        Stack<GameState> m_gameStates;
        Fade m_fadeEffect;

        public GameState CurrentState { get { return m_gameStates.Peek(); } set { Navigate(value); } }
        public Fade FadeEffect { get { return m_fadeEffect; } }

        public int BackStackDepth { get { return m_gameStates.Count; } }
        public bool Exit { get { return m_exit; } set { m_exit = value; } }
        #endregion

        public EventHandler OnExit;

        public StateManager(EmpiresOfTheIVGame game, Color fadeColor)
        {
            m_game = game;
            m_exit = false;

            m_gameStates = new Stack<GameState>();
            m_goingBack = false;

            m_fadeEffect = new Fade(game.GraphicsDevice, fadeColor, 0.003f);
            m_fadeEffect.ChangeWithoutFade(FadeStatus.FadingIn);
            m_fadeEffect.Completed += FadeEffect_Completed;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        #region Update/Draw
        public void Update(GameTime gameTime)
        {
            m_fadeEffect.ApplyEffect(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_fadeEffect.Draw(gameTime, spriteBatch);
        }
        #endregion

        public void ExitGame()
        {
            // Hide the Frame to prepare for Navigation
            MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            m_exit = true;

            // Lastly set the automation state
            m_fadeEffect.ChangeFadeStatus(FadeStatus.FadingIn);
        }

        #region Stack Management
        public void Navigate(GameState newState)
        {
            // Hide the Frame to prepare for Navigation
            MainPage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            m_gameStates.Push(newState);
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
        /// Removes the last GameState from the Stack
        /// </summary>
        /// <returns>The removed GameState</returns>
        public GameState RemoveOneFromStack()
        {
            if (m_gameStates.Count > 0) {
                return m_gameStates.Pop();
            }
            else {
                m_exit = true;
                return GameState.None;
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
                    switch (m_gameStates.Peek()) {
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
