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
    class Battery
    {
        public Texture2D outerline { get; set; }
        public Texture2D inner { get; set; }
        public double battey { get; set; }
        public Rectangle pos { get; set; }
        public SoundEffectPlay down;

        #region Battery
        public Battery(Texture2D outer, Texture2D inner,SoundEffect powerdown)
        {
            this.inner = inner;
            this.outerline = outer;
            this.battey = 2000;
            int w = 100;
            pos = new Rectangle(10, 80, w, w / 2);
            down = new SoundEffectPlay(0.0f, powerdown, 1.0f);
        }
        #endregion
        #region Reset
        public void Reset()
        {
            this.battey = 2000;
        }
        #endregion
        #region Draw
        public void Draw(SpriteBatch sb, TimeSpan t)
        {
            if (battey >= 250)
                BaseDraw(sb);
            else
            {
                if (t.Milliseconds >= 800)
                    BaseDraw(sb);
                if (battey <= 0 && t.Milliseconds > 400 && t.Milliseconds < 600)
                    BaseDraw(sb);

            }
        }
        #endregion
        #region BaseDraw
        public void BaseDraw(SpriteBatch sb)
        {
            for (int i = 0; i < 4; i++)
            {
                if (battey > i * 2000 / 4)
                    sb.Draw(this.inner, new Rectangle(pos.X + (i) * (pos.Width / 5) + (i + 1) * ((int)(pos.Width * (1.0 / 100.0))) + (int)(pos.Width * (1.0 / 30.0)), pos.Y + (int)(pos.Height * (1.0 / 10.0)), pos.Width / 5, (int)(pos.Height * (8.0 / 10.0))), Color.White);
            }
            if (battey > 2000)
                sb.Draw(this.inner, new Rectangle(pos.X + (4) * (pos.Width / 5) + (4 + 1) * ((int)(pos.Width * (1.0 / 90.0))) + (int)(pos.Width * (1.0 / 30.0)), pos.Y + (int)(pos.Height * (1.0 / 10.0)), pos.Width / 5, (int)(pos.Height * (8.0 / 10.0))), Color.Red);

            sb.Draw(this.outerline, pos, Color.White);
        }
        #endregion
        #region CalcBattery
        public double CalcBattery(bool monitor, bool rLight, bool lLight)
        {
            double remove = 1;
            double killsum = 0;
            if (battey >= 0)
            {
                killsum = killsum - remove;
                if (monitor)
                    if (rLight || lLight)
                        killsum = killsum - 0.5 * remove;
                    else
                        killsum = killsum - 1.5 * remove;
                if (rLight)
                    killsum = killsum - 3 * remove;
                if (lLight)
                    killsum = killsum - 4 * remove;
            }
            if (battey > 0 && battey + killsum <= 0)
                down.Play(0.0f, true);
            battey = battey + killsum;
            return battey;
        }
        #endregion
        #region isEmpty
        public bool isEmpty()
        {
            return this.battey <= 0;
        }
        #endregion
    }
}
