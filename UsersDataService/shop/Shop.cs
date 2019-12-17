using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace UsersDataService
{
    [CollectionDataContract]
    public class Shop:List<ShopItem>
    {

        public Shop()
        {
        }

        public Shop(string gameid)
        {
            List<ShopItem> t = DataLoad.GetShopItemsList(gameid);
            foreach (ShopItem item in t)
            {
                Add(item);
            }
        }
    }
}