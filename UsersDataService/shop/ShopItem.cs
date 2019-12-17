using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace UsersDataService
{
    [DataContract]
    public class ShopItem
    {
        [DataMember]
        string itemID { get; set; }
        [DataMember]
        string itemName { get; set; }
        [DataMember]
        int price { get; set; }
        [DataMember]
        string loaclPatch { get; set; }

        public ShopItem(string id,string itemName, int price, string loaclPatch)
        {
            this.itemName = itemName;
            this.price = price;
            this.loaclPatch = loaclPatch;
            this.itemID = id;
        }
    }
}