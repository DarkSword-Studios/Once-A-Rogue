using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Once_A_Rogue
{
    class Camera : Game1
    {
        public int xMod;
        public int yMod;
        public int progress;
        public string direction;
        public Boolean isMoving = false;


        public Camera(int x, int y)
        {
            xMod = x;
            yMod = y;
            isMoving = false;
        }

        public void Move(string dir)
        {
            //DON'T RUN THIS SECTION
            //if (isMoving)
            //{
            //    return;
            //}

            direction = dir;

            switch (direction)
            {
                case "up":

                    progress = 5;
                    yMod += 5;
                    isMoving = true;

                    break;

            }

        }

        public void Update()
        {
            yMod += 5;
            progress += 5;

            if (progress >= 1080)
            {
                isMoving = false;
                
            }
        }
    }
}
