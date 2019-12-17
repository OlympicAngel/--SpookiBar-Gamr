using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using UsersDataService;
using System.Text.RegularExpressions;
using System.Web.SessionState;

public class DataLoad
{
    public static Dbhelper helper = new Dbhelper();

    public static int GetGameIDByName(string name)
    {
        int tryer = 0;
        if (int.TryParse(name, out tryer))
            return tryer;
        string sql = string.Format(@"SELECT Games.* FROM Games WHERE (((Games.GameName)='{0}'));", name);
        OleDbDataReader dr = helper.GetData(sql);
        dr.Read();
        int ret = int.Parse(dr["GameID"].ToString());
        helper.CloseConnection();
        return ret;
    }

    public static List<ShopItem> GetShopItemsList(int gameid)
    {
        List<ShopItem> l = new List<ShopItem>();
        ShopItem s;
        string sql = string.Format(@"SELECT ShopItems.* FROM ShopItems WHERE (((ShopItems.GameID)={0}))  ORDER BY ShopItems.ItemID;", gameid);
        OleDbDataReader dr = helper.GetData(sql);
        while (dr.Read())
        {
            s = new ShopItem(dr["ItemID"].ToString(), dr["ItemName"].ToString(), int.Parse(dr["Price"].ToString()), dr["LoaclPatch"].ToString());
            l.Add(s);
        }
        helper.CloseConnection();
        return l;
    }

    public static List<ShopItem> GetShopItemsList(string gameid)
    {
        int game = DataLoad.GetGameIDByName(gameid);
        return DataLoad.GetShopItemsList(game);
    }


    public static string Login(string name, string password)
    {

        if (HttpContext.Current.Session["id"] != null && HttpContext.Current.Session["id"] != "")
            return "Allready logged in.";

        if (name.Length < 3)
            return "User name MUST be at least 3 letters length. and please dont messup the JS in the page.";
        if (password.Length < 4)
            return "Password MUST be at least 4 letters length. and please dont messup the JS in the page.";
        Regex r = new Regex("^[a-zA-Z0-9]*$");
        if (!r.IsMatch(name) || !r.IsMatch(password)) return "Please use EN letters (a-to-z, A-to-Z) AND numbers ONLY in your user name and password.";


        string sql = string.Format(@"SELECT BaseUser.*, BaseUser.UserName, BaseUser.UserPassword
FROM BaseUser
WHERE (((BaseUser.UserName)='{0}') AND ((BaseUser.UserPassword)='{1}'));", name, password);

        OleDbDataReader dr = helper.GetData(sql);
        if (dr.Read())
        {
            HttpContext.Current.Session["id"] = dr["UserID"].ToString();
            return "Logged in.";
        }

        return "Unknown error :/";
    }

    public static string Register(string name, string password)
    {
        if (HttpContext.Current.Session["id"] != null && HttpContext.Current.Session["id"] != "")
            return "Allready logged in.";

        if (HttpContext.Current.Session["regBlock"] != null || HttpContext.Current.Session["regBlock"].ToString() == "")
            return "It seems that you have allready created a user, please do not spam the servers.";

        if (name.Length < 3)
            return "Your user name MUST be at least 3 letters length. and please dont messup the JS in the page.";
        if (password.Length < 4)
            return "Your password MUST be at least 4 letters length. and please dont messup the JS in the page.";

        Regex r = new Regex("^[a-zA-Z0-9]*$");
        if (!r.IsMatch(name) || !r.IsMatch(password)) return "Please use EN letters (a-to-z, A-to-Z) AND numbers ONLY in your user name and password.";

        string sql;
        DateTime dt = DateTime.Now;
        sql = string.Format(@"SELECT BaseUser.*, BaseUser.UserName
FROM BaseUser
WHERE (((BaseUser.UserName)='{0}'));", name);
        OleDbDataReader dr = helper.GetData(sql);
        if (!dr.Read())
            return "User name taken! choose other one please.";

        sql = string.Format(@"INSERT INTO BaseUser (UserPassword,UserName,[Money],RegDate) 
                                                    VALUES ('{0}','{1}', 0, '{2}')", name, password, dt.ToString("MM/dd/yyyy"));
        if (helper.ChangeDB(sql))
        {
            HttpContext.Current.Session["regBlock"] = "1";
            return "You can now login to your new user.";
        }


        return "Error, it seems that the user name is not taken. But we couldn't add you :/";
    }
}