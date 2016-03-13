using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection; //Needed for string to variable calls
using System.IO; //Needed for finding rooms



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
            

        //Declare Room Textures
        /*
        Texture2D allDirections;
        Texture2D left;
        Texture2D up;
        Texture2D right;
        Texture2D down;
        Texture2D leftDown;
        Texture2D leftRight;
        Texture2D leftUp;
        Texture2D upRight;
        Texture2D upDown;
        Texture2D rightDown;
        Texture2D leftUpRight;
        Texture2D leftUpDown;
        Texture2D leftRightDown;
        Texture2D upRightDown;
        */
        Texture2D tilemap;

        LevelBuilder builderAlpha;


        bool buildLevel;

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
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            //Run Level Builder! Generate the first level
            gridSystem = new string[COLUMNS, ROWS];

            levelAnnex = new Room[COLUMNS, ROWS];

            builderAlpha = new LevelBuilder();

            numRooms = 10;

            gridSystem = builderAlpha.BuildLevel(gridSystem, numRooms);

            buildLevel = true;


            //Initializing the player
            player = new Player(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, 100, 200);

            //Initialize a new camera (origin at the center of the screen; dimensions of screen size)
            camera = new Camera(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT/2, SCREEN_WIDTH, SCREEN_HEIGHT);

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
            tilemap = Content.Load<Texture2D>("tilemap.png");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here 

            //Camera / Modular Testing Tool * Remove in Final Cut

            KeyboardState state = Keyboard.GetState();

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

            if (state.IsKeyDown(Keys.R))
            {
                gridSystem = new string[COLUMNS,ROWS];
                gridSystem = builderAlpha.BuildLevel(gridSystem, 10);
            }

            //Updating the player position
            player.Update(camera.screenWidth, camera.screenHeight, camera);

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
                            /*
                            switch(gridSystem[columnIndex,rowIndex].ToUpper())
                            {
                                case ("ALLDIRECTIONS"):

                                    room = allDirections;
                                    break;

                                case ("LEFT"):

                                    room = left;
                                    break;

                                case ("UP"):

                                    room = up;
                                    break;

                                case ("RIGHT"):

                                    room = right;
                                    break;

                                case ("DOWN"):

                                    room = down;
                                    break;

                                case ("LEFTDOWN"):

                                    room = leftDown;
                                    break;

                                case ("LEFTRIGHT"):

                                    room = leftRight;
                                    break;

                                case ("LEFTUP"):

                                    room = leftUp;
                                    break;

                                case ("UPRIGHT"):

                                    room = upRight;
                                    break;

                                case ("UPDOWN"):

                                    room = upDown;
                                    break;

                                case ("RIGHTDOWN"):

                                    room = rightDown;
                                    break;

                                case ("LEFTUPRIGHT"):

                                    room = leftUpRight;
                                    break;

                                case ("LEFTUPDOWN"):

                                    room = leftUpDown;
                                    break;

                                case ("LEFTRIGHTDOWN"):

                                    room = leftRightDown;
                                    break;

                                case ("UPRIGHTDOWN"):

                                    room = upRightDown;
                                    break;

                                default:
                                    room = up;
                                    break;
                            }
                            */

                            //Draw the node
                            //spriteBatch.Draw(room, new Vector2(xCoord, yCoord), Color.White);

                            string path = @"..\..\..\Content\Rooms\Test.txt";
                            //path += @"\Rooms\Test.txt";

                            if(levelAnnex[columnIndex, rowIndex] == null)
                            {
                                Room room = new Room(path, false, gridSystem[columnIndex, rowIndex]);
                                room.BuildRoom(xCoord, yCoord);
                                levelAnnex[columnIndex, rowIndex] = room;

                                //If the generated room is the starting room, it should be initially active
                                if(columnIndex == ((int)levelAnnex.GetLength(0)/2) && rowIndex == ((int)levelAnnex.GetLength(1)/2))
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

                                    switch (levelAnnex[columnIndex, rowIndex].CheckChangeRoom(player, camera, playerMove))
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
            player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
