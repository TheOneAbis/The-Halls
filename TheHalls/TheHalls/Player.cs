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
        public delegate void GameOver();

        private Vector2 arcLoc;
        private Texture2D arcImg;
        private float arcRotation;
        private float movementSpeed;
        private int health;
        private Weapon weapon;
        private GameObject attack;
        private GameOver gameOver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldLoc">location to spawn the player in</param>
        /// <param name="size">size of the player</param>
        /// <param name="image">image to display for the player</param>
        /// <param name="arcImage">image to display for the arc of the players attacks</param>
        /// <param name="gameOver">method to be called when the player dies</param>
        public Player(Vector2 worldLoc, Vector2 size, Texture2D image, Texture2D arcImage, GameOver gameOver) : base(worldLoc, size, image)
        {
            arcImg = arcImage;
            arcRotation = 0;
            movementSpeed = 3.5f;
            this.gameOver = gameOver;
            health = 3;
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
        /// spawns a gameObject 50 units away from the player in the direction of arcRotation. checks collision against passed in enemies, dealing damage if they overlap.
        /// </summary>
        /// <param name="targets"></param>
        public void Attack(List<Enemy> targets)
        {
            //create an attack object
            attack = new GameObject(new Vector2((float)(worldLoc.X + 50 * Math.Sin(arcRotation)), (float)(worldLoc.Y - 50 * Math.Cos(arcRotation))), new Vector2(50, 50), base.image);
            attack.Tint = Color.Orange;

            //check collisions against each enemy
            foreach (Enemy elem in targets)
            {
                if(attack.Collides(elem))
                {
                    elem.TakeDamage(1);
                }
            }
        }

        /// <summary>
        /// subtracts from the players health. if their health is 0, call gameOver.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            if(health <= 0)
            {
                gameOver();
            }
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
            if (attack != null)
            {
                attack.Draw(sb);
                attack = null;
            }
        }

        /// <summary>
        /// Gets the center of the player's location 
        /// relative to the camera (the pixel location)
        /// </summary>
        public Vector2 ScreenCenter
        {
            get { return (worldLoc - Game1.screenOffset) + (Size / 2); }
        }

        public int Health
        {
            get { return health; }
        }
    }
}
