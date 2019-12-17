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
    class MenuScreen : Screen
    {
        Random rnd = new Random();
        MenuText[] playOptions;
        TimeSpan time = new TimeSpan();
        Rectangle bgPos;
        Texture2D[] jump;
        int jumppos = 0;
        TimeSpan jumptime = new TimeSpan();
        Rectangle jumpDraw;
        TopMenuText del;
        TextItem version;

        public MenuScreen(Texture2D n, SpriteFont logo, SpriteFont slash, SpriteFont menu, Rectangle full, Texture2D[] jumpT)
            : base(n)
        {
            this.version = new TextItem(slash,"Build V:"+DataLoad.version,new Vector2(0,full.Height - slash.MeasureString(DataLoad.version).Y));
            del = new TopMenuText(slash, "RESET DATA", new Vector2(full.Width - slash.MeasureString("RESET DATA").X, full.Height - slash.MeasureString("RESET DATA").Y), (int)slash.MeasureString("RESET DATA").X, (int)slash.MeasureString("RESET DATA").Y);

            playOptions = new MenuText[4];

            Vector2 basePos = new Vector2((int)(full.Width * 0.0291666666666667), (int)(full.Height * 0.1962962962962963));
            int width = (int)(full.Width * 0.2927083333333333),
                height = (int)(full.Height * 0.1055555555555556);
            playOptions[0] = new MenuText(menu, "New Game", basePos, width, height);

            basePos.Y = basePos.Y + (int)(height * 1.056) + (int)(full.Height * 0.0205703703703704);
            playOptions[1] = new MenuText(menu, "Load Night #", basePos, width, height);
            playOptions[1].motion.delayTime_in = 0.8;
            playOptions[1].motion.totalTime_in = 0.7;

            basePos.Y = basePos.Y + (int)(height * 1.056) + (int)(full.Height * 0.0205703703703704);
            playOptions[2] = new MenuText(menu, "Nightmare", basePos, width, height);
            playOptions[2].motion.delayTime_in = 0.85;
            playOptions[2].motion.totalTime_in = 0.7;

            basePos.Y = basePos.Y + (int)(height * 1.056) + (int)(full.Height * 0.0205703703703704);
            playOptions[3] = new MenuText(menu, "Night 666", basePos, width, height);
            playOptions[3].motion.delayTime_in = 0.9;
            playOptions[3].motion.totalTime_in = 0.7;

            bgPos = new Rectangle(full.Width / 3, (int)(full.Height * 0.1037037037037037), (full.Width / 3) * 2, full.Height - (int)(full.Height * 0.1037037037037037));

            this.jump = jumpT;
            jumpDraw = new Rectangle((int)(full.Width * 1.1 / 4), full.Height - bgPos.Height, (int)(bgPos.Height * 0.6851851851851852), bgPos.Height);

            GC.Collect();
        }
        public void PrintBG(SpriteBatch sb, Rectangle full)
        {
            if (time.TotalSeconds < 1.2)
            {
                if (rnd.Next(0, 3) == 0)
                    EffectRender.RenderBroken(sb, this.screenBG, bgPos, 200, false, 10);
            }
            else
                EffectRender.RenderBroken(sb, this.screenBG, bgPos, 10, false, 1);
            if (rnd.Next(0, 40) == 1)
                CPixel.Blend(sb, BlendState.Additive, this.screenBG, bgPos, Color.White);

            int threeshold = 65;
            if (jumptime.TotalSeconds > threeshold && jumptime.Milliseconds < 800)
            {
                EffectRender.RenderBroken(sb, jump[jumppos], jumpDraw, 200, false, 10);
                CPixel.Blend(sb, BlendState.Additive, jump[jumppos], jumpDraw, Color.White*0.3f);
                //sb.Draw(jump[jumppos], jumpDraw, Color.White);
            }
            else if (jumptime.TotalSeconds > threeshold + 2)
            {
                jumptime = new TimeSpan();
                jumppos = rnd.Next(0, 3);
            }


        }
        public void Print(SpriteBatch sb, MouseState mouse, User u, Rectangle full, GameTime timer)
        {
            jumptime = jumptime.Add(timer.ElapsedGameTime);
            if (time.TotalSeconds < 0.5)
            {
                time = time.Add(timer.ElapsedGameTime);
            }
            else
            {
                if (timer.TotalGameTime.Milliseconds % 250 == 0 && time.TotalSeconds > 4)
                {
                    if (rnd.Next(0, 40) == 1)
                        time = new TimeSpan();
                }
                else
                    time = time.Add(timer.ElapsedGameTime);
            }
            playOptions[1].text = playOptions[1].text.Replace("#", u.save.ToString());

            playOptions[0].Draw(sb, mouse, timer, 1.0f);
            playOptions[1].Draw(sb, mouse, timer, 1.0f);
            playOptions[1].text = playOptions[1].text.Replace(u.save.ToString(),"#");

            if(!u.lock7)
                playOptions[3].Draw(sb, mouse, timer,0.2f);
            else
                playOptions[3].Draw(sb, mouse, timer, 1.0f);

            if (!u.lock6)
                playOptions[2].Draw(sb, mouse, timer, 0.2f);
            else
                playOptions[2].Draw(sb, mouse, timer, 1.0f);


            del.Draw(sb, mouse, timer);

            version.DrawShadow(sb, 25, Color.White * 0.03f);

            GC.Collect();
        }

        #region Update
        public int ChangeRoom(bool freez, MouseState mouse, User u)
        {
            if (freez && playOptions[0].IsClicked(mouse, true))
                return 2;
            if (freez && playOptions[1].IsClicked(mouse, true))
                return 3;
            if (u.lock6 && freez && playOptions[2].IsClicked(mouse, true))
                return 6;
            if (u.lock7 && freez && playOptions[3].IsClicked(mouse, true))
                return 7;

            if (del.IsClicked(mouse, freez))
                return 11;

            return -1;
        }
        #endregion
    }
}
