using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace UsersDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {

        [OperationContract]
        bool CheckUpdate(string version);

        [OperationContract]
        string UserLogin(string username, string password,string gameID);

        [OperationContract]
        List<string> GetTopAtGameAndUserRate(string userID, string gameID);

        [OperationContract]
        Shop GetShop(string gameID);

        [OperationContract]
        string BuyItemFromShop(string gameID, string userID, string item);

        [OperationContract]
        bool ResetSave(string userID, string gameID);

        [OperationContract]
        string UserData(string userID, string gameID);

        [OperationContract]
        bool UseItem(string userID, string itemID);

        [OperationContract]
        string InGameEnd(string userID, int night,int nightmareTime);

        [OperationContract]
        void UpdateAfterGame(string userID, bool isWin, int night);

        [OperationContract]
        void ResetUserData(string userID);

        [OperationContract]
        List<string> ItemOwnList(string userId, string gameid);

    }
}
