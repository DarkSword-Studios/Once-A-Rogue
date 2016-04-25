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
        //Tools for the Notification class
        public static SpriteFont font;
        public static Texture2D diagonalBar;
        public static Camera camera;
        private static Boolean updating = false;
        private static int ticksOnScreen;

        //Specific parameters that pertain to the current notification
        private static Boolean flip;
        private static string note;
        private static Rectangle barPosition;
        private static string displayMessage;
        private static Color color;

        //Manage backlogged alerts as well as the current alert
        private static Queue<Alert> alerts = new Queue<Alert>();
        private static Alert currentAlert;

        //Allow other classes to see if Notification is updating, do not allow a set
        public static Boolean Updating
        {
            get
            {
                return updating;
            }
        }

        //This method can be called globally, and handles creating a new alert
        public static void Alert(string message, Color newColor, int stationaryTicks, Boolean invert)
        {
            //Create a new alert object from the information given
            Alert alert = new Alert(message, newColor, stationaryTicks, invert);

            //Throw the new alert onto the queue to be processed
            alerts.Enqueue(alert);   
        }

        //This method handles updating the current alert
        public static void UpdateAlert()
        {
            //If Notification isn't updating and there are alerts to process
            if(!updating && alerts.Count != 0)
            {
                //Grab the next alert object
                currentAlert = alerts.Dequeue();

                //Set Notification's specific parameters to the alert's specifications
                color = currentAlert.color;
                note = currentAlert.message;
                ticksOnScreen = currentAlert.stationaryTicks;
                flip = currentAlert.invert;

                //Reset Notification objects for the new alert
                displayMessage = "";
                updating = true;
                barPosition = new Rectangle(0, camera.screenHeight, camera.screenWidth, camera.screenHeight);
                
            }
            //If there are no more alerts to process, return
            else if(!updating)
            {
                return;
            }

            //If the bar has been stationary, but its time on screen has expired
            if(ticksOnScreen <= 0)
            {           
                //Start moving the bar off screen either 10% of the distance left or 1 pixel (whatever moves faster)
                barPosition.Y += Math.Min((int)((barPosition.Y) * 0.1), -1);

                //If the display message is not empty, take away a character
                if(displayMessage != "")
                {
                    displayMessage = displayMessage.Substring(0, displayMessage.Length - 1);
                }

            }
            //If the bar should be moving upwards
            else
            {
                //Move the bar on screen either 10% of the distance left or 1 pixel (whatever moves faster)
                barPosition.Y -= Math.Max((int)((barPosition.Y) * 0.1), 1);
            }

            //If the bar's position is less than 0 and it should still be stationary on screen
            if(barPosition.Y < 0 && ticksOnScreen > 0)
            {
                //Set the bar's position to 0 and decrement the ticks left on screen
                barPosition.Y = 0;
                ticksOnScreen -= 1;

                //If the message to display has not been fully displayed
                if(note != "")
                {
                    //Add one character from the message to the display string
                    displayMessage += note[0];
                    note = note.Substring(1);
                }
            }

            //If the bar has moved off screen, then the alert is finished
            if(barPosition.Y < -camera.screenHeight)
            {
                //Notification is no longer updating and the message is null
                updating = false;
                note = null;
            }
        }

        //This method handles drawing the notification on screen
        public static void DrawAlert(SpriteBatch spriteBatch)
        {
            //If there isn't a current notification, just return
            if(!updating)
            {
                return;
            }

            //Manage the angle of the displayed text and convert it to radians
            float angle = (float)24f;
            float radians = (float)(-((angle / (float)180) * Math.PI));

            //Calculate the coordinates for the display message
            int messageX = camera.screenWidth / 2;
            int messageY = (camera.screenHeight / 2 + 18) + barPosition.Y;

            //Center the text so that the anchor point is the center of the message
            Vector2 measuredString = font.MeasureString(displayMessage);
            Vector2 position = new Vector2(messageX, messageY);
            Vector2 origin = measuredString * 0.5f;

            //If the alert requires the notification to not be flipped
            if (!flip)
            {
                //Draw the diagonal bar normally
                spriteBatch.Draw(diagonalBar, barPosition, color);

                //Draw the display message at the correct point, rotate one way (positive radians)
                spriteBatch.DrawString(font, displayMessage, position, Color.White, radians, origin, 1, SpriteEffects.None, 1);
            }
            else
            {
                //Draw the diagonal bar flipped horizontally
                spriteBatch.Draw(diagonalBar, barPosition, null, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 1);

                //Draw the display message at the correct point, rotate the other way (negative radians)
                spriteBatch.DrawString(font, displayMessage, position, Color.White, -radians, origin, 1, SpriteEffects.None, 1);
            }
            
        }
    }
}
