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

namespace spooki_bar.screens
{
    class StartScreen:Screen
    {
        TimeSpan time { get; set; }
        float alpha{ get; set; }
        bool start{ get; set; }
        SoundEffectPlay sound { get; set; }
        Texture2D black;
        int fadein = 6,wait=8, fadeout = 3;

        public StartScreen(Texture2D n):base(n)
        {
            Color[] c = new Color[1];
            black = new Texture2D(n.GraphicsDevice, 1, 1);
            c[0] = new Color(0, 0, 0);
            black.SetData<Color>(c);

            alpha = 0.0f;
            time = new TimeSpan();
            start = true;
        }

        public override void Print(SpriteBatch sb, Rectangle full)
        {
            base.Print(sb, full);
            sb.Draw(black, full, Color.Black * alpha);

        }

        public bool Update(GameTime GTime)
        {
            this.time = this.time.Add(GTime.ElapsedGameTime);
            if (start)
            {
                alpha = (fadein - (time.Seconds + (time.Milliseconds / 1000.0f))) / (fadein);
                if (alpha ==0.0f)
                    start = false;
            }
            else
            {
                if (time.Seconds >= wait)
                    alpha = 1.0f-((wait + fadeout - time.Seconds + ((1000 - time.Milliseconds) / 1000.0f)) / (fadeout-1f));
                if (time.Seconds > wait + fadeout)
                    return true;
            }
            return false;
        }
    }
}
