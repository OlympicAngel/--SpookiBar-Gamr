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
        public static Dbhelper helper = new Dbhelper();

        public static string LoadUserData(string name, string password)
        {
            UserServiceClient service = new UserServiceClient();
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
            string sql = string.Format(@"SELECT Settings.settingValue, Settings.settingNeeds FROM Settings WHERE (((Settings.settingRole)='multiplay'));");
            helper.CloseConnection();
            OleDbDataReader dr = helper.GetData(sql);

            double[] temp = new double[6];
            for (int i = 0; i < 6; i++)
            {
                dr.Read();
                temp[i] = double.Parse(dr[0].ToString());
            }
            helper.CloseConnection();
            return temp;
        }
    }
}
