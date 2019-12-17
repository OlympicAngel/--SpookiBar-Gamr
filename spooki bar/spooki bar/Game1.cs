using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Media;
using System.Threading;
using System.IO;

namespace spooki_bar
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region משתנים

        Thread loadFixThread, loadingTRD;
        int treadload = 0;
        User user;
        #region text & logo
        SpriteFont ingametext;
        SpriteFont logo;
        SpriteFont menu;
        SpriteFont login;

        string loadText = "loading Screens";
        #endregion
        #region Screens & items & sizes
        MenuOverlay overlay;
        Item[] items;
        Rectangle fullScreen;
        Screen[] screens;
        Texture2D mainBG;
        public int screenNum { get; set; }
        #endregion
        #region game must
        GraphicsDeviceManager graphics;
        SpriteBatch sB;
        MouseState mouse;
        KeyboardState keysSts;
        SoundEffectPlay[] mainScSo = new SoundEffectPlay[2];
        #endregion
        #region game logic
        Gameplay mainGameControl;
        #endregion
        #region GameScreen(load)
        Monitor monitor;
        Room[] rooms;
        Gif[] effects;
        Mob[] mobs;
        bool freez = false;
        #endregion
        #endregion

        #region Game1

        private void WidthHight()
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


            System.Drawing.Rectangle srv = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            graphics.PreferredBackBufferWidth = srv.Width;
            graphics.PreferredBackBufferHeight = srv.Height;

            this.fullScreen.Height = graphics.PreferredBackBufferHeight;
            this.fullScreen.Width = graphics.PreferredBackBufferWidth;
            this.fullScreen.X = 0;
            this.fullScreen.Y = 0;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content = new ContentManager2(Content.ServiceProvider);
            Content.RootDirectory = "Content";

            WidthHight();

            loadFixThread = new Thread(new ThreadStart(LoadUpdate));

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Form gForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            gForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Window.AllowUserResizing = false;
            Window.Title = "SpookiBar";

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);
        }
        #endregion

        #region unimportant
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        protected override void Initialize()
        {

            base.Initialize();
        }
        #endregion

        #region LoadConatan
        protected override void LoadContent()
        {
            Button.LoadClickSound(Content.Load<SoundEffect>("sound/click"));
            LoadScreenValues();
            #region items
            this.items = new Item[5];
            this.items[0] = new Item(Content.Load<Texture2D>("images/m"), new Rectangle(0, 0, 30, 30));
            this.items[1] = new Item(Content.Load<Texture2D>("images/m2"), new Rectangle(0, 0, 30, 30));
            this.mouse = new MouseState();
            this.screens = new Screen[12];
            #endregion
            #region font
            //try
            //{
            this.ingametext = Content.Load<SpriteFont>("fonts/Pre/ingametextLow");
            this.logo = Content.Load<SpriteFont>("fonts/Pre/logoLow");
            this.menu = Content.Load<SpriteFont>("fonts/MenuBIG");
            this.login = Content.Load<SpriteFont>("fonts/Pre/menuLow");
            menu.LineSpacing = 0;
            //}
            //catch
            //{
            //    this.ingametext = Content.Load<SpriteFont>("fonts/ingametextLow");
            //    this.logo = Content.Load<SpriteFont>("fonts/logoLow");
            //    this.menu = Content.Load<SpriteFont>("fonts/menuLow");
            //    this.login = Content.Load<SpriteFont>("fonts/loginLow");
            //}
            #endregion

            Texture2D loadscreen = Content.Load<Texture2D>("images/loadscreen");
            Texture2D abz = Content.Load<Texture2D>("images/ABZ logo");
            this.screens[0] = new screens.LoadingScreen(loadscreen, abz, ingametext, fullScreen);

            this.mainScSo[0] = new SoundEffectPlay(0.5f, Content.Load<SoundEffect>("sound/cloun"), 1.0f);
            this.mainScSo[1] = new SoundEffectPlay(-0.2f, Content.Load<SoundEffect>("sound/creepy"), 0.85f);
            this.mainScSo[0].PlayLoop(0.0f);

            this.mainBG = Content.Load<Texture2D>("images/mainscreen");

            this.screenNum = -1;

            sB = new SpriteBatch(GraphicsDevice);

            //to prevent load problem in thread
            Texture2D t = Content.Load<Texture2D>("images/login/login");

            screens[1] = new screens.LoginScreen(t, login, Content.Load<Texture2D>("images/login/text-inputk"), Content.Load<Texture2D>("images/login/blood"), Content.Load<Texture2D>("images/login/loading"), Content.Load<Texture2D>("images/login/button"), this.Window, fullScreen, logo);



        }
        #endregion
        #region Update
        protected override void Update(GameTime gameTime)
        {
            GC.Collect();
            UpdateMouseValues();
            HotKeysClick();

            if (loadingTRD != null && loadingTRD.IsAlive)
                return;

            if (!(this.screenNum <= 0 && this.screens[0] != null))
            {
                switch (this.screenNum)
                {
                    case 1:
                        UpdateLogin(gameTime); break;
                    case 2:
                        UpdateMenu(gameTime, -1); break;
                    case 3:
                        UpdateGame(gameTime); break;
                    case 5:
                        UpdateProfile(); break;
                    case 6:
                        screenNum = ((screens.TopScreen)screens[5]).Update(mouse, user, freez);
                        break;
                    case 9:
                        UpdateShop(gameTime); break;
                    case 10:
                        UpdateGameInfo(1); break;
                    case 11:
                        UpdateGameInfo(0); break;
                    default:
                        break;
                }
                if (this.screenNum != 3 && this.screenNum != 1)//play sound if not ingame
                {
                    mainScSo[1].PlayLoop(0.0f);
                    mainScSo[0].PlayLoop(0.0f);
                    int tempScreenValue = overlay.ChangeRoom(freez, mouse, user);
                    if (tempScreenValue != -1)
                    {

                        UpdateMenu(gameTime, tempScreenValue);
                    }
                }
            }
            else
                if (!loadFixThread.IsAlive)
                    loadFixThread.Start();
            this.freez = mouse.LeftButton == ButtonState.Released;
        }
        #endregion
        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sB.Begin();
            if (loadingTRD != null && loadingTRD.IsAlive)
            {
                treadload++;
                DrawGameScreen(gameTime);
                sB.Draw(CPixel.DarkRed, fullScreen, Color.Blue * (float)(Math.Min(0.9, Math.Max(0.1, Math.Abs(Math.Cos(treadload * 0.03))))));
            }
            else
            {
                if (!(this.screenNum <= 0 && this.screens[0] != null))
                {
                    switch (this.screenNum)
                    {
                        case 1:
                            DrawLoginScreen(gameTime); break;
                        case 2:
                            DrawMenuScreen(gameTime); break;
                        case 3:
                            DrawGameScreen(gameTime); break;
                        case 5:
                            DrawProfile(gameTime); break;
                        case 6:
                            DrawScoreboard(); break;
                        case 9:
                            DrawShop(gameTime); break;
                        case 10:
                            DrawGameCredit(1,gameTime); break;
                        case 11:
                            DrawGameCredit(0,gameTime); break;
                        default:
                            screens[screenNum - 1].Print(sB, fullScreen); break;
                    }
                    if (this.screenNum != 3 && this.screenNum != 1)
                        overlay.Print(sB, mouse, user, fullScreen, gameTime);
                }
                else
                    ((screens.LoadingScreen)screens[0]).Draw(sB, loadText, gameTime, fullScreen);

                DrawTopScreenOverlay();
            }
            sB.End();

            if (!IsActive)
                GraphicsDevice.Clear(new Color(0, 0, 0));

            base.Draw(gameTime);
            Gif.isUpdated = false;
        }
        #endregion

        #region Load Functions
        #region load gamescreen
        #region LoadGif
        public Texture2D[] LoadGif(string pathm, int count)
        {
            return LoadGif(pathm, false, count);
        }
        public Texture2D[] LoadGif(string pathm, bool firstFrame, int count)
        {
            return LoadGif(pathm, firstFrame, count, 0);
        }
        public Texture2D[] LoadGif(string pathm, bool firstFrame, int count, int starter)
        {
            Texture2D[] a = new Texture2D[count];
            string add = "00";
            if (starter >= 99)
                add = "";
            else if (starter >= 9)
                add = "0";

            if (firstFrame)
                a[0] = Content.Load<Texture2D>(pathm + add + "" + starter);

            for (int i = starter; i < count; i++)
            {
                if (i >= 99)
                    add = "";
                else if (i >= 9)
                    add = "0";
                if (!firstFrame)
                    a[i] = Content.Load<Texture2D>(pathm + add + (i + 1));
                else
                    a[i] = a[0];
                loadText = "Loading mass data " + Math.Round(((double)i / (double)count) * 100, 1) + "% " + pathm.Split('/')[pathm.Split('/').Length - 1];
            }
            return a;
        }
        #endregion
        #region LoadRooms
        public void LoadRooms()
        {
            this.rooms = new Room[12];
            Texture2D deaf;
            deaf = Content.Load<Texture2D>("images/credit");
            loadText = "Loading effects";
            Texture2D[] mainroomplaces = new Texture2D[3];
            mainroomplaces[0] = Content.Load<Texture2D>("gif/v2/right");
            mainroomplaces[1] = Content.Load<Texture2D>("gif/v2/mid");
            mainroomplaces[2] = Content.Load<Texture2D>("gif/v2/left");
            Gif[] aniTmp = new Gif[2];
            aniTmp[0] = new Gif(LoadGif("gif/v2/f/r0", false, 9, 0), 9);
            aniTmp[1] = new Gif(LoadGif("gif/v2/f/l0", false, 9, 0), 9);
            rooms[0] = (Room)(new MainRoom(mainroomplaces, aniTmp, Content.Load<SoundEffect>("sound/motion")));

            rooms[1] = new Room(CPixel.Grey, 1, "startroom");
            loadText = "Loading Room #1";
            rooms[2] = new Room(CPixel.Grey, 2, "toiletRoomOn");
            loadText = "Loading Room #2";
            rooms[3] = new Room(CPixel.Grey, 3, "3");
            loadText = "Loading Room #3";
            rooms[4] = new Room(CPixel.Grey, 4, "danceOn");
            loadText = "Loading Room #4";
            rooms[5] = new Room(CPixel.Grey, 5, "hallway");
            loadText = "Loading Room #5";
            rooms[6] = new Room(deaf, 6);//*****//
            loadText = "Loading Room #6";
            rooms[7] = new Room(CPixel.Grey, 7, "room7");
            loadText = "Loading Room #7";
            rooms[8] = new Room(CPixel.Grey, 8, "LeftAirVent");
            loadText = "Loading Room #8";
            rooms[9] = new Room(CPixel.Grey, 9, "9");
            loadText = "Loading Room #9";
            rooms[10] = new Room(CPixel.Grey, 10, "midAirVent");
            loadText = "Loading Room #10";
            rooms[11] = new Room(CPixel.Grey, 11, "6AirVent");

        }
        #endregion
        #region LoadEffects
        public void LoadEffects()
        {
            this.effects = new Gif[2];
            this.effects[1] = new Gif(LoadGif("gif/screen/", 7), 2);
            this.effects[0] = new Gif(LoadGif("gif/line/", 16), 5);
            loadText = "Loading Sound preset";

        }
        #endregion
        #region LoadItems
        public void LoadItems()
        {
            this.items[2] = new Button(Content.Load<Texture2D>("images/game/monitorbutton"), Content.Load<Texture2D>("images/game/monitorbutton"),
                new Rectangle(fullScreen.Width / 3, fullScreen.Height - (int)(fullScreen.Height * 0.09), fullScreen.Width / 3, (int)(fullScreen.Height * 0.075)));
            loadText = "Loading Light level";
            this.items[3] = new Button(Content.Load<Texture2D>("images/game/lightOn"), Content.Load<Texture2D>("images/game/lightOff"),
                new Rectangle(fullScreen.Width / 8, fullScreen.Height - (int)(fullScreen.Height * 0.08), fullScreen.Width / 15, (int)(fullScreen.Height * 0.05)));
            loadText = "Loading Light and Sahdows";
            this.items[4] = new Button(Content.Load<Texture2D>("images/game/lightOn"), Content.Load<Texture2D>("images/game/lightOff"),
                new Rectangle(fullScreen.Width / 8 * 7 - fullScreen.Width / 15, fullScreen.Height - (int)(fullScreen.Height * 0.08), fullScreen.Width / 15, (int)(fullScreen.Height * 0.05)));
        }
        #endregion
        #region LoadScreenValues
        public void LoadScreenValues()
        {
            this.fullScreen = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
        #endregion
        #region LoadMonitor
        public void LoadMonitor()
        {
            monitor = new Monitor(Content.Load<Texture2D>("images/game/monitor"),
                new Rectangle(fullScreen.Width - (int)(fullScreen.Width * 0.91), fullScreen.Height - (int)(fullScreen.Height * 0.9), (int)(fullScreen.Width * 0.9), (int)(fullScreen.Height * 0.9)),
                this.rooms,
                Content.Load<Texture2D>("images/game/monitorMap"), Content.Load<Texture2D>("images/game/batteryBar"));
        }
        #endregion
        #region LoadMobs
        public void LoadMobs()
        {
            //loadText = "Loading Summer Mob";
            loadText = "mob array";
            this.mobs = new Mob[3];
            loadText = "getting summer ai";
            string raw = DataLoad.GetMobAI(1);
            mobs[0] = new Mob("summer", int.Parse(raw.Split('*')[0]), raw.Split('*')[1], 3);
            mobs[0].LoadStep(0.0f, Content.Load<SoundEffect>("sound/stepE"), 0.5f);
            mobs[0].LoadTextures(Content, DataLoad.GetMobsRoom2TextureConfig(1));
            loadText = "Loading Bob Mob";

            WidthHight();

            raw = DataLoad.GetMobAI(2);
            mobs[1] = new Mob("bob", int.Parse(raw.Split('*')[0]), raw.Split('*')[1], 10);
            mobs[1].LoadStep(0.0f, Content.Load<SoundEffect>("sound/tp"), 0.5f);
            mobs[1].LoadTextures(Content, DataLoad.GetMobsRoom2TextureConfig(2));

            loadText = "Loading Bob Mob";
            raw = DataLoad.GetMobAI(3);
            mobs[2] = new Mob("chucki", int.Parse(raw.Split('*')[0]), raw.Split('*')[1], 5);
            mobs[2].LoadStep(0.0f, Content.Load<SoundEffect>("sound/spidermove"), 1.0f);
            mobs[2].LoadTextures(Content, DataLoad.GetMobsRoom2TextureConfig(3));
            //mobs[1].LoadStep(1.0f, Content.Load<SoundEffect>("sound/stepE"), 0.1f);
            //mobs[2] = new Mob("name", Content.Load<Texture2D>("images/m"), 3);
            //mobs[3] = new Mob("name", Content.Load<Texture2D>("images/m"), 4);
        }
        #endregion
        #endregion
        #endregion
        #region Draw Functions
        #region DrawLoginScreen
        public void DrawLoginScreen(GameTime time)
        {
            ((screens.LoginScreen)screens[1]).Print(sB, fullScreen);
            ((screens.GameScreen)screens[3]).DrawNoise(sB, fullScreen, time);
        }
        #endregion
        #region DrawMenuScreen
        public void DrawMenuScreen(GameTime timer2)
        {
            ((screens.MenuScreen)screens[2]).PrintBG(sB, fullScreen);

            ((screens.GameScreen)screens[3]).DrawNoise(sB, fullScreen,timer2);
            ((screens.MenuScreen)screens[2]).Print(sB, this.mouse, user, fullScreen, timer2);

        }
        #endregion
        #region DrawGameScreen
        public void DrawGameScreen(GameTime time)
        {
            this.mainGameControl.Draw(this.sB, this.mouse, this.fullScreen, time);
        }
        #endregion
        #region DrawProfile
        public void DrawProfile(GameTime tuimer)
        {
            ((screens.ProfileScreen)screens[4]).Print(sB, this.mouse, this.user, fullScreen, tuimer);
            //((screens.GameScreen)screens[3]).DrawNoise(sB, fullScreen);
        }
        #endregion
        #region DrawGameCredit
        public void DrawGameCredit(int val,GameTime time)
        {
            ((screens.GameInfoScreen)screens[10 - val]).Print(sB, this.mouse, fullScreen);
            ((screens.GameScreen)screens[3]).DrawNoise(sB, fullScreen,time);
        }
        #endregion
        #region DrawShop
        public void DrawShop(GameTime time)
        {
            ((screens.ShopScreen)screens[8]).Print(sB, mouse, fullScreen);
            ((screens.GameScreen)screens[3]).GetGifsEffect()[0].DrawTrans(sB, fullScreen, 50, true, time.ElapsedGameTime);
            ((screens.GameScreen)screens[3]).GetGifsEffect()[1].DrawTrans(sB, fullScreen, 20, true, time.ElapsedGameTime);
        }
        #endregion
        #region DrawTopScreenOverlay
        public void DrawTopScreenOverlay()
        {
            if (!freez)
                this.items[0].Draw(sB);
            else
                this.items[1].Draw(sB);
        }
        #endregion
        #region DrawScoreboard
        public void DrawScoreboard()
        {
            ((screens.TopScreen)screens[5]).Print(sB, mouse, fullScreen);
        }
        #endregion
        #endregion
        #region Update Functions
        #region UpdateMouseValues
        public void UpdateMouseValues()
        {
            this.mouse = Mouse.GetState();
            this.items[0].SetPos(this.mouse.X, this.mouse.Y);
            this.items[1].SetPos(this.mouse.X, this.mouse.Y);
        }
        #endregion
        #region HotKeysClick
        public void HotKeysClick()
        {
            keysSts = Keyboard.GetState();
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (keysSts.IsKeyDown(Keys.Escape)))
                this.Exit();

            if ((keysSts.IsKeyDown(Keys.F11)) && !(this.screenNum <= 0))
            {
                graphics.ToggleFullScreen();
            }

        }
        #endregion
        #region LoadUpdate
        public void LoadUpdate()
        {
            loadText = "Loading CPixels";
            CPixel.DarkGrey = Content.Load<Texture2D>("Pixel/pixelDaekGray");
            CPixel.Grey = Content.Load<Texture2D>("Pixel/pixelGray");
            CPixel.LightGrey = Content.Load<Texture2D>("Pixel/pixelLightGray");
            CPixel.DarkRed = Content.Load<Texture2D>("Pixel/pixelDarkRED");
            CPixel.LightRed = Content.Load<Texture2D>("Pixel/pixelLightRed");
            CPixel.RedFade = Content.Load<Texture2D>("Pixel/pixelRedFadeTrans");

            this.loadText = "Loading Contant";
            loadText = "Loading screen values";
            LoadScreenValues();
            loadText = "Loading in-game rooms";
            LoadRooms();
            loadText = "Loading effects";
            LoadEffects();
            loadText = "Loading Menu and Contols items";
            LoadItems();
            LoadMobs();
            loadText = "Loading Monitor";
            LoadMonitor();

            loadText = "Loading some more sound";
            SoundEffect seffect = Content.Load<SoundEffect>("sound/metalic");
            MenuText.SetSound(seffect);
            TopMenuText.SetSound(seffect);

            loadText = "Creating Objects";
            overlay = new MenuOverlay(mainBG, logo, ingametext, menu, fullScreen);
            loadText = "Loading some spooky things";
            Texture2D[] jump = new Texture2D[3];
            jump[0] = Content.Load<Texture2D>("room skins/bob");
            jump[1] = Content.Load<Texture2D>("room skins/spider");
            jump[2] = Content.Load<Texture2D>("room skins/summer");
            loadText = "Creating rooms!";
            screens[2] = new screens.MenuScreen(mainBG, logo, ingametext, menu, fullScreen, jump);
            loadText = "Creating more ROOMS";
            screens[3] = new screens.GameScreen(rooms, effects, items, monitor, new ForceFiels(Content), Content.Load<Texture2D>("gif/blur/blur"));
            loadText = "Collecting Credits";
            screens[8] = new screens.ShopScreen(Content.Load<Texture2D>("images/shop"), login, logo, Content, fullScreen);
            loadText = "You DONT EVEN KNOW THAT ROOM (6)";
            screens[9] = new screens.GameInfoScreen(screens[2].screenBG, Content.Load<Texture2D>("images/crinfo"), menu);
            loadText = "Ignores Credits";
            screens[10] = new screens.GameInfoScreen(screens[2].screenBG, Content.Load<Texture2D>("images/credit"), menu);
            loadText = "Make Things to connect";
            mainGameControl = new Gameplay(this.mobs, this.ingametext, Content.Load<SoundEffect>("sound/static"), Content.Load<Texture2D>("images/game/battery"), Content.Load<Texture2D>("images/game/batteryBar"), ((screens.GameScreen)screens[3]), Content.Load<SoundEffect>("sound/kill"), Content);
            this.screenNum = 1;
            ((screens.LoadingScreen)screens[0]).SetDis(true);
            loadText = "And.... (about 3 sec xD)";
            this.mainScSo[1].Togggle();
            Thread.Sleep(3000);
            loadText = "Go GO GO";
            ((screens.LoadingScreen)screens[0]).Dispose();
            screens[0] = null;
            GC.Collect();
            WidthHight();
        }
        #endregion
        #region UpdateLogin
        public void UpdateLogin(GameTime gameTime)
        {
            ((screens.LoginScreen)screens[1]).Update(gameTime, mouse, out user);
            if (user != null)
            {
                this.screenNum = 2;
                screens[4] = new screens.ProfileScreen(login, user, Content, fullScreen);
                screens[5] = new screens.TopScreen(mainBG, login, user, fullScreen);
                Thread dellThread = new Thread(new ThreadStart(DisposeLogin));
                dellThread.Start();
            }
        }
        public void DisposeLogin()
        {
            screens[1].Dispose();
            screens[1] = null;
            GC.Collect();
        }
        #endregion
        #region UpdateMenu
        public void UpdateMenu(GameTime gameTime, int set)
        {
            if (set == 5)
            {
                ((screens.TopScreen)screens[5]).LoadList(user);
            }
            if (set == 1)
            {
                screenNum = 2;
                return;
            }

            int tempScreenNum = ((screens.MenuScreen)screens[2]).ChangeRoom(this.freez, this.mouse, user);
            if (set != -1)
                tempScreenNum = set;
            mouse = Mouse.GetState();
            if ("2,3,7,6".Contains(tempScreenNum.ToString()))
            {
                int newGamePlayAt = 0;
                if (tempScreenNum == 2)
                {
                    user.save = 1;
                    DataLoad.ResetSaveNight(user);
                    newGamePlayAt = 1;
                }
                else if (tempScreenNum == 3)
                    newGamePlayAt = user.save;
                else if (tempScreenNum == 7)
                    newGamePlayAt = 6;
                else if (tempScreenNum == 6)
                    newGamePlayAt = 7;

                loadingTRD = new Thread(new ParameterizedThreadStart(LoadingThread));
                loadingTRD.Start(newGamePlayAt);

                //LoadingThread(playat);

            }
            else if (tempScreenNum == 11)
            {
                DataLoad.ResetUserData(user);
                DataLoad.UpdateUser(user);
            }
            else if (tempScreenNum == 4)
            {
                ((screens.ProfileScreen)screens[4]).Load(Content, user, (screens.ShopScreen)screens[8], fullScreen);
                screenNum = tempScreenNum + 1;
            }
            else if (tempScreenNum != -1 && freez)
            {
                try
                {
                    screens[tempScreenNum].ToString(); //if not error here
                    screenNum = tempScreenNum + 1;
                }
                catch { SystemSounds.Beep.Play(); }
                finally { }
            }
        }
        #endregion
        #region UpdateGame
        public void UpdateGame(GameTime gameTime)
        {
            mainScSo[0].Off();//when game play //if main sound on - make it off
            mainScSo[1].SetVol(0.05f);//when game play //if bg sound off - make it on in correct vol
            mainScSo[1].PlayLoop(0.0f);//play if not playing

            if (mainGameControl.Update(gameTime, this.mouse, this.fullScreen, this.freez, user, this.IsActive))//if game end
            {
                if (mainGameControl.GetEndValu() == 1)
                { }

                DataLoad.UpdateUser(user);

                this.screenNum = 2;//menu

                mainScSo[1].SetVol(0.8f);//update vol
                mainScSo[1].Off();

                mainScSo[1].PlayLoop(0.0f);
                mainScSo[0].PlayLoop(0.0f);
                mainGameControl.Reset();
            }
        }

        #endregion
        #region UpdateGameInfo
        public void UpdateGameInfo(int val)
        {
            if ((((screens.GameInfoScreen)screens[10 - val]).Update(mouse) == 2))
                screenNum = 2;
        }
        #endregion
        #region Update User Profile
        public void UpdateProfile()
        {
            this.screenNum = ((screens.ProfileScreen)screens[4]).Update(mouse);
            if (this.screenNum == 2)
                ((screens.ProfileScreen)screens[4]).Dispose();
        }
        #endregion
        #region UpdateShop()
        public void UpdateShop(GameTime gametime)
        {
            if (((screens.ShopScreen)screens[8]).Update(mouse, freez, user, gametime, fullScreen))
                this.screenNum = 2;

        }
        #endregion
        public void Pregame()
        {
            screenNum = 3;
            Mouse.SetPosition(fullScreen.Width / 2, fullScreen.Height / 2);
            ((screens.GameInfoScreen)screens[10]).Dispose();
            ((screens.GameInfoScreen)screens[9]).Dispose();
            WidthHight();
        }
        public void Aftergame()
        {
            Mouse.SetPosition(fullScreen.Width / 2, fullScreen.Height / 2);
            ((screens.GameInfoScreen)screens[10]).Reload(Content, "images/credit");
            ((screens.GameInfoScreen)screens[9]).Reload(Content, "images/crinfo");
            WidthHight();
        }
        public void LoadingThread(object newGamePlayAt)
        {
            mainGameControl.PlayNight((int)newGamePlayAt, Content, DataLoad.DoesUSerHaveItem(user, 1), DataLoad.DoesUSerHaveItem(user, 2));
            Pregame();
        }
        #endregion
    }
}
