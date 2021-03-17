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
        private float movementSpeed;

        public Player(Vector2 worldLoc, Vector2 size, Texture2D image, Texture2D arcImage) : base(worldLoc, size, image)
        {
            arcImg = arcImage;
            arcRotation = 0;
            movementSpeed = 3.5f;
        }

        /// <summary>
        /// Moves the character based on WASD
        /// </summary>
        public void Move(KeyboardState kb)
        {
            Vector2 moveDirection = new Vector2(0, 0);
            if (kb.IsKeyDown(Keys.W))
            {
                moveDirection.Y -= movementSpeed;
            }

            if (kb.IsKeyDown(Keys.S))
            {
                moveDirection.Y += movementSpeed;
            }

            if (kb.IsKeyDown(Keys.A))
            {
                moveDirection.X -= movementSpeed;
            }

            if (kb.IsKeyDown(Keys.D))
            {
                moveDirection.X += movementSpeed;
            }

            if (!(moveDirection.X == 0 && moveDirection.Y == 0))
            {
                moveDirection.Normalize();
            }

            worldLoc += (moveDirection * movementSpeed);
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
        public override void Draw(SpriteBatch sb)
        {
            // Draw player
            base.Draw(sb);


            // Draw player weapon slash arc
            sb.Draw(arcImg, new Rectangle((int)arcLoc.X,
                (int)arcLoc.Y, 80, 32),
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
            get { return (worldLoc - Game1.screenOffset) + (Size / 2); }
        }

        /// <summary>
        /// Checks the player's location against all of the game objects passed in, and stops the player if they are overlapping.
        /// </summary>
        /// <param name="obstacles"></param>
        public void ResolveCollisions(List<GameObject> obstacles)
        {
            Rectangle playerRect = GetRect();

            foreach (GameObject elem in obstacles)
            {
                if (!(elem == this || elem is Enemy))
                {
                    Rectangle obstacle = elem.GetRect();
                    if (obstacle.Intersects(playerRect))
                    {
                        Rectangle overlap = Rectangle.Intersect(obstacle, playerRect);
                        if (overlap.Width <= overlap.Height)
                        {
                            //X adjustment
                            if (obstacle.X > playerRect.X)
                            {
                                //obstacle is to the right of player 
                                playerRect.X -= overlap.Width;
                            }
                            else
                            {
                                //obstacle is to the left of the player
                                playerRect.X += overlap.Width;
                            }
                        }
                        else
                        {
                            //Y adjustment
                            if (obstacle.Y > playerRect.Y)
                            {
                                //obstacle is above the player
                                playerRect.Y -= overlap.Height;
                            }
                            else
                            {
                                //obstacle is below the player
                                playerRect.Y += overlap.Height;
                            }
                        }
                    }
                }
            }

            //sets the player location to the updated location
            worldLoc.X = (int)playerRect.X;
            worldLoc.Y = (int)playerRect.Y;

        }
    }
}
