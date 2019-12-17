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
    class CPixel
    {
        public static Texture2D DarkGrey { get; set; }
        public static Texture2D Grey { get; set; }
        public static Texture2D LightGrey { get; set; }
        public static Texture2D DarkRed { get; set; }
        public static Texture2D LightRed { get; set; }
        public static Texture2D RedFade { get; set; }

        public static void Blend(SpriteBatch sb,BlendState blend,Texture2D aa,Rectangle fullScreen,Color c)
        {
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, blend);
                sb.Draw(aa, fullScreen, c);
                sb.End();
                sb.Begin();
        }
    }
}
