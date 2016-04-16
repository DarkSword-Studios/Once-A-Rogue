using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

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
        private Boolean doDraw;
        private string type;

        private Rectangle relativeImageLocal;
        private Rectangle relativeLocation;

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

        public Interactable(string type, Rectangle relativeLocation, Rectangle relativeImageLocal, Boolean passable, Boolean interactable, Boolean doDraw)
        {
            this.type = type;
            this.passable = passable;
            this.interactable = interactable;
            this.doDraw = doDraw;
            this.relativeImageLocal = relativeImageLocal;
            this.relativeLocation = relativeLocation;
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
