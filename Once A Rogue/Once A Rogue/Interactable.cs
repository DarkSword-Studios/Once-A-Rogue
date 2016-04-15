using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private string type;

        private int xCoord;
        private int yCoord;

        public Interactable(int tileX, int tileY, string type, Boolean passable, Boolean interactable)
        {
            xCoord = tileX;
            yCoord = tileY;
            this.type = type;
            this.passable = passable;
            this.interactable = interactable;
        }

        public void Interact()
        {
            if (!interactable)
            {
                return;
            }
        }
    }
}
