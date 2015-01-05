using Anarian.Enumerators;
using Anarian.Interfaces;
using Anarian.Helpers;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Anarian.DataStructures.ScreenEffects
{
    public enum FadeStatus
    {
        None,
        FadingToContent,
        FadingIn
    }

    public class Fade : IScreenEffect, Anarian.Interfaces.IDrawable
    {
        #region Fields/Properties
        ProgressStatus m_progressStatus;
        public ProgressStatus Progress { get { return m_progressStatus; } set { m_progressStatus = value; } }

        FadeStatus m_fadeStatus;
        public FadeStatus FadeStatus { get { return m_fadeStatus; } protected set { m_fadeStatus = value; } }

        float m_fadePercentage;
        public float FadePercentage { get { return m_fadePercentage; } protected set { m_fadePercentage = value; } }

        float m_fadeRate;
        public float FadeRate { get { return m_fadeRate; } set { m_fadeRate = value; } }

        Texture2D m_fadeTexture;
        public Texture2D FadeTexture { get { return m_fadeTexture; } set { m_fadeTexture = value; } }

        Color m_fadeColor;
        public Color FadeColor { get { return m_fadeColor; } set { m_fadeColor = value; } }
        #endregion

        public event EventHandler Completed;

        public Fade(GraphicsDevice graphicsDevice, Color solidColor, float fadeRate = 0.003f)
        {
            m_progressStatus = ProgressStatus.None;
            m_fadeStatus = ScreenEffects.FadeStatus.FadingToContent;
            m_fadeRate = fadeRate;
            m_fadePercentage = 0.0f;

            ChangeFadeColor(solidColor, graphicsDevice);
        }

        public Fade(Texture2D texture, float fadeRate = 0.003f)
        {
            m_progressStatus = ProgressStatus.None;
            m_fadeStatus = ScreenEffects.FadeStatus.FadingToContent;
            m_fadeRate = fadeRate;
            m_fadePercentage = 0.0f;

            m_fadeTexture = texture;
            m_fadeColor = Color.White;
        }

        #region Interface Implimentation
        void IScreenEffect.PreformEffect(GameTime gameTime) { ApplyEffect(gameTime); }
        void IScreenEffect.Draw(GameTime gameTime, SpriteBatch spriteBatch) { Draw(gameTime, spriteBatch); }
        ProgressStatus IScreenEffect.Progress { get { return Progress; } set { Progress = value; } }

        void Anarian.Interfaces.IDrawable.Draw(GameTime gameTime, SpriteBatch spriteBatch) { Draw(gameTime, spriteBatch); }
        #endregion

        #region Helper Methods
        public void ChangeFadeStatus(FadeStatus fadeStaus)
        {
            m_fadeStatus = fadeStaus;

            switch (fadeStaus) {
                case FadeStatus.None:
                    Progress = ProgressStatus.Completed;
                    break;
                case FadeStatus.FadingToContent:
                case FadeStatus.FadingIn:
                default:
                    Progress = ProgressStatus.NotStarted;
                    break;
            }
        }

        public void ChangeFadeColor(Color color, GraphicsDevice graphicsDevice)
        {
            m_fadeColor = color;
            m_fadeTexture = m_fadeColor.CreateTextureFromSolidColor(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }
        #endregion

        public void ApplyEffect(GameTime gameTime)
        {
            if (m_progressStatus == ProgressStatus.None) return;
            if (m_progressStatus == ProgressStatus.Completed) return;
            if (m_fadeStatus == FadeStatus.None) return;
            
            switch (m_fadeStatus) {
                case FadeStatus.FadingToContent:    m_fadePercentage -= m_fadeRate * gameTime.DeltaTime();  break;
                case FadeStatus.FadingIn:           m_fadePercentage += m_fadeRate * gameTime.DeltaTime();  break;
            }
            
            m_fadePercentage = MathHelper.Clamp(m_fadePercentage, 0.0f, 1.0f);
            if (m_fadePercentage <= 0.0f || m_fadePercentage >= 1.0f) {
                m_progressStatus = ProgressStatus.Completed;

                if (Completed != null)
                    Completed(this, null);
            }
            else
                m_progressStatus = ProgressStatus.InProgress;
            
            Debug.WriteLine(ToString());
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            spriteBatch.Begin();
            spriteBatch.Draw(m_fadeTexture, Vector2.Zero, m_fadeColor * m_fadePercentage);
            spriteBatch.End();
        }

        public override string ToString()
        {
            return "Progress: " + m_progressStatus.ToString() + " | " +
                   "FadeStatus: " + m_fadeStatus.ToString() + " | " +
                   "FadePercentage: " + m_fadePercentage + " | " +
                   "FadeRate: " + m_fadeRate;
        }
    }
}
