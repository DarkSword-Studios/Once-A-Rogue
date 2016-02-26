using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection; //Needed for string to variable calls

namespace Once_A_Rogue
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Declare a grid to keep track of level space
        string [,] gridSystem;
        const int ROWS = 9;
        const int COLUMNS = 9;

        int numRooms;

        //Camera Mods
        int xMod = 0;
        int yMod = 0;

        //Declare Room Textures
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
            gridSystem = new string[ROWS, COLUMNS];

            builderAlpha = new LevelBuilder();

            numRooms = 30;

            gridSystem = builderAlpha.BuildLevel(gridSystem, 10);

            buildLevel = true;           

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

            //Initialize all room textures

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

            //Camera //Modular Testing Tool * Remove in Final Cut
        
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Right))
            {
                xMod += 10;
            }
            if (state.IsKeyDown(Keys.Left))
            {
                xMod -= 10;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                yMod -= 10;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                yMod += 10;
            }

            if (state.IsKeyDown(Keys.R))
            {
                gridSystem = builderAlpha.BuildLevel(gridSystem, 10);
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

            //If a new level has been generated update and redraw frame
            if (buildLevel)
            {
                //Reset build call
                //buildLevel = false;

                int rowIndex = 0;
                int columnIndex = 0;

                while (rowIndex < ROWS)
                {
                    while (columnIndex < COLUMNS)
                    {
                        if (gridSystem[columnIndex, rowIndex] != null)
                        {
                            int xCoord = ((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) + xMod);
                            int yCoord = ((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) + yMod);

                            xCoord += ((columnIndex - 4) * 640);
                            yCoord += ((rowIndex - 4) * 400);

                            Texture2D room;

                            switch(gridSystem[columnIndex,rowIndex])
                            {
                                case ("allDirections"):

                                    room = allDirections;
                                    break;

                                case ("left"):

                                    room = left;
                                    break;

                                case ("up"):

                                    room = up;
                                    break;

                                case ("right"):

                                    room = right;
                                    break;

                                case ("down"):

                                    room = down;
                                    break;

                                case ("leftDown"):

                                    room = leftDown;
                                    break;

                                case ("leftRight"):

                                    room = leftRight;
                                    break;

                                case ("leftUp"):

                                    room = leftUp;
                                    break;

                                case ("upRight"):

                                    room = upRight;
                                    break;

                                case ("upDown"):

                                    room = upDown;
                                    break;

                                case ("rightDown"):

                                    room = rightDown;
                                    break;

                                case ("leftUpRight"):

                                    room = leftUpRight;
                                    break;

                                case ("leftUpDown"):

                                    room = leftUpDown;
                                    break;

                                case ("leftRightDown"):

                                    room = leftRightDown;
                                    break;

                                case ("upRightDown"):

                                    room = upRightDown;
                                    break;

                                default:
                                    room = allDirections;
                                    break;
                            }

                            spriteBatch.Draw(room, new Vector2(xCoord, yCoord), Color.White);
                        }
                        columnIndex++;
                    }
                    columnIndex = 0;
                    rowIndex++;
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
