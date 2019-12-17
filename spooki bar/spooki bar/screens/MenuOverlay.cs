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
    class MenuOverlay : Screen
    {
        Random rnd = new Random();
        TopMenuText[] gameOption;
        TimeSpan time = new TimeSpan();
        Rectangle[] hud;

        TextItem text;

        public MenuOverlay(Texture2D n, SpriteFont logo, SpriteFont slash, SpriteFont menu, Rectangle full)
            : base(n)
        {
            int x = full.Width / 25;
            int y = full.Height / 11;
            text = new TextItem(logo, "SpookI Bar", new Vector2((int)(x * 1.5), y / 4));

            Vector2 basePos = Vector2.Zero;
            int height;


            string[] names = new string[6];
            names[0] = " H  ";
            names[1] = " Profile ";
            names[2] = " Scoreboard ";
            names[3] = " Store ";
            names[4] = " Cretures-Info ";
            names[5] = " About ";

            gameOption = new TopMenuText[6];
            basePos.X = (int)(full.Width * 0.350375);
            height = (int)(full.Height * 0.0694444444444444);
            basePos.Y = 0;
            for (int i = 0; i < gameOption.Length; i++)
            {
                gameOption[i] = new TopMenuText(slash, names[i], basePos, (int)slash.MeasureString(names[i]).X, height);
                basePos.X = basePos.X + (int)slash.MeasureString(names[i]).X;
            }

            hud = new Rectangle[4];
            hud[0] = new Rectangle(0, (int)(full.Height * 0.1037037037037037), (int)(full.Width * 0.01875), (int)(full.Height));
            hud[1] = new Rectangle(0, 0, (int)(full.Width), (int)(full.Height * 0.1037037037037037));
            hud[2] = new Rectangle(0, (int)(full.Height * 0.1037037037037037), (int)(full.Width), (int)(full.Height * 0.02));
            hud[3] = new Rectangle(0, (int)(full.Height * 0.1037037037037037) - (int)(full.Height * 0.001), (int)(full.Width), Math.Max((int)(full.Height * 0.001), 1));


            GC.Collect();
        }
        #region Print func

        public void Print(SpriteBatch sb, MouseState mouse, User u, Rectangle full, GameTime timer)
        {

            sb.Draw(CPixel.RedFade, hud[0], Color.White * 0.2f);
            sb.Draw(CPixel.DarkGrey, hud[1], Color.Black);
            sb.Draw(CPixel.RedFade, hud[2], Color.White * 0.35f);
            sb.Draw(CPixel.LightGrey, hud[3], Color.White);

            foreach (TopMenuText n in gameOption)
                if (n != null)
                    n.Draw(sb, mouse, timer);

            text.Shake();

            if (time.TotalSeconds < 0.5)
            {
                time = time.Add(timer.ElapsedGameTime);
                text.DrawShadow(sb, 15, Color.Blue * 0.1f);
                text.DrawPhantom(sb, 80);
            }
            else
            {

                text.DrawPhantom(sb, 10);
                text.DrawShadow(sb, 4, Color.Red * 0.5f);
                text.Draw(sb);
                if (timer.TotalGameTime.Milliseconds % 250 == 0 && time.TotalSeconds > 3)
                {
                    if (rnd.Next(0, 40) == 1)
                        time = new TimeSpan();
                }
                else
                    time = time.Add(timer.ElapsedGameTime);
            }

            GC.Collect();
        }
        #endregion

        #region Update
        public int ChangeRoom(bool freez, MouseState mouse, User u)
        {

            if (freez && gameOption[0].IsClicked(mouse, true))
                return 1;
            if (freez && gameOption[1].IsClicked(mouse, true))
                return 4;
            if (freez && gameOption[2].IsClicked(mouse, true))
                return 5;
            if (freez && gameOption[3].IsClicked(mouse, true))
                return 8;
            if (freez && gameOption[4].IsClicked(mouse, true))
                return 9;
            if (freez && gameOption[5].IsClicked(mouse, true))
                return 10;
            return -1;

        }
        #endregion
    }
}
