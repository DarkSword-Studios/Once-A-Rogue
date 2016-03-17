using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection; //Needed for string to variable calls
using System.IO; //Needed for finding rooms
using System.Collections.Generic; //Needed for lists
using System; //Needed for random objects



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
        enum ArrowState { pos1, pos2, pos3 }
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

        //Keeps track of the current frame of animation for the player
        int framesElapsed;
        int timePerFrame = 100;

        //Declare Room Textures
        //Texture2D allDirections, left, up, right, down, leftDown, leftRight, leftUp, upRight, upDown, rightDown, leftUpRight, leftUpDown, leftRightDown, upRightDown;
        
        Texture2D tilemap, playerIdle;

        //Declare HUD Textures
        Texture2D pause, exit, resume, context, select, control, controls, mage, ranger, sword, rogue, back;

        //Keyboard states
        KeyboardState kbs, previousKBS, state;

        //Mouse states
        MouseState mbs;

        //Level builder to create and connect rooms
        LevelBuilder builderAlpha;

        Cursor cur;

        //List to keep track of projectiles
        private List<Projectile> currProjectiles;

        public List<Projectile> CurrProjectiles
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

            //Initializing the gamestate
            gameState = GameState.Playing;
            arrowState = ArrowState.pos1;
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

            //Initializing the projectile list
            currProjectiles = new List<Projectile>();           

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
            playerIdle = Content.Load <Texture2D>("PlayerAnims.png");

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

            // TODO: Add your update logic here 

            if(gameState == GameState.MainMenu)
            {

            }

            if(gameState == GameState.Playing)
            {
                //Extremely important call to update all active rooms
                UpdateRooms();
            
                framesElapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds / timePerFrame);
                player.UpdateFrame(framesElapsed);

                state = Keyboard.GetState();
                mbs = Mouse.GetState();

                if (state.IsKeyDown(Keys.A))
                {
                    playerMove = "left";
                }
                if (state.IsKeyDown(Keys.D))
                {
                    playerMove = "right";
                }
                if (state.IsKeyDown(Keys.S))
                {
                    playerMove = "down";
                }
                if (state.IsKeyDown(Keys.W))
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

                if (kbs.IsKeyDown(Keys.Z))
                {
                    playWepState = PlayWepState.Sword;
                }
                if (kbs.IsKeyDown(Keys.X))
                {
                    playWepState = PlayWepState.Rogue;
                }
                if (kbs.IsKeyDown(Keys.C))
                {
                    playWepState = PlayWepState.Mage;
                }
                if (kbs.IsKeyDown(Keys.V))
                {
                    playWepState = PlayWepState.Ranger;
                }

                //REBUILD LEVEL ****USE FOR DEBUGGING ONLY

                //if (state.IsKeyDown(Keys.R))
                //{
                //    gridSystem = new string[COLUMNS,ROWS];
                //    gridSystem = builderAlpha.BuildLevel(gridSystem, 10);
                //}

                //Updating the player position
                player.Update(camera.screenWidth, camera.screenHeight, camera);

                if (CurrProjectiles.Count > 0)
                {
                    foreach (Projectile project in CurrProjectiles)
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
                if (SingleKeyPress(Keys.Enter))
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
                    if (kbs.IsKeyDown(Keys.S))
                    {
                        arrowState = ArrowState.pos2;
                    }
                }

                if (arrowState == ArrowState.pos2)
                {
                    if (kbs.IsKeyDown(Keys.W))
                    {
                        arrowState = ArrowState.pos1;
                    }
                    if (kbs.IsKeyDown(Keys.S))
                    {
                        arrowState = ArrowState.pos3;
                    }
                }
                if (arrowState == ArrowState.pos2)
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
                    if (kbs.IsKeyDown(Keys.W))
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

            if (gameState == GameState.MainMenu)
            {

            }

            //this is drawn no matter what so even when paused, the game is still "drawn", it will just be "idle"
            if (this.IsActive)
            {
                //Extremely important call to draw all active rooms
                DrawRooms();
                
                //Drawing the player
                player.Draw(spriteBatch, playerIdle, 140, 140);

                //Drawing the projectiles on the screen
                if (CurrProjectiles.Count > 0)
                {
                    foreach (Projectile project in CurrProjectiles)
                    {
                        project.Draw(spriteBatch);
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
                spriteBatch.Draw(back, new Vector2(927, 982), Color.White);
                spriteBatch.Draw(select, new Vector2(852, 983), Color.White);
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
            previousKBS = kbs;
            kbs = Keyboard.GetState();

            if ((previousKBS.IsKeyUp(key)) && (kbs.IsKeyDown(key)))
            {
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
