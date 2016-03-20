using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    //Class to define specific Fireball skill 
    class Fireball:Skills
    //Ian Moon
    //3/20/2016
    //This class represents a fireball skill that the Player will be a able to use
    {

        //Constructor that takes player object. Has no burst and a cooldown of 5, although it starts initially at 0
        public Fireball(Player player)
        {
            Cooldown = 0;
            CooldownTotal = 5;
            BurstRadius = 0;
            IsDirectional = true;
        }

        //Overide OnActivated method
        public override void OnActivated(Player player)
        {
            base.OnActivated(player);
        }
    }
}
