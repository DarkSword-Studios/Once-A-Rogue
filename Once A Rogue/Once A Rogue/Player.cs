using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Player : Character
    //Ian Moon
    //3/13/2016
    //Class that represents the player character
    {
        //List of known skills
        List<Skills> skillList;

        //Stuff added by Avix
        int currentFrame = 0;
        int numFrames = 6;

        enum PlayerState { IdleLeft, IdleRight, WalkingLeft, WalkingRight, AttackLeft, AttackRight };

        PlayerState playerState = PlayerState.IdleRight;

        //End of stuff added by Avix

        public Player(int x, int y, int width, int height)
        {
            //Setting default values
            MoveSpeedTotal = 5;
            MoveSpeed = 5;
            StunResist = 0;
            RootResist = 0;
            FireResist = 0;
            SnareResist = 0;
            PoisenResist = 0;
            MaxHealth = 20;
            CurrHealth = MaxHealth;
            skillList = new List<Skills>();
            PosRect = new Rectangle(x, y, width, height);
        }

        //Method for processing user input
        public void ProcessInput(int roomWidth, int roomHeight)
        {
            //Figuring out which keyboard is down
            KeyboardState kbs = Keyboard.GetState();

            //Moving the player based on which key is down
            if(kbs.IsKeyDown(Keys.A))
            {
                PosX -= MoveSpeed;
                playerState = PlayerState.WalkingLeft;
            }

            if (kbs.IsKeyDown(Keys.D))
            {
                PosX += MoveSpeed;
                playerState = PlayerState.WalkingRight;
            }

            if (kbs.IsKeyDown(Keys.S))
            {
                PosY += MoveSpeed;

                if (playerState == PlayerState.IdleRight)
                {
                    playerState = PlayerState.WalkingRight;
                }
                else if (playerState == PlayerState.IdleLeft)
                {
                    playerState = PlayerState.WalkingLeft;
                }
            }

            if (kbs.IsKeyDown(Keys.W))
            {
                PosY -= MoveSpeed;

                if (playerState == PlayerState.IdleRight)
                {
                    playerState = PlayerState.WalkingRight;
                }
                else if (playerState == PlayerState.IdleLeft)
                {
                    playerState = PlayerState.WalkingLeft;
                }
            }

            if (kbs.IsKeyUp(Keys.A) && kbs.IsKeyUp(Keys.D) && kbs.IsKeyUp(Keys.S) && kbs.IsKeyUp(Keys.W))
            {
                if (playerState == PlayerState.WalkingLeft)
                {
                    playerState = PlayerState.IdleLeft;
                }
                else if (playerState == PlayerState.WalkingRight)
                {
                    playerState = PlayerState.IdleRight;
                }

            }

            if (PosY > (roomHeight - PosRect.Height - 120))
            {
                PosY = roomHeight - PosRect.Height - 120;
            }

            if (PosY < 120)
            {
                PosY = 120;
            }

            if (PosX > (roomWidth - PosRect.Width - 120))
            {
                PosX = roomWidth - PosRect.Width - 120;
            }

            if (PosX < 120)
            {
                PosX = 120;
            }
        }

        public override void Update()
        {
            base.Update();
        }

        //Added by Avix
        public void Draw(SpriteBatch spritebatch, Texture2D texture, int frameWidth, int frameHeight)
        {
            Rectangle frame;

            switch (playerState)
            {
                case PlayerState.IdleRight:

                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White);
                    break;

                case PlayerState.IdleLeft:

                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.WalkingRight:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White);
                    break;

                case PlayerState.WalkingLeft:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;
            }
        }

        public void UpdateFrame(int framesElapsed)
        {
            
            currentFrame = framesElapsed % numFrames + 1;
        
        }
        //End of things added by Avix

        public void Update(int roomWidth, int roomHeight, Camera cam)
        {
            base.Update();
            
            //If the camera is not moving, process the player input
            if(!cam.isMoving)
            {
                ProcessInput(roomWidth, roomHeight);
            }

            //If the camera is moving, player input is not processed
            else if(cam.isMoving)
            {
                //Adjusting the player motion based on the camera movement direction
                switch(cam.direction)
                {
                    case "right":
                        if(cam.progress <= (1080/2))
                        {
                            PosX -= cam.panSpeed - 8;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX -= cam.panSpeed;
                            playerState = PlayerState.IdleRight;
                        }
                        break;

                    case "left":
                        if (cam.progress <= (1080 / 2))
                        {
                            PosX += cam.panSpeed - 8;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX += cam.panSpeed;
                            playerState = PlayerState.IdleLeft;
                        }
                        break;

                    case "down":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY -= cam.panSpeed - 4;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY -= cam.panSpeed;
                            if (playerState == PlayerState.WalkingLeft)
                            {
                                playerState = PlayerState.IdleLeft;
                            }
                            else if (playerState == PlayerState.WalkingRight)
                            {
                                playerState = PlayerState.IdleRight;
                            }
                        }
                        break;

                    case "up":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY += cam.panSpeed - 4;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY += cam.panSpeed;
                            if (playerState == PlayerState.WalkingLeft)
                            {
                                playerState = PlayerState.IdleLeft;
                            }
                            else if (playerState == PlayerState.WalkingRight)
                            {
                                playerState = PlayerState.IdleRight;
                            }
                        }
                        break;
                }
            }
        }
    }
}