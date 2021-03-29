using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//Creates weapons for player
namespace TheHalls
{
    enum weaponType
    {
        Sword,
        Spear
    }
    class Weapon : Item 
    {
        //Fields
        private int damage;
        private weaponType type;

        //Properties
        public int Damage { get { return damage; } set { damage = value; } }
        public weaponType Type { get { return type; } }

        //Consturctor
        public Weapon(Rectangle location, Texture2D image, int damage, weaponType type): base (location, image)
        {
            this.damage = damage;
            this.type = type;
        }

        //Methods

        /// <summary>
        /// player picks up weapon
        /// </summary>
        /// <param name="player"></param>
        public bool PickUp(Player player)
        {
            if (CheckCollison(player))
            {
                player.CurrentWeapon = Type;
                player.WeaponImage = image;
                player.Damage = damage;
                return true;
            }
            return false;
        }
    }
}
