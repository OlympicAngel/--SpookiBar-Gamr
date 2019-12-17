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
    class ForceFiels
    {
        Texture2D force;
        Texture2D intreapt;
        Random rnd = new Random(99);
        public SoundEffectPlay baser{get;set;}


        public void reset()
        {
            baser.Off();
        }

        public ForceFiels(ContentManager g)
        {
            baser = new SoundEffectPlay(1.0f, g.Load<SoundEffect>("sound/power"), 0.5f);
            baser.PlayLoop(0.0f);
            baser.Off();

            force = g.Load<Texture2D>("forcefield/force field");
            intreapt = g.Load<Texture2D>("forcefield/electro");
        }

        public void Draw(SpriteBatch sb, Rectangle full, int pos, bool right, bool left)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            if (right || left)
            {
                float vol = 0.0f, vol2 = 0.0f;
                if (right)
                    vol = (float)((((double)(Math.Min(Math.Max((pos - 90), 10) / (double)80, 1.0))) * 1.0f));
                if (left)
                    vol2 = (float)((((double)(Math.Min(Math.Max((80 - pos), 10) / (double)80, 1.0))) * 1.0f));
                if (pos == 90) vol2 = vol = 0.15f;
                baser.SetVol(Math.Max(vol2, vol));
                baser.PlayLoop((float)((((double)(90 - pos) / (double)180)) * 1.0f));
            }
            else
                baser.Off();

            Color c = Color.Transparent;
            if (left && pos <= 90)
                c = Color.White * (float)((((double)(Math.Abs(120 - pos)) / (double)140)) * 1.0f);
            if (right && pos >= 90)
                c = Color.White * (float)((((double)(Math.Abs(60 - pos)) / (double)140)) * 1.0f);
            c = c * (rnd.Next(10, 20) / 20.0f);
            sb.Draw(force, full, c);
            if (rnd.Next(1, 10) == 2)
                sb.Draw(intreapt, new Rectangle(rnd.Next(0, full.Width), rnd.Next(0, full.Height), intreapt.Width, intreapt.Height), new Rectangle(0, 0, intreapt.Width, intreapt.Height), c * (rnd.Next(1, 9) / 8.0f), rnd.Next(0, 360) / 1.0f, Vector2.Zero, SpriteEffects.None, 0);
            if (rnd.Next(1, 10) == 2)
                sb.Draw(intreapt, new Rectangle(rnd.Next(0, full.Width), rnd.Next(0, full.Height), intreapt.Width / 2, intreapt.Height / 2), new Rectangle(0, 0, intreapt.Width, intreapt.Height), c * (rnd.Next(1, 9) / 8.0f), rnd.Next(0, 360) / 1.0f, Vector2.Zero, SpriteEffects.None, 0);
            
            sb.End();
            sb.Begin();
        }
    }
}
