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
        Vector2 worldLoc;
        Vector2 size;
        Texture2D image;

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
        }

        /// <summary>
        /// draws the object, adjusted to the screenOffset.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="screenOffset"></param>
        public void Draw(SpriteBatch sb, Vector2 screenOffset)
        {
            sb.Draw(image, new Rectangle((int)(worldLoc.X - screenOffset.X), (int)(worldLoc.Y - screenOffset.Y), (int)size.X, (int)size.Y), Color.White);
        }
    }
}
