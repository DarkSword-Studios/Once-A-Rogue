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
        int velocityX;
        int velocityY;
        int duration;
        string direction;

        //Parameterized Constructor
        public Projectile(int vectorX, int vectorY, string dir, Texture2D text, Rectangle posRect, Game1 game)
        {
            vectorX = velocityX;
            vectorY = velocityY;
            direction = dir;
            Texture = text;
            PosRect = posRect;

            //Adding the projectile to a projectile list in the game class
            game.CurrProjectiles.Add(this);
        }

        //Overload Constructor
        public Projectile(int vectorX, int vectorY, int dur, string dir, Texture2D text, Rectangle posRect, Game1 game)
        {
            velocityX = vectorX;
            velocityY = vectorY;
            duration = dur;
            direction = dir;
            Texture = text;
            PosRect = posRect;
        }

        public void Update()
        {
            //Making sure the projectile is going the correct direction
            if(direction == "left")
            {
                velocityX = -velocityX;
            }

            if(direction == "down")
            {
                velocityY = -velocityY;
            }

            PosX = PosX + velocityX;

            PosY = PosY + velocityY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, PosRect, Color.White);
        }
    }
}
