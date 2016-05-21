using Microsoft.Xna.Framework;
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
        //Since there is no method here to minimize, a region
        //was added to increase visibility
        #region

        //For visibility sake, all of these are too small to be grouped into their own region.
        //Therefore, they're all grouped under one region, but still separated out based on function.
        #region

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        int timer = 0;

        //These variables are used to determine the screensize
        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        //Lore tracking
        Note note;
        Boolean readingNote = false;
        int loreIndex;
        int buffer = 15;
        int windowBuffer = 10;
        int scrollLines = 0;
        List<GameObject> loreEntries = new List<GameObject>();

        //Needed for skill tree stuff
        SkillTree skillTree;
        Boolean released = true;

        //The camera manages the viewport and location of drawn objects
        Camera camera;

        //Declare fonts here
        SpriteFont alertText;

        //HUD related elements
        float manaBarWidth;
        float healthBarWidth;

        #endregion


        //This region is for enums for different states.
        #region

        public enum GameState { MainMenu, Playing, GameOver, paused, howTo, Context }
        static public GameState gameState;

        enum ArrowState { pos1, pos2, pos3, menu1, menu2 }
        ArrowState arrowState;

        enum PlayWepState { Sword, Rogue, Mage, Ranger }
        PlayWepState playWepState;

        enum ContextState { Skills, Lore }
        ContextState contextState;

        enum ControlState { kb, gp }
        ControlState controlState;

        #endregion


        //Room and level related code
        #region

        //Level builder to create and connect rooms
        LevelBuilder builderAlpha;

        //Declare a grid to keep track of level space
        string[,] gridSystem;
        const int ROWS = 9;
        const int COLUMNS = 9;

        //Stage locking / unlocking a room
        Boolean lockRoom = false;
        Boolean unlockRoom = false;
        Boolean transitionToGame = false;
        bool debugMode = false;

        //Declare a number of rooms for a level
        int numRooms;
        //Track room structure via the grid system
        Room[,] levelAnnex;

        //Track active rooms
        Room activeRoom;

        //This flag indicates that enemies have just been spawned (in the current room most likely)
        Boolean done = false;

        //Manage room activity to minimize impact on CPU
        bool shifting;
        int oldRow = -1;
        int oldCol = -1;
        string playerMove = "none";

        //Track potential boss rooms
        List<Room> possibleBossRooms;

        //Keep track of whether or not we need to generate a new level (cannot be done immediately upon assignment)
        private Boolean levelTrigger;

        //Keeps track of whether or not the boss is spawned
        bool bossSpawned;

        #endregion


        //Texture handling       
        #region

        //Declare Room Textures
        Texture2D tilemap, playerTextures, projectileTextures;

        //Declare Minimap Textures
        Texture2D leftuprightdown, down, up, left, right, leftup, leftright, leftdown, upright, updown, rightdown;
        Texture2D leftupright, leftupdown, leftrightdown, uprightdown, blackSlate, whiteSlate, unknown;

        //Declare HUD Textures
        Texture2D pause, exit, resume, select, control, controls, mage, ranger, sword, rogue, back, main, sky, play, exitM, mana, health, container, controlsGP;
        Texture2D contextSkill, contextLore, skillSwitch, loreSwitch, skillMage, skillRogue, skillWarrior, skillRanger, loreEntry;

        //Declare Notification Tool Textures
        Texture2D diagonalBar;

        //Enemy textures
        Texture2D goblinEnemy;
        Texture2D koboldEnemy;
        Texture2D ghoulEnemy;

        //Handle Minimap Textures:
        Dictionary<string, Texture2D> mapTextures = new Dictionary<string, Texture2D>();

        #endregion


        //Input handling
        #region

        //Input device states
        KeyboardState previousKBS, kbs;
        GamePadState prevGPadState, gPadState;
        MouseState previousMS, mouseState;

        //Other needed things for handling input
        Cursor cur;
        float deadZone;
        Vector2 leftStickInput, prevLeftStickInput;

        #endregion


        //Music related code
        #region

        //Bool for music switch
        Boolean musicTransition = false;
        string oldSong = "";

        //Songs for music
        Song mainMusic, bossMusic, transientLoop, castleTheme, oasis, battleTheme;

        //Keep track of current song
        string currentSong;

        #endregion


        //Projectile related things.
        #region

        //List to keep track of projectiles
        private static List<Projectile> currProjectiles;
        private static List<Projectile> removeProj;
        private static List<Projectile> addProj;

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

        public static List<Projectile> AddProj
        {
            get { return addProj; }
            set { addProj = value; }
        }

        #endregion

        #endregion


        //Constructor
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

            //Initializing the gamestate
            gameState = GameState.MainMenu;
            arrowState = ArrowState.menu1;
            playWepState = PlayWepState.Sword;
            contextState = ContextState.Skills;

            //Initializing the Cursor
            cur = new Cursor();

            //Initialize a new camera (origin at the center of the screen; dimensions of screen size)
            camera = new Camera(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT, 10);

            //Rig environment port
            Atmosphere.Camera = camera;

            //Setting up other things needed for the game to run
            currProjectiles = new List<Projectile>();
            deadZone = .25f;
            bossSpawned = false;
            Notes.GatherNotes();

            base.Initialize();
        }

       
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here

            //For visibility sake, all of these are too small to be grouped into their own region.
            //Therefore, they're all grouped under one region, but still separated out based on function.
            #region

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //player.Texture = Content.Load<Texture2D>("Player");

            //Load fonts here
            alertText = Content.Load<SpriteFont>("alertText");

            //Notification Textures
            diagonalBar = Content.Load<Texture2D>("DiagonalNotificationBar.png");

            //Rig font and texture to Notification system
            Notification.font = alertText;
            Notification.diagonalBar = diagonalBar;

            //Load in tilemap and player spritesheet
            tilemap = Content.Load<Texture2D>("Tileset.png");
            playerTextures = Content.Load <Texture2D>("PlayerAnims.png");
            projectileTextures = Content.Load<Texture2D>("Projectile Spritesheet.png");

            //Rig environment filter
            whiteSlate = Content.Load<Texture2D>("whiteSlate.png");
            mapTextures.Add("whiteSlate", whiteSlate);
            Atmosphere.Filter = whiteSlate;

            #endregion


            //Texture initialization for the HUD and Minimap
            #region

            //Initialize HUD textures
            pause = Content.Load<Texture2D>("HUDStuff/HUDpause.png");
            exit = Content.Load<Texture2D>("HUDStuff/HUDexitmain.png");
            select = Content.Load<Texture2D>("HUDStuff/HUDselect.png");
            resume = Content.Load<Texture2D>("HUDStuff/HUDresume.png");
            control = Content.Load<Texture2D>("HUDStuff/HUDcontrol.png");
            controls = Content.Load<Texture2D>("HUDStuff/HUDcontrols.png");
            controlsGP = Content.Load<Texture2D>("HUDStuff/HUDcontrolsgp.png");
            sword = Content.Load<Texture2D>("HUDStuff/HUDsword.png");
            rogue = Content.Load<Texture2D>("HUDStuff/HUDrogue.png");
            mage = Content.Load<Texture2D>("HUDStuff/HUDmage.png");
            ranger = Content.Load<Texture2D>("HUDStuff/HUDranger.png");
            back = Content.Load<Texture2D>("HUDStuff/HUDback.png");
            main = Content.Load<Texture2D>("HUDStuff/MainMenuBG.png");
            sky = Content.Load<Texture2D>("HUDStuff/Sky.png");
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
            loreEntry = Content.Load<Texture2D>("HUDStuff/LoreEntry.png");

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

            #endregion


            //Music handling
            #region
    
            //Loads and plays the music. Can't have it in update or it will keep attempting to play the same track over and over
            mainMusic = Content.Load<Song>("Music/A Hero is Born.wav");
            bossMusic = Content.Load<Song>("Music/RogueRequiem.wav");
            transientLoop = Content.Load<Song>("Music/Transient Loop.wav");
            castleTheme = Content.Load<Song>("Music/Castle Theme.wav");
            oasis = Content.Load<Song>("Music/Oasis2.wav");
            battleTheme = Content.Load<Song>("Music/BattleTheme.wav");
            MediaPlayer.Play(mainMusic);
            MediaPlayer.Volume = (float)(MediaPlayer.Volume * .40);
            MediaPlayer.IsRepeating = true;
            currentSong = "mainMusic";

            #endregion

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
            //For visibility sake, all of these are too small to be grouped into their own region.
            //Therefore, they're all grouped under one region, but still separated out based on function.
            #region

            //Handles all the input
            previousKBS = kbs;
            kbs = Keyboard.GetState();
            prevGPadState = gPadState;
            gPadState = GamePad.GetState(PlayerIndex.One);
            leftStickInput = new Vector2(gPadState.ThumbSticks.Left.X, gPadState.ThumbSticks.Left.Y);
            prevLeftStickInput = new Vector2(prevGPadState.ThumbSticks.Left.X, prevGPadState.ThumbSticks.Left.Y);
            
            //Upon pressing N, the game runs in windowed mode
            if(SingleKeyPress(Keys.N) && Window.IsBorderless)
            {
                Window.IsBorderless = false;
            }

            //If the game is not active, then music is paused
            if (this.IsActive == false)
            {
                MediaPlayer.Pause();
            }
            else
            {
                MediaPlayer.Resume();
            }
            #endregion


            //Code for the main menu
            #region

            //If the game is on the main menu
            if (gameState == GameState.MainMenu)
            {
                //For visibility sake, all of these are too small to be grouped into their own region.
                //Therefore, they're all grouped under one region, but still separated out based on function.
                #region

                //updates the timer and gets mouse input info
                timer += gameTime.ElapsedGameTime.Milliseconds;
                mouseState = Mouse.GetState();

                //Updates player info
                if(player == null)
                {
                    player = new Player(400, 750, 140, 140);
                }
                else
                {
                    player.UpdateFrame(gameTime);
                }

                #endregion


                //checks for user input
                #region

                if (((arrowState == ArrowState.menu1) && SingleKeyPress(Keys.Enter)) || ((mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 784 && mouseState.X <= 1117) && (mouseState.Y >= 522 && mouseState.Y <= 598))) || ((arrowState == ArrowState.menu1) && gPadState.IsButtonDown(Buttons.A)))
                {
                    transitionToGame = true;
                }
                if ((arrowState == ArrowState.menu1) && kbs.IsKeyDown(Keys.S) || (mouseState.X >= 405 && mouseState.X <= 1531) && (mouseState.Y >= 652 && mouseState.Y <= 728) || ((arrowState == ArrowState.menu1) && SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y > deadZone))
                {
                    arrowState = ArrowState.menu2;
                }
                if (((arrowState == ArrowState.menu2) && SingleKeyPress(Keys.Enter)) || ((mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 405 && mouseState.X <= 1531) && (mouseState.Y >= 652 && mouseState.Y <= 728))) || ((arrowState == ArrowState.menu2) && gPadState.IsButtonDown(Buttons.A)))
                {
                    Exit();
                }
                if ((arrowState == ArrowState.menu2) && kbs.IsKeyDown(Keys.W) || (mouseState.X >= 784 && mouseState.X <= 1117) && (mouseState.Y >= 522 && mouseState.Y <= 598) || ((arrowState == ArrowState.menu2) && SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y < -deadZone))
                {
                    arrowState = ArrowState.menu1;   
                }

                #endregion


                //music handling
                #region

                if(currentSong == "mainMusic" && timer > 48000)
                {
                    if (Atmosphere.Intensity < 0.3)
                    {
                        Atmosphere.IncreaseIntensity();
                    }
                    else if (Atmosphere.SecondIntensity < 0.7)
                    {
                        Atmosphere.IncreaseSecondIntensity();
                    }

                    if(timer > 72000)
                    {
                        if (Atmosphere.ThirdIntensity < 0.8)
                        {
                            Atmosphere.IncreaseThirdIntensity();

                        }
                    }
                    if(timer > 100000)
                    {
                        currentSong = "transientLoop";
                        MediaPlayer.Play(transientLoop);
                        MediaPlayer.IsRepeating = true;
                    }   
                }

                #endregion


                //Handles player animation during opening sequence
                #region

                if (transitionToGame)
                {
                    if (player.PosX < 1170)
                    {
                        if (player.PlayerStates == Player.PlayerState.IdleRight)
                        {
                            player.PlayerStates = Player.PlayerState.WalkingRight;
                        }

                        player.PosX += 3;
                    }
                    else
                    {
                        if (player.PlayerStates == Player.PlayerState.WalkingRight)
                        {
                            player.PlayerStates = Player.PlayerState.IdleRight;
                        }

                        if (player.colorPhase > 0)
                        {
                            player.color = Color.White * player.colorPhase;
                            player.colorPhase -= (float)0.05;
                        }
                        else
                        {
                            player.color = Color.Transparent;
                        }
                    }

                    if (player.color == Color.Transparent && Atmosphere.Intensity < 1)
                    {
                        Atmosphere.RapidIncreaseIntensity();
                        if(MediaPlayer.Volume > 0)
                        {
                            MediaPlayer.Volume -= (float) 0.01;
                        }                       
                    }
                    else if (player.color == Color.Transparent)
                    {
                        gameState = GameState.Playing;
                        CurrProjectiles.Clear();
                        MediaPlayer.Play(castleTheme);
                        MediaPlayer.Volume = 0.4f;
                        currentSong = "CastleTheme";
                        oldSong = currentSong;
                        MediaPlayer.IsRepeating = false;
                        NewLevelGen(true);
                        Notification.Alert("Press Tab to spend a skill point!", Color.Purple, 60, false);
                    }
                }

                #endregion

            }

            #endregion


            //Code for while the game is active and playing
            #region

            if(gameState == GameState.Playing && this.IsActive)
            {
                //For visibility sake, all of these are too small to be grouped into their own region.
                //Therefore, they're all grouped under one region, but still separated out based on function.
                #region

                //sets atmospheric intensities
                if (Atmosphere.Intensity > 0)
                {
                    Atmosphere.AnimateResetIntensities();
                }
                else
                {
                    transitionToGame = false;
                }


                //Music handling
                if (!musicTransition & activeRoom != null)
                {
                    if(activeRoom.enemyList.Count != 0 && !activeRoom.Boss)
                    {
                        UpdateSongBattle();
                    }
                    else if(activeRoom.enemyList.Count == 0)
                    {
                        UpdateSongPassive(gameTime);
                    }
                }
                else if(activeRoom != null)
                {
                    SongTransition();
                }
       

                //Extremely important call to update all active rooms
                UpdateRooms(gameTime);
                

                //Update the player's animation
                player.UpdateFrame(gameTime);


                //Update the camera if it is moving (transitions between rooms)
                if (camera.isMoving)
                {
                    camera.Update();
                }
                else
                {
                    shifting = false;
                }


                //Updating the player position
                player.Update(camera.screenWidth, camera.screenHeight, camera, gameTime);

                manaBarWidth = 300f * player.PercentMP;
                healthBarWidth = 300f * player.PercentHP;


                //Make sure the notifications are getting updated properly
                Notification.UpdateAlert();


                //If a call has been made to build a new level
                if (levelTrigger)
                {
                    //Call the new level generator
                    NewLevelGen(false);

                    //Set the new level trigger to be false to avoid further unintended calls
                    levelTrigger = false;
                }


                //If the player loses all of their health, move them to the game over screen
                if (player.CurrHealth <= 0)
                {
                    gameState = GameState.GameOver;
                }


                //Sets player weapon state
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
                #endregion


                //Projectile handling
                #region
                //Handling projectiles
                if (Game1.CurrProjectiles.Count > 0)
                {
                    removeProj = new List<Projectile>();
                    addProj = new List<Projectile>();

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

                        if (project.PosRect.Intersects(player.HitBox) && project.Owner != player)
                        {
                            if((project.PosX + project.PosRect.Width <= player.HitBoxX + 20 && player.IsBlocking && player.Direction == "left") || (project.PosX >= (player.HitBoxX - player.HitBox.Width) + 20 && player.IsBlocking && player.Direction == "right"))
                            {
                                project.Damage = project.Damage - player.BlockAmount;
                                if(project.Damage < 0)
                                {
                                    project.Damage = 0;
                                }

                                project.OnCollision(player);
                            }
                            else
                            {
                                project.OnCollision(player);
                            }
                        }

                        foreach (Interactable interact in activeRoom.interactables)
                        {
                            interact.HandleCollisions(project, camera);
                        }

                        foreach (Enemy enemy in activeRoom.enemyList)
                        {
                            if (project.PosRect.Intersects(enemy.PosRect) && (project.Owner == player || (project.isCombo && project.Owner != enemy)))
                            {
                                project.OnCollision(enemy);
                            }
                        }
                    }

                    foreach(Projectile project in removeProj)
                    {
                        Game1.currProjectiles.Remove(project);              
                    }

                    foreach(Projectile project in addProj)
                    {
                        Game1.currProjectiles.Add(project);
                    }

                    addProj.Clear();
                }

                #endregion


                //User input handling
                #region

                //Set W A S D keys to four different directions
                if (kbs.IsKeyDown(Keys.A) || leftStickInput.X < -player.deadZone)
                {
                    playerMove = "left";
                }
                else if (kbs.IsKeyDown(Keys.D) || leftStickInput.X > player.deadZone)
                {
                    playerMove = "right";
                }
                if (kbs.IsKeyDown(Keys.S) || leftStickInput.Y > player.deadZone)
                {
                    playerMove = "down";
                }
                else if (kbs.IsKeyDown(Keys.W) || leftStickInput.Y < -player.deadZone)
                {
                    playerMove = "up";
                }


                //Death button
                if (kbs.IsKeyDown(Keys.K) /*|| gPadState.IsButtonDown(Buttons.Back)*/)
                {
                    player.CurrHealth = 0;
                }


                if (SingleKeyPress(Keys.Escape) || gPadState.IsButtonDown(Buttons.Start))
                {
                    gameState = GameState.paused;
                    arrowState = ArrowState.pos1;
                }

                if(SingleKeyPress(Keys.Tab) || SingleKeyPress(Buttons.Back))
                {
                    gameState = GameState.Context;
                    contextState = ContextState.Skills;
                }


                //If M is pressed, toggle the visibility of the minimap
                if (SingleKeyPress(Keys.M) || SingleKeyPress(Buttons.Y))
                {
                    Minimap.Visible = !Minimap.Visible;
                }


                //If R is pressed, build a new level on the spot
                if (SingleKeyPress(Keys.R))
                {
                    levelTrigger = true;
                }


                //Debug mode
                if (SingleKeyPress(Keys.P))
                {
                    debugMode = !debugMode;
                }


                //---- FOR DEBUGGING USE ONLY (Remove) ----
                //If L is pressed, initiate the room locking procedure
                if (SingleKeyPress(Keys.L))
                {
                    lockRoom = true;
                }
                //If U is pressed, unlock the room
                if (SingleKeyPress(Keys.U))
                {
                    unlockRoom = true;
                }
                //---- END OF DEBUG CODE!!!! ----

                #endregion
            }

            #endregion
            

            //Code for game over
            #region

            //If you hit enter on the Game over screen it sends you to the main menu
            if (gameState == GameState.GameOver)
            {
                kbs = Keyboard.GetState();
                if (kbs.IsKeyDown(Keys.Enter) || gPadState.IsButtonDown(Buttons.Start))
                {
                    gameState = GameState.MainMenu;
                    arrowState = ArrowState.menu1;
                    player = null;
                    Atmosphere.ResetIntensities();
                    timer = 0;
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(mainMusic);
                    currentSong = "mainMusic";
                    transitionToGame = false;
                    MediaPlayer.Volume = (float)0.40;
                }
            }

            #endregion


            //Code for the context menu
            #region

            if (gameState == GameState.Context)
            {
                //For visibility sake, all of these are too small to be grouped into their own region.
                //Therefore, they're all grouped under one region, but still separated out based on function.
                #region

                //Music handling
                if (currentSong != "Oasis")
                {
                    MediaPlayer.Play(oasis);
                    MediaPlayer.IsRepeating = true;
                    currentSong = "Oasis";
                }


                //Checks button input while in context menu
                if(SingleKeyPress(Keys.Tab) || SingleKeyPress(Keys.Escape) || SingleKeyPress(Buttons.Back) || SingleKeyPress(Buttons.B))
                {
                    if (readingNote)
                    {
                        note = null;
                        readingNote = false;
                    }
                    else
                    {
                        gameState = GameState.Playing;
                    }   
                }

                #endregion


                //Skill menu code
                #region

                //If the context menu is on the skills menu
                if (contextState == ContextState.Skills)
                {
                    //retrieve the mouse state
                    mouseState = Mouse.GetState();


                    if(mouseState.LeftButton == ButtonState.Released)
                    {
                        released = true;
                    }


                    //checks for clicking on the "lore" button
                    if (mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 313 && mouseState.X <= 568) && (mouseState.Y >= 478 && mouseState.Y <= 565) || (SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y > deadZone))
                    {
                        contextState = ContextState.Lore;
                    }
                    else if (mouseState.LeftButton == ButtonState.Pressed && released)
                    {
                        if(skillTree.UpdateButtons(player, new Rectangle(mouseState.Position, new Point(1, 1))))
                        {
                            released = false;
                        }
                    }
                }

                #endregion


                //Lore menu code
                #region

                //If the context menu is on the lore menu
                if (contextState == ContextState.Lore)
                {
                    //retrieve the mouse state
                    mouseState = Mouse.GetState();


                    //checks for clicking on the "skills" button
                    if (mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 313 && mouseState.X <= 568) && (mouseState.Y >= 350 && mouseState.Y <= 440) || (SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y < -deadZone))
                    {
                        contextState = ContextState.Skills;
                        readingNote = false;
                        note = null;
                        scrollLines = 0;
                    }
                    else if (mouseState.LeftButton == ButtonState.Pressed && !readingNote)
                    {
                        foreach(GameObject entry in loreEntries)
                        {
                            if(new Rectangle(mouseState.Position.X, mouseState.Position.Y, 1, 1).Intersects(entry.PosRect))
                            {
                                int index = loreIndex + loreEntries.IndexOf(entry);
                                note = Notes.gatheredNotes[index];
                                note.read = true;
                                Notes.gatheredNotes.RemoveAt(index);
                                Notes.gatheredNotes.Add(note);
                                readingNote = true;
                            }
                        }
                    }


                    //handles scrolling on a piece of lore
                    if(SingleKeyPress(Keys.Up))
                    {
                        if(scrollLines < 10)
                        {
                            scrollLines++;
                        }
                    }
                    else if(SingleKeyPress(Keys.Down))
                    {
                        if(scrollLines > 0)
                        {
                            scrollLines--;
                        }
                    }
                }

                #endregion


            }

            #endregion


            //Code for the controls menu
            #region

            if (gameState == GameState.howTo)
            {
                //retrieve mouse state
                mouseState = Mouse.GetState();


                //if the control menu shows keyboard controls
                if (controlState == ControlState.kb)
                {
                    //exits to pause menu when escape is pressed
                    if (SingleKeyPress(Keys.Escape) || SingleKeyPress(Buttons.B))
                    {
                        gameState = GameState.paused;
                    }


                    //detects clicking on a certain region to change controls from keyboard to gamepad
                    if ((mouseState.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released) && (mouseState.X >= 858 && mouseState.X <= 1025) && (mouseState.Y >= 619 && mouseState.Y <= 723))
                    {
                        controlState = ControlState.gp;
                    }


                    //resets the old mouse state
                    previousMS = mouseState;
                }


                //if the control menu shows gamepad controls
                if (controlState == ControlState.gp)
                {
                    //exits to pause menu when escape is pressed
                    if (SingleKeyPress(Keys.Escape) || SingleKeyPress(Buttons.B))
                    {
                        gameState = GameState.paused;
                    }


                    //detects clicking on a certain region to change controls from gamepad to keyboard
                    if ((mouseState.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released) && (mouseState.X >= 885 && mouseState.X <= 1014) && (mouseState.Y >= 694 && mouseState.Y <= 765))
                    {
                        controlState = ControlState.kb;
                    }


                    //resets the old mouse state
                    previousMS = mouseState;
                }
            }

            #endregion


            //Code for the pause menu
            #region

            //if the game is paused
            if (gameState == GameState.paused)
            {
                //For visibility sake, all of these are too small to be grouped into their own region.
                //Therefore, they're all grouped under one region, but still separated out based on function.
                #region

                //retrieve mouse state
                mouseState = Mouse.GetState();


                //resumes game
                if (SingleKeyPress(Keys.Escape) || SingleKeyPress(Buttons.B))
                {
                    gameState = GameState.Playing;
                }

                #endregion


                //handles user input in the menu
                #region

                if ((arrowState == ArrowState.pos1) && (kbs.IsKeyDown(Keys.Enter)) || ((mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 100 && mouseState.X <= 283) && (mouseState.Y >= 351 && mouseState.Y <= 405))) || (arrowState == ArrowState.pos1) && (gPadState.IsButtonDown(Buttons.A)) || (arrowState == ArrowState.pos1) && (gPadState.IsButtonDown(Buttons.A)))
                {
                    gameState = GameState.Playing;
                }
                if ((arrowState == ArrowState.pos1) && (SingleKeyPress(Keys.S)) || (((mouseState.X >= 100 && mouseState.X <= 338) && (mouseState.Y >= 426 && mouseState.Y <= 480))) || (arrowState == ArrowState.pos1) && (SingleKeyPress(Keys.S)) || ((arrowState == ArrowState.pos1) && SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y > deadZone))
                {
                    arrowState = ArrowState.pos2;
                }
                else if(arrowState == ArrowState.pos2)
                {
                    if (SingleKeyPress(Keys.W) || (mouseState.X >= 100 && mouseState.X <= 283) && (mouseState.Y >= 351 && mouseState.Y <= 398) || (SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y < -deadZone))
                    {
                        arrowState = ArrowState.pos1;
                    }
                    if (SingleKeyPress(Keys.S) || (mouseState.X >= 100 && mouseState.X <= 598) && (mouseState.Y >= 501 && mouseState.Y <= 555) || (SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y > deadZone))
                    {
                        arrowState = ArrowState.pos3;
                    }
                }
                if((arrowState == ArrowState.pos2) && (kbs.IsKeyDown(Keys.Enter)) || ((mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 100 && mouseState.X <= 338) && (mouseState.Y >= 426 && mouseState.Y <= 480))) || ((arrowState == ArrowState.pos2) && (gPadState.IsButtonDown(Buttons.A))))
                {
                    gameState = GameState.howTo;
                }
                if ((arrowState == ArrowState.pos3) && (kbs.IsKeyDown(Keys.Enter)) || ((mouseState.LeftButton == ButtonState.Pressed && (mouseState.X >= 100 && mouseState.X <= 598) && (mouseState.Y >= 501 && mouseState.Y <= 555))) || ((arrowState == ArrowState.pos3) && (gPadState.IsButtonDown(Buttons.A))))
                {
                    gameState = GameState.MainMenu;
                    arrowState = ArrowState.menu1;
                    player = null;
                    Atmosphere.ResetIntensities();
                    timer = 0;
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(mainMusic);
                    currentSong = "mainMusic";
                    transitionToGame = false;
                    MediaPlayer.Volume = (float) 0.40;
                }
                if ((arrowState == ArrowState.pos3) && (SingleKeyPress(Keys.W)) || (mouseState.X >= 100 && mouseState.X <= 338) && (mouseState.Y >= 426 && mouseState.Y <= 480) || ((arrowState == ArrowState.pos3) && SingleGamePadMove(prevLeftStickInput, leftStickInput) && leftStickInput.Y < -deadZone))
                {
                    arrowState = ArrowState.pos2;
                }

                if((mouseState.X >= 100 && mouseState.X <= 283) && (mouseState.Y >= 351 && mouseState.Y <= 398))
                {
                    arrowState = ArrowState.pos1;
                }
                if((mouseState.X >= 100 && mouseState.X <= 338) && (mouseState.Y >= 426 && mouseState.Y <= 480))
                {
                    arrowState = ArrowState.pos2;
                }
                if((mouseState.X >= 100 && mouseState.X <= 598) && (mouseState.Y >= 501 && mouseState.Y <= 555))
                {
                    arrowState = ArrowState.pos3;
                }

                #endregion

            }

            #endregion


            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //clear and begin drawing
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            
            //This draws the main menu
            #region

            if (gameState == GameState.MainMenu)
            {
                //draw the sky
                spriteBatch.Draw(sky, new Vector2(0, 0), Color.White);


                //draw based on atmospheric intensity
                if (Atmosphere.ThirdIntensity > 0)
                {
                    Atmosphere.AmberDarkPurpleTransition(spriteBatch, 0, 0);
                }
                else if (Atmosphere.SecondIntensity > 0)
                {
                    Atmosphere.AmberPurpleTransition(spriteBatch, 0, 0);
                }
                else
                {
                    Atmosphere.AmberTransition(spriteBatch, 0, 0);
                }


                //draw the menu buttons
                spriteBatch.Draw(main, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(play, new Vector2(784, 522), Color.White);
                spriteBatch.Draw(exitM, new Vector2(405, 652), Color.White);


                //draw the selector arrwos
                if(arrowState == ArrowState.menu1)
                {
                    spriteBatch.Draw(select, new Vector2(707, 535), Color.White);
                }
                if (arrowState == ArrowState.menu2)
                {
                    spriteBatch.Draw(select, new Vector2(329, 667), Color.White);
                }


                //draw the player if it has not been drawn yet
                if(player != null)
                {
                    player.Draw(spriteBatch, playerTextures, 140, 140);
                }

                Atmosphere.Darken(spriteBatch, 0, 0);
                
            }
            #endregion


            //this is drawn as long as the game is not on the main menu and it is not over
            #region

            if ((this.IsActive) && (gameState != GameState.MainMenu && gameState != GameState.GameOver))
            {
                //For visibility sake, all of these are too small to be grouped into their own region.
                //Therefore, they're all grouped under one region, but still separated out based on function.
                #region

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


                //If there is a notification to update
                if (Notification.Updating)
                {
                    //Draw the notification on screen
                    Notification.DrawAlert(spriteBatch);
                }


                //Opening animation
                if (transitionToGame)
                {
                    Atmosphere.Darken(spriteBatch, 0, 0);
                }
                #endregion


                //Draws HUD stuff
                #region

                //Makes this translucent if player is in the area
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


                //Draw HUD health and mana bar
                if (player.PosX < 606 && player.PosY < 270)
                {
                    //draws it translucent if player is in that area on the screen
                    spriteBatch.Draw(container, new Vector2(0, 0), (Color.White) * 0.25f);
                    spriteBatch.Draw(health, new Vector2(189, 56), new Rectangle(0, 0, (int)(healthBarWidth), 31), (Color.White) * 0.25f);
                    spriteBatch.Draw(mana, new Vector2(189, 109), new Rectangle(0, 0, (int)(manaBarWidth), 31), (Color.White) * 0.25f);
                }
                else
                {
                    spriteBatch.Draw(container, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(health, new Vector2(189, 56), new Rectangle(0, 0, (int)(healthBarWidth), 31), Color.White);
                    spriteBatch.Draw(mana, new Vector2(189, 109), new Rectangle(0, 0, (int)(manaBarWidth), 31), Color.White);
                }

                //Drawn only if the gamestate is playing
                if (gameState == GameState.Playing)
                {
                    if (player.PosX < 606 && player.PosY < 270)
                    {
                        //Allow the player to see their current equipped skill and current soul count
                        spriteBatch.DrawString(alertText, "Current Skill: " + player.CurrSkill.ToString(), new Vector2(169, 140), (Color.White) * 0.25f);
                        spriteBatch.DrawString(alertText, "Level: " + player.Level, new Vector2(169, 180), (Color.White) * 0.25f);
                        spriteBatch.DrawString(alertText, "Souls: " + player.Souls + "/" + player.SoulsNeeded, new Vector2(169, 220), (Color.White) * 0.25f);
                    }
                    else
                    {
                        spriteBatch.DrawString(alertText, "Current Skill: " + player.CurrSkill.ToString(), new Vector2(169, 140), Color.White);
                        spriteBatch.DrawString(alertText, "Level: " + player.Level, new Vector2(169, 180), Color.White);
                        spriteBatch.DrawString(alertText, "Souls: " + player.Souls + "/" + player.SoulsNeeded, new Vector2(169, 220), Color.White);
                    }
                }
                #endregion


                //Draws the minimap
                #region

                //If the minimap should be visible and there isn't a notification onscreen
                if (Minimap.Visible && !Notification.Updating)
                {
                    //Draw the minimap based on the current level
                    //Debug mode currently == true
                    if(debugMode == true)
                    {
                        Minimap.Draw(camera, spriteBatch, mapTextures, levelAnnex, true);
                    }

                    else
                    {
                        Minimap.Draw(camera, spriteBatch, mapTextures, levelAnnex, false);
                    }
                }
                #endregion
            }
            #endregion


            //draws the following if the game is paused
            #region

            if(gameState == GameState.paused && this.IsActive)
            {
                //draws the buttons
                spriteBatch.Draw(pause, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(resume, new Vector2(100, 351), Color.White);
                spriteBatch.Draw(control, new Vector2(100, 426), Color.White);
                spriteBatch.Draw(exit, new Vector2(100, 501), Color.White);


                //draws the selector arrow
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
            #endregion


            //draws if the game is on the controls page
            #region

            if (gameState == GameState.howTo)
            {
                //draws the keyboard controls
                if (controlState == ControlState.kb)
                {
                    spriteBatch.Draw(controls, new Vector2(0, 0), Color.White);
                }


                //draws the gamepad controls
                if (controlState == ControlState.gp)
                {
                    spriteBatch.Draw(controlsGP, new Vector2(0, 0), Color.White);
                }
            }
            #endregion


            //draws if the game is over
            #region

            if (gameState == GameState.GameOver)
            {
                spriteBatch.DrawString(alertText, "Level(s) acheived: " + player.Level, new Vector2(740, 400), Color.White);
                spriteBatch.DrawString(alertText, "Collected Souls: " + player.Souls, new Vector2(750, 500), Color.White);
                spriteBatch.DrawString(alertText, "Press Enter/Start to Continue...", new Vector2(600, 600), Color.White);
            }
            #endregion


            //draws if the game is on the context menu
            #region

            if (gameState == GameState.Context)
            {
                //if the context menu is on the skill menu
                #region
                if(contextState == ContextState.Skills)
                {
                    //draw the skill tree and some buttons
                    spriteBatch.Draw(contextSkill, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(loreSwitch, new Vector2(313, 474), Color.White);
                    skillTree.DrawButtons(spriteBatch, alertText, 6, 600, 200);
                    spriteBatch.DrawString(alertText, "Skill Points To Spend: " + player.SkillPoints, new Vector2(600, 80), Color.White);
                }
                #endregion


                //if the context menu is on the lore menu
                #region

                if(contextState == ContextState.Lore)
                {
                    //draw the lore menu and some buttons
                    spriteBatch.Draw(contextLore, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(skillSwitch, new Vector2(313, 350), Color.White);


                    //if you are not reading any notes
                    #region
                    if (!readingNote)
                    {
                        loreEntries.Clear();

                        for (int i = loreIndex; i < Notes.gatheredNotes.Count; i++)
                        {
                            int pos = i - loreIndex;

                            Note currentNote = Notes.gatheredNotes[i];
                            string title = currentNote.title;

                            if (currentNote.read)
                            {
                                spriteBatch.Draw(loreEntry, new Vector2(SCREEN_WIDTH - loreEntry.Width - 10, pos * loreEntry.Height + pos * buffer + windowBuffer), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(loreEntry, new Vector2(SCREEN_WIDTH - loreEntry.Width - 10, pos * loreEntry.Height + pos * buffer + windowBuffer), Color.Gold);
                            }

                            //Center the text so that the anchor point is the center of the message
                            Vector2 measuredString = alertText.MeasureString(title);
                            Vector2 position = new Vector2(SCREEN_WIDTH - (loreEntry.Width / 2), pos * loreEntry.Height + pos * buffer + windowBuffer + (loreEntry.Height / 2));
                            Vector2 origin = measuredString * 0.5f;

                            spriteBatch.DrawString(alertText, title, position, Color.White, 0, origin, 1, SpriteEffects.None, 1);

                            GameObject entry = new GameObject();
                            int x = SCREEN_WIDTH - loreEntry.Width - 10;
                            int y = pos * loreEntry.Height + pos * buffer + windowBuffer;
                            entry.PosRect = new Rectangle(x, y, loreEntry.Width, loreEntry.Height);
                            loreEntries.Add(entry);
                        }
                    }
                    #endregion


                    //if you are currently reading a note
                    #region

                    else
                    {
                        int lineNumber = 0;
                
                        foreach(string line in note.message)
                        {
                            List<string> lines = new List<string>();
                            int startChar = 0;
                            int numChars = 0;

                            for(numChars = 0; numChars < line.Length; numChars++)
                            {
                                if(numChars % 80 == 0 && numChars != 0)
                                {
                                    if (lineNumber < scrollLines)
                                    {
                                        lineNumber++;
                                        startChar = numChars;
                                        continue;
                                    }

                                    if (lineNumber - scrollLines > 18)
                                    {
                                        break;
                                    }
                                    string newLine = line.Substring(startChar, 80);

                                    if(newLine[79] != ' ' && line[numChars] != ' ')
                                    {
                                        newLine += '-';
                                    }

                                    //Center the text so that the anchor point is the center of the message
                                    Vector2 stringLength = alertText.MeasureString(newLine);
                                    Vector2 pos = new Vector2(SCREEN_WIDTH - (loreEntry.Width / 2), windowBuffer + alertText.LineSpacing + (lineNumber - scrollLines) * alertText.LineSpacing * 0.8f);
                                    Vector2 org = stringLength * 0.5f;

                                    spriteBatch.DrawString(alertText, newLine, pos, Color.White, 0, org, 0.8f, SpriteEffects.None, 1);

                                    startChar = numChars;

                                    lineNumber++;
                                }
                            }

                            if (lineNumber < scrollLines)
                            {
                                lineNumber++;
                                continue;
                            }
                            if (lineNumber - scrollLines > 18)
                            {
                                break;
                            }

                            if (startChar == 0)
                            {
                                //Center the text so that the anchor point is the center of the message
                                Vector2 measuredString = alertText.MeasureString(line);
                                Vector2 position = new Vector2(SCREEN_WIDTH - (loreEntry.Width / 2), windowBuffer + alertText.LineSpacing + (lineNumber - scrollLines) * alertText.LineSpacing * 0.8f);
                                Vector2 origin = measuredString * 0.5f;

                                spriteBatch.DrawString(alertText, line, position, Color.White, 0, origin, 0.8f, SpriteEffects.None, 1);
                                lineNumber++;
                            }  
                            else if(numChars % 80 != 0)
                            {
                                string lineSegment = line.Substring(startChar, line.Length - startChar);

                                Vector2 measuredString = alertText.MeasureString(lineSegment);
                                Vector2 position = new Vector2(SCREEN_WIDTH - (loreEntry.Width / 2), windowBuffer + alertText.LineSpacing + (lineNumber - scrollLines) * alertText.LineSpacing * 0.8f);
                                Vector2 origin = measuredString * 0.5f;

                                spriteBatch.DrawString(alertText, lineSegment, position, Color.White, 0, origin, 0.8f, SpriteEffects.None, 1);
                                lineNumber++;
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }

            #endregion


            //stop drawing
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


        //checks for a single button press on the gamepad
        public bool SingleKeyPress(Buttons button)
        {
            if ((prevGPadState.IsButtonUp(button) && (gPadState.IsButtonDown(button))))
            {
                prevGPadState = gPadState;
                gPadState = GamePad.GetState(PlayerIndex.One);
                return true;
            }
            else
            {
                return false;
            }
        }


        //checks for a single flick of the gamepad stick
        public bool SingleGamePadMove(Vector2 prevLeftStick, Vector2 currLeftStick)
        {
            if(prevLeftStickInput.Length() <= deadZone && leftStickInput.Length() > deadZone)
            {
                return true;
            }
            return false;
        }


        //Updates the rooms on gametime
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

                            if(!shifting)
                            {
                                foreach (Enemy enemy in activeRoom.enemyList)
                                {
                                    enemy.Update(gameTime, player);

                                    if (enemy.pathFinding)
                                    {
                                        foreach (Enemy enemy2 in activeRoom.enemyList)
                                        {
                                            if (!enemy.Equals(enemy2) && enemy.PosRect.Intersects(enemy2.PosRect) && !enemy2.slowed)
                                            {
                                                if (!enemy.slowed)
                                                {
                                                    enemy.distanceSlowed = 10;
                                                    enemy.MoveSpeed = 1;
                                                    enemy.slowed = true;
                                                }
                                            }
                                        }
                                    }                                                                      
                                }
                            }

                            //If the camera is shifting
                            if (shifting)
                            {
                                //Clear the list of projectiles
                                currProjectiles.Clear();
                            }
                            //If there are spawn points left in the room
                            while (levelAnnex[columnIndex, rowIndex].spawnTiles.Count != 0)
                            {
                                musicTransition = true;
                                //Spawn a goblin on that tile (The default enemy for now)
                                levelAnnex[columnIndex, rowIndex].SpawnGoblin(player, goblinEnemy, camera);
                                System.Threading.Thread.Sleep(1);
                                done = true;
                            }

                            if(activeRoom.Boss && activeRoom.enemyList.Count == 0 && !camera.isMoving && bossSpawned == false)
                            {
                                activeRoom.SpawnUlmog(player, goblinEnemy, camera);
                                done = true;
                                bossSpawned = true;
                            }

                            if(levelAnnex[columnIndex, rowIndex].RequestUnlock(player, camera))
                            {
                                musicTransition = true;
                            }

                            //Two birds with one stone; update collisions check and adjust active rooms if necessary
                            //Cannot run check if the frame is shifting
                            if (!shifting)
                            {
                                //If enemies have been spawned in the room and the room is done shifting
                                if (done)
                                {
                                    //Lock the room
                                    levelAnnex[columnIndex, rowIndex].Lock();
                                    done = false;
                                }

                                //Debugger requested a locked room!
                                if (lockRoom)
                                {
                                    levelAnnex[columnIndex, rowIndex].Lock();
                                    lockRoom = false;
                                }
                                //Debuggder requested an unlocked room
                                else if (unlockRoom)
                                {
                                    //Clear enemies and unlock the room
                                    levelAnnex[columnIndex, rowIndex].enemyList.Clear();
                                    levelAnnex[columnIndex, rowIndex].RequestUnlock(player, camera);
                                    unlockRoom = false;
                                }

                                //Update song to be either boss music or roaming music
                                if (levelAnnex[columnIndex, rowIndex].Boss && currentSong != "bossMusic")
                                {
                                    MediaPlayer.Play(bossMusic);                                  
                                    currentSong = "bossMusic";
                                }

                                //else if (!levelAnnex[columnIndex, rowIndex].Boss && currentSong != "mainMusic")
                                //{
                                //    MediaPlayer.Play(mainMusic);
                                //    currentSong = "mainMusic";
                                //}

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

                                    if(enemy is Ulmog == false)
                                    {
                                        if (!enemy.IsHostile)
                                        {
                                            foreach (Interactable post in levelAnnex[columnIndex, rowIndex].posts)
                                            {
                                                int x = post.RelativeLocation.X;
                                                int y = post.RelativeLocation.Y;
                                                x = ((x %= camera.screenWidth) < 0) ? x + camera.screenWidth : x;
                                                y = ((y %= camera.screenHeight) < 0) ? y + camera.screenHeight : y;
                                                if (enemy.PosX == x && enemy.PosY == y)
                                                {
                                                    enemy.UpdatePathDirection(post.SubType);
                                                }
                                            }
                                            enemy.UpdatePathPosition();
                                        }
                                        else if (enemy.path == null && !enemy.pathFinding)
                                        {
                                            if (enemy.PosX % 120 != 0 || enemy.PosY % 120 != 0)
                                            {
                                                enemy.UpdatePathPosition();
                                            }

                                            if (enemy.PosX % 120 == 0 && enemy.PosY % 120 == 0)
                                            {
                                                enemy.pathFinding = true;
                                                enemy.path = PathFinder.FindPath(levelAnnex[columnIndex, rowIndex], camera, enemy, player);
                                                enemy.pathIndex = -1;
                                            }
                                        }
                                        else if (enemy.path != null && enemy.path.Count > 0)
                                        {
                                            enemy.UpdatePathFindPosition();
                                        }

                                        else if (enemy.path == null || enemy.path.Count == 0)
                                        {
                                            enemy.path = PathFinder.FindPath(levelAnnex[columnIndex, rowIndex], camera, enemy, player);
                                            enemy.pathIndex = -1;
                                        }
                                    }
                                    
                                    enemy.Update(gameTime);

                                    //Pathfinding Algorithms --- Abandoned until further notice ---

                                    //int pathNum
                                    //if(enemy hasn't moved distance)
                                    //Move
                                    //else
                                    //pathNum + 1
                                    //

                                    
                                    //Vector2 travel = new Vector2(path[0].x, path[0].y);
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

            if(activeRoom.enemyList != null)
            {
                foreach (Enemy enemy in activeRoom.enemyList)
                {
                    if (enemy.IsHostile)
                    {
                        spriteBatch.Draw(health, new Rectangle(enemy.PosX, enemy.PosY - 20, (int)((float)enemy.PosRect.Width * (enemy.CurrHealth / enemy.TotalHealth)), 10), Color.White);
                    }
                }
            }
        }


        //This method handles creating a new level, and can be called whenever a new level needs to be generated, not just when the game starts
        private void NewLevelGen(bool reset)
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

            bossSpawned = false;

            if(reset)
            {
                //Initializing the player
                player = new Player((SCREEN_WIDTH / 2) - 110, (SCREEN_HEIGHT / 2) - 110, 110, 110);

                Notes.gatheredNotes.Clear();

                Notes.entriesToDiscover.Clear();

                Notes.GatherNotes();

                SetupSkillTree();
            }
        }


        //Method for ambient music
        public void UpdateSongPassive(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (currentSong == "CastleTheme" && timer > 140000)
            {
                timer = 0;
                MediaPlayer.Play(transientLoop);
                currentSong = "TransientLoop";
                oldSong = "TransientLoop";
            }
            else if (currentSong == "TransientLoop" && timer > 222000)
            {
                timer = 0;
                MediaPlayer.Play(castleTheme);
                currentSong = "CastleTheme";
                oldSong = "CastleTheme";
            }


            if(currentSong == "BattleTheme" || currentSong == "Oasis")
            {
                if(oldSong == "CastleTheme")
                {
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(castleTheme);
                    currentSong = "CastleTheme";
                }
                else if(oldSong == "TransientLoop")
                {
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(transientLoop);
                    currentSong = "TransientLoop";
                }
            }
        }


        //Method for battle music
        public void UpdateSongBattle()
        {
            if(currentSong != "BattleTheme")
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(battleTheme);
                currentSong = "BattleTheme";
            }
        }


        //Method for transitioning music
        public void SongTransition()
        {
            //if (MediaPlayer.Volume > 0)
            //{
            //    MediaPlayer.Volume -= (float)0.01;
            //}
            //else
            //{
            //    MediaPlayer.Volume = 0.4f;
            //    musicTransition = false;
            //}
            musicTransition = false;
        }


        //Method for setting up the skill tree
        public void SetupSkillTree()
        {
            //sets up the needed stuff
            Button root = new Button(null, null, "NULL", "NULL", 0, null);
            root.isBought = true;
            skillTree = new SkillTree(root);


            //Creates new buttons for the skill tree
            #region

            Button basicRogue = skillTree.Insert("Basic Rogue Abilities", "rogue", 0, root, player, whiteSlate);
            Button speed1 = skillTree.Insert("Increase Speed 1", "move", 1, basicRogue, player, whiteSlate);
            Button speed2 = skillTree.Insert("Increase Speed 2", "move", 1, speed1, player, whiteSlate);
            Button speed3 = skillTree.Insert("Increase Speed 3", "move", 1, speed2, player, whiteSlate);
            skillTree.Insert("Increase Speed 4", "move", 1, speed3, player, whiteSlate);

            Button basicWarrior = skillTree.Insert("Basic Warrior Abilities", "warrior", 0, root, player, whiteSlate);
            Button healthRegen1 = skillTree.Insert("Health Regen 1", "healthRegen", 1, basicWarrior, player, whiteSlate);
            Button healthRegen2 = skillTree.Insert("Health Regen 2", "healthRegen", 1, healthRegen1, player, whiteSlate);
            Button healthRegenRate1 = skillTree.Insert("Health Regen Rate 1", "healthRegenRate", 20, healthRegen1, player, whiteSlate);
            Button healthRegenRate2 = skillTree.Insert("Health Regen Rate 2", "healthRegenRate", 40, healthRegenRate1, player, whiteSlate);

            Button basicMage = skillTree.Insert("Basic Mage Abilities", "mage", 0, root, player, whiteSlate);
            Button manaIncrease1 = skillTree.Insert("Max Mana Increase 1", "mana", 50, basicMage, player, whiteSlate);
            Button manaIncrease2 = skillTree.Insert("Max Mana Increase 2", "mana", 50, manaIncrease1, player, whiteSlate);
            Button manaRegen1 = skillTree.Insert("Mana Regen 1", "manaRegen", 1, basicMage, player, whiteSlate);
            Button manaRegen2 = skillTree.Insert("Mana Regen 2", "manaRegen", 1, manaRegen1, player, whiteSlate);

            Button basicRanger = skillTree.Insert("Basic Ranger Abilities", "ranger", 0, root, player, whiteSlate);

            #endregion

        }


    }
}
