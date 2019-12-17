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
    class Monitor : Item
    {
        int correctRoom = 1;
        int yPos = 0;
        Rectangle inner;
        public Room[] rooms { get; set; }
        bool down = true;
        Texture2D map;
        Texture2D point;
        Rectangle innerMap;
        Item[] clickCamera;

        #region GetDown
        public bool GetDown()
        {
            return this.down;
        }
        #endregion
        #region GetCorrectRoom
        public int GetCorrectRoom()
        {
            return correctRoom;
        }
        #endregion
        #region Monitor
        public Monitor(Texture2D texture, Rectangle pos, Room[] room, Texture2D t, Texture2D point)
            : base(texture, pos)
        {
            this.point = point;
            this.map = t;
            this.inner = new Rectangle((int)(this.pos.X + this.pos.Width * 0.0498), (int)(this.pos.Y + this.pos.Width * 0.011) - yPos, (int)(this.pos.Width * 0.9044), (int)(this.pos.Height * 0.964));
            this.rooms = new Room[room.Length - 1];
            for (int i = 1; i < room.Length; i++)
            {
                this.rooms[i - 1] = room[i];
            }
            yPos = this.pos.Height;
            this.innerMap = new Rectangle(this.inner.X + this.inner.Width - this.inner.Width / 3, this.inner.Y + (int)(this.inner.Height / 2.1), this.inner.Width / 3, this.inner.Height / 2);
            this.clickCamera = new Item[11];
            this.clickCamera[0] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.53), innerMap.Y + (int)(innerMap.Height * 0.001), innerMap.Width / 4, innerMap.Height / 10));
            this.clickCamera[1] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.20), innerMap.Y + (int)(innerMap.Height * 0.16), (int)(innerMap.Width * 0.1625), innerMap.Height / 10));
            this.clickCamera[2] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.5), innerMap.Y + (int)(innerMap.Height * 0.17), (int)(innerMap.Width * 0.2), (int)(innerMap.Height * 0.1)));
            this.clickCamera[3] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.625), innerMap.Y + (int)(innerMap.Height * 0.471), (int)(innerMap.Width * 0.18), (int)(innerMap.Height * 0.1)));
            this.clickCamera[4] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.8375), innerMap.Y + (int)(innerMap.Height * 0.628), (int)(innerMap.Width * 0.15), (int)(innerMap.Height * 0.2)));
            this.clickCamera[5] = new Item(null, new Rectangle(-1000, -1000, 0, 0));
            this.clickCamera[6] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.0875), innerMap.Y + (int)(innerMap.Height * 0.3142), (int)(innerMap.Width * 0.1), (int)(innerMap.Height * 0.1)));
            this.clickCamera[7] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.09), innerMap.Y + (int)(innerMap.Height * 0.913), (int)(innerMap.Width * 0.15), (int)(innerMap.Height * 0.07)));
            this.clickCamera[8] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.088), innerMap.Y + (int)(innerMap.Height * 0.785), (int)(innerMap.Width * 0.11), (int)(innerMap.Height * 0.1)));
            this.clickCamera[9] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.225), innerMap.Y + (int)(innerMap.Height * 0.55), (int)(innerMap.Width * 0.15), (int)(innerMap.Height * 0.07)));
            this.clickCamera[10] = new Item(null, new Rectangle(innerMap.X + (int)(innerMap.Width * 0.0875), innerMap.Y + (int)(innerMap.Height * 0.16), (int)(innerMap.Width * 0.1), (int)(innerMap.Height * 0.1)));
        }
        #endregion
        #region Draw
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(this.texture, new Rectangle(this.pos.X, this.pos.Y + yPos, this.pos.Width, this.pos.Height), Color.White);
            rooms[correctRoom - 1].PrintRoom(sb, new Rectangle(this.inner.X, this.inner.Y + yPos, this.inner.Width, this.inner.Height));
        }
        #endregion
        #region DrawMap
        public void DrawMap(SpriteBatch sb, Mob[] moib, bool cheat, MouseState m)
        {
            bool opa = innerMap.Contains(m.X, m.Y);
            this.innerMap.Y += yPos;
            sb.Draw(this.map, this.innerMap, Color.White * (0.2f + Convert.ToInt32(opa) * 0.5f));
            if (cheat && !down)
                DrawMobsCheat(sb, moib);
            this.innerMap.Y -= yPos;
        }
        #endregion
        #region SetCorrectRoom
        public void SetCorrectRoom(int room)
        {
            this.correctRoom = room;
        }
        #endregion
        #region IsSlide
        public bool IsSlide()
        {
            if (down)
                yPos -= 10;
            else
                yPos += 10;

            if (yPos <= 0)
            {
                yPos = 0;
                down = false;
                return true;
            }
            if (yPos >= this.pos.Height)
            {
                yPos = this.pos.Height;
                down = true;
                return true;
            }
            return false;
        }
        #endregion
        #region IsSlide
        public bool IsSlide(int num)
        {
            if (down)
                yPos -= num;
            else
                yPos += num;

            if (yPos <= -5)
            {
                yPos = -5;
                down = false;
                return true;
            }
            if (yPos >= this.pos.Height)
            {
                yPos = this.pos.Height;
                down = true;
                return true;
            }
            return false;
        }
        #endregion
        #region Click_IsSwitchRoom
        public int Click_IsSwitchRoom(MouseState mouse)
        {
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                int x = mouse.X;
                int y = mouse.Y;
                for (int i = 0; i < clickCamera.Length; i++)
                {
                    if (clickCamera[i] != null && correctRoom != i + 1 && clickCamera[i].IsClicked(mouse, true))
                        correctRoom = i + 1;
                }

            }
            else
            {
                KeyboardState keysSts = Keyboard.GetState();
                Keys[] pressed = keysSts.GetPressedKeys();
                int preRoom = correctRoom;
                foreach (Keys key in pressed)
                {
                    switch (key)
                    {
                        case Keys.D1:
                            correctRoom = 1;
                            break;
                        case Keys.D2:
                            correctRoom = 2;
                            break;
                        case Keys.D3:
                            correctRoom = 3;
                            break;
                        case Keys.D4:
                            correctRoom = 4;
                            break;
                        case Keys.D5:
                            correctRoom = 5;
                            break;
                        case Keys.D7:
                            correctRoom = 7;
                            break;
                        case Keys.D8:
                            correctRoom = 8;
                            break;
                        case Keys.D9:
                            correctRoom = 9;
                            break;
                        case Keys.D0:
                            correctRoom = 10;
                            break;
                        case Keys.OemMinus:
                            correctRoom = 11;
                            break;
                        default:
                            break;
                    }
                }
                if (preRoom != correctRoom)
                    this.sound.Play(0.0f, true);
            }
            return correctRoom;
        }
        #endregion
        #region GetInner
        public Rectangle GetInner()
        {
            return new Rectangle(this.inner.X, this.inner.Y + yPos, this.inner.Width, this.inner.Height);
        }
        #endregion
        #region Reset
        public void Reset()
        {
            down = true;
            correctRoom = 1;
            yPos = this.pos.Height;
        }
        #endregion
        public void DrawMobsCheat(SpriteBatch sb, Mob[] mobs)
        {
            for (int i = 0; i < mobs.Length; i++)
            {
                if (mobs[i].GetRoom() >= 0)
                {
                    Rectangle cam = clickCamera[mobs[i].GetRoom() - 1].pos;
                    cam.Y += cam.Height / 2 + cam.Height / 5;
                    cam.X += cam.Width / 5;
                    cam.Width = 15;
                    cam.Height = 15;
                    cam.X += i * cam.Width;
                    cam.Y += yPos;
                    sb.Draw(point, cam, new Color(Convert.ToInt32(i == 0) * 100 + 50, Convert.ToInt32(i == 1) * 100 + 50, Convert.ToInt32(i == 2) * 100 + 50));
                }
            }
        }
    }
}
