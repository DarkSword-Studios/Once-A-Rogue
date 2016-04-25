using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class GhoulSiphon : Skills
    {
        public GhoulSiphon(int dam, Character own): base(dam, own)
        {
            Cooldown = 0;
            CooldownTotal = 10000;
            BurstRadius = 0;
            RangeX = 3;
            RangeY = 3;
            
        }

        public override void OnActivated(Player player)
        {
            base.OnActivated(player);
        }


    }
}
