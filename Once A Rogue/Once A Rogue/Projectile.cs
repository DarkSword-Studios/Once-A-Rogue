using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    public class Projectile:GameObject
    //Ian Moon
    //3/9/2016
    //This class is for any projectile that needs to be produced
    {
        //Attributes
        private Vector2 projPos;

        public Vector2 ProjPos
        {
            get { return projPos; }
            set { projPos = value; }
        }

        //Checking to see whether or not to limit the range
        bool limitRange;
        
        //Normalized Vector
        Vector2 vector;

        //Vector from player to end point
        Vector2 vecDist;

        //Animation handling
        int rowY;
        int numFrames;
        int currentFrame = 0;
        int framesElapsed = 0;
        int timeElapsed = 0;
        int timePerFrame = 100;
        const int FRAMEWIDTH = 40;
        const int FRAMEHEIGHT = 40;

        //Starting X and Y cordinates.
        double startX;
        double startY;

        //Projectile speed
        int speed;

        //Distance the projectile has travelled
        float distTravelled;

        //Distance the projectile needs to travel
        float distTravel;


        //Damage dealt by the projectile
        private int damage;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        //Tag for combo system
        private string tag;

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        //Character attributes used for single hit detection
        Character prevChar = null;
        Character currChar = null;

        //Owner of the projectile
        private Character owner;

        public Character Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public bool isCombo;

        //Parameterized Constructor if range is not an issue
        public Projectile(int dam, string tg, Character own, Vector2 vec, int rowY, int numFrames, int height, int width, int x, int y, int spd, bool combo)
        {
            Tag = tg;
            Damage = dam;
            owner = own;
            ProjPos = new Vector2(x, y);
            vector = vec;
            startX = x;
            startY = y;
            speed = spd;
            this.rowY = rowY;
            this.numFrames = numFrames;
            PosRect = new Rectangle(x, y, width, height);
            limitRange = false;
            isCombo = combo;
        }

        //Overload Constructor if range is an issue
        public Projectile(int dam, string tg, Character own, Vector2 vec, Vector2 destination, int rowY, int numFrames, int height, int width, int x, int y, int spd, bool combo)
        {
            Tag = tg;
            Damage = dam;
            owner = own;
            ProjPos = new Vector2(x, y);
            vector = vec;
            startX = x;
            startY = y;
            speed = spd;
            vecDist = destination;
            this.rowY = rowY;
            this.numFrames = numFrames;
            PosRect = new Rectangle(x, y, width, height);
            limitRange = true;
            distTravelled = 0;
            distTravel = vecDist.Length();
            isCombo = combo;
        }

        public void Update(GameTime gameTime)
        {
            //If the distance is meant to be limited and the distance meant to travelled hasn't been hit yet
            if ((distTravelled < distTravel) || !limitRange)
            {
                //Adjust the vectors
                projPos.X += vector.X * speed;
                projPos.Y += vector.Y * speed;

                //Increment the distance travelled
                distTravelled += vector.Length() * speed;

                //Adjust the position rectangle
                PosRect = new Rectangle((int)projPos.X, (int)projPos.Y, 40, 40);
            }

            else
            {
                //Removing the projectile
                Game1.RemoveProj.Add(this);
            }

            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            framesElapsed = (int)(timeElapsed / timePerFrame);
            currentFrame = framesElapsed % numFrames;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            //spriteBatch.Draw(Texture, PosRect, Color.White);
            
            //For some reason projectiles spawn with an extra pixel row? (Currently fixed - take a look at soon)
            Rectangle frame = new Rectangle(currentFrame * FRAMEWIDTH, FRAMEHEIGHT * rowY, FRAMEWIDTH, FRAMEHEIGHT - 1);
            spriteBatch.Draw(texture, PosRect, frame, Color.White, (float) Math.Atan2(vector.Y, vector.X), Vector2.Zero, SpriteEffects.None, 1);
        }

        public void OnCollision(Character target)
        {
            currChar = target;
            if (prevChar != currChar)
            {
                //Deal damage and remember that character
                prevChar = currChar;
                target.CurrHealth -= Damage;
            }

            //If the tag is not to pass through
            if (Tag != "pass")
            {
                //Subtract damage and remove
                Game1.RemoveProj.Add(this);
            }

            //Setting up combo situations
            switch(Tag)
            {
                case "fire":
                    //Set the duration of the fire and damage
                    target.FireDur = 4000;
                    target.FireDmg = 1 + Owner.Level;
                    if(target.IsExplosive)
                    {
                        //Create projectiles which go off in 8 directions
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(1, 0), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(-1, 0), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(0, 1), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(0, -1), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(.707f, .707f), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(-.707f, .707f), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(.707f, -.707f), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        Game1.AddProj.Add(new Projectile(0, "fire", target, new Vector2(-.707f, -.707f), 0, 7, 10, 10, target.PosX + target.PosRect.Width / 2, target.PosY + target.PosRect.Height / 2, 5, true));
                        target.FireDur = 0;
                    }
                    break;

                case "poison":
                    //Set the poison
                    target.PoisonDur = 4000;
                    target.PoisonDmg = 4;
                    break;

                case "stun":
                    target.StunDur = 4;
                    break;

                case "snare":
                    target.MoveSpeed -= 1;
                    if(target.MoveSpeed <= 0)
                    {
                        target.MoveSpeed = 0;
                    }
                    break;

                case "oil":
                    target.ExplosiveDur = 4000;
                    target.MoveSpeed -= 1;
                    if (target.MoveSpeed <= 0)
                    {
                        target.MoveSpeed = 0;
                    }
                    break;

                case "root":
                    target.MoveSpeed = 0;
                    target.RootDur = 4000;
                    break;

            }
        }
    }
}
