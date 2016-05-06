using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Puts a tint of color on every object in the game (in a certain area)
//Date Modified: 4/17/16

namespace Once_A_Rogue
{
    class Atmosphere
    {
        //The Atmosphere class needs a filter texture and a camera
        private static Texture2D filter = null;
        private static Camera camera = null;

        private static float intensity;
        private static float secondIntensity;
        private static float thirdIntensity;

        public static float ThirdIntensity
        {
            get
            {
                return thirdIntensity;
            }
        }

        public static float Intensity
        {
            get
            {
                return intensity;
            }
        }

        public static float SecondIntensity
        {
            get
            {
                return secondIntensity;
            }
        }

        //This is a public property to set the Atmosphere's filter
        public static Texture2D Filter
        {
            set
            {
                filter = value;
            }          
        }

        //This is a public property to set the Atmosphere's camera
        public static Camera Camera
        {
            set
            {
                camera = value;
            }
        }

        //This method handles drawing an atmosphere for the boss room
        public static void BossFilter(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            //Put a purple tint on everything in the room
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Purple * 0.2f);
        }


        public static void AmberTransition(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.OrangeRed * intensity);
        }

        public static void AmberPurpleTransition(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.OrangeRed * intensity);
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Purple * secondIntensity);
        }

        public static void AmberDarkPurpleTransition(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.OrangeRed * intensity);
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Purple * secondIntensity);
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.DarkBlue * thirdIntensity);
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Black * thirdIntensity);

        }

        public static void Darken(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Black * intensity);
        }

        public static void IncreaseIntensity()
        {
            intensity += (float) 0.001;
        }

        public static void IncreaseSecondIntensity()
        {
            secondIntensity += (float) 0.001;
        }

        public static void IncreaseThirdIntensity()
        {
            thirdIntensity += (float)0.001;
        }

        public static void AnimateResetIntensities()
        {
            if(intensity > 0)
            {
                intensity -= (float) 0.01;
            }

            if(secondIntensity > 0)
            {
                secondIntensity -= (float) 0.01;
            }

            if(thirdIntensity > 0)
            {
                thirdIntensity -= (float) 0.01;
            }
        }

        public static void ResetIntensities()
        {
            intensity = 0;
            secondIntensity = 0;
            thirdIntensity = 0;
        }
    }
}
