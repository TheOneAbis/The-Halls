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
        private int health;
        private float movementSpeed;
        
        public Enemy(Vector2 worldLoc, Vector2 size, Texture2D image) : base(worldLoc, size, image)
        {
            movementSpeed = 2.5f;
        }

        public void Move(GameObject target)
        {
            Vector2 moveDirection = target.WorldLoc - worldLoc;

            if (!(moveDirection.X == 0 && moveDirection.Y == 0))
            {
                moveDirection.Normalize();
            }

            worldLoc += (moveDirection * movementSpeed);
        }


        public void ResolveCollisions(List<GameObject> obstacles)
        {
            Rectangle enemyRect = GetRect();

            foreach (GameObject elem in obstacles)
            {
                Rectangle obstacle = elem.GetRect();
                if (obstacle.Intersects(enemyRect))
                {
                    Rectangle overlap = Rectangle.Intersect(obstacle, enemyRect);
                    if (overlap.Width <= overlap.Height)
                    {
                        //X adjustment
                        if (obstacle.X > enemyRect.X)
                        {
                            //obstacle is to the right of the enemy 
                            enemyRect.X -= overlap.Width;
                        }
                        else
                        {
                            //obstacle is to the left of the enemy
                            enemyRect.X += overlap.Width;
                        }
                    }
                    else
                    {
                        //Y adjustment
                        if (obstacle.Y > enemyRect.Y)
                        {
                            //obstacle is above the enemy
                            enemyRect.Y -= overlap.Height;
                        }
                        else
                        {
                            //obstacle is below the enemy
                            enemyRect.Y += overlap.Height;
                        }
                    }
                }
            }

            //sets the enemy location to the updated location
            worldLoc.X = (int)enemyRect.X;
            worldLoc.Y = (int)enemyRect.Y;

        }
    }
}
