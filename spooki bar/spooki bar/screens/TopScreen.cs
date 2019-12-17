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
using System.Data;

namespace spooki_bar.screens
{
    class TopScreen : Screen
    {
        TextItem theuser;
        List<TextItem> top;
        Rectangle full;
        SpriteFont menu;

        public TopScreen(Texture2D n, SpriteFont menu,User user,        Rectangle full)
            : base(n)
        {
            this.menu = menu;
            top = new List<TextItem>();
            this.theuser = new TextItem(menu, "R", new Vector2(0, full.Height/9));
            this.full = full;
            LoadList(user);
        }
        public void Print(SpriteBatch sb, MouseState m, Rectangle full)
        {
            base.Print(sb, full);

            theuser.Draw(sb);

            foreach (TextItem item in top)
            {
                item.Draw(sb);
            }
        }

        public int Update(MouseState m, User user,bool freez)
        {
            if (theuser.IsClicked(m, freez))
                LoadList(user);
            return 6;
        }

        public void LoadList(User user)
        {
            string[] str = DataLoad.GetTop(user);
            top = null;
            GC.Collect();
            top = new List<TextItem>();
            int i = 0;
            foreach (string item in str)
            {
                top.Add(new TextItem(menu, item.Split('|')[0].Replace(",", " Points/ Rank:" + (i + 1) + " - "), new Vector2(full.Width / 3, full.Height / 9 + i * (menu.MeasureString("I").Y + full.Height / 100))));
                i++;
            }
        }
    }
}
