using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class Fireball:Skills
    {
        public Fireball(Player player)
        {
            Cooldown = 0;
            CooldownTotal = 5;
            BurstRadius = 0;
            IsDirectional = true;
        }

        public override void OnActivated(Player player)
        {
            base.OnActivated(player);
        }
    }
}
