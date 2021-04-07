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
        private SpriteFont dmgFont;

        //Properties
        public int Damage { get { return damage; } set { damage = value; } }
        public weaponType Type { get { return type; } }

        //Constructor
        public Weapon(Rectangle location, Texture2D image, int damage, weaponType type, SpriteFont dmgFont): base (location, image)
        {
            this.damage = damage;
            this.type = type;
            this.dmgFont = dmgFont;
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

        public override void Draw(SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(sb);
                sb.DrawString(dmgFont, damage.ToString(), worldLoc - Game1.screenOffset, Color.Black);
            }
        }
    }
}
