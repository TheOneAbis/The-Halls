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

        public Player(Vector2 worldLoc, Vector2 size, Texture2D image) : base(worldLoc, size, image)
        {

        }

        /// <summary>
        /// Moves the character based on WASD
        /// </summary>
        public void Move()
        {
            KeyboardState kb = Keyboard.GetState();

            if(kb.IsKeyDown(Keys.W))
            {
                worldLoc.Y--;
            }

            if(kb.IsKeyDown(Keys.S))
            {
                worldLoc.Y++;
            }

            if (kb.IsKeyDown(Keys.A))
            {
                worldLoc.X--;
            }

            if (kb.IsKeyDown(Keys.D))
            {
                worldLoc.X++;
            }
        }
    }
}
