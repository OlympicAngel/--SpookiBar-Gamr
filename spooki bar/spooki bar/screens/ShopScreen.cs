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
using System.Threading;
using System.Media;

namespace spooki_bar.screens
{
    class ShopScreen : Screen
    {
        SpriteFont font;
        TextItem title;
        TextItem info;
        public ServiceReference1.Shop shop{get;set;}
        TextItem money;
        Texture2D load;
        Thread buyer;
        int loading = 0;
        int maxX;
        TimeSpan timer = new TimeSpan(0, 0, 3);

        public ShopScreen(Texture2D tex, SpriteFont font, SpriteFont title, ContentManager graphics, Rectangle full)
            : base(tex)
        {
            this.buyer = new Thread(new ParameterizedThreadStart(BuyItem));
            this.title = new TextItem(title, "SHOP", new Vector2(full.Width / 2 - title.MeasureString("Shop").X / 2, full.Height/9));
            this.money = new TextItem(font, "", new Vector2(0, full.Height / 9));
            info = new TextItem(font, "", new Vector2(0, full.Height / 2 * 1.5f));
            this.font = font;
            this.shop = DataLoad.GetShopItems();
            maxX = 4;
            int x = 0, y = 0, space = 10, width = full.Width / maxX - space * maxX, height = (int)(width * 0.56);
            int topOffset = height;
            Texture2D border = graphics.Load<Texture2D>("shop/border");
            foreach (ShopItem item in this.shop)
            {
                Texture2D t = graphics.Load<Texture2D>("shop/" + item.loaclPatch);
                double ratio = (double)t.Height / (double)t.Width;
                this.controls.Add(new Button(t, t, new Rectangle((x + space + width * x) + (width - (int)(height * 0.8 / ratio)) / 2, topOffset + space + y * height, (int)((double)height * 0.8 / ratio), (int)(height * 0.8))));//image
                this.controls.Add(new Item(border, new Rectangle(x + space * x + width * x, topOffset + space + y * height, width, height)));//image border
                this.controls.Add(new TextItem(font, item.itemName + "\n cost:" + item.price + ".00 OA points", new Vector2(x + (int)((space + width) * 0.25) + space * x + width * x, topOffset + font.MeasureString("1").Y * y + space + (int)((y + 1) * height * 0.9))));
                x++;
                if (x > maxX)
                {
                    x = 0;
                    y++;
                }
            }

            Color[] c = new Color[10];
            load = new Texture2D(tex.GraphicsDevice, 2, 5);

            c[0] = Color.Transparent;
            c[1] = Color.Transparent;

            c[2] = Color.Red * 0.0f;
            c[3] = Color.DarkRed;
            c[4] = Color.Black * 0.0f;
            c[5] = Color.Purple;
            c[6] = Color.Red * 0.0f;
            c[7] = Color.DarkRed;

            c[8] = Color.Transparent;
            c[9] = Color.Transparent;
            load.SetData<Color>(c);
        }


        public void BuyItem(object list)
        {
            User user = null;
            string itemID = "";
            foreach (object item in ((List<object>)list))
            {
                if (item is User)
                    user = (User)item;
                else
                    itemID = item.ToString();
            }

            itemID = this.shop[int.Parse(itemID)].itemID;

            this.info.text = DataLoad.BuyShopItem(itemID, user).Replace("owned","own");
            DataLoad.UpdateUser(user);

            money.text = user.money.ToString() + ".0$ OA points";
        }

        public void Print(SpriteBatch sb, MouseState m, Rectangle full)
        {

            sb.Draw(screenBG, full, Color.White);
            foreach (Item item in controls)
            {
                if (item is Button)
                {
                    if (item.pos.Contains(m.X, m.Y) && !buyer.IsAlive)
                        item.Draw(sb, Color.White);
                    else
                        item.Draw(sb, Color.DarkGray);
                }
                else if (item is TextItem)
                {
                    ((TextItem)item).Draw(sb, 1.0f - (float)(maxX * 0.08), Color.Black, 1);
                    ((TextItem)item).Draw(sb, 1.0f - (float)(maxX * 0.08), Color.Black, -1);
                    ((TextItem)item).Draw(sb, 1.0f - (float)(maxX * 0.08), Color.White, 0);
                }
                else
                    item.Draw(sb);
            }
            money.Draw(sb);

                if (buyer.IsAlive || timer.Seconds < 3)
                {
                    sb.Draw(screenBG, full, Color.Black * 0.6f);

                    CPixel.Blend(sb, BlendState.Additive, load, new Rectangle(full.Width / 3 + loading-2, full.Height / 4, full.Height / 20, full.Width / 15), Color.White * 0.3f);
                    CPixel.Blend(sb, BlendState.Additive, load, new Rectangle(full.Width / 3 + loading, full.Height / 4, full.Height / 20, full.Width / 15), Color.White);
                    loading = loading + 2;
                    if (loading > full.Width / 4)
                        loading = 0;

                    
                    CPixel.Blend(sb, BlendState.Additive, load, new Rectangle(full.Width / 3 + loading, full.Height / 4, full.Height / 20, full.Width / 15), Color.White);

                    this.info.pos.X = full.Width/2 - (int)(this.info.font.MeasureString(this.info.text).X / 2);
                    info.DrawShadow(sb, 20);
                    info.Draw(sb);
                    title.Draw(sb, 0.6f);
                }
            title.DrawShadow(sb, 10, Color.Black * 0.7f);
            title.Draw(sb);
        }

        public bool Update(MouseState m, bool freez, User user, GameTime t, Rectangle full)
        {
            if (t.TotalGameTime.Milliseconds % 200 == 0)
                money.text = user.money.ToString() + ".0$ OA points";

            money.pos.X = full.Width - (int)money.font.MeasureString(money.text).X;

            if (!buyer.IsAlive && timer.Seconds < 3)
                timer = timer.Add(t.ElapsedGameTime);

            if (!buyer.IsAlive && timer.Seconds >= 3)
            {

                if (title.text != "SHOP")
                {
                    this.title.text = "SHOP";
                    title.pos.X = (int)(full.Width / 2 - title.font.MeasureString("Shop").X / 2);
                }

                int itempPos = 0;
                foreach (Item item in this.controls)
                {
                    if (item is Button && ((Button)item).IsClicked(m, freez))
                    {
                        if (this.shop[itempPos].price <= user.money)
                        {
                            timer = new TimeSpan(0, 0, 0);
                            List<Object> LObj = new List<object>();
                            LObj.Add(itempPos);
                            LObj.Add(user);
                            this.buyer = new Thread(new ParameterizedThreadStart(BuyItem));
                            if (!buyer.IsAlive)
                                buyer.Start(LObj);
                        }
                        else
                        {
                            SystemSounds.Beep.Play();
                        }
                    }
                    else if (item is Button)
                        itempPos++;
                }
            }
            return false;
        }
    }
}
