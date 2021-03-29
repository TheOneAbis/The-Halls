using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//Creates items for player to pick up
namespace TheHalls
{
    class Item : GameObject
    {
        //Fields
        private Rectangle location;
        private bool active;

        //Properties
        public Rectangle Location { get { return location; } set { location = value; } }
        public bool Active { get { return active; } }

        //Constructor
        public Item(Rectangle location, Texture2D image) : base(new Vector2(location.X, location.Y), new Vector2(location.Width,location.Height), image)
        {
            this.location = location;
            active = true;
        }

        /// <summary>
        /// Checks if  player is colliding with item
        /// </summary>
        /// <param name="player">player object</param>
        /// <returns>true if colliding with object</returns>
        public bool CheckCollison(Player player)
        {
            if (location.Contains(player.WorldLoc) && active == true)
            {
                active = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Draws the item when active
        /// </summary>
        /// <param name="sb">sprite batch</param>
        public override void Draw(SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(sb);
            }
        }
    }
}
