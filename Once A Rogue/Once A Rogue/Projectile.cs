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

        bool limitRange;
        Vector2 vector;
        double range;
        int rowY;
        int numFrames;
        int currentFrame = 0;
        int framesElapsed = 0;
        int timeElapsed = 0;
        int timePerFrame = 100;
        const int FRAMEWIDTH = 40;
        const int FRAMEHEIGHT = 40;

        //Parameterized Constructor
        public Projectile(Vector2 vec, int rowY, int numFrames, int height, int width, int x, int y)
        {
            projPos = new Vector2(x, y);
            vector = vec;
            this.rowY = rowY;
            this.numFrames = numFrames;
            PosRect = new Rectangle(x, y, width, height);
            limitRange = false;

            //Adding the projectile to a projectile list in the game class
            Game1.CurrProjectiles.Add(this);
        }

        //Overload Constructor
        public Projectile(Vector2 vec, int rangeX, int rangeY, int rowY, int numFrames, int height, int width, int x, int y)
        {
            projPos = new Vector2(x, y);
            vector = vec;
            this.rowY = rowY;
            this.numFrames = numFrames;
            PosRect = new Rectangle(x, y, width, height);
            range = Math.Sqrt((x + (rangeX * rangeX)) + (y + (rangeY * rangeY)));
            limitRange = true;

            //Adding the projectile to a projectile list in the game class
            Game1.CurrProjectiles.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            float projLength = projPos.Length();
            if ((projLength < range) || !limitRange)
            {
                projPos += vector;
            }

            else
            {
                Game1.RemoveProj.Add(this);
            }

            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            framesElapsed = (int)(timeElapsed / timePerFrame);
            currentFrame = framesElapsed % numFrames + 1;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            //spriteBatch.Draw(Texture, PosRect, Color.White);

            Rectangle frame = new Rectangle(currentFrame * FRAMEWIDTH, FRAMEHEIGHT * rowY, FRAMEWIDTH, FRAMEHEIGHT);
            spriteBatch.Draw(texture, projPos, frame, Color.White);
        }
    }
}
