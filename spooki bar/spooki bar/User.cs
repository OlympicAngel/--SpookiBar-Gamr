using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spooki_bar
{
    class User
    {
        public string id { get; set; }//user id for updating
        public string name { get; set; }//name
        public int playCount { get; set; }//night played
        public int money { get; set; }
        public int point { get; set; }//points - money
        public string regiDate { get; set; }//registertion date
        public bool lock6 { get; set; }//is locked night 6
        public bool lock7 { get; set; }//is locked nightmare mode
        public int save { get; set; }//last night saved

        public User(string rawInfo)
        {
            string[] parts = rawInfo.Split('|');
                this.id = parts[0];
                this.name = parts[1];
                this.playCount = int.Parse(parts[2]);
                this.money = int.Parse(parts[3]);
                this.regiDate = parts[4];
                this.lock6 = parts[5] == "true" || parts[5] == "True";
                this.lock7 = parts[6] == "true" || parts[6] == "True";
                this.save = int.Parse(parts[7]);
                this.point = int.Parse(parts[8]);
        }
    }
}
