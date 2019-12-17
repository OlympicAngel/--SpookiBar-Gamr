using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace spooki_bar
{
    class TopMenuText:TextItem
    {

        public static SoundEffectPlay hoverSound;
        bool playonce = false;
        Rectangle bg;
        Rectangle newbg;

        public static void SetSound(SoundEffect s)
        {
            hoverSound = new SoundEffectPlay(0f, s, 0.2f);
        }

        public TopMenuText(SpriteFont font, string text, Vector2 vec, int width, int height)
            : base(font, text, new Vector2(vec.X, vec.Y + (int)(height*0.2)))
        {
            bg = new Rectangle((int)vec.X, (int)vec.Y, width, height);
            newbg = new Rectangle((int)vec.X, (int)vec.Y, width, (int)(height*1.08));
        }

        public override bool IsClicked(MouseState m, bool freez)
        {

            m = Mouse.GetState();
            bool sts = (m.LeftButton == ButtonState.Pressed && this.bg.Contains(m.X, m.Y));
            if (sound != null && sts && freez)
            {
                sound.SetVol(0.3f);
                sound.Play(0.0f, false);
            }
            return sts;
        }

        public virtual void Draw(SpriteBatch sb, MouseState m, GameTime timer)
        {
            if (bg.Contains(m.X, m.Y))
            {
                if (hoverSound != null && !playonce)
                {
                    hoverSound.Play(0.0f, true);
                    playonce = true;
                }
            }
            else playonce = false;


            sb.Draw(CPixel.LightGrey, newbg, Color.White);
            sb.Draw(CPixel.DarkGrey, bg, Color.Black);

            if (bg.Contains(m.X, m.Y))
            {
                sb.Draw(CPixel.LightGrey, bg, Color.Gray);
            }



            if (bg.Contains(m.X, m.Y))
            {
                base.DrawPhantom(sb, 0);
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.Gold * 0.1f);

            }
            else
            {
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.White * 0.5f);

            }
        }

    }
}
