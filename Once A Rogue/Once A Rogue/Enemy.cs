using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class Enemy : Character
    {
        //Value for fear level of enemy
        private int fearLevel;

        //Property for Fear Level
        public int FearLevel
        {
            get { return fearLevel; }

            set
            {
                //If FearLevel is over 100, Player IsFeared and fearLevel is set to 100 
                if (fearLevel > 100)
                {
                    IsFeared = true;
                    fearLevel = 100;
                }
                else
                {
                    fearLevel = value;
                }
            }
        }
        //BRAINSTORM: Based on the armor level of enemies, the players will do reduced damage to them. 
        private int armorLevel;

        public int ArmorLevel
        {
            get { return armorLevel; }

            set
            {
                if(armorLevel < 25)
                {
                    //-
                }
            }
        }
        public void Attack()
        {

        }



    }
}
