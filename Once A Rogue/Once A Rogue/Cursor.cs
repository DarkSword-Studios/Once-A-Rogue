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
        private MouseState msState;
        private MouseState prevMsState;
        
        public void Update(Room room, Player player)
        {
            room.TagTiles(msState.Position, player.CurrSkill.BurstRadius, player.CurrSkill.BurstRadius, player.CurrSkill.BurstRadius, player.CurrSkill.BurstRadius, player.PosRect, player.CurrSkill.RangeX, player.CurrSkill.RangeY);
            msState = Mouse.GetState();

            if (msState.LeftButton == ButtonState.Pressed && prevMsState.LeftButton == ButtonState.Released)
            {
                player.CurrSkill.OnActivated(player);
            }
        }
    }
}
