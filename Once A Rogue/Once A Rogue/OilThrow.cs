using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class OilThrow:Skills
    {
        //Constructor that takes player object. Has no burst and a cooldown of 5, although it starts castable
        public OilThrow(Character own) : base(own)
        {
            Owner = own;
            Cooldown = 0;
            CooldownTotal = 1000;
            Cost = 10;
            Name = "Oil Cask";
        }

        public override void OnActivated()
        {
            //If the owner is a player
            if (Owner is Player)
            {
                Player player = (Player)Owner;

                //If the cooldown is reset and the player's current mana is more than the cost
                if (Cooldown == 0 && player.CurrMana >= Cost)
                {
                    base.OnActivated(player);

                    //Get the mouse position
                    MouseState ms = Mouse.GetState();

                    //Subtract mana
                    player.CurrMana -= Cost;

                    //If the player is attcking left
                    if (player.PlayerStates == Player.PlayerState.AttackLeft)
                    {
                        //Create a vector between the player and the mouse
                        Vector2 target = new Vector2(ms.X, ms.Y) - new Vector2(player.PosX - 40, player.PosY + player.PosRect.Height / 2);

                        //If the vector is not zero
                        if (target != Vector2.Zero)
                        {
                            //Normalize the vector to get the direction
                            target.Normalize();
                        }

                        //Add the projectile to the list
                        Game1.CurrProjectiles.Add(new Projectile(0, "oil", Owner, target, 0, 7, 40, 40, player.PosX - 40, player.PosY + player.PosRect.Height / 2));
                    }

                    else
                    {
                        //Getting vector from the player position to the mouse position
                        Vector2 target = new Vector2(ms.X, ms.Y) - new Vector2(player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2);

                        //Normalizing the vector
                        if (target != Vector2.Zero)
                        {
                            target.Normalize();
                        }

                        //Creating a vector equal to the normal vector
                        Vector2 vectorLength = target;

                        //stretching the normal vector to the desired range
                        vectorLength.X = (vectorLength.X * RangeX * 120) - 60;
                        vectorLength.Y = (vectorLength.Y * RangeY * 120) - 60;

                        Game1.CurrProjectiles.Add(new Projectile(0, "oil", Owner, target, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2));
                    }
                }
            }
        }
    }
}
