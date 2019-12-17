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
    class Transition
    {
        public string mode { get; set; }

        TimeSpan time { get; set; }
        public double totalTime_in { get; set; }
        public double totalTime_out { get; set; }

        public double delayTime_in { get; set; }
        public double delayTime_out { get; set; }

        public double add_X { get; set; }
        public double add_Y { get; set; }

        public int total_X { get; set; }
        public int total_Y { get; set; }

        int alpha { get; set; }

        bool go_In { get; set; }
        bool go_Out { get; set; }

        public Transition(string mod, double startTime, double endTime)
        {
            this.mode = mod;
            this.totalTime_in = startTime;
            this.totalTime_out = endTime;
            this.time = new TimeSpan();
            this.delayTime_in = 0.0;
            this.delayTime_out = 0.0;
            this.add_X = 0;
            this.add_Y = 0;
            this.total_X = 0;
            this.total_Y = 0;
            this.alpha = 0;
            this.go_In = true;
            this.go_Out = false;
        }

        public string Update(GameTime timer)
        {
            this.time = this.time.Add(timer.ElapsedGameTime);
            go_In = go_Out = false;

            if (ConvertTime() >= delayTime_in && ConvertTime() <= delayTime_in + totalTime_in)
                In(timer);
            else
                if (ConvertTime() > delayTime_in + totalTime_in + delayTime_out && ConvertTime() <= delayTime_in + totalTime_in + delayTime_out + totalTime_out)
                {//Out(timer);
                }

            return Return(timer);
        }

        public void In(GameTime timer)
        {
            go_In = true;
            if (mode == "motion")
            {
                double addXOnce = (total_X / (totalTime_in / timer.ElapsedGameTime.TotalSeconds));
                add_X = add_X + addXOnce;

                double addYOnce = (total_Y / (totalTime_in / timer.ElapsedGameTime.TotalSeconds));
                add_Y = add_Y + addYOnce;
            }
        }

        public void Out(GameTime timer)
        {
            go_Out = true;
            if (mode == "motion")
            {
                double removeXOnce = (total_X / (totalTime_out / timer.ElapsedGameTime.TotalSeconds));
                add_X = add_X - removeXOnce;

                double removeYOnce = (total_Y / (totalTime_out / timer.ElapsedGameTime.TotalSeconds));
                add_Y = add_Y - removeYOnce;
            }
        }

        public string Return(GameTime timer)
        {
            if (ConvertTime() >= delayTime_in && ConvertTime() <= delayTime_in + totalTime_in)
                return "in";
            if (go_Out)
                return "out";
            if (ConvertTime() >= totalTime_out + delayTime_in + delayTime_out + totalTime_in)
                return "end";
            if (ConvertTime() <= delayTime_in)
                return "start";
            return "hold";
        }

        public double ConvertTime()
        {
            double tim = 0.0;
            tim = time.TotalSeconds;
            return tim;
        }

    }
}
