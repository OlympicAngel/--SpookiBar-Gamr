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
    static class TimeController
    {
        public static TimeSpan BatteyUpdate { get; set; }
        public static TimeSpan MonitorUnopen { get; set; }//not update evry update!!!!        
        public static TimeSpan NightTime { get; set; }
        public static TimeSpan FastAction { get; set; }
        public static TimeSpan NightmareUpdate { get; set; }
        public static TimeSpan[] MobBaseMoveTimer { get; set; }
        public static TimeSpan RoomMovementWait { get; set; }
        public static TimeSpan RandomScreenEffect { get; set; }
        public static TimeSpan ElapsedGameTime { get; set; }

        public static void AddTimeToALL(GameTime time)
        {
            if (MobBaseMoveTimer == null)
                MobBaseMoveTimer = new TimeSpan[3];
            for (int i = 0; i < MobBaseMoveTimer.Length; i++)
                MobBaseMoveTimer[i] += time.ElapsedGameTime;
            BatteyUpdate += time.ElapsedGameTime;
            NightTime += time.ElapsedGameTime;
            FastAction += time.ElapsedGameTime;
            NightmareUpdate += time.ElapsedGameTime;
            RoomMovementWait += time.ElapsedGameTime;
            RandomScreenEffect += time.ElapsedGameTime;
            ElapsedGameTime = time.ElapsedGameTime;
        }

        public static void ResetAll()
        {
            BatteyUpdate = new TimeSpan();
            MonitorUnopen = new TimeSpan();
            NightTime = new TimeSpan();
            FastAction = new TimeSpan();
            NightmareUpdate = new TimeSpan();
            MobBaseMoveTimer = new TimeSpan[3];
            RoomMovementWait = new TimeSpan();
            RandomScreenEffect = new TimeSpan();
            ElapsedGameTime = new TimeSpan();
        }
    }
}
