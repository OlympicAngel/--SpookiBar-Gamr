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
    class Gif
    {
        Texture2D[] imgs;
        double timemulti = 8;
        int pos;
        int perFrame;
        int trans = 255;
        SoundEffectPlay statics;
        TimeSpan timebypos;
        public static bool isUpdated {get;set;}


        public Gif(Texture2D[] t,int perFrame)
        {
            isUpdated = false;
            trans = 255;
            this.perFrame = perFrame;
            this.pos = 0;
            this.imgs = new Texture2D[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                this.imgs[i] = t[i];
            }
            timebypos = new TimeSpan();
        }

        public void Update(TimeSpan time)
        {
            if (!isUpdated)
            {
                timebypos += time;
                isUpdated = true;
            }
        }

        public Texture2D[] GetImg()
        {
            return this.imgs;
        }

        public int GetFrames()
        {
            return this.imgs.Length;
        }

        public bool Draw(SpriteBatch sp,Rectangle r)
        {
            timebypos += TimeController.ElapsedGameTime;

            int pos = (int)(timebypos.TotalMilliseconds / (imgs.Length * timemulti));
            if (pos >= imgs.Length)
            {
                timebypos = new TimeSpan();
                sp.Draw(this.imgs[imgs.Length-1], r, Color.White);
                return true;
            }
            sp.Draw(this.imgs[pos], r, Color.White);
            return false;
        }

        public void DrawFrame(SpriteBatch sp, Rectangle r,int frame)
        {
            if (frame >= 180)
                frame = 179;
            sp.Draw(this.imgs[frame], r, Color.White * 0.95F);
            pos = frame;
        }

        public bool Draw(SpriteBatch sp, Rectangle r,int step,bool istrans)
        {
            timebypos += TimeController.ElapsedGameTime;
            int pos = (int)(timebypos.TotalMilliseconds / (imgs.Length * timemulti));
            if (pos >= imgs.Length)
            {
                timebypos = new TimeSpan();
                pos = imgs.Length - 1;
            }

            sp.Draw(this.imgs[pos], r, Color.White*(trans/255F));

            if (istrans)
            trans -= step;

            if(this.trans <=0)
                this.trans = 0;
            if (this.trans >= 255)
                this.trans = 255;

            if (pos  == 0)
            {
                return true;
            }
            return false;
        }

        public bool DrawTrans(SpriteBatch sp, Rectangle r, int step, bool stepInto, TimeSpan remainTime)
        {
            Update(remainTime);

            int pos = (int)(timebypos.TotalMilliseconds / (imgs.Length * timemulti));
            if (pos >= imgs.Length)
            {
                timebypos = new TimeSpan();
                sp.Draw(this.imgs[imgs.Length - 1], r, Color.White * ((step) / 255F));
                return true;
            }
            sp.Draw(this.imgs[pos], r, Color.White * ((step) / 255F));
            return false;
        }

        public bool InAnimation()
        {
            return this.trans == 0;
        }

        public void Reset(bool sts,bool lookinmg)
        {
            if (sts)
            {
                this.trans = 255;
                if (statics != null && lookinmg)
                    statics.Play(0.0f, true);
            }
            else
                this.trans = 0;
        }

        public void LoadSound(SoundEffect m)
        {
            this.statics = new SoundEffectPlay(-1.0f, m, 0.4f);
        }

    }
}
