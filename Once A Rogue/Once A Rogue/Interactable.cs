using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Declares how the environment handles an interactable
//Date Modified: 4/22/16

namespace Once_A_Rogue
{
    class Interactable
    {
        //These are the different tags an interactable can have (affect how it is seen and dealt with in the world)
        private Boolean interactable;
        public Boolean passable;
        private Boolean doDraw;
        private Boolean functioning;
        private Boolean levelTrigger;
        private string type;
        private string subType;
        public Rectangle adjustedLocal;

        //Just like a tile, an interactable has a relative location and a relative image location
        private Rectangle relativeImageLocal;
        private Rectangle relativeLocation;

        //An interactable's attributes are private, but each one has a property
        public string SubType
        {
            get
            {
                return subType;
            }

            set
            {
                subType = value;
            }
        }

        public Boolean LevelTrigger
        {
            get
            {
                return levelTrigger;
            }
        }

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
        //This is the constructor for an interactable - it has to have initial entries for several of the tags
        public Interactable(string type, Rectangle relativeLocation, Rectangle relativeImageLocal, Boolean passable, Boolean interactable, Boolean doDraw)
        {
            this.type = type;
            this.passable = passable;
            this.interactable = interactable;
            this.doDraw = doDraw;
            this.relativeImageLocal = relativeImageLocal;
            this.relativeLocation = relativeLocation;
        }

        //This method handles the situation when an interactable is interacted with
        public void Interact(Player player, Camera camera)
        {
            //If the interactable cannot be interacted with
            if (!interactable)
            {
                //Exit the method
                return;
            }

            //If the interactable is a note
            if(type == "Note")
            {
                //We need REAL modulo, not remainders
                int xCoord = this.relativeLocation.X;
                int yCoord = this.relativeLocation.Y;
                xCoord = ((xCoord %= camera.screenWidth) < 0) ? xCoord + camera.screenWidth : xCoord;
                yCoord = ((yCoord %= camera.screenHeight) < 0) ? yCoord + camera.screenHeight : yCoord;

                //Calculate the relative local coordinates of the note
                adjustedLocal = new Rectangle(xCoord, yCoord, this.relativeLocation.Width, this.relativeLocation.Height);

                //If the player is intersecting with the note
                if (player.PosRect.Intersects(adjustedLocal))
                {
                    //Spawn a new notification to alert the player that they have picked up the note
                    Notification.Alert("New Journal Entry Added: Unsent Love Letter", Color.Black, 120, false);
                    //Make sure the note cannot be interacted with twice
                    this.interactable = false;
                }
            }
            //If the interactable is a ladder
            else if(type == "Ladder")
            {
                //We need REAL modulo, not remainders
                int xCoord = this.relativeLocation.X;
                int yCoord = this.relativeLocation.Y;
                xCoord = ((xCoord %= camera.screenWidth) < 0) ? xCoord + camera.screenWidth : xCoord;
                yCoord = ((yCoord %= camera.screenHeight) < 0) ? yCoord + camera.screenHeight : yCoord;
                Rectangle adjustedLocal = new Rectangle(xCoord, yCoord, this.relativeLocation.Width, this.relativeLocation.Height);

                //if the player is on top of the ladder
                if (player.PosRect.Intersects(adjustedLocal))
                {
                    //Set the trigger to spawn a new level to be true
                    levelTrigger = true;
                }
                
            }
        }
        //This method handles detecting and processing collisions between the interactable and the player
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
                    //If X collision is greater than y, handle y first
                    player.PosY -= (int) depth.Y;
                    HandleCollisions(player, camera);
                }
                else
                {
                    //If Y collision is greater than x, handle x first
                    player.PosX -= (int) depth.X;
                    HandleCollisions(player, camera);
                }
            }
        }

        public void HandleCollisions(Projectile project, Camera camera)
        {
            //Find the collision depth
            //We need REAL modulo, not remainders
            int xCoord = this.relativeLocation.X;
            int yCoord = this.relativeLocation.Y;
            xCoord = ((xCoord %= camera.screenWidth) < 0) ? xCoord + camera.screenWidth : xCoord;
            yCoord = ((yCoord %= camera.screenHeight) < 0) ? yCoord + camera.screenHeight : yCoord;
            Rectangle adjustedLocal = new Rectangle(xCoord, yCoord, this.relativeLocation.Width, this.relativeLocation.Height);

            //If there has been a collision
            if (project.PosRect.Intersects(adjustedLocal) && !this.Passable)
            {
                Game1.RemoveProj.Add(project);
            }
        }

        //This method handles finding the specific collision depth between two rectangles 
        public Vector2 FindIntersectionDepth(Rectangle rec1, Rectangle rec2)
        {
            Vector2 depth = new Vector2(0, 0);

            if (rec1.Intersects(rec2))
            {
                //Gets the amount of intersection from the left, right, top and bottom of the rectangles given (player and interactable)
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
        //Gives the interactable a subtype classification (Currently used for spawner and post directions)
        public void AssignSubType(int tileLocal)
        {
            //Based on the tile code given, it is known which direction that tile represents
            switch (tileLocal)
            {
                case 82:

                    subType = "All";
                    break;

                case 85:

                    subType = "up";
                    break;

                case 86:

                    subType = "left";
                    break;

                case 87:

                    subType = "right";
                    break;

                case 88:

                    subType = "down";
                    break;

                case 96:

                    subType = "up";
                    break;

                case 97:

                    subType = "left";
                    break;

                case 98:

                    subType = "right";
                    break;

                case 99:

                    subType = "down";
                    break;

                case 100:

                    subType = "upleft";
                    break;

                case 101:

                    subType = "upright";
                    break;

                case 102:

                    subType = "downleft";
                    break;

                case 103:

                    subType = "downright";
                    break;
            }
        }
    }
}
