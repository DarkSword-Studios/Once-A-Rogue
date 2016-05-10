using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class Button : GameObject
    {
        //necessary attributes
        Player player;
        bool isBought;
        string tag;

        //properties for parent and child buttons
        private Button parent;

        public Button Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private Button child;

        public Button Child
        {
            get { return child; }
            set { child = value; }
        }

        //constructor sets the stuff 
        public Button(Button p, Player pl, string s)
        {
            player = pl;
            parent = p;
            isBought = false;
            tag = s;
        }

        //modifies an ability
        public void Purchase(int skillList, int index, int mod)
        {
            if (parent.isBought || parent == null)
            {
                if (tag == "damage")
                {
                    switch (skillList)
                    {
                        case 0:
                            player.warriorSkillList[index].Damage += mod;
                            break;

                        case 1:
                            player.rogueSkillList[index].Damage += mod;
                            break;

                        case 2:
                            player.rangerSkillList[index].Damage += mod;
                            break;

                        case 3:
                            player.mageSkillList[index].Damage += mod;
                            break;
                    }
                }


                //add tag and speed to skills
                if (tag == "cooldown")
                {
                    switch (skillList)
                    {
                        case 0:
                            player.warriorSkillList[index].CooldownTotal -= mod;
                            break;

                        case 1:
                            player.rogueSkillList[index].CooldownTotal -= mod;
                            break;

                        case 2:
                            player.rangerSkillList[index].CooldownTotal -= mod;
                            break;

                        case 3:
                            player.mageSkillList[index].CooldownTotal -= mod;
                            break;
                    }
                }
            }
        }

        //modifies player stats
        public void Purchase(int mod)
        {
            if (parent.isBought || parent == null)
            {
                if (tag == "health")
                {
                    player.TotalHealth += mod;
                    player.CurrHealth = player.TotalHealth;
                }

                if (tag == "mana")
                {
                    player.TotalMana += mod;
                    player.CurrMana = player.TotalMana;
                }

                if (tag == "move")
                {
                    player.MoveSpeedTotal += mod;
                }

                if (tag == "fireball")
                {
                    player.mageSkillList.Add(new Fireball(4, player));
                }

                if (tag == "piercing")
                {
                    player.rangerSkillList.Add(new PiercingShot(6, player));
                }

                if (tag == "fan")
                {
                    player.rogueSkillList.Add(new FanOfKnives(6, player));
                }

                if (tag == "oil")
                {
                    player.mageSkillList.Add(new OilThrow(player));
                }
            }
        }
    }
}