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
    class Item
    {
        static SoundEffect ton;
        public SoundEffectPlay sound;
        public Texture2D texture { get; set; }
        public Rectangle pos;


        public static void LoadClickSound(SoundEffect ton2)
        {
            ton = ton2;
        }

        public Item(Texture2D texture, Rectangle pos)
        {
            this.texture = texture;
            this.pos = pos;
            this.sound = new SoundEffectPlay(0.0f, ton, 0.5f);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (texture != null)
            sb.Draw(texture, pos, Color.White);
        }

        public virtual void Draw(SpriteBatch sb,Color c)
        {
            if (texture != null)
                sb.Draw(texture, pos, c);
        }

        public void DrawAs(SpriteBatch sb,int pos)
        {
            if (texture != null)
                sb.Draw(texture, new Rectangle(this.pos.X+pos,this.pos.Y,this.pos.Width,this.pos.Height), Color.White);
        }

        public void SetPos(int x,int y)
        {
            this.pos.X = x;
            this.pos.Y = y;
        }

        public virtual bool IsClicked(MouseState m)
        {
            m = Mouse.GetState();
            bool sts = (m.LeftButton == ButtonState.Pressed && this.pos.Contains(m.X, m.Y));
            return sts;
        }

        public virtual bool IsClicked(MouseState m, bool freez)
        {
           bool sts = IsClicked(m);
           if (sound != null && sts && freez)
           {
               sound.SetVol(0.3f);
               sound.Play(0.0f,true);
           }
            return sts;
        }

        public virtual void Dispose()
        {
            try
            {
                texture.Dispose();
                pos = new Rectangle();
                texture = null;
            }
            catch { }
        }
    }
}
