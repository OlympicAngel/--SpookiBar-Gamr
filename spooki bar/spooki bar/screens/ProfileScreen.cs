using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using spooki_bar.ServiceReference1;

namespace spooki_bar.screens
{
    class ProfileScreen:Screen
    {
        List<TextItem> items = new List<TextItem>();
        List<Texture2D> itemsTex = new List<Texture2D>();
        Texture2D bgBG;
        int interval;
        bool isToRight;
        Rectangle playAt, takeFrom;
        string[] itemsOwn; 

        public ProfileScreen( SpriteFont menu, User user, ContentManager contetn,Rectangle full)
            : base(contetn.Load<Texture2D>("images/progile"))
        {
            this.controls.Add(new TextItem(menu, "<-Back", new Vector2((int)(full.Width * 0.01), 0)));

            Vector2 baseV = new Vector2((int)(full.Width * 0.08),(int)(full.Height * 0.3));

            this.controls.Add(new TextItem(menu, "User ID: " + user.id, baseV));
            baseV.Y = baseV.Y + (int)(full.Height * 0.11);
            this.controls.Add(new TextItem(menu, "Register At: " + user.regiDate, baseV));
            baseV.Y = baseV.Y + (int)(full.Height * 0.11);
            this.controls.Add(new TextItem(menu, "OA Money: #.0$", baseV));
            baseV.Y = baseV.Y + (int)(full.Height * 0.11);
            this.controls.Add(new TextItem(menu, "You played: # nights!", baseV));
            baseV.Y = baseV.Y + (int)(full.Height * 0.11);
            this.controls.Add(new TextItem(menu, "Last Save: Night #", baseV));
            baseV.Y = baseV.Y + (int)(full.Height * 0.11);
            this.controls.Add(new TextItem(menu, "InGame Points: #", baseV));

            this.controls.Add(new TextItem(menu, user.name, new Vector2((int)(full.Width * 0.140625), (int)(full.Height * 0.1130555555555556))));

            
            interval = 0;
            isToRight = true;
        }

        public void Print(SpriteBatch sb, MouseState m, User user,Rectangle full,GameTime time)
        {
            playAt = new Rectangle(0, (int)(full.Height * 0.0486111111111111), full.Width, (int)(full.Height * 0.555));
            takeFrom = new Rectangle(interval, 0, (int)(bgBG.Width * 0.6), bgBG.Height);

            if (interval + takeFrom.Width > bgBG.Width)
                isToRight = false;
            if (interval <= 0)
                isToRight = true;

            if (time.TotalGameTime.Milliseconds % 50 == 0)
            if (isToRight)
                interval++;
            else
                interval--;
       
            sb.Draw(bgBG, playAt, takeFrom, Color.White);

            sb.Draw(this.screenBG, full, Color.White);
            int crazy = 2;
            string[] str = new string[4];
            str[0] = user.playCount.ToString();
            str[1] = user.money.ToString();
            str[2] = user.save.ToString();
            str[3] = user.point.ToString();

            int itemPos = 0;

            foreach (Item item in this.controls)
            {

                if (itemPos != 3 && itemPos != 4 && itemPos != 5 && itemPos != 6 && item is TextItem)
                {
                    ((TextItem)item).DrawPhantom(sb, crazy);
                    ((TextItem)item).Draw(sb, 1);
                }
                itemPos++;
            }

            ((TextItem)this.controls[3]).DrawPhantom(sb, crazy+2);
            ((TextItem)this.controls[3]).Draw(sb, str[1]);

            ((TextItem)this.controls[4]).DrawPhantom(sb, crazy);
            ((TextItem)this.controls[4]).Draw(sb, str[0]);

            ((TextItem)this.controls[5]).DrawPhantom(sb, crazy);
            ((TextItem)this.controls[5]).Draw(sb, str[2]);

            ((TextItem)this.controls[6]).DrawPhantom(sb, crazy);
            ((TextItem)this.controls[6]).Draw(sb, str[3]);

            ((TextItem)this.controls[0]).DrawHover(sb, m);

            for (int i = 0; i < items.Count; i++)
            {
                sb.Draw(itemsTex[i], new Rectangle((int)(items[i].vec.X*0.8),(int)(items[i].vec.Y),(int)(full.Width*0.14),(int)(full.Width*0.08)), Color.White);
                items[i].Draw(sb);
            }
        }

        public int Update(MouseState m)
        {
            if (((TextItem)this.controls[0]).IsClicked(m, true))
                return 2;
            return 5;
        }

        public void Load(ContentManager contetn,User user,ShopScreen shop,Rectangle full)
        {
            
            screenBG = contetn.Load<Texture2D>("images/progile");
            bgBG = contetn.Load<Texture2D>("images/progileScrool");
            itemsOwn = DataLoad.GetItemOwnList(user);

            ServiceReference1.Shop itemlist = shop.shop;

            Vector2 vec = new Vector2((int)(full.Width * 0.8), (int)(full.Height * 0.15));

            items.Clear();

            foreach (string itemO in itemsOwn)
            {
                foreach (ServiceReference1.ShopItem item in itemlist)
                {
                    if (itemO.Split('|')[0] == item.itemID)
                    {
                        items.Add(new TextItem(((TextItem)this.controls[0]).font, item.itemName + " X" + itemO.Split('|')[1], vec));
                        itemsTex.Add(contetn.Load<Texture2D>("shop/" + item.loaclPatch));
                        vec.Y = vec.Y + (int)(full.Height * 0.21);
                        break;
                    }
                }
            }

        }

        public override void Dispose()
        {
            screenBG.Dispose();
            bgBG.Dispose();
            foreach (TextItem item in items)
            {
                item.Dispose();
            }

            foreach (Texture2D item in itemsTex)
            {
                item.Dispose();
            }

             items = new List<TextItem>();
             itemsTex = new List<Texture2D>(); 
        }
    }
}
