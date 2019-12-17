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
    class Screen
    {
        public List<Item> controls;

        public Texture2D screenBG { get; set; }

        public Screen(Texture2D n)
        {
            this.screenBG = n;
            this.controls = new List<Item>();
        }

        public virtual void Print(SpriteBatch sb,Rectangle full)
        {
            if (screenBG != null)
            sb.Draw(screenBG, full, Color.White);
            PrintC(sb);
        }
        public void PrintC(SpriteBatch sb)
        {
            foreach (Item item in controls)
            {
                item.Draw(sb);
            }
        }

        public virtual void Dispose()
        {
            try
            {
                foreach (Item item in this.controls)
                { item.Dispose(); }
                screenBG.Dispose();
            }
            catch { }
        }
    }
}
