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
    class GameInfoScreen : Screen
    {
        TextItem text;
        Texture2D info;

        public GameInfoScreen(Texture2D n,Texture2D info, SpriteFont menu)
            : base(n)
        {
            this.text = new TextItem(menu, "<-Back", new Vector2(0, 0));
            this.info = info;
        }
        public void Print(SpriteBatch sb,MouseState m,Rectangle full)
        {
            base.Print(sb,full);
            sb.Draw(this.info,full, Color.White);
            this.text.DrawPhantom(sb,0);
            this.text.DrawHover(sb,m);
        }

        public int Update(MouseState m)
        {
            if(text.IsClicked(m,true))
             return 2;
            return -1;
        }

        public override void Dispose()
        {
            
        }

        public void Reload(ContentManager contetn,string path)
        {
            
        }
    }
}
