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
    class Room
    {
        public Texture2D texture;
        int num;
        string path;

        public Room(int num)
        {
            this.texture = null;
            this.num = num;
        }

        public Room(Texture2D texture, int num)
        {
            this.texture = texture;
            this.num = num;
        }

        public Room(Texture2D texture, int num,string path)
        {
            this.texture = texture;
            texture.Dispose();
            this.num = num;
            this.path = "images/game /"  + path;
        }

        public virtual void PrintRoom(SpriteBatch sb,Rectangle pos)
        {
            sb.Draw(this.texture, pos, Color.White);
        }

        public virtual void PrintMobs(SpriteBatch sb, Mob[] mobs, Rectangle monitor)
        {
            for (int i = 0; i < mobs.Length; i++)
            {
                if (mobs[i].GetRoom() == this.num)
                    mobs[i].Draw(sb,monitor);
            }
        }

        public int GetNum()
        {
            return this.num;
        }

        public virtual void PlaySoundMove()
        {

        }

        public void Dispose()
        {
            if (path != null && path != "")
            {
                texture.Dispose();
            }
        }

        public void Reload(ContentManager c)
        {
            if (path!= null && path!="") 
                texture = c.Load<Texture2D>(path);
        }
    }
}
