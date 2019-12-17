using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace spooki_bar
{
    class SoundEffectPlay
    {
        SoundEffect main;
        public float soundPitch { get; set; }
        float vol;
        SoundEffectInstance inte = null;

        #region SoundEffectPlay
        public SoundEffectPlay(float pitch, SoundEffect m,float vol)
        {
            this.vol = vol;
            this.soundPitch = pitch;
            this.main = m;
          
            this.inte = main.CreateInstance();
        }
        #endregion
        #region Play
        public void Play(float pos,bool overrider)
        {
            if (overrider)
                inte.Stop(true);

            if (inte.State != SoundState.Playing)
            {
                inte.Pan = pos;
                inte.Pitch = soundPitch;
                inte.Volume = vol;
                inte.Play();
            }
        }
        #endregion
        #region PlayAt
        public void PlayAt(float pos, float vol, bool overrider)
        {
            if (overrider)
                inte.Stop(true);

            if (inte.State != SoundState.Playing)
           {
               inte.Pan = pos;
               inte.Volume = this.vol * vol;
               inte.Play();
           }
        }
        #endregion
        #region PlayLoop
        public void PlayLoop(float pos)
        {
            if (!inte.IsLooped)
            {
                inte.Stop(true);
                inte.Dispose();
                inte = main.CreateInstance();
                inte.IsLooped = true;
                inte.Volume = this.vol;
                inte.Pitch = this.soundPitch;
                inte.Play();
            }
            else if (inte.State != SoundState.Playing)
            {
                inte.Volume = this.vol;
                inte.Pitch = this.soundPitch;
                inte.Play();
            }
        }
        #endregion
        #region Togggle
        public void Togggle()
        {
            if (inte != null)
            {
                if (inte.State == SoundState.Playing)
                    inte.Stop();
                else if (inte.State == SoundState.Stopped || inte.State == SoundState.Paused)
                    inte.Play();
            }
            else
                PlayLoop(0); 
        }
        #endregion
        #region Off
        public void Off()
        {
            if (inte != null && inte.State != SoundState.Stopped)
            {
                inte.Stop();
            }
        }
        public void Off(bool ass)
        {
            if (inte != null && inte.State != SoundState.Stopped)
            {
                inte.Stop(ass);
            }
        }
        #endregion
        #region SetVol
        public void SetVol(float f)
        {
            this.vol = f;
            this.inte.Volume = f;
        }
        #endregion
        #region PlayAs
        public void PlayAs(float pitch, float vol)
        {
            float tv = inte.Volume, tp = inte.Pitch;
            inte.Volume = vol;
            inte.Pitch = pitch;
            inte.Play();
            inte.Volume = tv;
            inte.Pitch = tp;
        }
        #endregion
        public bool IsPlaying()
        {
            return inte.State == SoundState.Playing;
        }
    }
}
