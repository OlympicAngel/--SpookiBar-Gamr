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
    class ProfileScreen:Screen
    {
        TextItem text;

        public ProfileScreen(Texture2D tex, SpriteFont menu)
            : base(tex)
        {
            this.text = new TextItem(menu, "<-Back", new Vector2(0, 0));
        }

        public void Print(SpriteBatch sb, MouseState m)
        {
            base.Print(sb);

            this.text.DrawPhantom(sb, 0);
            this.text.DrawHover(sb, m);
        }

        public int Update(MouseState m)
        {
            if (text.IsClicked(m, true))
                return 2;
            return -1;
        }
    }
}
