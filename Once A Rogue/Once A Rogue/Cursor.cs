using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Once_A_Rogue
{
    class Cursor
    //Ian Moon
    //3/14/2016
    //This class will keep track of the mouse attributes
    {
        //Mouse States
        private MouseState msState;
        private MouseState prevMsState;
        
        public void Update(Room room, Player player)
        {
            //Avix (A.K.A: Stasha's room tagging)
            room.TagTiles(msState.Position, player.CurrSkill.BurstRadius, player.CurrSkill.BurstRadius, player.CurrSkill.BurstRadius, player.CurrSkill.BurstRadius, player.PosRect, player.CurrSkill.RangeX, player.CurrSkill.RangeY);

            //Getting the current state
            msState = Mouse.GetState();

            //Checking for a single press
            if (msState.LeftButton == ButtonState.Pressed && prevMsState.LeftButton == ButtonState.Released)
            {
                //Activating the player's current skill
                player.CurrSkill.OnActivated(player);
            }
        }
    }
}
