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
        private bool colliding;   // this is used specifically to display the tooltip when player gets close to weapon pickup

        //Properties
        public int Damage { get { return damage; } set { damage = value; } }
        public weaponType Type { get { return type; } }

        //Constructor
        public Weapon(Rectangle location, Texture2D image, int damage, weaponType type, SpriteFont dmgFont): base (location, image)
        {
            this.damage = damage;
            this.type = type;
            this.dmgFont = dmgFont;
            colliding = false;
        }

        //Methods

        /// <summary>
        /// player picks up weapon
        /// </summary>
        /// <param name="player"></param>
        public bool PickUp(Player player)
        {
            if (Collides(player))
            {
                colliding = true;
                if (player.IsInteracting)
                {
                    player.CurrentWeapon = Type;
                    //player.WeaponImage = image;
                    player.Damage = damage;
                    active = false;
                    return true;
                }
            }
            else
            {
                colliding = false;
            }
            return false;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(sb);
                sb.DrawString(dmgFont, "Dmg: " + damage.ToString(), worldLoc - Game1.screenOffset + new Vector2(-10, -10), Color.LightGray);

                if (colliding)
                {
                    sb.DrawString(dmgFont, $"Press [E] to equip {type}.", new Vector2(
                        worldLoc.X - Game1.screenOffset.X - 90, worldLoc.Y - Game1.screenOffset.Y + 50), 
                        Color.LightGreen);
                }
            }
        }
    }
}
