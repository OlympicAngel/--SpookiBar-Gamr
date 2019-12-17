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
    class EffectRender
    {
        public static Random rnd = new Random();

        public static void RenderBroken(SpriteBatch sb, Texture2D image, Rectangle drawAs, int amount, bool isGliching, int valu)
        {

            int fix = amount;
            while (drawAs.Width % fix != 0)
            {
                fix--;
            }
            int width = (image.Width / fix);
            Rectangle disten = new Rectangle(0, 0, width, image.Height);
            for (int i = 0; i <= fix; i++)
            {
                disten.X = (width * i) + rnd.Next(0 - valu, valu + 1);
                disten.Y = rnd.Next(0 - valu, valu + 1);
                if (isGliching)
                    disten.Width += rnd.Next(0 - valu, valu + 1) * 2;


                sb.Draw(image,
                    new Rectangle(drawAs.X +
                                  ((drawAs.Width / fix) * i), drawAs.Y, (drawAs.Width / fix), drawAs.Height),
                    disten,
                    Color.White);
            }
            GC.Collect();
        }

            public static void RenderBroken(SpriteBatch sb,Texture2D image,Rectangle drawAs,int amount,int valu)
        {
            int fix = amount + 1;
            while (drawAs.Width % fix != 0)
            {
                fix--;
            }
            int width = (image.Width / fix);
            Rectangle disten = new Rectangle(0, 0, width, image.Height);
            for (int i = 0; i <= fix; i++)
            {
                disten.X = (width * i) + rnd.Next(valu - 1, valu + 1);
                disten.Y = rnd.Next(valu-2, valu + 2);
                if(i%2==0)
                    disten.Width += rnd.Next(valu - 1, valu + 1) / ((Math.Abs(valu)+2)/2);
                else
                    disten.Width -= rnd.Next(valu - 1, valu + 1);


                sb.Draw(image,
                    new Rectangle(drawAs.X +
                                  ((drawAs.Width / fix) * i), drawAs.Y, (drawAs.Width / fix), drawAs.Height),
                    disten,
                    Color.White);
            }
            GC.Collect();
        }

            public static Texture2D RenderBlured(SpriteBatch sb, Texture2D image, Rectangle drawAs, double amount)
            {
                
                Rectangle outer = new Rectangle(0, 0, (int)(drawAs.Width / amount), (int)(drawAs.Height / amount));
                RenderTarget2D renderTarget2DBLUR = new RenderTarget2D(image.GraphicsDevice, (int)(drawAs.Width / amount), (int)(drawAs.Height / amount));

                image.GraphicsDevice.SetRenderTarget(renderTarget2DBLUR);
                sb.End();

                sb.Begin();

                sb.Draw(image, outer, Color.White);
                sb.End();

                image.GraphicsDevice.SetRenderTargets(null);

                sb.Begin();

                return renderTarget2DBLUR;

            }
    }
}
