using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game
{
    public class AudioManager
    {
        private static AudioManager m_instance;
        public static AudioManager Instance 
        { 
            get 
            {
                if (m_instance == null) m_instance = new AudioManager();
                return m_instance; 
            } 
        }

        public Dictionary<SoundName, SoundEffect> SoundEffects;
        public Dictionary<SoundName, Song> Songs;

        public const float MaxFrameworkVolume = 1.0f;

        private float m_maxVolume;
        public float MaxVolume
        {
            get { return m_maxVolume; }
            set { m_maxVolume = MathHelper.Clamp(value, 0.0f, MaxFrameworkVolume); }
        }

        private AudioManager()
        {
            SoundEffects = new Dictionary<SoundName, SoundEffect>();
            Songs = new Dictionary<SoundName, Song>();

            MaxVolume = 1.0f;
        }

        public SoundEffectInstance CreateInstanceOfSoundEffect(SoundName name)
        {
            return SoundEffects[name].CreateInstance();
        }

        public void PlaySoundEffect(SoundName name, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            float playVolume = MathHelper.Clamp(volume, 0.0f, MaxVolume);
            SoundEffects[name].Play(playVolume, pitch, pan);
        }

        public SoundEffectInstance Play3DSoundEffect(AudioListener listener, AudioEmitter emitter, SoundName name, float volume = 1.0f, bool isLooped = false)
        {
            var instance = CreateInstanceOfSoundEffect(name);
            instance.Volume = MathHelper.Clamp(volume, 0.0f, MaxVolume);
            instance.IsLooped = isLooped;

            try
            {
                instance.Apply3D(listener, emitter);
                instance.Play();
            }
            catch (Exception) { }

            return instance;
        }
    }
}
