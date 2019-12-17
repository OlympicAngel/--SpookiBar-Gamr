using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using spooki_bar.ServiceReference1;
using System.Threading;

namespace spooki_bar
{
    class DataLoad
    {
        public static string version = "1.0";
        public static Dbhelper helper = new Dbhelper();
        public static UserServiceClient service = new UserServiceClient();
        

        public static bool CheckUpdate()
        {
            return service.CheckUpdate(version);
        }


        public static string BuyShopItem(string itemID, User user)
        {
            return service.BuyItemFromShop("SpookiBar", user.id, itemID);
        }

        public static ServiceReference1.Shop GetShopItems()
        {
           return service.GetShop("SpookiBar");
        }

        public static string LoadUserData(string name, string password)
        {
            return service.UserLogin(name, password, "SpookiBar");
        }

        public static string GetMobAI(int id)
        {
            //#|#!#&#!#&#!#&#!#&#!#&.... #=num
            string sql = string.Format(@"SELECT Mob.startRoom, Mob.mobMoveChance, Ai.fromRoom, Ai.toRoom, Ai.multi
FROM Mob INNER JOIN Ai ON Mob.MobID = Ai.mobID
WHERE (((Mob.MobID)={0}));",id);
            OleDbDataReader dr = helper.GetData(sql);
            dr.Read();
            string ai = "";
            if (dr.HasRows)
            {
                ai = dr[0] + "*" + dr[1] + "|";
                int i = 0;
                do
                {
                    int c = int.Parse(dr["multi"].ToString());
                    if (c == 99)
                        return ai + "99";
                    for (int z = 0; z < c; z++)
                    {
                        ai = ai + dr["fromRoom"] + "!" + dr["toRoom"];
                        if (z != c)
                            ai = ai + "&";
                    }
                    i++;
                }
                while (dr.Read());
                helper.CloseConnection();
                return ai;
            }
            else
                helper.CloseConnection();
                return "";
        }

        public static double[] GetNightMultiplay()
        {
            string sql = string.Format(@"SELECT Settings.settingID, Settings.settingValue, Settings.settingNeeds FROM Settings WHERE (((Settings.settingRole)='multiplay')) ORDER BY Settings.settingID;");
            helper.CloseConnection();
            OleDbDataReader dr = helper.GetData(sql);

            double[] temp = new double[8];
            for (int i = 0; i < 8; i++)
            {
                dr.Read();
                temp[i] = double.Parse(dr[1].ToString());
            }
            helper.CloseConnection();
            return temp;
        }

        public static string GetMobsRoom2TextureConfig(int id)
        {
            string sql = string.Format(@"SELECT MobTexture.roomsToLoad, MobTexture.skinOwnedBy FROM MobTexture WHERE (((MobTexture.skinOwnedBy)={0}));",id);
            helper.CloseConnection();
            OleDbDataReader dr = helper.GetData(sql);
            string output = "";
            if (dr.HasRows)
            {
                dr.Read();
                output = dr[0].ToString();
            }
            helper.CloseConnection();
            return output;
        }

        public static void UpdateUser(User user)
        {
            User u = new User(service.UserData(user.id, "SpookiBar"));
            user.money = u.money;
            user.save = u.save;
            user.lock6 = u.lock6;
            user.lock7 = u.lock7;
            user.playCount = u.playCount;
        }

        public static bool DoesUSerHaveItem(User user,int itemID)
        {
            return service.UseItem(user.id,itemID.ToString());
        }

        public static void AddPoints(User user,int night, int nightmareTime)
        {
            string str = service.InGameEnd(user.id, night, nightmareTime);
            user.money = int.Parse(str.Split('|')[0]);
            user.point = int.Parse(str.Split('|')[1]);
        }

        public static void ResetSaveNight(User user)
        {
            service.ResetSave(user.id, "SpookiBar");
        }

        public static void UpdateAfterGame(User user, int night,bool isWin,bool isNightmare)
        {
            if (isNightmare)
                night = user.save;
            if (isWin && isNightmare)
                night--;
            service.UpdateAfterGame(user.id, isWin, night);
        }

        public static void ResetUserData(User user)
        {
            service.ResetUserData(user.id);
        }

        public static string[] GetItemOwnList(User u)
        {
            return service.ItemOwnList(u.id, "SpookiBar");
        }

        public static string[] GetTop(User u )
        {
            return service.GetTopAtGameAndUserRate(u.id, "SpookiBar");
        }
    }
}
