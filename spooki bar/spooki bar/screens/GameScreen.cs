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
namespace spooki_bar.screens
{
    class GameScreen : Screen
    {
        #region משתנים
        int cameranoiseTeans = 50;
        bool cameranoiseTeansUp = false;
        Random rnd = new Random(15548);
        Texture2D blur;
        float blurTrans = 0f;
        public Room[] rooms { get; set; }
        public int roomView { get; set; }
        public Gif[] effects { get; set; }
        Item[] items;
        public Monitor monitor { get; set; }
        public bool cameraMod { get; set; }
        public bool motion { get; set; }
        public bool lightRight { get; set; }
        public bool lightLeft { get; set; }
        #endregion
        #region GameScreen
        public GameScreen(Room[] rooms, Gif[] effects, Item[] items, Monitor monitor, ForceFiels f,Texture2D blurFX)
            : base(null)
        {
            motion = false;
            this.rooms = rooms;
            this.effects = effects;
            this.items = items;
            this.monitor = monitor;
            roomView = 0;
            lightLeft = false;
            lightRight = false;
            cameraMod = false;
            ((MainRoom)(rooms[0])).force = f;
            blur = blurFX;
        }
        #endregion

        #region DrawAll
        public void DrawAll(SpriteBatch sB, Rectangle fullScreen, MouseState mouse, Mob[] m, bool cheat, bool isEmpty, GameTime t)
        {
            DrawRoom(sB, fullScreen, isEmpty);
            DrawMonitorSystem(m, sB, cheat, mouse, t,t);
            DrawButtons(sB);
            sB.Draw(blur,fullScreen,Color.White * Math.Abs(1-blurTrans));
        }
        #endregion
        #region DrawButtons
        public void DrawButtons(SpriteBatch sB)
        {
            this.items[2].Draw(sB);
            if (!cameraMod && !motion)
            {
                if(((MainRoom)rooms[0]).pos == 1)
                this.items[3].Draw(sB);
                if (((MainRoom)rooms[0]).pos == -1)
                this.items[4].Draw(sB);
            }
        }
        #endregion
        #region DrawMonitorSystem
        public void DrawMonitorSystem(Mob[] m, SpriteBatch sB, bool cheat, MouseState mou, GameTime t,GameTime time)
        {
            if (cameraMod || motion)
            {
                monitor.Draw(sB);
                DrawMobInRoom(m, sB);
                this.effects[0].Draw(sB, monitor.GetInner());
                if (motion && monitor.GetDown())
                    this.effects[1].Draw(sB, monitor.GetInner(), -1, true);
                else if (motion)
                    this.effects[1].Draw(sB, monitor.GetInner(), -15, true);
                if (cameraMod)
                {
                    this.effects[1].DrawTrans(sB, monitor.GetInner(), Math.Max(cameranoiseTeans, 1), false, time.ElapsedGameTime);
                    if (t.TotalGameTime.Milliseconds % 100 == 0)
                    {
                        if (rnd.Next(0, 10) == 1)
                        {
                            cameranoiseTeans = cameranoiseTeans + rnd.Next(-15, 15);
                           if( rnd.Next(0, 20) ==1)
                               cameranoiseTeansUp = !cameranoiseTeansUp;
                        }
                        if (cameranoiseTeansUp)
                            cameranoiseTeans++;
                        else
                            cameranoiseTeans--;
                        if (cameranoiseTeans >= 90 || cameranoiseTeans < 10)
                        {
                            cameranoiseTeansUp = !cameranoiseTeansUp;
                            cameranoiseTeans = Math.Max(cameranoiseTeans, 10);
                            cameranoiseTeans = Math.Min(cameranoiseTeans, 89);
                        }
                        if(lightRight || lightLeft)
                            cameranoiseTeans = Math.Max(cameranoiseTeans, 150);
                    }
                }

                this.effects[1].Draw(sB, monitor.GetInner(), 8, true);

                monitor.DrawMap(sB, m, cheat, mou);
            }
        }
        #endregion
        #region DrawRoom
        public void DrawRoom(SpriteBatch sB, Rectangle fullScreen, bool isEmpty)
        {
            ((MainRoom)this.rooms[0]).PrintRoom(sB, fullScreen, lightRight, lightLeft, isEmpty, motion);

        }
        #endregion
        #region DrawNoise
        public void DrawNoise(SpriteBatch sB, Rectangle full,GameTime time)
        {
            this.effects[1].DrawTrans(sB, full, 30, true,time.ElapsedGameTime);
        }
        #endregion
        #region UpdateMonitor
        public void UpdateMonitor(MouseState mouse, bool emptyBattery)
        {
            if (!emptyBattery)
            {
                KeyboardState keysSts = Keyboard.GetState();

                if ((items[2].IsClicked(mouse) && !motion) || (!motion && (keysSts.IsKeyDown(Keys.Space))))//if click monitor button - toggle monitor button
                    ((Button)items[2]).Toggle();

                if (((Button)items[2]).IsOn() && !motion)//if monitor button on and monitor motion off
                {
                    cameraMod = !cameraMod; //toggle camera mod
                    motion = !this.monitor.IsSlide();//update motion sts
                    if (motion)//if motion true
                        ((Button)items[2]).Toggle();//toggle to false (allow click again)
                }
                if (!monitor.GetDown() && effects[1].InAnimation())
                {
                    int currectRoom = monitor.Click_IsSwitchRoom(mouse);//get room to dispaly (will not change if not clicked)
                    if (currectRoom != roomView)//if not currect room
                    {
                        this.effects[1].Reset(true, false);//display screen effect
                        roomView = currectRoom;//update room
                    }
                }
            }
            else
            {
                if (!monitor.GetDown())
                    this.Reset();
            }
            if (motion)//if motion - update monior motion pos
                motion = !this.monitor.IsSlide(20);
        }
        #endregion
        #region UpdateMainRoom
        public void UpdateMainRoom(GameTime gameTime, MouseState mouse, Rectangle fullScreen, bool freez, bool isEmptyBattery,bool isGameFocus)
        {
            blurTrans = (float)((gameTime.TotalGameTime.TotalMilliseconds % 4000) / 1900);
            if (!cameraMod)
                ((MainRoom)(this.rooms[0])).CalcView(mouse, fullScreen, gameTime, isGameFocus);
            if (!isEmptyBattery)
            {
                if (this.items[3].IsClicked(mouse) && freez && !cameraMod && ((MainRoom)rooms[0]).pos == 1)
                {
                    lightLeft = ((Button)this.items[3]).Toggle();
                    freez = false;
                }
                if (this.items[4].IsClicked(mouse) && freez && !cameraMod && ((MainRoom)rooms[0]).pos == -1)
                {
                    lightRight = ((Button)this.items[4]).Toggle();
                    freez = false;
                }
            }
            else
            {
                if (lightRight)
                {
                    lightRight = false;
                    lightLeft = ((Button)this.items[3]).Toggle();

                }
                if (lightLeft)
                {
                    lightLeft = false;
                    lightLeft = ((Button)this.items[4]).Toggle();
                }
                ((MainRoom)(this.rooms[0])).force.reset();

            }
        }
        #endregion
        #region HotkeyActions
        public void HotkeyActions(KeyboardState keysSts)
        {
            if (keysSts.IsKeyDown(Keys.Enter) && !motion)
                ((Button)items[2]).Toggle();
        }
        #endregion
        #region GetMonitorSize
        public Rectangle GetMonitorSize()
        {
            return this.monitor.GetInner();
        }
        #endregion
        #region DrawMobInRoom
        public void DrawMobInRoom(Mob[] m, SpriteBatch sB)
        {
            this.rooms[roomView].PrintMobs(sB, m, monitor.GetInner());
        }
        #endregion
        #region GetRooms
        public Room[] GetRooms()
        {
            return this.rooms;
        }
        #endregion
        #region GetGifsEffect
        public Gif[] GetGifsEffect()
        {
            return this.effects;
        }
        #endregion
        #region reset
        public void Reset()
        {
            roomView = 1;
            lightRight = false;
            lightLeft = false;
            motion = false;
            cameraMod = false;
            monitor.Reset();
            ((MainRoom)(rooms[0])).pos = 0;
            ((MainRoom)(rooms[0])).force.reset();
            if (((Button)this.items[3]).IsOn()) ((Button)this.items[3]).Toggle(true);
            if (((Button)this.items[2]).IsOn()) ((Button)this.items[2]).Toggle(true);
            if (((Button)this.items[4]).IsOn()) ((Button)this.items[4]).Toggle(true);
        }
        #endregion
    }
}
