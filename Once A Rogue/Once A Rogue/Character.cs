using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class Character
    {
        //Attributes applicable to all characters
        string name;
        int currentHitPoints;
        int maxHitPoints;
        int currentSpeed;
        int maxSpeed;
        int currentMana;
        int maxMana;
        int stamina;
        bool alive;

        //Properties to set up attributes

        /*'Character' refers to player character and enemy*/

        private string Name //Name of the character
        {
            get { return name; }
            set { name = value; }
        }
        private int CurrentHitPoints //Current amount of health of character
        {
            get { return currentHitPoints; }
            set { currentHitPoints = value; }
        }
        private int MaxHitPoints //Maximum health that a character can have
        {
            get { return maxHitPoints; }
            set { maxHitPoints = value; }
        }
        private int CurrentSpeed //Current movement speed that a character is moving at
        {
            get { return currentSpeed; }
            set { currentSpeed = value; }
        }
        private int MaxSpeed //Maximum speed a character can move
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        private int CurrentMana //Current amount of mana/energy a character has
        {
            get { return currentMana; }
            set { currentMana = value; }
        }
        private int MaxMana //Maximum amount of mana/energy a character can have
        {
            get { return maxMana; }
            set { maxMana = value; }
        }
        private int Stamina //???(Don't really know what this is yet)
        {
            get { return stamina; }
            set { stamina= value; }
        }
        private bool Alive //Boolean to determine if character is alive (true) or dead (false)
        {
            get { return alive; }
            set { alive = value; }
        }

        //Constructor for character class
        public Character(string nm, int curHP, int maxHP, int curSpd, int maxSpd, int curMan, int maxMan, int stam, bool alv)
        {

            //Set values equal to attributes

            name = nm;
            currentHitPoints = curHP;
            maxHitPoints = maxHP;
            currentSpeed = curSpd;
            maxSpeed = maxSpd;
            currentMana = curMan;
            maxMana = maxMan;
            stamina = stam;
            alive = alv;
            
        }
    }
}
