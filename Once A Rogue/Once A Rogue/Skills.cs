using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Once_A_Rogue
{
    class Skills
    {
        //Attributes

        //Cooldown handling
        private double cooldown;

        public double Cooldown
        {
            get { return cooldown; }
            set { cooldown = value; }
        }

        private double cooldownTotal;

        public double CooldownTotal
        {
            get { return cooldownTotal; }
            set { cooldownTotal = value; }
        }

        //Range values of the skill
        private int rangeX;

        public int RangeX
        {
            get { return rangeX; }
            set { rangeX = value; }
        }

        private int rangeY;

        public int RangeY
        {
            get { return rangeY; }
            set { rangeY = value; }
        }


        //How far out the burst is of the skill is from a center point
        private int burstRadius;

        public int BurstRadius
        {
            get { return burstRadius; }
            set
            {
                if(burstRadius <= 0)
                {
                    burstRadius = 0;
                }
                else
                {
                    burstRadius = value;
                }
            }
        }

        virtual public void OnActivated(Player player)
        {

        }
    }
}
