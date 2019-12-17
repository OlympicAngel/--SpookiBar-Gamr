using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spooki_bar
{
    class MobAi
    {
        protected string action; //#!#& example from!to&next
        protected int chance;
        protected Random randomizer;

        public MobAi()
        {
            this.chance = 5;
            this.action = "";
            this.randomizer = new Random();
        }

        /// <summary>
        /// will create object with Fill Ai data from raw string ai
        /// </summary>
        /// <param name="ai">a string that looks like: #|#!#&#!#&#!#&#!#&#!#&.... #=num, first part of | = chance to move</param>
        public MobAi(string ai)
        {
            this.chance = 5;
            this.action = "";

            this.FeedAI(ai);
            this.randomizer = new Random(chance);
        }

        /// <summary>
        /// Fell Ai data from raw string ai
        /// </summary>
        /// <param name="ai">a string that looks like: #|#!#&#!#&#!#&#!#&#!#&.... #=num, first part of | = chance to move</param>
        protected void FeedAI(string ai)
        {
            if (ai.Contains("99"))
            {
                action = "tp";
            }
            else
            {
                this.action = ai.Split('|')[1];
            }
            this.chance = int.Parse(ai.Split('|')[0]);
        }

        protected int RandomRoomFromAI(int from, string preffer, int looks, bool doeslock)
        {
            if (action != "tp")
            {
                if (from == 5 && looks >= 2 && !doeslock)
                    return -1;
                string final = "";
                string[] aIParts = action.Split('&');
                for (int i = 0; i < aIParts.Length; i++)
                {
                    string[] courrectPart = aIParts[i].Split('!');
                    if (courrectPart[0] == from.ToString())
                        final = final + courrectPart[1] + "|";
                }
                if (final == "")
                    return from;
                if (preffer != "")
                    final = final + "|" + preffer;
                string[] able = final.Split('|');
                if (doeslock)
                {
                    final = "";
                    for (int i = 0; i < able.Length; i++)
                    {
                        if (able[i] == "-1")
                            able[i] = "";
                        final = final + able[i];
                        if (able[i] != "" && i != able.Length - 1)
                            final = final + "|";
                    }
                    able = final.Split('|');

                }
                return int.Parse(able[randomizer.Next(0, able.Length - 1)]);
            }
            else
            {
                if (from == 6)
                    return randomizer.Next(8, 10);

                if (from == 9 && !doeslock)
                    if (looks == 0 && randomizer.Next(0, 10) <= 4)
                        return -1;
                int temp = 0;
                do
                {
                    temp = randomizer.Next(1, 11);
                    if (temp == 10) temp = 6;
                }
                while (temp == from);
                return temp;
            }

        }
    }
}
