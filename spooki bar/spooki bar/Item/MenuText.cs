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
    class MenuText : TextItem
    {

        public static SoundEffectPlay hoverSound;
        bool playonce = false;
        Rectangle bg;
        Rectangle border;
        Rectangle hover;
        public Transition motion { get; set; }
        int X;

        public static void SetSound(SoundEffect s)
        {
            hoverSound = new SoundEffectPlay(-1f, s, 0.5f);
        }

        public MenuText(SpriteFont font, string text, Vector2 vec, int width, int height)
            : base(font, text, new Vector2(vec.X, vec.Y + height))
        {
            motion = new Transition("motion", 1.5, 1.5);
            motion.total_X = width;
            int y = (int)vec.Y;
            bg = new Rectangle(0 - width, y, width, height);

            hover = new Rectangle(0 - width, y + height - height / 10, width, height / 10);

            int height2 = (int)(height * 1.056);
            int y2 = (int)vec.Y - (height2 - height) / 2;

            border = new Rectangle(0 - width, y2, width + (height2 - height) / 2, height2);

            this.pos.Y = bg.Y + bg.Height / 2 - pos.Height / 2;
            X = (int)vec.X;
            vec.X = vec.X - width;
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

        public void Draw(SpriteBatch sb, MouseState m, GameTime timer,float alpha)
        {
            if (alpha == 1.0f && bg.Contains(m.X, m.Y))
            {
                if (hoverSound != null && !playonce)
                {
                    hoverSound.Play(-0.8f, true);
                    playonce = true;
                }
            }
            else playonce = false;


            string state = motion.Return(timer);
            if (state == "start" || state == "in")
            {
                motion.Update(timer);
                border.X = bg.X = hover.X = 0 - bg.Width + (int)motion.add_X;
                pos.X = X - bg.Width + (int)motion.add_X;
            }
            else if (state == "hold" && border.X > 0 - border.Width / 2)
                border.X = bg.X = hover.X = 0;

            sb.Draw(CPixel.DarkRed, border, Color.White * alpha);
            sb.Draw(CPixel.DarkGrey, bg, Color.White * alpha);

            if (bg.Contains(m.X, m.Y))
            {
                sb.Draw(CPixel.LightGrey, hover, Color.White * alpha);

            }

            base.DrawShadow(sb, 10);
            base.DrawPhantom(sb, 0);

            if (bg.Contains(m.X, m.Y))
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.Gold * 0.9f * alpha);
            else
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.White * 0.9f * alpha);

        }
    }
}
