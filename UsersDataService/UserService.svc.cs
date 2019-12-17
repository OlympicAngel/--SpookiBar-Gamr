using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace UsersDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class UserService : IUserService
    {
        Dbhelper helper = new Dbhelper();
        string current = "1.0";


        public UserService()
        {
        }



        string IUserService.UserData(string userID, string gameID)
        {
            if (DataLoad.GetGameIDByName(gameID) == 1)
            {
                string sql = string.Format(@"SELECT BaseUser.UserName, SBUser.Points, BaseUser.UserID, BaseUser.Money, BaseUser.RegDate, SBUser.PlayC, SBUser.Save, SBUser.Lock6, SBUser.Lock7 FROM Games INNER JOIN ((SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID) INNER JOIN HasGame ON BaseUser.UserID = HasGame.UserID) ON Games.GameID = HasGame.GameID WHERE ((BaseUser.UserID)={0}) AND ((HasGame.GameID)=1);",
                           userID);
                OleDbDataReader reader;
                reader = helper.GetData(sql);
                reader.Read();
                string temp = reader["UserID"].ToString() + "|";
                temp += reader["UserName"].ToString() + "|";
                temp += reader["PlayC"].ToString() + "|";
                temp += reader["Money"].ToString() + "|";
                temp += reader["RegDate"].ToString() + "|";
                temp += reader["Lock6"].ToString() + "|";
                temp += reader["Lock7"].ToString() + "|";
                temp += reader["Save"].ToString() + "|";
                temp += reader["Points"].ToString() + "|";
                return temp;
            }
            else return "";
        }

        string IUserService.UserLogin(string username, string password, string gameID)
        {

            //helper.ChangeDB(string.Format(@"UPDATE BaseUser SET BaseUser.[Money] = {0} Where BaseUser.UserID = 1", 1000));

            string sql = "";
            switch (gameID)
            {
                case "SpookiBar":
                    #region sql
                    sql = string.Format(@"SELECT BaseUser.UserName, SBUser.Points, BaseUser.UserID, BaseUser.Money, BaseUser.RegDate, SBUser.PlayC, SBUser.Save, SBUser.Lock6, SBUser.Lock7 FROM Games INNER JOIN ((SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID) INNER JOIN HasGame ON BaseUser.UserID = HasGame.UserID) ON Games.GameID = HasGame.GameID WHERE (((BaseUser.UserName)='{0}') AND ((BaseUser.UserPassword)='{1}') AND ((HasGame.GameID)={2}));",
                        username, password, 1);
                    #endregion
                    break;
                default:
                    break;
            }
            if (sql == "")
                return ("~Error 0: Game id is not vaild! please report.");
            #region error returns
            OleDbDataReader reader;
            try { reader = helper.GetData(sql); reader.Read(); }
            catch { return ("~Error 1: Could not get ANY data! please report to OABZ(service error)"); }

            if (!reader.HasRows)
            {
                #region sql
                sql = string.Format(@"SELECT BaseUser.UserName FROM BaseUser WHERE (((BaseUser.UserPassword)='{1}') AND ((BaseUser.UserName)='{0}'));"
                , username, password);
                #endregion
                reader = helper.GetData(sql);
                reader.Read();
                if (reader.HasRows)
                    return ("~Error 2: " + reader[0] + ", you dont own this game!");
                return ("~Error 3: Incorrect [password] OR [user name]");
            }
            #endregion
            else try
                {
                    switch (gameID)
                    {
                        case "SpookiBar":
                            #region user
                            string temp = reader["UserID"].ToString() + "|";
                            temp += reader["UserName"].ToString() + "|";
                            temp += reader["PlayC"].ToString() + "|";
                            temp += reader["Money"].ToString() + "|";
                            temp += reader["RegDate"].ToString() + "|";
                            temp += reader["Lock6"].ToString() + "|";
                            temp += reader["Lock7"].ToString() + "|";
                            temp += reader["Save"].ToString() + "|";
                            temp += reader["Points"].ToString() + "|";
                            return temp;
                            #endregion
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    return (@"~Error 4: Could not collect User data. please create ticket on OABZ.
" + e.Message);
                }
            return ("~Error 404");
        }

        List<string> IUserService.GetTopAtGameAndUserRate(string userID, string gameID)
        {
            if (DataLoad.GetGameIDByName(gameID) == 1)
            {
                string sql = string.Format(@"SELECT SBUser.Points, BaseUser.UserName, SBUser.PlayC, BaseUser.UserID
FROM SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID
ORDER BY SBUser.Points DESC;");
                OleDbDataReader reader = helper.GetData(sql);
                List<string> users = new List<string>();
                DataTable t = new DataTable();
                t.Load(reader);
                foreach (DataRow item in t.Rows)
	            {
                    users.Add(string.Format("{0},{1}|{2}#{3}", item["Points"].ToString(), item["UserName"].ToString(), item["UserID"].ToString(), item["UserID"].ToString() == userID));
                }
                return users;
            }
            throw new Exception("Error -1: unknown GameId");
        }

        Shop IUserService.GetShop(string gameID)
        {
            return new Shop(gameID);
        }

        string IUserService.BuyItemFromShop(string gameID, string userID, string item)
        {
            if (item == "2")
            {
                item = "2";
            }
            string sql = "";
            sql = string.Format(@"Select
  ShopItems.Price, ItemName
From
  ShopItems
Where
  ShopItems.ItemID = {0}", item);
            OleDbDataReader reader = helper.GetData(sql);
            if (reader.Read())
            {
                string name = reader["ItemName"].ToString();
                int price = int.Parse(reader["Price"].ToString());
                sql = string.Format(@"Select
  BaseUser.[Money]
From
  BaseUser Inner Join
  HasGame
    On HasGame.UserID = BaseUser.UserID
Where
  BaseUser.[Money] >= {0} And
  HasGame.GameID = {1} And
  BaseUser.UserID = {2}", price, DataLoad.GetGameIDByName(gameID), userID);
                reader = helper.GetData(sql);
                if (reader.Read())
                {
                    int oldM = int.Parse(reader["Money"].ToString());
                    int newM = oldM - price;
                    sql = string.Format(@"UPDATE BaseUser SET BaseUser.[Money] = {0} Where BaseUser.UserID = {1}",newM,userID);
                    if (!helper.ChangeDB(sql))
                        return ("~Error 4: Faild to modify money balance");
                    sql = string.Format(@"INSERT INTO ItemsOwn (UserID,ItemID) VALUES ({0},{1})",userID,item);
                    if (helper.ChangeDB(sql))
                    {
                        sql = string.Format("SELECT Count([ItemsOwn.ItemID]) AS ItemCount FROM ItemsOwn WHERE (((ItemsOwn.ItemID)={0}) AND ((ItemsOwn.UserID)={1}));",item,userID);
                        reader = helper.GetData(sql);
                        reader.Read();
                        return string.Format("You now owned {0} - {1}", reader[0].ToString(), name);
                    }
                    sql = string.Format(@"UPDATE BaseUser SET BaseUser.[Money] = {0} Where BaseUser.UserID = {1}", oldM, userID);
                    helper.ChangeDB(sql);
                    return ("~Error 5: Could not add the item to user.");
                }
                else
                {
                    return ("~Error 3: Faild to get User data");
                }
            }
            else
            {
                return ("~Error 2: Faild to load item! please restart and try again");
            }
        }

        bool IUserService.UseItem(string userID, string item)
        {
            string sql = string.Format(@"DELETE FROM (SELECT TOP 1 * FROM ItemsOwn WHERE ItemsOwn.UserID={0} AND ItemsOwn.ItemID={1});", userID, item);
            return helper.ChangeDB(sql);
        }

        string IUserService.InGameEnd(string userID, int night, int nightmareTime)
        {
            string sql="", baseSql = string.Format("UPDATE SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID SET BaseUser.[Money] = [BaseUser].[Money]+$, SBUser.Points = [SBUser].[Points]+@ WHERE (((BaseUser.UserID)={0}));", userID);
            sql = string.Format(@"DELETE FROM (SELECT TOP 1 * FROM ItemsOwn WHERE ItemsOwn.UserID={0} AND ItemsOwn.ItemID={1});", userID, 3);
            bool xPoint =  helper.ChangeDB(sql);
            sql = string.Format(@"DELETE FROM (SELECT TOP 1 * FROM ItemsOwn WHERE ItemsOwn.UserID={0} AND ItemsOwn.ItemID={1});", userID, 4);
            bool xMoney = helper.ChangeDB(sql);
            switch (night)
            {
                case 1:
                    sql = baseSql.Replace("$", (1 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (1 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                case 2:
                    sql = baseSql.Replace("$", (1 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (5 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                case 3:
                    sql = baseSql.Replace("$", (2 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (10 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                case 4:
                    sql = baseSql.Replace("$", (3 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (20 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                case 5:
                    sql = baseSql.Replace("$", (4 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (30 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                case 6:
                    sql = baseSql.Replace("$", (5 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (50 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                case 7:
                    sql = baseSql.Replace("$", (nightmareTime / 2 * (Convert.ToInt32(xMoney) + 1)).ToString()).Replace("@", (nightmareTime * 15 * (Convert.ToInt32(xPoint) + 1)).ToString());
                    break;
                default:
                    break;
            }
            if (sql == "")
                return "-1";
            helper.ChangeDB(sql);
            OleDbDataReader r = helper.GetData("SELECT BaseUser.Money, SBUser.Points FROM SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID WHERE (((BaseUser.UserID)=" + userID + "));");
            if (r.Read())
                return r[0].ToString() + "|" + r[1].ToString();
            else
                return "-1";
        }


        bool IUserService.ResetSave(string userID, string gameID)
        {
            if (DataLoad.GetGameIDByName(gameID) == 1)
                return helper.ChangeDB(string.Format("UPDATE SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID SET SBUser.Save = 1 WHERE (((BaseUser.UserID)={0}));",userID));
            throw new Exception("Error -1: Unknown gameID");
        }


        void IUserService.UpdateAfterGame(string userID, bool isWin, int night)
        {
            int nightUp = night;
            if (isWin)
                nightUp++;
            if (nightUp > 5)
                nightUp = 5;

            helper.ChangeDB(string.Format("UPDATE SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID SET SBUser.Save = {0}, SBUser.PlayC = [SBUser].[PlayC]+1 WHERE (((BaseUser.UserID)={1}));", nightUp, userID));
            if (isWin && night == 4 && isWin)
                helper.ChangeDB(string.Format("UPDATE SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID SET SBUser.Lock6 = True WHERE (((SBUser.Lock6)=False) AND ((BaseUser.UserID)={0}));",userID));
            if (isWin && night == 5 && isWin)
                helper.ChangeDB(string.Format("UPDATE SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID SET SBUser.Lock7 = True WHERE (((SBUser.Lock7)=False) AND ((BaseUser.UserID)={0}));", userID));
        }


        void IUserService.ResetUserData(string userID)
        {
            helper.ChangeDB(string.Format("UPDATE SBUser INNER JOIN BaseUser ON SBUser.SBUserID = BaseUser.UserID SET SBUser.Points = 0, SBUser.Save = 1, SBUser.PlayC = 0, SBUser.Lock6 = 0, SBUser.Lock7 = 0 WHERE (((BaseUser.UserID)={0}));", userID));
        }


        List<string> IUserService.ItemOwnList(string userId, string gameid)
        {
            gameid = DataLoad.GetGameIDByName(gameid).ToString();
            List<string> end = new List<string>();
            string sql = string.Format(@"SELECT ItemsOwn.ItemID, ItemsOwn.UserID
FROM (Games INNER JOIN ShopItems ON Games.GameID = ShopItems.GameID) INNER JOIN ItemsOwn ON ShopItems.ItemID = ItemsOwn.ItemID
WHERE (((ShopItems.GameID)={0}))
GROUP BY ItemsOwn.ItemID, ItemsOwn.UserID
HAVING (((ItemsOwn.UserID)={1}));", gameid, userId);
            OleDbDataReader dr = helper.GetData(sql);
            DataTable tr = new DataTable();
            tr.Load(dr);
            if(tr.Rows.Count>0)
            foreach (DataRow drow in tr.Rows)
            {
                sql = "SELECT Count(*) As ItemCount FROM ItemsOwn WHERE (((ItemsOwn.UserID)=" + userId + ") AND ((ItemsOwn.ItemID)=" + drow[0] + "));";
                OleDbDataReader dr2 = helper.GetData(sql);
                if(dr2.Read())
                    end.Add(drow[0] + "|" + dr2[0]);
            }
                
               return end;

        }

        public bool CheckUpdate(string version)
        {
            if (version == current)
                return true;
            return false;
        }
    }
}
