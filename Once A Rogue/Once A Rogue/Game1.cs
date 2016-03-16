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
        enum GameState { MainMenu, Playing, GameOver, paused, }
        GameState gameState;

        //Enums for selector arrow
        enum ArrowState { pos1, pos2, pos3 }
        ArrowState arrowState;

        //Declare a grid to keep track of level space
        string [,] gridSystem;       
        const int ROWS = 9;
        const int COLUMNS = 9;

        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        int numRooms;

        //Track room structure via the grid system
        Room[,] levelAnnex;

        //Camera Mods
        //int xMod = 0;
        //int yMod = 0;

        Camera camera;

        //Manage room activity to minimize impact on CPU
        bool shifting;
        int oldRow = -1;
        int oldCol = -1;
        string playerMove = "none";


        int framesElapsed;
        int timePerFrame = 100;

        //Declare Room Textures
        //Texture2D allDirections, left, up, right, down, leftDown, leftRight, leftUp, upRight, upDown, rightDown, leftUpRight, leftUpDown, leftRightDown, upRightDown;
        
        Texture2D tilemap, playerIdle;

        //Declare HUD Textures
        Texture2D pause, exit, resume, context, select;

        //Keyboard states
        KeyboardState kbs, previousKBS, state;

        //Mouse states
        MouseState mbs;

        LevelBuilder builderAlpha;

        Cursor cur;

        bool buildLevel;

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
            arrowState = ArrowState.pos2;

            //Run Level Builder! Generate the first level
            gridSystem = new string[COLUMNS, ROWS];

            levelAnnex = new Room[COLUMNS, ROWS];

            builderAlpha = new LevelBuilder();

            numRooms = 10;

            gridSystem = builderAlpha.BuildLevel(gridSystem, numRooms);

            buildLevel = true;


            //Initializing the player
            player = new Player(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, 140, 140);

            //Initializing the Cursor
            cur = new Cursor();

            //Initializing the projectile list
            currProjectiles = new List<Projectile>();

            //Initialize a new camera (origin at the center of the screen; dimensions of screen size)
            camera = new Camera(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT/2, SCREEN_WIDTH, SCREEN_HEIGHT, 10);

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

            player.Texture = Content.Load<Texture2D>("Player");
            //Initialize all room textures
            /*
            allDirections = Content.Load<Texture2D>("AllDirections-RoomCode.png");
            left = Content.Load<Texture2D>("Left-RoomCode.png");
            up = Content.Load<Texture2D>("Up-RoomCode.png");
            right = Content.Load<Texture2D>("Right-RoomCode.png");
            down = Content.Load<Texture2D>("Down-RoomCode.png");
            leftDown = Content.Load<Texture2D>("LeftDown-RoomCode.png");
            leftRight = Content.Load<Texture2D>("LeftRight-RoomCode.png");
            leftUp = Content.Load<Texture2D>("LeftUp-RoomCode.png");
            upRight = Content.Load<Texture2D>("UpRight-RoomCode.png");
            upDown = Content.Load<Texture2D>("UpDown-RoomCode.png");
            rightDown = Content.Load<Texture2D>("RightDown-RoomCode.png");
            leftUpRight = Content.Load<Texture2D>("LeftUpRight-RoomCode.png");
            leftUpDown = Content.Load<Texture2D>("LeftUpDown-RoomCode.png");
            leftRightDown = Content.Load<Texture2D>("LeftRightDown-RoomCode.png");
            upRightDown = Content.Load<Texture2D>("UpRightDown-RoomCode.png");
            */
            tilemap = Content.Load<Texture2D>("Tileset.png");
            playerIdle = Content.Load <Texture2D>("PlayerAnims.png");

            //Initialize HUD textures
            pause = Content.Load<Texture2D>("HUDpause.png");
            exit = Content.Load<Texture2D>("HUDexittowindowsbutton.png");
            select = Content.Load<Texture2D>("HUDselect.png");
            resume = Content.Load<Texture2D>("HUDresume.png");
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
                framesElapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds / timePerFrame);
                player.UpdateFrame(framesElapsed);

                //Camera / Modular Testing Tool * Remove in Final Cut

                state = Keyboard.GetState();
                mbs = Mouse.GetState();

                //If the camer is not currently running a motion, a call can be made to adjust it up down left or right
                if (state.IsKeyDown(Keys.Right))
                {
                    //xMod += 10;
                    if (!camera.isMoving)
                    {
                        camera.Move("right");
                    }
                }
                if (state.IsKeyDown(Keys.Left))
                {
                    //xMod -= 10;
                    if (!camera.isMoving)
                    {
                        camera.Move("left");
                    }
                }
                if (state.IsKeyDown(Keys.Up))
                {
                    //yMod -= 10;
                    if (!camera.isMoving)
                    {
                        camera.Move("up");
                    }

                }
                if (state.IsKeyDown(Keys.Down))
                {
                    //yMod += 10;
                    if (!camera.isMoving)
                    {
                        camera.Move("down");
                    }
                }

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
                }
            }
            
            if(gameState == GameState.GameOver)
            {

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
                }

                if (arrowState == ArrowState.pos2)
                {
                    if (kbs.IsKeyDown(Keys.Enter))
                    {
                        Exit();
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
                //If we want to display the build map
                if (buildLevel)
                {
                    //Reset build call
                    //buildLevel = false;

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
                                //Build coordinates based on camera mods and grid placement
                                int xCoord = ((SCREEN_WIDTH / 2) + camera.xMod);
                                int yCoord = ((SCREEN_HEIGHT / 2) + camera.yMod);

                                xCoord += ((columnIndex - 4) * 1920);
                                yCoord += ((rowIndex - 4) * 1080);

                                //Texture2D room;

                                //Based on the node's structure, use the appropriate texture

                                int roomCode;

                                switch (gridSystem[columnIndex, rowIndex].ToUpper())
                                {
                                    case ("ALLDIRECTIONS"):

                                        roomCode = 1234;
                                        break;

                                    case ("LEFT"):

                                        roomCode = 1;
                                        break;

                                    case ("UP"):

                                        roomCode = 2;
                                        break;

                                    case ("RIGHT"):

                                        roomCode = 3;
                                        break;

                                    case ("DOWN"):

                                        roomCode = 4;
                                        break;

                                    case ("LEFTDOWN"):

                                        roomCode = 14;
                                        break;

                                    case ("LEFTRIGHT"):

                                        roomCode = 13;
                                        break;

                                    case ("LEFTUP"):

                                        roomCode = 12;
                                        break;

                                    case ("UPRIGHT"):

                                        roomCode = 23;
                                        break;

                                    case ("UPDOWN"):

                                        roomCode = 24;
                                        break;

                                    case ("RIGHTDOWN"):

                                        roomCode = 34;
                                        break;

                                    case ("LEFTUPRIGHT"):

                                        roomCode = 123;
                                        break;

                                    case ("LEFTUPDOWN"):

                                        roomCode = 124;
                                        break;

                                    case ("LEFTRIGHTDOWN"):

                                        roomCode = 134;
                                        break;

                                    case ("UPRIGHTDOWN"):

                                        roomCode = 234;
                                        break;

                                    default:
                                        roomCode = 2;
                                        break;
                                }

                                string roomCodeStr = roomCode.ToString();

                                List<string> possibleRooms = new List<string>();

                                foreach (string file in Directory.GetFiles(@"..\..\..\Content\Rooms"))
                                {
                                    if (file.Contains(roomCodeStr))
                                    {
                                        char[] letterArray = new char[file.Length];
                                        List<int> numberArray = new List<int>();
                                        List<int> roomCodeList = new List<int>();

                                        for (int x = 0; x < file.Length; x++)
                                        {
                                            letterArray[x] = file[x];
                                        }

                                        for (int x = 0; x < roomCodeStr.Length; x++)
                                        {
                                            int roomCodeDigit;

                                            int.TryParse(roomCodeStr[x].ToString(), out roomCodeDigit);

                                            roomCodeList.Add(roomCodeDigit);
                                        }

                                        int fileNumber;

                                        foreach (char c in letterArray)
                                        {
                                            string charString = c.ToString();
                                            if (int.TryParse(charString, out fileNumber))
                                            {
                                                numberArray.Add(fileNumber);
                                            }
                                        }

                                        if (numberArray.Count == roomCodeList.Count)
                                        {
                                            int truthCount = 0;
                                            for (int x = 0; x < numberArray.Count; x++)
                                            {
                                                if (numberArray[x] == roomCodeList[x])
                                                {
                                                    truthCount += 1;
                                                }
                                            }

                                            if (truthCount == roomCodeStr.Length)
                                            {
                                                possibleRooms.Add(file);
                                            }
                                        }
                                    }
                                }

                                Random random = new Random();

                                string roomPath = possibleRooms[random.Next(0, possibleRooms.Count)];


                                //Draw the node
                                //spriteBatch.Draw(room, new Vector2(xCoord, yCoord), Color.White);
                                //path += @"\Rooms\Test.txt";

                                if (levelAnnex[columnIndex, rowIndex] == null)
                                {
                                    Room room = new Room(roomPath, false, gridSystem[columnIndex, rowIndex]);
                                    room.BuildRoom(xCoord, yCoord);
                                    levelAnnex[columnIndex, rowIndex] = room;

                                    //If the generated room is the starting room, it should be initially active
                                    if (columnIndex == ((int)levelAnnex.GetLength(0) / 2) && rowIndex == ((int)levelAnnex.GetLength(1) / 2))
                                    {
                                        levelAnnex[columnIndex, rowIndex].Active = true;
                                    }
                                }

                                if (!shifting && oldRow != -1)
                                {
                                    levelAnnex[oldCol, oldRow].Active = false;
                                    oldRow = -1;
                                    oldCol = -1;
                                }

                                if (levelAnnex[columnIndex, rowIndex].Active)
                                {
                                    //Two birds with one stone; update collisions check and adjust active rooms if necessary
                                    if (!shifting)
                                    {
                                        cur.Update(levelAnnex[columnIndex, rowIndex], player);
                                        switch (levelAnnex[columnIndex, rowIndex].UpdateEvents(player, camera, playerMove))
                                        {
                                            case ("right"):

                                                levelAnnex[columnIndex + 1, rowIndex].Active = true;
                                                shifting = true;
                                                oldRow = rowIndex;
                                                oldCol = columnIndex;
                                                break;

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

                                    levelAnnex[columnIndex, rowIndex].DrawRoom(spriteBatch, tilemap, xCoord, yCoord);
                                }
                            }
                            columnIndex++;
                        }
                        //Reset column index after running through each column
                        columnIndex = 0;
                        rowIndex++;
                    }
                }

                //Drawing the player
                player.Draw(spriteBatch, playerIdle, 140, 140);

                //Drawing the projectiles on the screen
                if (CurrProjectiles.Count > 0) ;
                {
                    foreach (Projectile project in CurrProjectiles)
                    {
                        project.Draw(spriteBatch);
                    }
                }
            }

            //draws the following if the game is paused
            if(gameState == GameState.paused)
            {
                spriteBatch.Draw(pause, new Vector2(0, 0), Color.White);
                spriteBatch.Draw(resume, new Vector2(100, 351), Color.White);
                spriteBatch.Draw(exit, new Vector2(100, 426), Color.White);

                if (arrowState == ArrowState.pos1)
                {
                    spriteBatch.Draw(select, new Vector2(30, 354), Color.White);
                }

                if (arrowState == ArrowState.pos2)
                {
                    spriteBatch.Draw(select, new Vector2(30, 429), Color.White);
                }
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
    }
}
