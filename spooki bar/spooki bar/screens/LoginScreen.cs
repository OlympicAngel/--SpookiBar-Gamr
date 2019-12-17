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
using System.Threading;

namespace spooki_bar.screens
{
    class LoginScreen : Screen
    {
        Random rnd = new Random();
        TextItem title;
        TextItem spook;
        Item load;
        TextBox username;
        TextBox password;
        Button enter;
        KeyboardDispatcher key;
        GameTime gameTime2;
        Thread oThread;
        string data = "";
        int pos = 0;
        MouseState mouse;
        bool isUpToDate = true;

        #region LoginScreen
        public LoginScreen(Texture2D n, SpriteFont font, Texture2D input, Texture2D cruss, Texture2D loading, Texture2D but, GameWindow Window, Rectangle full, SpriteFont fontT)
            : base(n)
        {

            spook = new TextItem(fontT, "Spook-I Bar", new Vector2(full.Width / 2 - fontT.MeasureString("Spook-I Bar").X / 2, full.Height / 5));
            oThread = new Thread(new ThreadStart(LoginCheck));
            this.username = new TextBox(input, cruss, font);
            this.username.Width = 300;
            this.username.X = full.Width / 2 - this.username.Width / 2;
            this.username.Y = full.Height / 2 - (this.username.Height * 4) / 2;
            this.username.Text = "User Name";//***************************************//
            this.username.Selected = true;
            this.password = new TextBox(input, cruss, font);
            this.password.X = this.username.X;
            this.password.Y = this.username.Y + 5 * this.username.Height / 3;
            this.password.Width = this.username.Width;
            this.password.Text = "1234";
            this.password.PasswordBox = true;
            this.key = new KeyboardDispatcher(Window);

            this.enter = new Button(but, but, new Rectangle(this.username.X + this.username.Width / 4, this.password.Y + 5 * this.username.Height / 3, this.username.Width / 2, this.username.Height));

            username.Clicked += new TextBoxEvent(ToggleSelected);
            password.Clicked += new TextBoxEvent(ToggleSelected);

            this.title = new TextItem(font, "Login:", new Vector2(full.Width / 2 - this.username.Width / 2, full.Height / 2 - (this.username.Height * 4)));
            this.load = new Item(loading, new Rectangle(full.Width / 4, this.password.Y + 5 * this.username.Height / 3, loading.Width, loading.Height));



            Vector2 basePos = new Vector2((int)(full.Width * 0.0291666666666667), (int)(full.Height * 0.1962962962962963));
            int width = (int)(full.Width * 0.2927083333333333),
                height = (int)(full.Height * 0.1055555555555556);
            playOptions = new MenuText(spook.font, "New Game", basePos, width, height);

            isUpToDate = DataLoad.CheckUpdate();
            if (!isUpToDate)
                title.text = "This version of the game is out of date,\n please update the game at \n http://www.abz.somme";
        }
        MenuText playOptions;
        #endregion

        #region ToggleSelected
        private void ToggleSelected(TextBox sender)
        {
            if (sender.Equals(username))
            {
                username.Selected = true;
                if (password.Text == "")
                    password.Text = "1234";
            }
            else
            {
                username.Selected = false;
                if (username.Text == "")
                    username.Text = "User Name";
            }
            sender.Text = "";
        }
        #endregion

        public int Update(GameTime gameTime, MouseState m, out User u)
        {
            if (isUpToDate)
            {
                mouse = m;
                #region control update
                if (!oThread.IsAlive)
                {
                    if (username.Selected)
                    {
                        password.Selected = false;
                        key.Subscriber = username;
                    }
                    else
                    {
                        password.Selected = true;
                        key.Subscriber = password;
                    }
                    username.Update(gameTime);
                    password.Update(gameTime);
                }
                this.gameTime2 = gameTime;
                #endregion
                if (this.enter.IsClicked(m, false) || (Keyboard.GetState().IsKeyDown(Keys.Enter)))
                {
                    if (oThread != null && !oThread.IsAlive && password.Text != "" && password.Text != null && password.Text.Length >= 4 && username.Text != null && username.Text != "")
                    {
                        oThread = new Thread(new ThreadStart(LoginCheck));
                        oThread.Start();
                    }
                }
            }
            if (data != "")
            {
                u = null;
                GC.Collect();
                u = new User(data);
                return 2;
            }
            u = null;

            return 1;
        }

        private void LoginCheck()
        {
            this.data = "";
            string pass = password.Text;
            password.Text = "";
            data = DataLoad.LoadUserData(username.Text, pass);
            if (data.Contains("~Error"))
            {
                title.text = "Login:\n" + data;
                data = "";
            }
        }

        public override void Print(SpriteBatch sb, Rectangle full)
        {
            if (rnd.Next(0, 20) == 1)
                EffectRender.RenderBroken(sb, screenBG, full, full.Width / 6, false, 15);
            else if (rnd.Next(0, 3) == 1)
                EffectRender.RenderBroken(sb, screenBG, full, full.Width / 6, false, 2);
            else if (rnd.Next(0, 20) == 1)
                EffectRender.RenderBroken(sb, screenBG, full, full.Width / 6, true, 2);
            else
                base.Print(sb, full);


            title.DrawShadow(sb, 5);
            title.DrawPhantom(sb, 2);
            title.Draw(sb);
            spook.DrawShadow(sb, 15);
            spook.DrawPhantom(sb, full.Width / 50);
            spook.Draw(sb);

            if (isUpToDate)
            {
                username.Draw(sb, gameTime2);
                password.Draw(sb, gameTime2);
                if (!oThread.IsAlive)
                    enter.Draw(sb);

                if (oThread.IsAlive)
                {
                    load.DrawAs(sb, pos);
                    pos = pos + 8;
                    if (pos > (double)(full.Width / 2)) pos = 0;
                }
            }

        }

        public override void Dispose()
        {
            base.Dispose();
            rnd = null;
            title.Dispose();
            title = null;
            spook.Dispose();
            spook = null;
            load.Dispose();
            username.Dispose();
            username = null;
            password.Dispose();
            password = null;
            enter.Dispose();
            enter = null;
            key = null;
            oThread = null;
            data = null;
            pos = 0;
            GC.Collect();
        }
    }
}
