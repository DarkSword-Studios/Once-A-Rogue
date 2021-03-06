﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Button : GameObject
    {
        //necessary attributes
        Player player;
        public bool isBought;
        string tag;
        int mod;
        public string description;

        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        //properties for parent and child buttons
        private List<Button> children;

        public List<Button> Children
        {
            get { return children; }
            set { children = value; }
        }

        private List<Button> parents;

        public List<Button> Parents
        {
            get { return parents; }
            set { parents = value; }
        }

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        //constructor sets the stuff 
        public Button(Button parent, Player pl, string description, string tag, int mod, Texture2D texture)
        {
            player = pl;
            this.texture = texture;
            children = new List<Button>();
            parents = new List<Button>();
            parents.Add(parent);
            isBought = false;
            Tag = tag;
            this.description = description;
            this.mod = mod;
        }

        //modifies an ability
        public void Purchase(int skillList, int index, int mod)
        {
            Boolean allBought = true;
            foreach(Button parent in parents)
            {
                if (!parent.isBought)
                {
                    allBought = false;
                }
            }
            if (allBought || parents.Count == 0)
            {
                if (Tag == "damage")
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
                    isBought = true;
                }


                //add tag and speed to skills
                if (Tag == "cooldown")
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
                    isBought = true;
                }
            }
        }

        //modifies player stats
        public void Purchase()
        {
            Boolean allBought = true;
            foreach (Button parent in parents)
            {
                if (parent != null && !parent.isBought)
                {
                    allBought = false;
                }
            }
            if (allBought || parents.Count == 0)
            {
                if (Tag == "health")
                {
                    player.TotalHealth += mod;
                    player.CurrHealth = player.TotalHealth;
                }

                if (Tag == "healthRegen")
                {
                    player.HealthRegen += mod;
                }

                if (Tag == "healthRegenRate")
                {
                    player.HealthRegenRate -= mod;
                }

                if (Tag == "manaRegen")
                {
                    player.ManaRegen += mod;
                }

                if (Tag == "manaRegenRate")
                {
                    player.ManaRegenRate -= mod;
                }

                if (Tag == "mana")
                {
                    player.TotalMana += mod;
                    player.CurrMana = player.TotalMana;
                }

                if (Tag == "move")
                {
                    player.MoveSpeedTotal += mod;
                    player.MoveSpeed = player.MoveSpeedTotal;
                }

                if (Tag == "mage")
                {
                    player.mageSkillList.Add(new Fireball(2, player));
                    player.mageSkillList.Add(new OilThrow(player));
                }

                if (Tag == "ranger")
                {
                    player.rangerSkillList.Add(new PiercingShot(6, player));
                }

                if (Tag == "rogue")
                {
                    player.rogueSkillList.Add(new FanOfKnives(6, player));
                }

                if (Tag == "warrior")
                {
                    player.warriorSkillList.Add(new Block(player));
                }

                isBought = true;
            }
        }
    }
}