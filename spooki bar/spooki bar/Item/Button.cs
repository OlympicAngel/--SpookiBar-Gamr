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
    class Button:Item
    {
        Texture2D textureOn;
        bool isOn;

        public Button(Texture2D tOn, Texture2D tOff, Rectangle pos):base(tOff,pos)
        {
            this.textureOn = tOn;
            this.isOn = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isOn)
                sb.Draw(textureOn, pos, Color.White);
            else
                base.Draw(sb);
        }

        public bool IsOn()
        {
            return this.isOn;
        }

        public bool Toggle()
        {
            this.sound.PlayAs(0.0f,0.1f);
            this.isOn = !this.isOn;
            return this.isOn;
        }
        public bool Toggle(bool sts)
        {
            this.isOn = !this.isOn;
            return this.isOn;
        }

        public override void Dispose()
        {
            base.Dispose();
            textureOn.Dispose();
        }
    }
}
