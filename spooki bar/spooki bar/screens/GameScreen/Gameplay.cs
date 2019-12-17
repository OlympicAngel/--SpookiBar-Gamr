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
    class Gameplay
    {
        #region מתשנים
        SpriteFont font;
        int night;
        Battery battery;
        Mob[] mobs;
        int endValu;//0:not finished||1:night at 6||2:got killed
        int killer; //if endValu ==> kkiller id;
        SoundEffectPlay bgSound;
        SoundEffectPlay endSound;
        SoundEffectPlay winSound, looseSound;
        bool usedRL = false, usedLL = false, nightmare = false;
        screens.GameScreen g;
        double multiplay = 1;
        double[] nightMultiplay;
        VideoPlayer[] player;
        Video[] killSene;
        Texture2D videoTexture, endScreen, winScreen;
        TimeSpan endtimer;
        bool cheat = false;
        Random rnd = new Random(42);

        #endregion

        #region Gameplay
        public Gameplay(Mob[] mobs, SpriteFont ingametext, SoundEffect sound, Texture2D outer, Texture2D inner, screens.GameScreen g, SoundEffect endSound, ContentManager contetn)
        {
            TimeController.BatteyUpdate = new TimeSpan();
            TimeController.NightTime = new TimeSpan();
            this.battery = new Battery(outer, inner, contetn.Load<SoundEffect>("sound/powerdown"));
            this.mobs = mobs;
            this.night = 6;
            this.font = ingametext;
            this.g = g;
            nightMultiplay = DataLoad.GetNightMultiplay();

            this.bgSound = new SoundEffectPlay(1f, sound, 0.2f);
            this.endSound = new SoundEffectPlay(0.0f, endSound, 1.0f);

            player = new VideoPlayer[mobs.Length];
            killSene = new Video[mobs.Length];
            killSene[0] = contetn.Load<Video>("kills/summerkill");
            killSene[1] = contetn.Load<Video>("kills/bobkill");
            killSene[2] = contetn.Load<Video>("kills/spooderkill");

            for (int i = 0; i < 3; i++)
            {
                player[i] = new VideoPlayer();
                player[i].Play(killSene[i]);
                player[i].Pause();
            }

            TimeController.MonitorUnopen = new TimeSpan();

            endScreen = contetn.Load<Texture2D>("images/endscreen");
            winScreen = contetn.Load<Texture2D>("images/win");
            winSound = new SoundEffectPlay(1.0f, contetn.Load<SoundEffect>("sound/win"), 1.0f);
            looseSound = new SoundEffectPlay(1.0f, contetn.Load<SoundEffect>("sound/loose"), 1.0f);
        }
        #endregion
        #region Reset
        public void Reset()
        {
            TimeController.ResetAll();

            cheat = false;
            this.night = 1;

            this.battery.Reset();
            for (int i = 0; i < mobs.Length; i++)
                mobs[i].Reset();
            this.endValu = 0;
            this.killer = 0;
            nightmare = false;
            usedRL = false;
            usedLL = false;
            this.g.Reset();
            this.multiplay = 1;
            killer = 0;
            endValu = 0;
            player = new VideoPlayer[3];
            for (int i = 0; i < 3; i++)
            {
                player[i] = new VideoPlayer();
            }
            videoTexture = null;
            endtimer = new TimeSpan(1, 0, 0, 0, 0);
        }
        #endregion
        #region PlayNight
        public void PlayNight(int nighto, ContentManager c, bool cheatSts, bool BaterryFix)
        {
            this.Reset();
            this.Reload(c);
            if (BaterryFix)
                battery.battey = (int)(battery.battey * 1.25);
            this.night = nighto;
            if (nighto == 7)
            {
                night = 3;
                nightmare = true;
            }
            this.multiplay = nightMultiplay[this.night - 1];
            cheat = cheatSts;
        }
        #endregion
        #region MobTryStep
        public void MobTryStep(Room[] rooms, int mionitorRoom, bool looking, GameTime t)
        {
            bool preform = NightCalcAndTime(t),
                timeCanMove;
            bool fastActionTimerCheck = (TimeController.FastAction.TotalMilliseconds) >= (int)(660 / multiplay);
            if (fastActionTimerCheck)
                TimeController.FastAction = new TimeSpan();

            for (int i = 0; i < mobs.Length; i++)
            {
                bool checkForFastAction = ((this.g.lightLeft && "8,9".Contains(mobs[i].GetRoom().ToString())) || (this.g.lightRight && mobs[i].GetRoom() == 9 && i == 0)) && fastActionTimerCheck;

                timeCanMove = CanMoveTMobs(preform, this.multiplay,i);

                if (timeCanMove || checkForFastAction)
                    if (mobs[i].TrayMove(mionitorRoom, looking, multiplay, checkForFastAction))
                    {
                        TimeController.MobBaseMoveTimer[i] = new TimeSpan();

                        int old = mobs[i].GetRoom();
                        int next = mobs[i].RandomMoveTo(g.monitor.GetCorrectRoom() == old, g.lightRight, g.lightLeft);
                        if (next != -1)
                            rooms[next].PlaySoundMove();

                        mobs[i].PlayStepSound();
                        if (old == mionitorRoom)
                            g.GetGifsEffect()[1].Reset(true, false);
                        if (next == mionitorRoom)
                            g.GetGifsEffect()[1].Reset(true, false);
                    }
            }
        }
        #endregion
        #region CheckForEnd
        public void CheckForEnd()
        {
            if (!nightmare && TimeController.NightTime.Minutes == 6)
            {
                endValu = 1;
                return;
            }
            for (int i = 0; i < mobs.Length; i++)
            {
                if (mobs[i].GetRoom() == -1)
                {
                    endValu = 2;
                    killer = i;
                }
            }
        }
        #endregion
        #region End
        public bool End()
        {
            bool check = endValu == 1 || endValu == 2;
            if (check)
            {
                bgSound.Off();
                g.lightLeft = false;
                g.lightRight = false;
                ((MainRoom)g.rooms[0]).force.baser.Off();
            }

            if (endValu == 2)
            {
                endSound.Play(0.0f, true);
                ((MainRoom)g.rooms[0]).pos = 0;
                g.effects[1].Reset(true, false);
                endtimer = new TimeSpan(1, 0, 0, 0, 0);
            }

            if (endValu == 1)
            {
                winSound.Play(0.0f, true);
            }

            if (check)
                return true;
            return false;
        }
        #endregion
        #region GetEndValu
        public int GetEndValu()
        {
            return endValu;
        }
        #endregion
        #region DrawInfo
        public void DrawInfo(SpriteBatch sb, Rectangle fullScreen)
        {
            string loadText = "0" + TimeController.NightTime.Minutes + ":00";
            Vector2 FontOrigin = font.MeasureString(loadText);
            sb.DrawString(this.font, "0" + TimeController.NightTime.Minutes + ":00", new Vector2(fullScreen.Width - (int)(FontOrigin.X * 1.1), fullScreen.Height / 90), Color.White * 0.7f);
            if (!nightmare)
                sb.DrawString(this.font, "Night-" + night, new Vector2(fullScreen.Height / 100, fullScreen.Height / 90), Color.White * 0.7f);
            else
                sb.DrawString(this.font, "Nightmare", new Vector2(fullScreen.Height / 100, fullScreen.Height / 90), Color.White * 0.7f);
            battery.Draw(sb, TimeController.NightTime);
        }
        #endregion
        #region Update
        /// <summary>
        /// base command for logic game(not menu/buttons/moitor etc)
        /// </summary>
        /// <param name="time">game time of the game</param>
        /// <param name="g">screens.GameScreen object</param>
        /// <returns>true if game over / false if not</returns>
        public bool Update(GameTime time, MouseState mouse, Rectangle fullScreen, bool freez, User user, bool isGameFocus)
        {
            if (g != null)
            {
                TimeController.AddTimeToALL(time);
                if (endValu != 0)
                    this.endtimer = this.endtimer.Add(time.ElapsedGameTime);

                if (endValu == 0 || (endtimer.Days == 1 && endtimer.Milliseconds < 100 && endtimer.Seconds == 0))
                {
                    bgSound.PlayLoop(0.0f);
                    UpdateBattery(time);
                    UpdateForView(time, mouse, fullScreen, freez, isGameFocus);
                    if (endValu == 0)
                    {
                        foreach (Mob item in mobs)
                        {
                            item.delay = item.delay.Add(time.ElapsedGameTime);
                            if (!g.monitor.GetDown())
                                item.SoundSacre(g.roomView);
                        }

                        MobTryStep(g.GetRooms(), g.roomView, !g.monitor.GetDown(), time);

                        CheckForEnd();


                        End();

                    }

                    return false;
                }
                else
                {
                    if (endValu == 2)
                    {
                        if (endtimer.Seconds <= 2)
                        {
                            if (player[killer].Video == null)
                                player[killer].Play(killSene[killer]);
                            if (player[killer].State != MediaState.Stopped)
                            {
                                player[killer].Play(killSene[killer]);
                                return false;
                            }
                        }
                        if (endSound.IsPlaying())
                            return false;
                        if (endtimer.Seconds < 5)
                            return false;
                    }
                    else if (endValu == 1 && endtimer.Seconds <= 2)
                    {
                        return false;
                    }
                    if (winSound.IsPlaying())
                        return false;
                    looseSound.Play(0.0f, true);

                    this.Despose();
                    if (endValu == 1)
                        DataLoad.AddPoints(user, night, time.TotalGameTime.Minutes);
                    DataLoad.UpdateAfterGame(user, night, endValu == 1, nightmare || (!nightmare && night == 6));
                }

            }
            return true;
        }
        #endregion
        #region UpdateForView
        public void UpdateForView(GameTime time, MouseState mouse, Rectangle fullScreen, bool freez, bool isGameFocus)
        {
            this.g.UpdateMainRoom(time, mouse, fullScreen, freez, this.battery.isEmpty(), isGameFocus);
            this.g.UpdateMonitor(mouse, this.battery.isEmpty());
        }
        #endregion
        #region UpdateBattery
        public void UpdateBattery(GameTime timercalc)
        {
            usedRL = usedRL || g.lightRight;
            usedLL = usedLL || g.lightLeft;

            if ((TimeController.BatteyUpdate.TotalMilliseconds >= 500 && nightmare == false) || (TimeController.BatteyUpdate.TotalMilliseconds >= 1000 && nightmare == true))
            {
                this.battery.CalcBattery(g.cameraMod, g.lightLeft || usedLL, g.lightRight || usedRL);
                usedRL = false;
                usedLL = false;
                TimeController.BatteyUpdate = new TimeSpan();
            }
        }
        #endregion
        #region NightCalcAndTime
        public bool NightCalcAndTime(GameTime time)
        {
            if (!g.cameraMod)
                TimeController.MonitorUnopen += time.ElapsedGameTime;
            else
                TimeController.MonitorUnopen = new TimeSpan();

            multiplay = nightMultiplay[this.night - 1];

            if (nightmare && TimeController.NightmareUpdate.Minutes >= 2 && night < nightMultiplay.Length)
            {
                TimeController.NightmareUpdate = new TimeSpan();
                night++;
                this.multiplay = nightMultiplay[this.night - 1];
            }

            bool preform = true;
            if (night == 1 && TimeController.NightTime.TotalMinutes <= 2)
                preform = false;
            else
                if (night == 2 && TimeController.NightTime.TotalMinutes <= 1)
                    preform = false;

            if ((night >= 3 && night <= 4 && TimeController.NightTime.TotalSeconds <= 20) || (night > 4 && TimeController.NightTime.TotalSeconds <= 5))
                preform = false;

            if (!preform && TimeController.MonitorUnopen.TotalSeconds > 10 / multiplay)
            {
                preform = true;
                if (TimeController.MonitorUnopen.TotalSeconds < 8 / multiplay + 0.1)
                    endSound.PlayAt(0.0f, 0.0003f, true);
            }

            return preform;
        }
        #endregion
        #region CanMoveTMobs
        public bool CanMoveTMobs(bool preform, double multiplay,int mobid)
        {
            bool canMoveViaTime = (TimeController.MobBaseMoveTimer[mobid].TotalMilliseconds) >= (int)((1500 + (2-mobid)*200) / multiplay) || (battery.battey <= 0 && TimeController.MobBaseMoveTimer[mobid].TotalMilliseconds >= 100);
            if (canMoveViaTime && rnd.Next(1, 100) != 52)
                TimeController.MobBaseMoveTimer[mobid] = new TimeSpan();
            return preform && canMoveViaTime;
        }
        #endregion
        #region Draw
        public void Draw(SpriteBatch sb, MouseState mouse, Rectangle fullScreen, GameTime time)
        {
            if (endtimer.Seconds >= 5)
                return;
            if (g != null)
            {
                if (endValu == 0 || (endtimer.Milliseconds < 100 && endtimer.Seconds == 0))
                {
                    g.DrawAll(sb, fullScreen, mouse, mobs, time.TotalGameTime.Milliseconds >= 400 && time.TotalGameTime.Seconds % 2 == 0 && cheat, this.battery.isEmpty(), time);
                    DrawInfo(sb, fullScreen);
                }
                else if (player[killer].State != MediaState.Stopped && endValu == 2)
                {
                    videoTexture = player[killer].GetTexture();
                    if (videoTexture != null)
                        sb.Draw(videoTexture, fullScreen, Color.White);
                    DrawInfo(sb, fullScreen);
                }
                else if (endValu == 2 && !endScreen.IsDisposed)
                {
                    if (endtimer.Seconds < 4)
                        sb.Draw(endScreen, fullScreen, Color.White * 0.8f);
                    else if (endtimer.Seconds < 5)
                        sb.Draw(endScreen, fullScreen, Color.White * ((1000 - endtimer.Milliseconds) / 1000f) * 0.8f);
                    g.DrawNoise(sb, fullScreen, time);
                }
                else if (endValu == 1 && !winScreen.IsDisposed)
                {
                    if (endtimer.Seconds < 4)
                        sb.Draw(winScreen, fullScreen, Color.White * 0.8f);
                    else if (endtimer.Seconds < 5)
                        sb.Draw(winScreen, fullScreen, Color.White * ((1000 - endtimer.Milliseconds) / 1000f) * 0.8f);
                    g.DrawNoise(sb, fullScreen, time);
                    sb.DrawString(font, "For Now mohahahahaha", new Vector2(fullScreen.Width / 2 - font.MeasureString("For Now mohahahahaha").X / 2, fullScreen.Height / 3 * 2), Color.Red * ((1000 - endtimer.Milliseconds) / 1000f) * 0.8f);
                }
            }
        }
        #endregion

        public void Despose()
        {
            foreach (Room item in g.monitor.rooms)
                item.Dispose();
            if (videoTexture != null)
                videoTexture.Dispose();
            videoTexture = null;
            for (int i = 0; i < killSene.Length; i++)
                killSene[i] = null;
            foreach (VideoPlayer item in player)
            {
                if (item.IsDisposed)
                    item.Dispose();
            }


            killSene = null;
            if (endScreen != null)
                endScreen.Dispose();
            endScreen = null;
            if (winScreen != null)
                winScreen.Dispose();
        }

        public void Reload(ContentManager contetn)
        {

            foreach (Room item in g.monitor.rooms)
                item.Reload(contetn);
            endScreen = contetn.Load<Texture2D>("images/endscreen");
            winScreen = contetn.Load<Texture2D>("images/win");

            killSene = new Video[mobs.Length];
            killSene[0] = contetn.Load<Video>("kills/summerkill");
            killSene[1] = contetn.Load<Video>("kills/bobkill");
            killSene[2] = contetn.Load<Video>("kills/spooderkill");

            player = new VideoPlayer[3];
            for (int i = 0; i < 3; i++)
            {
                player[i] = new VideoPlayer();
                player[i].Play(killSene[i]);
                player[i].Pause();
            }
        }
    }
}
