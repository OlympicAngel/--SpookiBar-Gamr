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
    class TextItem:Item
    {
        public SpriteFont font { get; set; }
        public string text { get; set; }
        public Vector2 vec { get; set; }
        int stop;

        public override void Dispose()
        {
            base.Dispose();
            font = null;
            text = null;
            vec = new Vector2();
            stop = 0;
        }

        public TextItem(SpriteFont font, string text,Vector2 vec)
            : base(null, new Rectangle((int)vec.X, (int)vec.Y, (int)(font.MeasureString(text)).X, (int)(font.MeasureString(text)).Y))
        {
            this.font = font;
            this.text = text;
            this.vec = vec;
            stop = 0;
        }

        public void DrawShadow(SpriteBatch sb,int shadow)
        {
            for (int i = 0; i < shadow; i++)
            {
                float tra = 1.0f - ((i * 1.0f) / (shadow * 1.0f));
                sb.DrawString(font, text, new Vector2(pos.X,pos.Y+i), Color.Black * tra, 0, new Vector2(0f,0f), 1.0f, SpriteEffects.None, 0.5f);
            }
        }

        public void DrawShadow(SpriteBatch sb, int shadow,Color c)
        {
            for (int i = 0; i < shadow; i++)
            {
                float tra = 1.0f - ((i * 1.0f) / (shadow * 1.0f));
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y + i), c * tra, 0, new Vector2(0f, 0f), 1.0f, SpriteEffects.None, 0.5f);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.White*0.9f);
        }

        public void Draw(SpriteBatch sb,string replace)
        {
            sb.DrawString(font, text.Replace("#", replace), new Vector2(pos.X, pos.Y), Color.White * 0.9f);
        }

        public void Draw(SpriteBatch sb, float scale)
        {
            sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.White, 0, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
        }
        public void Draw(SpriteBatch sb, float scale,Color c,int move)
        {
            sb.DrawString(font, text, new Vector2(pos.X, pos.Y + move), c, 0, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
        }

        public void DrawAs(SpriteBatch sb, string replace)
        {
            sb.DrawString(font, replace, new Vector2(pos.X, pos.Y), Color.White);
        }

        public void DrawAs(SpriteBatch sb, string replace,int x)
        {
            sb.DrawString(font, replace, new Vector2(pos.X+x, pos.Y), Color.White);
        }

        public void DrawHover(SpriteBatch sb, MouseState hoverMouse)
        {
            if (pos.Contains(hoverMouse.X, hoverMouse.Y))
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.Gold * 0.9f);
            else
                sb.DrawString(font, text, new Vector2(pos.X, pos.Y), Color.White * 0.9f);
        }

        public void DrawPhantom(SpriteBatch sb,int add)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            Random r3 = new Random();
            float value = 0.7f;
            if (add < 50)
            {
                sb.DrawString(font, text, new Vector2(pos.X - r3.Next(2, 6), pos.Y - r3.Next(1, 5) - r3.Next(0, add / 2)), Color.Blue * (value - 0.2f), 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                sb.DrawString(font, text, new Vector2(pos.X + r3.Next(1, 7), pos.Y + r3.Next(1, 6) + r3.Next(0, add / 2)), Color.Blue * (value - 0.2f), 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);

                sb.DrawString(font, text, new Vector2(pos.X + r3.Next(2, 7) + r3.Next(0, add), pos.Y + r3.Next(-2, 2)), Color.Red * value);
                sb.DrawString(font, text, new Vector2(pos.X - r3.Next(2, 7) - r3.Next(0, add), pos.Y + r3.Next(-2, 2)), Color.Green * value);
            }
            else
            {
                sb.DrawString(font, text, new Vector2(pos.X - r3.Next((int)(add * 0.1), add / 4), pos.Y - r3.Next((int)(add * 0.1) / 2, add / 2) - r3.Next(0, add / 2)), Color.Blue * (value-0.2f), 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                sb.DrawString(font, text, new Vector2(pos.X + r3.Next((int)(add * 0.1), add / 4), pos.Y + r3.Next((int)(add * 0.1) / 2, add / 2) + r3.Next(0, add / 2)), Color.Blue * (value - 0.2f), 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);

                sb.DrawString(font, text, new Vector2(pos.X + r3.Next((int)(add * 0.2), add / 2) + r3.Next(0, add / 2), pos.Y + r3.Next(-2, 2)), Color.Red * value);
                sb.DrawString(font, text, new Vector2(pos.X - r3.Next((int)(add * 0.2), add / 2) - r3.Next(0, add / 2), pos.Y + r3.Next(-2, 2)), Color.Green * value);
            }
            sb.End();
            sb.Begin();
        }

        public void Shake()
        {
            Random r3 = new Random();
            Random r2 = new Random(r3.Next(-99958,454125));
            Random r = new Random(r2.Next(-9000,9000));
            if (stop == 0)
            {
                this.pos.X = (int)(this.vec.X) + r.Next(-3, 3);
            }
            stop = stop +r2.Next(0, 2);
            if (stop > 3 || stop > 3)
                stop = 0;
        }

    }
}
