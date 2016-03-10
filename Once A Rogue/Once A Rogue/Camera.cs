using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Controls viewport with modded camera
//Date Modified: 3/9/16

namespace Once_A_Rogue
{
    class Camera
    {
        public int xMod;
        public int yMod;
        public int progress;
        public string direction;
        public Boolean isMoving;

        public int screenWidth;
        public int screenHeight;

        //A camera must be initialized with an origin point and dimensions
        public Camera(int x, int y, int scrWidth, int scrHeight)
        {
            xMod = x;
            yMod = y;
            isMoving = false;
            screenWidth = scrWidth;
            screenHeight = scrHeight;
        }

        //If called to move, the camera will allow for motion and signal that the process is running
        //This should only be called once per camera move
        public void Move(string dir)
        {
            direction = dir;
            isMoving = true;
            progress = 0;
        }

        //When called to update, the camera will adjust its current position. NEVER CALL UPDATE BEFORE INITIALIZING A CAMERA MOVE WITH Move()
        public void Update()
        {
            switch (direction)
            {
                case "up":

                    progress += 5;
                    yMod += 5;

                    if (progress >= screenHeight)
                    {
                        isMoving = false;

                    }

                    break;

                case "down":

                    progress += 5;
                    yMod -= 5;

                    if (progress >= screenHeight)
                    {
                        isMoving = false;

                    }

                    break;

                case "left":

                    progress += 5;
                    xMod += 5;

                    if (progress >= screenWidth)
                    {
                        isMoving = false;

                    }

                    break;

                case "right":

                    progress += 5;
                    xMod -= 5;

                    if (progress >= screenWidth)
                    {
                        isMoving = false;

                    }

                    break;
            }
           
        }
    }
}
