﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class FanOfKnives:Skills
    //Ian Moon
    //4/24/2016
    //This rogue skill unleashes a flurry of short range knives
    {
        public FanOfKnives(int dam, Character own) : base(dam, own)
        {
            //Setting default values
            CooldownTotal = 2000;
            Cooldown = 0;
            RangeX = 6;
            RangeY = 0;
            BurstRadius = 0;
            Cost = 10;
            Name = "Fan of Knives";
        }

        public override void OnActivated()
        {
            if (Owner is Player)
            {
                Player player = (Player)Owner;

                //If the cooldown is reset and the player's current mana is more than the cost
                if (Cooldown == 0 && player.CurrMana >= Cost)
                {
                    base.OnActivated(player);

                    //Get the mouse position
                    MouseState ms = Mouse.GetState();
                    GamePadState gPadState = GamePad.GetState(PlayerIndex.One);

                    //Subtract mana
                    player.CurrMana -= Cost;

                    if (gPadState.IsConnected)
                    {
                        float deadZone = 0.25f;

                        Vector2 stickInput = new Vector2(gPadState.ThumbSticks.Right.X, gPadState.ThumbSticks.Right.Y);
                        if (stickInput.Length() > deadZone)
                        {
                            stickInput.Normalize();

                            Vector2 target = stickInput;
                            Vector2 vectorLength = target;

                            if (stickInput.X < 0)
                            {
                                player.PlayerStates = Player.PlayerState.AttackLeft;
                                vectorLength.X = (vectorLength.X * RangeX * 120) + (60 * RangeX);
                                vectorLength.Y = (vectorLength.Y * RangeY * 120) + (60 * RangeY);
                                Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, target, vectorLength, 0, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2, 10, false));
                                Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y + .3f), vectorLength, 0, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2, 10, false));
                                Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y - .3f), vectorLength, 0, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2, 10, false));
                            }

                            if(stickInput.X > 0)
                            {
                                player.PlayerStates = Player.PlayerState.AttackRight;
                                vectorLength.X = (vectorLength.X * RangeX * 120) - (60 * RangeX);
                                vectorLength.Y = (vectorLength.Y * RangeY * 120) - (60 * RangeY);
                                Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, target, vectorLength, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2, 10, false));
                                Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y + .3f), vectorLength, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2, 10, false));
                                Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y - .3f), vectorLength, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2, 10, false));
                            }

                            return;
                        }
                    }

                    //If the player is attcking left
                    if (player.PlayerStates == Player.PlayerState.AttackLeft)
                    {
                        //Create a vector between the player and the position 120 units to the left
                        Vector2 target = new Vector2(player.PosX - (RangeX * 120), player.PosY + player.PosRect.Height / 2) - new Vector2(player.PosX - 10, player.PosY + player.PosRect.Height / 2);

                        //If the vector is not zero
                        if (target != Vector2.Zero)
                        {
                            //Normalize the vector to get the direction
                            target.Normalize();
                        }

                        //Creating a vector equal to the normal vector
                        Vector2 vectorLength = target;

                        //stretching the normal vector to the desired range
                        vectorLength.X = (vectorLength.X * RangeX * 120) + (60 * RangeX);
                        vectorLength.Y = (vectorLength.Y * RangeY * 120) + (60 * RangeY);

                        //Add the projectiles to the list
                        Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, target, vectorLength, 0, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2, 10, false));
                        Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y + .3f), vectorLength, 0, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2, 10, false));
                        Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y - .3f), vectorLength, 0, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2, 10, false));
                    }

                    else
                    {
                        //Create a vector between the player and the position 120 units to the right
                        Vector2 target = new Vector2(player.PosX + player.PosRect.Width + 10 + (RangeX * 120), player.PosY + player.PosRect.Height / 2) - new Vector2(player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2);

                        //Normalizing the vector
                        if (target != Vector2.Zero)
                        {
                            target.Normalize();
                        }

                        //Creating a vector equal to the normal vector
                        Vector2 vectorLength = target;

                        //stretching the normal vector to the desired range
                        vectorLength.X = (vectorLength.X * RangeX * 120) - (60 * RangeX);
                        vectorLength.Y = (vectorLength.Y * RangeY * 120) - (60 * RangeY);

                        //Add the projectiles to the list
                        Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, target, vectorLength, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2, 10, false));
                        Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y + .3f), vectorLength, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2, 10, false));
                        Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, new Vector2(target.X, target.Y - .3f), vectorLength, 0, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2, 10, false));
                    }
                }
            }

            else
            {
                Enemy currentE = (Enemy)Owner;

                if (Cooldown == 0)
                {

                }
            }
        }
    }
}
