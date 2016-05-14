using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class Block:Skills
    {
        public Block(Character own) : base(own)
        {
            Owner = own;
            CooldownTotal = 2000;
            Cooldown = 0;
            Cost = 2;
            Name = "Shields Up!";
        }

        public override void OnActivated()
        {
            if(Owner is Player)
            {
                Player player = (Player)Owner;
                {
                    if(Cooldown == 0 && player.CurrMana >= Cost)
                    {
                        base.OnActivated(player);
                        player.IsBlocking = !player.IsBlocking;
                    }
                }
            }
        }
    }
}
