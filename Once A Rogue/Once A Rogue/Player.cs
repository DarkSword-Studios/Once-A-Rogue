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
        //Attributes
        //List containing known skills
        List<Skills> skillList;
        
        //String array of weapon choices
        string[] weaponArray;

        //Soul Count
        private int souls;

        public int Souls
        {
            get { return souls; }
            set { souls = value; }
        }

        //Currently selected weapon
        private string currWeapon;

        //Keeping track of the direction the player is facing
        string direction;

        public string CurrWeapon
        {
            get { return currWeapon; }
            set { currWeapon = value; }
        }

        //Currently selected skill
        private Skills currSkill;

        public Skills CurrSkill
        {
            get { return currSkill; }
            set { currSkill = value; }
        }

        //Stuff added by Stasha
        int currentFrame = 0;
        int numFrames = 6;

        public enum PlayerState { IdleLeft, IdleRight, WalkingLeft, WalkingRight, AttackLeft, AttackRight };

        private PlayerState playerState;

        public PlayerState PlayerStates
        {
            get { return playerState; }
            set { playerState = value; }
        }

        //End of stuff added by Stasha

        public Player(int x, int y, int width, int height)
        {
            //Initializing the collections
            skillList = new List<Skills>();
            skillList.Add(new MeleeAttack(this));
            //skillList.Add(new Fireball())
            weaponArray = new string[4];
            weaponArray[0] = "Sword";
            weaponArray[1] = "Daggers";
            weaponArray[2] = "Bow";
            weaponArray[3] = "Staff";

            //Setting default values
            playerState = PlayerState.IdleRight;
            MoveSpeedTotal = 5;
            MoveSpeed = 5;
            StunResist = 0;
            RootResist = 0;
            FireResist = 0;
            SnareResist = 0;
            PoisenResist = 0;
            MaxHealth = 20;
            CurrHealth = MaxHealth;
            PosRect = new Rectangle(x, y, width, height);
            currWeapon = weaponArray[0];
            CurrSkill = skillList[0];
        }

        //Method for processing user input
        public void ProcessInput(int roomWidth, int roomHeight)
        {
            //Figuring out which key is down
            KeyboardState kbs = Keyboard.GetState();

            //Figuring out which mouse buttn is being pressed
            MouseState msState = Mouse.GetState();

            //Moving the player based on which key is down
            if(kbs.IsKeyDown(Keys.A))
            {
                PosX -= MoveSpeed;
                playerState = PlayerState.WalkingLeft;
                direction = "left";
            }

            if (kbs.IsKeyDown(Keys.D))
            {
                PosX += MoveSpeed;
                playerState = PlayerState.WalkingRight;
                direction = "right";
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

            if(msState.LeftButton == ButtonState.Pressed)
            {
                CurrSkill.OnActivated(this);
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

            foreach(Skills sk in skillList)
            {
                if(sk.Cooldown > 0)
                {
                    sk.Cooldown -= 1;
                }
                else if(sk.Cooldown < 0)
                {
                    sk.Cooldown = 0;
                }
                else
                {
                    
                }
            }
        }

        //Added by Stasha
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
        //End of things added by Stasha

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