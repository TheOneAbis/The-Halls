using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//potions heal the player.
namespace TheHalls
{
    class Potion : Item
    {
        //Fields
        private int heal;

        //Properties
        public int Heal { get { return heal; } }

        //Class
        public Potion(Rectangle location, Texture2D image, int heal) : base(location, image)
        {
            this.heal = heal;
        }

        //Method

        public bool PickUp(Player player)
        {
            if (CheckCollison(player))
            {
                if (player.Health + heal > 5)
                {
                    //player.Health = 5;
                }
                else
                {
                    player.Health = player.Health + heal;
                }
                return true;
            }
            return false;
        }
    }
}
