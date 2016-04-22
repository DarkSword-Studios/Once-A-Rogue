﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection; //Needed for string to variable calls
using System.IO; //Needed for finding rooms
using System.Collections.Generic; //Needed for lists
using System; //Needed for random objects
using Microsoft.Xna.Framework.Media; //Needed for music
using System.Threading;



namespace Once_A_Rogue
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;

        //Enums for game state
        enum GameState { MainMenu, Playing, GameOver, paused, howTo, Context }
        GameState gameState;

        //Enums for selector arrow
        enum ArrowState { pos1, pos2, pos3, menu1, menu2 }
        ArrowState arrowState;

        //Enums for player weapon state
        enum PlayWepState { Sword, Rogue, Mage, Ranger }
        PlayWepState playWepState;

        //Enums for context menu state
        enum ContextState { Skills, Lore }
        ContextState contextState;

        //Declare a grid to keep track of level space
        string [,] gridSystem;       
        const int ROWS = 9;
        const int COLUMNS = 9;

        //These variables are used to determine the screensize
        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        //Stage locking a room
        Boolean lockRoom = false;
        Boolean unlockRoom = false;

        //Declare a number of rooms for a level
        int numRooms;
        
        //Track room structure via the grid system
        Room[,] levelAnnex;

        //Track active rooms
        Room activeRoom;

        //Track potential boss rooms
        List<Room> possibleBossRooms;

        //The camera manages the viewport and location of drawn objects
        Camera camera;

        //Declare fonts here
        SpriteFont alertText;

        //Manage room activity to minimize impact on CPU
        bool shifting;
        int oldRow = -1;
        int oldCol = -1;
        string playerMove = "none";

        //Keep track of whether or not we need to generate a new level (cannot be done immediately upon assignment)
        private Boolean levelTrigger;

        //Declare Room Textures
        Texture2D tilemap, playerTextures, projectileTextures;

        //Declare Minimap Textures
        Texture2D leftuprightdown, down, up, left, right, leftup, leftright, leftdown, upright, updown, rightdown;
        Texture2D leftupright, leftupdown, leftrightdown, uprightdown, blackSlate, whiteSlate, unknown;

        //Declare Notification Tool Textures
        Texture2D diagonalBar;

        //Enemy textures
        Texture2D goblinEnemy;
        Texture2D koboldEnemy;
        Texture2D ghoulEnemy;

        //Handle Minimap Textures:
        Dictionary<string, Texture2D> mapTextures = new Dictionary<string, Texture2D>();

        //Declare HUD Textures
        Texture2D pause, exit, resume, select, control, controls, mage, ranger, sword, rogue, back, main, play, exitM, mana, health, container;
        Texture2D contextSkill, contextLore, skillSwitch, loreSwitch, skillMage, skillRogue, skillWarrior, skillRanger;

        //Keyboard states
        KeyboardState previousKBS, kbs;

        //Mouse states
        MouseState previouseMS, mouseState;

        //Level builder to create and connect rooms
        LevelBuilder builderAlpha;

        Cursor cur;

        //Song for music
        Song mainMusic, bossMusic;

        //Keep track of current song
        string currentSong;

        //List to keep track of projectiles
        private static List<Projectile> currProjectiles;
        private static List<Projectile> removeProj;

        public static List<Projectile> CurrProjectiles
        {
            get { return currProjectiles; }
            set { currProjectiles = value; }
        }

        public static List<Projectile> RemoveProj
        {
            get { return removeProj; }
            set { removeProj = value; }
        }

        float manaBarWidth;
        float healthBarWidth;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Allow the game to run in windowed borderless
            this.IsMouseVisible = true;
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
            graphics.ApplyChanges();

            currProjectiles = new List<Projectile>();

            //Initializing the gamestate
            gameState = GameState.MainMenu;
            arrowState = ArrowState.menu1;
            playWepState = PlayWepState.Sword;
            contextState = ContextState.Skills;

            //Starting level please!
            NewLevelGen();

            //Initializing the Cursor
            cur = new Cursor();          

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //player.Texture = Content.Load<Texture2D>("Player");

            //Load fonts here
            alertText = Content.Load<SpriteFont>("alertText");

            //Notification Textures
            diagonalBar = Content.Load<Texture2D>("DiagonalNotificationBar.png");

            Notification.font = alertText;
            Notification.diagonalBar = diagonalBar;

            //Load in tilemap and player spritesheet
            tilemap = Content.Load<Texture2D>("Tileset.png");
            playerTextures = Content.Load <Texture2D>("PlayerAnims.png");
            projectileTextures = Content.Load<Texture2D>("Projectile Spritesheet.png");

            //Initialize HUD textures
            pause = Content.Load<Texture2D>("HUDStuff/HUDpause.png");
            exit = Content.Load<Texture2D>("HUDStuff/HUDexittowindowsbutton.png");
            select = Content.Load<Texture2D>("HUDStuff/HUDselect.png");
            resume = Content.Load<Texture2D>("HUDStuff/HUDresume.png");
            control = Content.Load<Texture2D>("HUDStuff/HUDcontrol.png");
            controls = Content.Load<Texture2D>("HUDStuff/HUDcontrols.png");
            sword = Content.Load<Texture2D>("HUDStuff/HUDsword.png");
            rogue = Content.Load<Texture2D>("HUDStuff/HUDrogue.png");
            mage = Content.Load<Texture2D>("HUDStuff/HUDmage.png");
            ranger = Content.Load<Texture2D>("HUDStuff/HUDranger.png");
            back = Content.Load<Texture2D>("HUDStuff/HUDback.png");
            main = Content.Load<Texture2D>("HUDStuff/HUDMain.png");
            play = Content.Load<Texture2D>("HUDStuff/HUDplay.png");
            exitM = Content.Load<Texture2D>("HUDStuff/HUDexit.png");
            mana = Content.Load<Texture2D>("HUDStuff/HUDManaBar.png");
            health = Content.Load<Texture2D>("HUDStuff/HUDHealthBar.png");
            container = Content.Load<Texture2D>("HUDStuff/HUDContainer.png");
            contextSkill = Content.Load<Texture2D>("HUDStuff/HUDcontext_skills.png");
            contextLore = Content.Load<Texture2D>("HUDStuff/HUDcontext_lore.png");
            skillSwitch = Content.Load<Texture2D>("HUDStuff/HUDcontext_skills_switch");
            loreSwitch = Content.Load<Texture2D>("HUDStuff/HUDcontext_lore_switch");
            skillMage = Content.Load<Texture2D>("HUDStuff/HUDskillmage");
            skillRanger = Content.Load<Texture2D>("HUDStuff/HUDskillranger");
            skillWarrior = Content.Load<Texture2D>("HUDStuff/HUDskillwarrior");
            skillRogue = Content.Load<Texture2D>("HUDStuff/HUDskillrogue");

            //Initialize MiniMap textures and add them to the map texture dictionary
            mapTextures.Add("BlackSlate", blackSlate = Content.Load<Texture2D>("BlackSlate.png"));
            mapTextures.Add("Unknown", unknown = Content.Load<Texture2D>("Unknown.png"));
            mapTextures.Add("LEFTUPRIGHTDOWN", leftuprightdown = Content.Load<Texture2D>("LEFTUPRIGHTDOWNmap.png"));
            mapTextures.Add("DOWN", down = Content.Load<Texture2D>("DOWNmap.png"));
            mapTextures.Add("UP", up = Content.Load<Texture2D>("UPmap.png"));
            mapTextures.Add("LEFT", left = Content.Load<Texture2D>("LEFTmap.png"));
            mapTextures.Add("RIGHT", right = Content.Load<Texture2D>("RIGHTmap.png"));
            mapTextures.Add("LEFTUP", leftup = Content.Load<Texture2D>("LEFTUPmap.png"));
            mapTextures.Add("LEFTRIGHT", leftright = Content.Load<Texture2D>("LEFTRIGHTmap.png"));
            mapTextures.Add("LEFTDOWN", leftdown = Content.Load<Texture2D>("LEFTDOWNmap.png"));
            mapTextures.Add("UPRIGHT", upright = Content.Load<Texture2D>("UPRIGHTmap.png"));
            mapTextures.Add("UPDOWN", updown = Content.Load<Texture2D>("UPDOWNmap.png"));
            mapTextures.Add("RIGHTDOWN", rightdown = Content.Load<Texture2D>("RIGHTDOWNmap.png"));
            mapTextures.Add("LEFTUPRIGHT", leftupright = Content.Load<Texture2D>("LEFTUPRIGHTmap.png"));
            mapTextures.Add("LEFTUPDOWN", leftupdown = Content.Load<Texture2D>("LEFTUPDOWNmap.png"));
            mapTextures.Add("LEFTRIGHTDOWN", leftrightdown = Content.Load<Texture2D>("LEFTRIGHTDOWNmap.png"));
            mapTextures.Add("UPRIGHTDOWN", uprightdown = Content.Load<Texture2D>("UPRIGHTDOWNmap.png"));

            //Enemy textures
            goblinEnemy = Content.Load<Texture2D>("GoblinSpriteSheet.png");
            koboldEnemy = Content.Load<Texture2D>("KoboldSpriteSheet.png");
            ghoulEnemy = Content.Load<Texture2D>("GhoulSpriteSheet.png");


            //Rig environment filter
            whiteSlate = Content.Load<Texture2D>("whiteSlate.png");
            mapTextures.Add("whiteSlate", whiteSlate);
            Atmosphere.Filter = whiteSlate;

            //Loads and plays the music. Can't have it in update or it will keep attempting to play the same track over and over
            //Song is Finding The Balance by Kevin Macleod
            mainMusic = Content.Load<Song>("Music/music.wav");
            bossMusic = Content.Load<Song>("Music/RogueRequiem.wav");
            MediaPlayer.Play(mainMusic);
            MediaPlayer.Volume = (float)(MediaPlayer.Volume * .40);
            MediaPlayer.IsRepeating = true;

            currentSong = "mainMusic";

            manaBarWidth = 300f * player.PercentMP;
            healthBarWidth = 300f * player.PercentHP;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            previousKBS = kbs;
            kbs = Keyboard.GetState();

            if (this.IsActive == false)
            {
                MediaPlayer.Pause();
            }

            else
            {
                MediaPlayer.Resume();
            }

            // TODO: Add your update logic here 
            if (gameState == GameState.MainMenu)
            {
                if (arrowState == ArrowState.menu1)
                {
                    if (SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.Playing;
                        NewLevelGen();
                    }
                }
                if (arrowState == ArrowState.menu1)
                {
                    if (kbs.IsKeyDown(Keys.S))
                    {
                        arrowState = ArrowState.menu2;
                    }
                }
                if (arrowState == ArrowState.menu2)
                {
                    if (SingleKeyPress(Keys.Enter))
                    {
                        Exit();
                    }
                }
                if (arrowState == ArrowState.menu2)
                {
                    if (kbs.IsKeyDown(Keys.W))
                    {
                        arrowState = ArrowState.menu1;
                    }
                }
            }

            if(gameState == GameState.Playing && this.IsActive)
            {
                //Extremely important call to update all active rooms
                UpdateRooms(gameTime);
                
                player.UpdateFrame(gameTime);

                //Set W A S D keys to four different directions
                if (kbs.IsKeyDown(Keys.A))
                {
                    playerMove = "left";
                }
                else if (kbs.IsKeyDown(Keys.D))
                {
                    playerMove = "right";
                }
                if (kbs.IsKeyDown(Keys.S))
                {
                    playerMove = "down";
                }
                else if (kbs.IsKeyDown(Keys.W))
                {
                    playerMove = "up";
                }
                if (camera.isMoving)
                {
                    camera.Update();
                }
                else
                {
                    shifting = false;
                }

                if (player.CurrWeapon == "Sword")
                {
                    playWepState = PlayWepState.Sword;
                }
                if (player.CurrWeapon == "Daggers")
                {
                    playWepState = PlayWepState.Rogue;
                }
                if (player.CurrWeapon == "Bow")
                {
                    playWepState = PlayWepState.Ranger;
                }
                if (player.CurrWeapon == "Staff")
                {
                    playWepState = PlayWepState.Mage;
                }

                //REBUILD LEVEL ****USE FOR DEBUGGING ONLY

                //if (state.IsKeyDown(Keys.R))
                //{
                //    gridSystem = new string[COLUMNS,ROWS];
                //    gridSystem = builderAlpha.BuildLevel(gridSystem, 10);
                //}

                //Updating the player position
                player.Update(camera.screenWidth, camera.screenHeight, camera, gameTime);

                manaBarWidth = 300f * player.PercentMP;
                healthBarWidth = 300f * player.PercentHP;

                if (Game1.CurrProjectiles.Count > 0)
                {
                    removeProj = new List<Projectile>();

                    foreach (Projectile project in Game1.CurrProjectiles)
                    {
                        if ((project.ProjPos.X >= 120 && project.ProjPos.X <= camera.screenWidth - 120) && (project.ProjPos.Y > 120 && project.ProjPos.Y < camera.screenHeight - 120))
                        {
                            project.Update(gameTime);
                        }

                        else
                        {
                            removeProj.Add(project);
                        }

                        if (project.PosRect.Intersects(player.PosRect) && project.Owner != player)
                        {
                            project.OnCollision(player);
                        }

                        foreach (Enemy enemy in activeRoom.enemyList)
                        {
                            if (project.PosRect.Intersects(enemy.PosRect) && project.Owner != enemy)
                            {
                                project.OnCollision(enemy);
                            }
                        }
                    }

                    foreach(Projectile project in removeProj)
                    {
                        Game1.currProjectiles.Remove(project);              
                    }
                }

                if(SingleKeyPress(Keys.Escape))
                {
                    gameState = GameState.paused;
                    arrowState = ArrowState.pos1;
                }

                if(SingleKeyPress(Keys.Tab))
                {
                    gameState = GameState.Context;
                }

                if (SingleKeyPress(Keys.M))
                {
                    Minimap.Visible = !Minimap.Visible;
                }

                if (SingleKeyPress(Keys.R))
                {
                    levelTrigger = true;
                }

                //---- FOR DEBUGGING USE ONLY (Remove) ----
                if (SingleKeyPress(Keys.L))
                {
                    lockRoom = true;
                }

                if (SingleKeyPress(Keys.U))
                {
                    unlockRoom = true;
                }
                //---- END OF DEBUG CODE!!!! ----

                Notification.UpdateAlert();

                if (levelTrigger)
                {
                    NewLevelGen();
                    levelTrigger = false;
                }

            }
            
            //if(gameState == GameState.Playing && !this.IsActive)
            //{
            //    gameState = GameState.paused;
            //    arrowState = ArrowState.pos1;
            //}

            if(gameState == GameState.GameOver)
            {

            }

            if (gameState == GameState.Context)
            {
                if(SingleKeyPress(Keys.Tab))
                {
                    gameState = GameState.Playing;
                }

                /*
                if (contextState == ContextState.Skills)
                {
                    if ((SingleMouseClick()) && (mouseState.X >= 313 && mouseState.X <= 568) && (mouseState.Y >= 478 && mouseState.Y <= 565))
                    {
                        contextState = ContextState.Lore;
                    }
                }

                if (contextState == ContextState.Lore)
                {
                    if ((SingleMouseClick()) && (mouseState.X >= 313 && mouseState.X <= 568) && (mouseState.Y >= 350 && mouseState.Y <= 440))
                    {
                        contextState = ContextState.Skills;
                    }
                }
                */
            }

            if (gameState == GameState.howTo)
            {
                if (SingleKeyPress(Keys.Escape))
                {
                    gameState = GameState.paused;
                }
            }

            if(gameState == GameState.paused)
            {
                //These will check for player input in the menu and adjust accordingly
                if (SingleKeyPress(Keys.Escape))
                {
                    gameState = GameState.Playing;
                }

                if (arrowState == ArrowState.pos1)
                {
                    if (kbs.IsKeyDown(Keys.Enter))
                    {
                        gameState = GameState.Playing;
                    }
                }
                if (arrowState == ArrowState.pos1)
                {
                    if (SingleKeyPress(Keys.S))
                    {
                        arrowState = ArrowState.pos2;
                    }
                }

                if(arrowState == ArrowState.pos2)
                {
                    if (SingleKeyPress(Keys.W))
                    {
                        arrowState = ArrowState.pos1;
                    }
                    if (SingleKeyPress(Keys.S))
                    {
                        arrowState = ArrowState.pos3;
                    }
                }
                if(arrowState == ArrowState.pos2)
                {
                    if (kbs.IsKeyDown(Keys.Enter))
                    {
                        gameState = GameState.howTo;
                    }
                }

                if (arrowState == ArrowState.pos3)
                {
                    if (kbs.IsKeyDown(Keys.Enter))
                    {
                        Exit();
                    }
                }

                if (arrowState == ArrowState.pos3)
                {
                    if (SingleKeyPress(Keys.W))
                    {
                        arrowState = ArrowState.pos2;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            //This draws the main menu
            if (gameState == GameState.MainMenu)
            {
                spriteBatch.Draw(main, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(play, new Vector2(784, 522), Color.White);
                spriteBatch.Draw(exitM, new Vector2(405, 652), Color.White);

                if(arrowState == ArrowState.menu1)
                {
                    spriteBatch.Draw(select, new Vector2(707, 535), Color.White);
                }
                if (arrowState == ArrowState.menu2)
                {
                    spriteBatch.Draw(select, new Vector2(329, 667), Color.White);
                }
            }

            //this is drawn no matter what so even when paused, the game is still "drawn", it will just be "idle"
            if ((this.IsActive) && (gameState != GameState.MainMenu))
            {
                //Extremely important call to draw all active rooms
                DrawRooms();
                
                //Drawing the player
                player.Draw(spriteBatch, playerTextures, 140, 140);

                //Drawing the projectiles on the screen
                if (Game1.CurrProjectiles.Count > 0)
                {
                    foreach (Projectile project in Game1.CurrProjectiles)
                    {
                        project.Draw(spriteBatch, projectileTextures);
                    }
                }

                //Draws HUD Elements
                if (playWepState == PlayWepState.Sword)
                {
                    spriteBatch.Draw(sword, new Vector2(1699, 868), Color.White);
                }
                if (playWepState == PlayWepState.Rogue)
                {
                    spriteBatch.Draw(rogue, new Vector2(1699, 868), Color.White);
                }
                if (playWepState == PlayWepState.Mage)
                {
                    spriteBatch.Draw(mage, new Vector2(1699, 868), Color.White);
                }
                if (playWepState == PlayWepState.Ranger)
                {
                    spriteBatch.Draw(ranger, new Vector2(1699, 868), Color.White);
                }

                spriteBatch.Draw(container, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(health, new Vector2(189, 56), new Rectangle(0, 0, (int)(healthBarWidth), 31), Color.White);
                spriteBatch.Draw(mana, new Vector2(189, 109), new Rectangle(0, 0, (int)(manaBarWidth), 31), Color.White);

                if (Minimap.Visible && !Notification.Updating)
                {
                    Minimap.Draw(camera, spriteBatch, mapTextures, levelAnnex, true);
                }
                if (Notification.Updating)
                {
                    Notification.DrawAlert(spriteBatch);
                }
                
            }

            //draws the following if the game is paused
            if(gameState == GameState.paused && this.IsActive)
            {
                spriteBatch.Draw(pause, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(resume, new Vector2(100, 351), Color.White);
                spriteBatch.Draw(control, new Vector2(100, 426), Color.White);
                spriteBatch.Draw(exit, new Vector2(100, 501), Color.White);

                if (arrowState == ArrowState.pos1)
                {
                    spriteBatch.Draw(select, new Vector2(30, 354), Color.White);
                }
                if (arrowState == ArrowState.pos2)
                {
                    spriteBatch.Draw(select, new Vector2(30, 429), Color.White);
                }
                if (arrowState == ArrowState.pos3)
                {
                    spriteBatch.Draw(select, new Vector2(30, 504), Color.White);
                }
            }

            if(gameState == GameState.howTo)
            {
                spriteBatch.Draw(controls, new Vector2(0, 0), Color.White);
            }

            if (gameState == GameState.GameOver)
            {

            }

            if (gameState == GameState.Context)
            {
                if(contextState == ContextState.Skills)
                {
                    spriteBatch.Draw(contextSkill, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(loreSwitch, new Vector2(313, 474), Color.White);
                }
                if(contextState == ContextState.Lore)
                {
                    spriteBatch.Draw(contextLore, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(skillSwitch, new Vector2(313, 350), Color.White);
                }

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //checks for a single key press
        public bool SingleKeyPress(Keys key)
        {
            if ((previousKBS.IsKeyUp(key)) && (kbs.IsKeyDown(key)))
            {
                previousKBS = kbs;
                kbs = Keyboard.GetState();
                return true;
            }

            else
            {
                return false;
            }
        }

        //checks for a single left click
        public bool SingleMouseClick()
        {
            if (mouseState.LeftButton == ButtonState.Pressed && previouseMS.LeftButton == ButtonState.Released)
            {
                previouseMS = mouseState;
                mouseState = Mouse.GetState();
                return true;
            }

            else
            {
                return false;
            }
        }

        public void UpdateRooms(GameTime gameTime)
        {
            //Start at the beginning of the grid
            int rowIndex = 0;
            int columnIndex = 0;

            //Work across and then down
            while (rowIndex < ROWS)
            {
                while (columnIndex < COLUMNS)
                {
                    //If the grid node is NOT EMPTY it MUST BE FULFILLED
                    if (gridSystem[columnIndex, rowIndex] != null)
                    {
                        //If the camera frame isn't shifting and the oldRow signal is not -1 (default is -1 in the inactive state)
                        if (!shifting && oldRow != -1)
                        {
                            //Set the old room to be inactive
                            levelAnnex[oldCol, oldRow].Active = false;

                            //Reset the old room signals to avoid constant setting to inactive
                            oldRow = -1; 
                            oldCol = -1;
                        }

                        //If the current room is active it should be updated
                        if (levelAnnex[columnIndex, rowIndex].Active)
                        {
                            activeRoom = levelAnnex[columnIndex, rowIndex];

                            if (shifting)
                            {
                                currProjectiles.Clear();
                            }
                            while (levelAnnex[columnIndex, rowIndex].spawnTiles.Count != 0)
                            {
                                levelAnnex[columnIndex, rowIndex].SpawnGoblin(player, goblinEnemy, camera);
                            }

                            //Two birds with one stone; update collisions check and adjust active rooms if necessary
                            //Cannot run check if the frame is shifting
                            if (!shifting)
                            {
                                
                                

                                //Debugger requested a locked room!
                                if (lockRoom)
                                {
                                    levelAnnex[columnIndex, rowIndex].Lock();
                                    lockRoom = false;
                                }
                                else if (unlockRoom)
                                {
                                    levelAnnex[columnIndex, rowIndex].RequestUnlock(player, camera);
                                    unlockRoom = false;
                                }

                                //Update song to be either boss music or roaming music
                                if (levelAnnex[columnIndex, rowIndex].Boss && currentSong != "bossMusic")
                                {
                                    MediaPlayer.Play(bossMusic);                                  
                                    currentSong = "bossMusic";
                                }

                                else if (!levelAnnex[columnIndex, rowIndex].Boss && currentSong != "mainMusic")
                                {
                                    MediaPlayer.Play(mainMusic);
                                    currentSong = "mainMusic";
                                }

                                if(levelAnnex[columnIndex, rowIndex].LevelTrigger)
                                {
                                    levelTrigger = true;
                                }

                                //Update the cursor, which will in turn update tile detection
                                cur.Update(levelAnnex[columnIndex, rowIndex], player);

                                //Check to see if the player is trying to move to another room
                                switch (levelAnnex[columnIndex, rowIndex].UpdateEvents(player, camera, playerMove, gameTime))
                                {
                                    //If the player has indicated that they want to switch to an adjacent room
                                    case ("right"):

                                        //The new room becomes active
                                        levelAnnex[columnIndex + 1, rowIndex].Active = true;
                                        Minimap.UpdatePeripherals(levelAnnex, columnIndex + 1, rowIndex);

                                        //The frame is shifting !!!! HALT ROOM UPDATES UNTIL TRANSITION IS COMPLETE
                                        shifting = true;

                                        //Set the old room coordinates so the old room can become inactive after the frame switch
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;

                                    //Examine the case above; this code operates similarly but with a slightly different situation
                                    case ("left"):

                                        levelAnnex[columnIndex - 1, rowIndex].Active = true;
                                        Minimap.UpdatePeripherals(levelAnnex, columnIndex - 1, rowIndex);
                                        shifting = true;
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;

                                    case ("up"):

                                        levelAnnex[columnIndex, rowIndex - 1].Active = true;
                                        Minimap.UpdatePeripherals(levelAnnex, columnIndex, rowIndex - 1);
                                        shifting = true;
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;

                                    case ("down"):

                                        levelAnnex[columnIndex, rowIndex + 1].Active = true;
                                        Minimap.UpdatePeripherals(levelAnnex, columnIndex, rowIndex + 1);
                                        shifting = true;
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;
                                }

                                foreach (Enemy enemy in levelAnnex[columnIndex, rowIndex].enemyList)
                                {
                                    enemy.UpdateFrame(gameTime);

                                    foreach(Interactable post in levelAnnex[columnIndex, rowIndex].posts)
                                    {
                                        if (enemy.PosRect.Intersects(post.RelativeLocation))
                                        {
                                            enemy.UpdatePathDirection(post.SubType);
                                        }
                                    }

                                    enemy.UpdatePathPosition();
                                    //PathFinderNode node = PathFinder.FindPath(levelAnnex[columnIndex, rowIndex], camera, enemy, player);
                                    //Vector2 travel = new Vector2(node.x, node.y);
                                    //if(travel != Vector2.Zero)
                                    //{
                                    //    travel.Normalize();
                                    //}

                                    //int travelX = (int)(travel.X * enemy.MoveSpeed);
                                    //int travelY = (int)(travel.Y * enemy.MoveSpeed);

                                    //enemy.PosX += travelX;
                                    //enemy.PosY += travelY;

                                    ////foreach(Enemy enemyMoving in levelAnnex[columnIndex, rowIndex].enemyList)
                                    ////{
                                    ////    if(enemyMoving != enemy && enemy.PosRect.Intersects(enemyMoving.PosRect))
                                    ////    {
                                    ////        travelX = 0;
                                    ////        travelY = 0;
                                    ////    }
                                    ////}

                                }
                            }
                        }
                    }
                    columnIndex++;
                }
                //Reset column index after running through each column
                columnIndex = 0;
                rowIndex++;
            }
        }

        //Method for drawing the rooms.
        public void DrawRooms()
        {
            //Start at the beginning of the grid
            int rowIndex = 0;
            int columnIndex = 0;

            //Work across and then down
            while (rowIndex < ROWS)
            {
                while (columnIndex < COLUMNS)
                {
                    //If the room in the level annex is not null and then is active, then we should draw it
                    if (levelAnnex[columnIndex, rowIndex] != null && levelAnnex[columnIndex, rowIndex].Active)
                    {
                        //Get the relative x and y coordinates with which to draw
                        int xCoord = ((camera.screenWidth / 2) + camera.xMod);
                        int yCoord = ((camera.screenHeight / 2) + camera.yMod);

                        xCoord += ((columnIndex - 4) * 1920);
                        yCoord += ((rowIndex - 4) * 1080);

                        //Call the room's draw method which will map out each individual tile
                        levelAnnex[columnIndex, rowIndex].DrawRoom(spriteBatch, tilemap, xCoord, yCoord, camera);
                    }
                    columnIndex++;

                }
                //Reset column index after running through each column
                columnIndex = 0;
                rowIndex++;
            }
        }
        private void NewLevelGen()
        {
            //Run Level Builder! Generate the first level
            gridSystem = new string[COLUMNS, ROWS];

            //Level annex is like the grid system except that it keeps track of the actual initialized rooms
            levelAnnex = new Room[COLUMNS, ROWS];

            builderAlpha = new LevelBuilder();

            numRooms = 35;

            //Initialize a new camera (origin at the center of the screen; dimensions of screen size)
            camera = new Camera(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT, 10);

            //Rig environment port
            Atmosphere.Camera = camera;

            //Rig Notification port
            Notification.camera = camera;

            //Fill the grid with room codes
            gridSystem = builderAlpha.BuildLevel(gridSystem, numRooms);

            int rowIndex = 0;
            int columnIndex = 0;

            possibleBossRooms = new List<Room>();

            //For each space in the grid
            while (rowIndex < ROWS)
            {
                while (columnIndex < COLUMNS)
                {
                    //Attempt to build the room with the specified room code and put it into the level annex
                    levelAnnex = builderAlpha.BuildRoom(gridSystem, levelAnnex, possibleBossRooms, camera, rowIndex, columnIndex);
                    columnIndex++;
                }
                //Reset column index after running through each column
                columnIndex = 0;
                rowIndex++;
            }

            //Pick a boss room
            Random random = new Random();
            possibleBossRooms[random.Next(0, possibleBossRooms.Count)].Boss = true;

            //Update beginning room peripherals - every subsequent room can be updated on the map in a different location
            Minimap.UpdatePeripherals(levelAnnex, levelAnnex.GetLength(0) / 2, levelAnnex.GetLength(1) / 2);

            //Initializing the player
            player = new Player(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, 140, 140);
        }
    }
}
