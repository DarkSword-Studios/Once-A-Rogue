using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Declares how the environment handles an interactable
//Date Modified: 4/15/16

namespace Once_A_Rogue
{
    class Interactable
    {
        private Boolean interactable;
        private Boolean passable;
        private Boolean doDraw;
        private Boolean functioning;
        private string type;

        private Rectangle relativeImageLocal;
        private Rectangle relativeLocation;

        public Boolean Functioning
        {
            get
            {
                return functioning;
            }

            set
            {
                functioning = value;
            }
        }

        public Boolean Activatable
        {
            get
            {
                return interactable;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public Boolean DoDraw
        {
            get
            {
                return doDraw;
            }
        }

        public Boolean Passable
        {
            get
            {
                return passable;
            }
        }

        public Rectangle RelativeImageLocal
        {
            get
            {
                return relativeImageLocal;
            }
        }
        public Rectangle RelativeLocation
        {
            get
            {
                return relativeLocation;
            }

            set
            {
                relativeLocation = value;
            }
        }

        public Interactable(string type, Rectangle relativeLocation, Rectangle relativeImageLocal, Boolean passable, Boolean interactable, Boolean doDraw)
        {
            this.type = type;
            this.passable = passable;
            this.interactable = interactable;
            this.doDraw = doDraw;
            this.relativeImageLocal = relativeImageLocal;
            this.relativeLocation = relativeLocation;
        }

        public void Interact(Player player, Camera camera)
        {
            if (!interactable)
            {
                return;
            }

            if(type == "Note")
            {
                //We need REAL modulo, not remainders
                int xCoord = this.relativeLocation.X;
                int yCoord = this.relativeLocation.Y;
                xCoord = ((xCoord %= camera.screenWidth) < 0) ? xCoord + camera.screenWidth : xCoord;
                yCoord = ((yCoord %= camera.screenHeight) < 0) ? yCoord + camera.screenHeight : yCoord;
                Rectangle adjustedLocal = new Rectangle(xCoord, yCoord, this.relativeLocation.Width, this.relativeLocation.Height);
                if (player.PosRect.Intersects(adjustedLocal))
                {
                    Notification.Alert("New Journal Entry Added: Unsent Love Letter", Color.Black, 120, false);
                    this.interactable = false;
                }
            }
        }

        public void HandleCollisions(Player player, Camera camera)
        {
            //Find the collision depth
            //We need REAL modulo, not remainders
            int xCoord = this.relativeLocation.X;
            int yCoord = this.relativeLocation.Y;
            xCoord = ((xCoord %= camera.screenWidth) < 0) ? xCoord + camera.screenWidth : xCoord;
            yCoord = ((yCoord %= camera.screenHeight) < 0) ? yCoord + camera.screenHeight : yCoord;
            Rectangle adjustedLocal = new Rectangle(xCoord, yCoord, this.relativeLocation.Width, this.relativeLocation.Height);
            Vector2 depth = FindIntersectionDepth(player.PosRect, adjustedLocal);

            //If there has been a collision
            if (depth != Vector2.Zero)
            {
                //Resolve the smallest distance first, then call HandleCollions()
                //again to resolve on the next axis
                if (Math.Abs(depth.X) > Math.Abs(depth.Y))
                {
                    player.PosY -= (int) depth.Y;
                    HandleCollisions(player, camera);
                }
                else
                {
                    player.PosX -= (int) depth.X;
                    HandleCollisions(player, camera);
                }
            }
        }


        public Vector2 FindIntersectionDepth(Rectangle rec1, Rectangle rec2)
        {
            Vector2 depth = new Vector2(0, 0);

            if (rec1.Intersects(rec2))
            {
                //Gets the amount of intersection from the left, right, top and bottom of the sprite
                int x1 = rec1.Left - rec2.Right;
                int x2 = rec1.Right - rec2.Left;
                int y1 = rec1.Top - rec2.Bottom;
                int y2 = rec1.Bottom - rec2.Top;

                //Returns the smallest intersections
                if(Math.Abs(x1) < Math.Abs(x2))
                {
                    depth.X = x1;
                }
                else
                {
                    depth.X = x2;
                }

                if(Math.Abs(y1) < Math.Abs(y2))
                {
                    depth.Y = y1;
                }
                else
                {
                    depth.Y = y2;
                }
            }
            return depth;
        }



    }
}
