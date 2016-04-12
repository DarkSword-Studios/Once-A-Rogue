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
        enum GameState { MainMenu, Playing, GameOver, paused, howTo }
        GameState gameState;

        //Enums for selector arrow
        enum ArrowState { pos1, pos2, pos3, menu1, menu2 }
        ArrowState arrowState;

        //Enums for player weapon state
        enum PlayWepState { Sword, Rogue, Mage, Ranger }
        PlayWepState playWepState;

        //Declare a grid to keep track of level space
        string [,] gridSystem;       
        const int ROWS = 9;
        const int COLUMNS = 9;

        //These variables are used to determine the screensize
        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        //Declare a number of rooms for a level
        int numRooms;

        //Track room structure via the grid system
        Room[,] levelAnnex;

        //Track active rooms
        List<Room> activeRooms;

        //The camera manages the viewport and location of drawn objects
        Camera camera;

        //Manage room activity to minimize impact on CPU
        bool shifting;
        int oldRow = -1;
        int oldCol = -1;
        string playerMove = "none";

        //Declare Room Textures
        Texture2D tilemap, playerTextures, projectileTextures;

        //Declare HUD Textures
        Texture2D pause, exit, resume, select, control, controls, mage, ranger, sword, rogue, back, main, play, exitM;

        //Keyboard states
        KeyboardState previousKBS, kbs;

        //Level builder to create and connect rooms
        LevelBuilder builderAlpha;

        Cursor cur;

        //Song for music
        Song mainMusic;

        //List to keep track of projectiles
        private static List<Projectile> currProjectiles;

        public static List<Projectile> CurrProjectiles
        {
            get { return currProjectiles; }
            set { currProjectiles = value; }
        }

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

            //Allow the game to run in fullscreen
            this.IsMouseVisible = true;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            currProjectiles = new List<Projectile>();

            //Initializing the gamestate
            gameState = GameState.MainMenu;
            arrowState = ArrowState.menu1;
            playWepState = PlayWepState.Sword;

            //Run Level Builder! Generate the first level
            gridSystem = new string[COLUMNS, ROWS];

            //Level annex is like the grid system except that it keeps track of the actual initialized rooms
            levelAnnex = new Room[COLUMNS, ROWS];

            builderAlpha = new LevelBuilder();

            numRooms = 10;

            //Initialize a new camera (origin at the center of the screen; dimensions of screen size)
            camera = new Camera(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT, 10);

            //Fill the grid with room codes
            gridSystem = builderAlpha.BuildLevel(gridSystem, numRooms);

            int rowIndex = 0;
            int columnIndex = 0;

            //For each space in the grid
            while (rowIndex < ROWS)
            {
                while (columnIndex < COLUMNS)
                {
                    //Attempt to build the room with the specified room code and put it into the level annex
                    levelAnnex = builderAlpha.BuildRoom(gridSystem, levelAnnex, camera, rowIndex, columnIndex);
                    columnIndex++;
                }
                //Reset column index after running through each column
                columnIndex = 0;
                rowIndex++;
            }

            //Initializing the player
            player = new Player(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, 140, 140);

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

            //Load in tilemap and player spritesheet
            tilemap = Content.Load<Texture2D>("Tileset.png");
            playerTextures = Content.Load <Texture2D>("PlayerAnims.png");
            projectileTextures = Content.Load<Texture2D>("Projectile Spritesheet.png");

            //Initialize HUD textures
            pause = Content.Load<Texture2D>("HUDpause.png");
            exit = Content.Load<Texture2D>("HUDexittowindowsbutton.png");
            select = Content.Load<Texture2D>("HUDselect.png");
            resume = Content.Load<Texture2D>("HUDresume.png");
            control = Content.Load<Texture2D>("HUDcontrol.png");
            controls = Content.Load<Texture2D>("HUDcontrols.png");
            sword = Content.Load<Texture2D>("HUDsword.png");
            rogue = Content.Load<Texture2D>("HUDrogue.png");
            mage = Content.Load<Texture2D>("HUDmage.png");
            ranger = Content.Load<Texture2D>("HUDranger.png");
            back = Content.Load<Texture2D>("HUDback.png");
            main = Content.Load<Texture2D>("HUDMain.png");
            play = Content.Load<Texture2D>("HUDplay.png");
            exitM = Content.Load<Texture2D>("HUDexit.png");

            //Loads and plays the music. Can't have it in update or it will keep attempting to play the same track over and over
            //Song is Finding The Balance by Kevin Macleod
            //mainMusic = Content.Load<Song>("music.wav");
            //MediaPlayer.Play(mainMusic);
            //MediaPlayer.Volume = (float)(MediaPlayer.Volume * .20);
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

            // TODO: Add your update logic here 
            if (gameState == GameState.MainMenu)
            {
                if (arrowState == ArrowState.menu1)
                {
                    if (SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.Playing;
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

            if(gameState == GameState.Playing)
            {
                //Extremely important call to update all active rooms
                UpdateRooms();
            
                
                player.UpdateFrame(gameTime);
                player.UpdateCooldowns(gameTime);

                //Set W A S D keys to four different directions
                if (kbs.IsKeyDown(Keys.A))
                {
                    playerMove = "left";
                }
                if (kbs.IsKeyDown(Keys.D))
                {
                    playerMove = "right";
                }
                if (kbs.IsKeyDown(Keys.S))
                {
                    playerMove = "down";
                }
                if (kbs.IsKeyDown(Keys.W))
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
                player.Update(camera.screenWidth, camera.screenHeight, camera);

                if (Game1.CurrProjectiles.Count > 0)
                {
                    foreach (Projectile project in Game1.CurrProjectiles)
                    {
                        project.Update();
                    }
                }

                if(SingleKeyPress(Keys.Escape))
                {
                    gameState = GameState.paused;
                    arrowState = ArrowState.pos1;
                }
            }
            
            if(gameState == GameState.GameOver)
            {

            }

            if(gameState == GameState.howTo)
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
            }

            //draws the following if the game is paused
            if(gameState == GameState.paused)
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

        public void UpdateRooms()
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

                            //Two birds with one stone; update collisions check and adjust active rooms if necessary
                            //Cannot run check if the frame is shifting
                            if (!shifting)
                            {
                                //Update the cursor, which will in turn update tile detection
                                cur.Update(levelAnnex[columnIndex, rowIndex], player);

                                //Check to see if the player is trying to move to another room
                                switch (levelAnnex[columnIndex, rowIndex].UpdateEvents(player, camera, playerMove))
                                {
                                    //If the player has indicated that they want to switch to an adjacent room
                                    case ("right"):

                                        //The new room becomes active
                                        levelAnnex[columnIndex + 1, rowIndex].Active = true;

                                        //The frame is shifting !!!! HALT ROOM UPDATES UNTIL TRANSITION IS COMPLETE
                                        shifting = true;

                                        //Set the old room coordinates so the old room can become inactive after the frame switch
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;

                                    //Examine the case above; this code operates similarly but with a slightly different situation
                                    case ("left"):

                                        levelAnnex[columnIndex - 1, rowIndex].Active = true;
                                        shifting = true;
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;

                                    case ("up"):

                                        levelAnnex[columnIndex, rowIndex - 1].Active = true;
                                        shifting = true;
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;

                                    case ("down"):

                                        levelAnnex[columnIndex, rowIndex + 1].Active = true;
                                        shifting = true;
                                        oldRow = rowIndex;
                                        oldCol = columnIndex;
                                        break;
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
                        levelAnnex[columnIndex, rowIndex].DrawRoom(spriteBatch, tilemap, xCoord, yCoord);
                    }
                    columnIndex++;

                }
                //Reset column index after running through each column
                columnIndex = 0;
                rowIndex++;
            }
        }
    }
}
