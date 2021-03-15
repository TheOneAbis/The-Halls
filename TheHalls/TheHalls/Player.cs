using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    class Player : GameObject
    {
        private Vector2 arcLoc;
        private Texture2D arcImg;
        private float arcRotation;
        private Vector2 offset;

        public Player(Vector2 worldLoc, Vector2 screenOffset, Vector2 size, Texture2D image, Texture2D arcImage) : base(worldLoc, size, image)
        {
            arcImg = arcImage;
            arcRotation = 0;
            offset = screenOffset;
        }

        /// <summary>
        /// Moves the character based on WASD
        /// </summary>
        public void Move(KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.W))
            {
                worldLoc.Y--;
                offset.Y--;
            }

            if (kb.IsKeyDown(Keys.S))
            {
                worldLoc.Y++;
                offset.Y++;
            }

            if (kb.IsKeyDown(Keys.A))
            {
                worldLoc.X--;
                offset.X--;
            }

            if (kb.IsKeyDown(Keys.D))
            {
                worldLoc.X++;
                offset.X++;
            }
        }

        /// <summary>
        /// Updates player's weapon slash arc vector and arc rotation
        /// </summary>
        /// <param name="mouse">Mouse cursor location</param>
        public void Aim(MouseState mouse)
        {
            arcLoc = ScreenCenter + (Vector2.Normalize(new Vector2(mouse.X, mouse.Y) - ScreenCenter) * 100);

            arcRotation = mouse.Y - ScreenCenter.Y > 0 ?
                (float)Math.Acos((arcLoc.X - ScreenCenter.X) / (arcLoc - ScreenCenter).Length()) + (float)(Math.PI / 2) :
                -1 * (float)Math.Acos((arcLoc.X - ScreenCenter.X) / (arcLoc - ScreenCenter).Length()) + (float)(Math.PI / 2);
        }

        /// <summary>
        /// Draws the player and the slash arc to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch from Game1.Draw()</param>
        public virtual void Draw(SpriteBatch sb)
        {
            // Draw player
            base.Draw(sb, offset);

            // Draw player weapon slash arc
            sb.Draw(arcImg, new Rectangle((int)arcLoc.X,
                (int)arcLoc.Y, image.Width, image.Height / 3),
                new Rectangle(0, 0, arcImg.Width, arcImg.Height), Color.White,
                arcRotation,
                new Vector2(arcImg.Width / 2, arcImg.Height / 2), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Gets the center of the player's location 
        /// relative to the camera (the pixel location)
        /// </summary>
        public Vector2 ScreenCenter
        {
            get { return (worldLoc - offset) + (Size / 2); }
        }
    }
}
