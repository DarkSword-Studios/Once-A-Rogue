using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{

    //Enemy ghoul Siphon Soul Ability
    class SiphonSoul: Skills
    {
        private int damage;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public SiphonSoul(Ghoul ghoul)
        {
            //Set values
            CooldownTotal = 6000;
            Cooldown = 0;
            RangeX = 1;
            RangeY = 0;
        }

        public void OnActivation(Player pay, Ghoul ghoul)
        {
            Random rand = new Random();
            int chance = rand.Next(0, 11);
            if (chance >= 7 || chance <= 9)
            {
                if (pay.CurrHealth <= pay.MaxHealth / 4)
                {
                    pay.Souls = pay.Souls - 4;
                    pay.CurrHealth = pay.CurrHealth - pay.MaxHealth / 10;
                    ghoul.GhoulSouls++;
                    ghoul.ArmorLevel = ghoul.ArmorLevel + ghoul.GhoulSouls;
                }
            }

            if (chance == 10)
            {
                pay.CurrHealth = pay.CurrHealth - pay.MaxHealth / 5;
                pay.Souls = pay.Souls - 10;
                ghoul.GhoulSouls = ghoul.GhoulSouls + 10;
                
            }
            else
            {
                pay.Souls = pay.Souls - 2;
                pay.CurrHealth = pay.CurrHealth - pay.MaxHealth / 15;
            }

        }
    }
}
