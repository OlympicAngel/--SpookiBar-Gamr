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
    class MobTexture
    {
        public Texture2D texture { get; set; }
        public int roomID { get; set; }

        public MobTexture(int room, Texture2D t)
        {
            this.texture = t;
            this.roomID = room;
        }
    }
}
