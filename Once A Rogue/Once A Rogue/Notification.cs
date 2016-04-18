using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Allows for notifications to be put onscreen
//Date Modified: 4/16/16

namespace Once_A_Rogue
{
    class Notification
    {
        public static SpriteFont font;
        public static Texture2D diagonalBar;
        public static Camera camera;
        private static Boolean updating = false;
        private static int ticksOnScreen;

        private static Boolean flip;

        private static string note;
        private static Rectangle barPosition;
        private static string displayMessage;
        private static Color color;

        private static Queue<Alert> alerts = new Queue<Alert>();
        private static Alert currentAlert;

        public static Boolean Updating
        {
            get
            {
                return updating;
            }
        }


        public static void Alert(string message, Color newColor, int stationaryTicks, Boolean invert)
        {
            Alert alert = new Alert(message, newColor, stationaryTicks, invert);
            alerts.Enqueue(alert);   
        }

        public static void UpdateAlert()
        {
            if(!updating && alerts.Count != 0)
            {
                currentAlert = alerts.Dequeue();
                color = currentAlert.color;
                note = currentAlert.message;
                displayMessage = "";
                updating = true;
                barPosition = new Rectangle(0, camera.screenHeight, camera.screenWidth, camera.screenHeight);
                ticksOnScreen = currentAlert.stationaryTicks;
                flip = currentAlert.invert;
            }
            else if(!updating)
            {
                return;
            }

            if(ticksOnScreen <= 0)
            {           
                barPosition.Y += Math.Min((int)((barPosition.Y) * 0.1), -1);

                if(displayMessage != "")
                {
                    displayMessage = displayMessage.Substring(0, displayMessage.Length - 1);
                }

            }
            else
            {
                barPosition.Y -= Math.Max((int)((barPosition.Y) * 0.1), 1);
            }

            if(barPosition.Y < 0 && ticksOnScreen > 0)
            {
                barPosition.Y = 0;
                ticksOnScreen -= 1;

                if(note != "")
                {
                    displayMessage += note[0];
                    note = note.Substring(1);
                }
            }

            if(barPosition.Y < -camera.screenHeight)
            {
                updating = false;
                note = null;
            }
        }

        public static void DrawAlert(SpriteBatch spriteBatch)
        {
            if(!updating)
            {
                return;
            }

            float angle = (float)24f;
            float radians = (float)(-((angle / (float)180) * Math.PI));

            int messageX = camera.screenWidth / 2;
            int messageY = (camera.screenHeight / 2 + 18) + barPosition.Y;

            Vector2 measuredString = font.MeasureString(displayMessage);
            Vector2 position = new Vector2(messageX, messageY);
            Vector2 origin = measuredString * 0.5f;

            if (!flip)
            {
                spriteBatch.Draw(diagonalBar, barPosition, color);

                spriteBatch.DrawString(font, displayMessage, position, Color.White, radians, origin, 1, SpriteEffects.None, 1);
            }
            else
            {
                spriteBatch.Draw(diagonalBar, barPosition, null, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 1);

                spriteBatch.DrawString(font, displayMessage, position, Color.White, -radians, origin, 1, SpriteEffects.None, 1);
            }
            
        }
    }
}
