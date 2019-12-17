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

namespace spooki_bar.screens
{
    class LoadingScreen:Screen
    {
        Texture2D abz;
        SpriteFont font;
        TextItem info;
        int intreval = 0;
        Random rnd = new Random();
        bool dis = false,glich = false;

        public void SetDis(bool val)
        {
            dis = val;
        }

        public LoadingScreen(Texture2D t,Texture2D abz,SpriteFont font,Rectangle fullScreen)
            : base(t)
        {
            this.abz = abz;
            this.font = font;
            info = new TextItem(font, "", new Vector2(10, fullScreen.Height - fullScreen.Height/9));
        }

        public void Draw(SpriteBatch sb, string text, GameTime timer, Rectangle fullScreen)
        {
            if (!dis)
            {
                Texture2D aa = EffectRender.RenderBlured(sb, this.screenBG, fullScreen, 5); ;
                if (timer.TotalGameTime.Milliseconds % 50 == 0 && intreval < fullScreen.Width / 6)
                    intreval = intreval + 1;
                EffectRender.RenderBroken(sb, this.screenBG, fullScreen, fullScreen.Width/6 - intreval, 2);
                info.text = text;

                if (glich)
                    info.DrawPhantom(sb, 10);
                glich = rnd.Next(0, 20) == 1;
                if (glich)
                    info.DrawPhantom(sb, 3);
                else
                {
                    info.DrawShadow(sb, 20);
                    info.Draw(sb);

                    if (rnd.Next(0, 30) == 1)
                        info.DrawPhantom(sb, 3);
                }
                sb.Draw(abz, new Vector2((fullScreen.Width / 2) - abz.Width / 2, (fullScreen.Height) / 2 - abz.Height / 2), Color.White * (((timer.TotalGameTime.Milliseconds + 800) / 1800f) * (intreval / 490.0f)));

            }
        }

        public override void Dispose()
        {
            dis = true;
            base.Dispose();
            abz.Dispose();
            abz = null;
            font = null;
            info.Dispose();
            info = null;
            intreval = 0;
            GC.Collect();
        }
    }
}
