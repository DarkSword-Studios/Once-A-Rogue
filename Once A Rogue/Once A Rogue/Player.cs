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
            }

            if (kbs.IsKeyDown(Keys.D))
            {
                PosX += MoveSpeed;
            }

            if (kbs.IsKeyDown(Keys.S))
            {
                PosY += MoveSpeed;
            }

            if (kbs.IsKeyDown(Keys.W))
            {
                PosY -= MoveSpeed;
            }

            if (PosY > (roomHeight - PosRect.Height))
            {
                PosY = roomHeight - PosRect.Height;
            }

            if (PosY < 0)
            {
                PosY = 0;
            }

            if (PosX > (roomWidth - PosRect.Width))
            {
                PosX = roomWidth - PosRect.Width;
            }

            if (PosX < 0)
            {
                PosX = 0;
            }
        }

        public override void Update()
        {
            base.Update();
        }

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
                            PosX -= 2;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX -= 5;
                        }
                        break;

                    case "left":
                        if (cam.progress <= (1080 / 2))
                        {
                            PosX += 2;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX += 5;
                        }
                        break;

                    case "down":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY -= 3;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY -= 5;
                        }
                        break;

                    case "up":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY += 3;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY += 5;
                        }
                        break;
                }
            }
        }
    }
}
