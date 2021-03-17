using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    class Enemy : GameObject
    {
        protected int health;
        protected float movementSpeed;
        
        public Enemy(Vector2 worldLoc, Vector2 size, Texture2D image) : base(worldLoc, size, image)
        {
            movementSpeed = 2.5f;
        }

        /// <summary>
        /// This has the enemy move directly towards the target - generally the player.
        /// </summary>
        /// <param name="target"></param>
        public virtual void Move(GameObject target)
        {
            Vector2 moveDirection = target.WorldLoc - worldLoc;

            if (!(moveDirection.X == 0 && moveDirection.Y == 0))
            {
                moveDirection.Normalize();
            }

            worldLoc += (moveDirection * movementSpeed);
        }
    }
}
