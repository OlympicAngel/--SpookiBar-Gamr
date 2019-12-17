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
using spooki_bar.screens;
using System.Media;

namespace spooki_bar
{
    class Mob : MobAi
    {
        int room;
        int startroom;
        string name;
        int freezCount;
        int maxLooking = 3;
        bool didLook = false;
        SoundEffectPlay steps;
        public static SoundEffectPlay stress, boo;
        MobTexture[] textures;
        public TimeSpan delay { get; set; }


        #region Mob
        public Mob(string name, int room, string ai, int max)
            : base(ai)
        {
            delay = new TimeSpan();
            this.room = room;
            this.name = name;
            freezCount = 0;
            maxLooking = max;
            startroom = room;
        }
        #endregion
        #region LoadStep
        public void LoadStep(float p, SoundEffect m, float vol)
        {
            this.steps = new SoundEffectPlay(p, m, vol);
        }
        #endregion
        #region GetTextureFromRoom
        public Texture2D GetTextureFromRoom()
        {
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i].roomID == room)
                    return textures[i].texture;
            }
            return null;
        }
        #endregion
        #region GetSet
        public void SetRoom(int room)
        {
            this.room = room;
        }
        public string GetName()
        {
            return this.name;
        }
        public int GetRoom()
        {
            return this.room;
        }
        #endregion
        #region DrawAt (main room)
        public virtual void DrawAt(SpriteBatch sb, int frame)
        {
            //
        }
        #endregion
        #region Draw
        public virtual void Draw(SpriteBatch sb, Rectangle rec)
        {
            sb.Draw(GetTextureFromRoom(), rec, Color.White);
        }
        #endregion
        #region TrayMove
        public virtual bool TrayMove(int monitorRoom, bool looking, double multi,bool monitorside)
        {
            double time = delay.Seconds + (double)(delay.Milliseconds) / 1000;
            if (delay.TotalSeconds >= 10 / (multi / 2) || (monitorside && delay.TotalSeconds >= 5 / (multi / 2)))
            {
                bool move = false;
                move = (this.randomizer.Next(0, this.chance + 1) == 1);

                if (move && looking && monitorRoom == room && this.randomizer.Next(0, 100) <= 97)//if moved and same room - freez
                {
                    freezCount++;
                    move = false;
                    if (freezCount >= maxLooking && this.randomizer.Next(0, 11) <= 9)//if moved and same room - freez|but if freezed for max times - move!
                        move = true;

                    string lookingStr = freezCount + name + room;
                    switch (lookingStr)
                    {
                        case "1summer5":
                            move = true;
                            break;
                        case "3bob9":
                            move = false;
                            break;
                        case "3chucki8":
                            move = true;
                            break;
                        default:
                            break;
                    }
                    move = true;
                }

                if (move && name == "chucki" && looking && monitorRoom != room)//אם אמור לזוז והמוב הוא עכביש והמשתמש מסתכל לא על המוב
                    move = (this.randomizer.Next(0, this.chance + 1) == 1);

                if (move && name == "bob" && !looking)//אם אמור לזוז והמוב הוא בוב והשחקן לא מסתכל בכלל
                    move = (this.randomizer.Next(0, this.chance + 1) == 1);

                if (move)
                {
                    freezCount = 0;
                    delay = new TimeSpan();
                }

                return move;
            }
            return false;
        }
        #endregion
        #region RandomMoveTo
        public virtual int RandomMoveTo(bool looking, bool isSafeR, bool isSafeL)
        {
            Random rnd = new Random(7);
            int before = room;
            this.room = RandomRoomFromAI(this.room, "", freezCount, ((before == 5 && isSafeR) || (isSafeL && before != 5)));
            if (room == -1 && ((before == 5 && isSafeR) || (isSafeL && before != 5)))//if killed while safe reroll till not killed!;
                for (int i = 0; i < 2; i++)
                {
                    this.room = RandomRoomFromAI(before, "2", freezCount, ((before == 5 && isSafeR) || (isSafeL && before != 5)));
                    if (room == -1)
                        i = 1;
                    else
                        break;
                }


            if (looking && room == -1) //if killed while looking make a chance of 20% to re roll the step!
            {
                
                if (rnd.Next(0, 5) == 1)
                    this.room = RandomRoomFromAI(this.room, "2|2", freezCount, ((before == 5 && isSafeR) || (isSafeL && before != 5)));
            }



            return this.room;
        }
        #endregion
        #region PlayStepSound
        public void PlayStepSound()
        {
            float mody = 0.4f;
            if (steps != null)
                switch (room)
                {
                    case 1:
                        this.steps.PlayAt(0.2f, 0.15f * mody, true); break;//
                    case 2:
                        this.steps.PlayAt(-0.3f, 0.25f * mody, true); break;//
                    case 3:
                        this.steps.PlayAt(0.1f, 0.28f * mody, true); break;//
                    case 4:
                        this.steps.PlayAt(0.5f, 0.35f * mody, true); break;
                    case 5:
                        this.steps.PlayAt(0.9f, 0.3f * mody, true); break;//
                    case 6:
                        this.steps.PlayAt(-1.0f, 0.20f * mody, true); break;//
                    case 7:
                        this.steps.PlayAt(-0.5f, 0.33f * mody, true); break;
                    case 8:
                        this.steps.PlayAt(-0.95f, 0.43f * mody, true); break;
                    case 9:
                        this.steps.PlayAt(-1.0f, 0.45f * mody, true); break;
                    case 10:
                        this.steps.PlayAt(-0.4f, 0.4f * mody, true); break;
                    case 11:
                        this.steps.PlayAt(-0.6f, 0.24f * mody, true); break;
                    default:
                        break;
                }

        }
        #endregion
        public void SoundSacre(int lookingAt)
        {
            if ("5,9,8".Contains(lookingAt.ToString()))
            {

                if (!didLook && (room == 5 || room == 9 || room == 8) && lookingAt == room)
                {
                    didLook = true;
                    float pitch = (float)((double)randomizer.Next(5, 20) / (double)20);
                    int rsndomscar = this.randomizer.Next(1, 3);
                    if (rsndomscar == 1)
                    {
                        stress.Play(0.0f, true);
                        stress.soundPitch = pitch;
                    }
                    else if (rsndomscar == 2)
                    {
                        boo.Play(0.0f, true);
                        boo.soundPitch = pitch;
                    }
                    else didLook = false;

                }
            }
            else if (freezCount >= 2)
                didLook = false;

        }
        public void LoadTextures(ContentManager content, string rooms)
        {
            string[] each = rooms.Split(',');
            this.textures = new MobTexture[each.Length];
            for (int i = 0; i < each.Length; i++)
            {
                this.textures[i] = new MobTexture(int.Parse(each[i]), content.Load<Texture2D>("./room skins/" + this.name + "/room " + each[i]));
            }

            if (stress == null)
            {
                stress = new SoundEffectPlay(0.0f, content.Load<SoundEffect>("sound/creep"), 0.5f);
                boo = new SoundEffectPlay(0.0f, content.Load<SoundEffect>("sound/sacre"), 0.7f);
            }
        }
        public void Reset()
        {
            didLook = false;
            delay = new TimeSpan();
            freezCount = 0;
            room = startroom;
        }
    }
}
