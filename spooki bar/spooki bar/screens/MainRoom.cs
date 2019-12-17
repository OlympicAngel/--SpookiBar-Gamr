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
using System.IO;

namespace spooki_bar
{
    class MainRoom : Room
    {
        public int pos { get; set; }
        int prepos = -9;//1 0 -1 as right mid left
        Texture2D[] postion;
        int add, drawDirection = 0, drawTime = 0;
        double horscrool = 5;
        Rectangle top, bot;
        public ForceFiels force { get; set; }
        Texture2D blackfade;
        Gif[] animation;
        SoundEffectPlay motion;
        Random rnd = new Random();

        public MainRoom(Texture2D[] t, Gif[] move, SoundEffect mover)
            : base(0)
        {
            t = postion = t;
            pos = 0;
            add = 0;
            blackfade = new Texture2D(postion[0].GraphicsDevice, 1, 2);
            Color[] c = new Color[2];
            c[0] = Color.Black;
            c[1] = Color.Black * 0.5f;
            blackfade.SetData<Color>(c);
            animation = move;
            TimeController.RoomMovementWait = new TimeSpan();

            motion = new SoundEffectPlay(1.0f, mover, 0.05f);
        }

        public override void PrintRoom(SpriteBatch sb, Rectangle pos2)
        {
            this.PrintRoom(sb, pos2, false, false, false, false);
        }

        public void PrintRoom(SpriteBatch sb, Rectangle pos2, bool a2, bool a1, bool emptyBattery, bool inMonitor)
        {
            double height = postion[0].Height * (1 - 1 / horscrool); //low then 1
            double y = 0;

            y = ((add + pos2.Height / 2) / (double)pos2.Height) * (1 / horscrool) * (double)postion[0].Height;


            Rectangle take = new Rectangle(0, (int)y, postion[0].Width, (int)height);

            sb.Draw(this.postion[pos + 1], pos2, take, Color.White);

            if (!emptyBattery && (inMonitor || (prepos == -9 && TimeController.NightTime.Milliseconds >= 970 && rnd.Next(0,10) == 1)))
                TimeController.RandomScreenEffect = new TimeSpan();

            if (TimeController.RandomScreenEffect.Milliseconds < 100 && TimeController.RandomScreenEffect.Seconds < 2)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                if (rnd.Next(0, 40) == 1)
                    sb.Draw(this.postion[pos + 1], pos2, take, Color.White);
                if (rnd.Next(0, 5) == 2)
                    EffectRender.RenderBroken(sb, this.postion[pos + 1], pos2, 150, false, 20);
                sb.End();
                sb.Begin();
            }

            if (prepos != -9)
                sb.Draw(animation[drawDirection].GetImg()[drawTime], pos2, take, Color.White);

            if (emptyBattery)
            {
                sb.Draw(blackfade, pos2, Color.White * 0.6f);
            }
            else
            {
                int tpos = 90;
                if (prepos != -9)
                {
                    if (drawDirection == 0)
                        tpos = drawTime * 10;
                    if (drawDirection == 1)
                        tpos = 180 - drawTime * 10;
                    force.Draw(sb, pos2, tpos, a1, a2);
                }
                else
                {
                    if (this.pos == -1) tpos = 10;
                    if (this.pos == 1) tpos = 170;
                    force.Draw(sb, pos2, tpos, a1, a2);
                }
            }

        }

        public void CalcView(MouseState m, Rectangle screen, GameTime g, bool isGameFocus)
        {
            KeyboardState keysSts = Keyboard.GetState();

            if (isGameFocus)
            {
                int adder = 0;
                if (keysSts.IsKeyDown(Keys.W))
                    adder = -1;
                if (keysSts.IsKeyDown(Keys.S))
                    adder = 1;
                Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y + adder * 10);
            }

            #region dinamic view
            if (this.top.Width == 0)
            {
                this.top = new Rectangle(0, 0, screen.Width, screen.Height / 2);
                this.bot = new Rectangle(0, screen.Height / 2, screen.Width, screen.Height / 2);
            }

            if (this.top.Contains(m.X, m.Y))
            {
                add = top.Height - m.Y;
            }
            if (this.bot.Contains(m.X, m.Y))
            {
                add = bot.Y - m.Y;
            }
            #endregion

            if (TimeController.RoomMovementWait.TotalMilliseconds >= 500 && prepos == -9)
            {
                prepos = -9;
                #region simple move
                if (m.X < screen.Width / 2 - screen.Width / 3 && pos < 1 || (keysSts.IsKeyDown(Keys.A) && pos < 1))
                {
                    prepos = pos;
                    pos++;
                }
                else if (m.X > screen.Width / 2 + screen.Width / 3 && pos > -1 || (keysSts.IsKeyDown(Keys.D) && pos > -1))
                {
                    prepos = pos;
                    pos--;
                }
                #endregion
                if (prepos != -9)
                {
                    #region define
                    if (prepos == 1 && pos == 0)
                    {
                        drawTime = 0;
                        drawDirection = 1;
                    }
                    else if (prepos == 0 && pos == 1)
                    {
                        drawTime = 8;
                        drawDirection = 1;
                    }
                    else if (prepos == 0 && pos == -1)
                    {
                        drawTime = 8;
                        drawDirection = 0;
                    }
                    else if (prepos == -1 && pos == 0)
                    {
                        drawTime = 0;
                        drawDirection = 0;
                    }
                    else
                    {
                        prepos = -9;
                    }
                    #endregion
                    motion.Off(true);
                    motion.PlayAt(0.0f, 0.15f, false);
                }

            }
            else if (prepos != -9)
            {
                if (!motion.IsPlaying())
                    motion.PlayAt(0.0f, 0.15f, false);
                TimeController.RoomMovementWait = new TimeSpan();
                #region preform
                if (prepos == 1 && pos == 0)
                {
                    drawTime++;
                    if (drawTime >= 9)
                    {
                        prepos = -9;
                        drawTime = 8;
                    }
                }
                else if (prepos == 0 && pos == 1)
                {
                    drawTime--;
                    if (drawTime < 0)
                    {
                        prepos = -9;
                        drawTime = 0;
                    }
                }

                else if (prepos == -1 && pos == 0)
                {
                    drawTime++;
                    if (drawTime >= 9)
                    {
                        prepos = -9;
                        drawTime = 8;
                    }
                }
                else if (prepos == 0 && pos == -1)
                {
                    drawTime--;
                    if (drawTime < 0)
                    {
                        prepos = -9;
                        drawTime = 0;
                    }
                }
                #endregion
            }
        }
    }
}
