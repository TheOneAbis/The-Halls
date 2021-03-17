using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TheHalls
{
    /// <summary>
    /// gameObject only takes a world location and size, and draw will adapt that to screen coordinates. 
    /// </summary>
    class GameObject
    {
        protected Vector2 worldLoc;
        protected Vector2 size;
        protected Texture2D image;
        protected Color tint;

        public Vector2 WorldLoc
        {
            get { return worldLoc; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Color Tint
        {
            get { return tint; }
            set { tint = value; }
        }

        /// <summary>
        /// creates a new gameObject at the given location with the given size and image.
        /// </summary>
        /// <param name="worldLoc"></param>
        /// <param name="size"></param>
        /// <param name="image"></param>
        public GameObject(Vector2 worldLoc, Vector2 size, Texture2D image)
        {
            this.worldLoc = worldLoc;
            this.size = size;
            this.image = image;
            tint = Color.White;
        }

        /// <summary>
        /// draws the object, adjusted to the screenOffset.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="screenOffset"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle((int)(worldLoc.X - Game1.screenOffset.X), (int)(worldLoc.Y - Game1.screenOffset.Y), (int)size.X, (int)size.Y), tint);
        }

        /// <summary>
        /// returns true if the two objects are colliding.
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public bool Collides(GameObject toCheck)
        {
            if(this == toCheck)
            {
                return false;
            }
            return GetRect().Intersects(toCheck.GetRect());
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)worldLoc.X, (int)worldLoc.Y, (int)size.X, (int)size.Y);
        }
    }
}
