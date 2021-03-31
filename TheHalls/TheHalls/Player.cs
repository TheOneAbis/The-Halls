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
        private Vector2 rightArcSide;
        private Vector2 leftArcSide;
        private Texture2D arcImg;
        private Texture2D weaponImage;
        private float arcRotation;
        private float movementSpeed;
        private int health;
        private int damage;
        private int attackRadius;
        private weaponType weapon;
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
        public Player(Vector2 worldLoc, Vector2 size, Texture2D image, Texture2D arcImage,Texture2D weaponImage, GameOver gameOver) : base(worldLoc, size, image)
        {
            arcImg = arcImage;
            arcRotation = 0;
            movementSpeed = 3.5f;
            this.gameOver = gameOver;
            health = 3;
            attackRadius = 100;
            this.weaponImage = weaponImage;
            damage = 1;
            weapon = weaponType.Sword;
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
            arcLoc = ScreenLoc + (Vector2.Normalize(new Vector2(mouse.X, mouse.Y) - ScreenLoc) * attackRadius);

            arcRotation = mouse.Y - ScreenLoc.Y > 0 ?
                (float)Math.Acos((arcLoc.X - ScreenLoc.X) / (arcLoc - ScreenLoc).Length()) + (float)(Math.PI / 2) :
                -1 * (float)Math.Acos((arcLoc.X - ScreenLoc.X) / (arcLoc - ScreenLoc).Length()) + (float)(Math.PI / 2);
        }

        /// <summary>
        /// spawns a gameObject 50 units away from the player in the direction of arcRotation. checks collision against passed in enemies, dealing damage if they overlap.
        /// </summary>
        /// <param name="targets"></param>
        public void Attack(List<Enemy> targets)
        {
            Vector2 attackScanner;
            Rectangle enemyScreenRect;

            // Iterate for each enemy
            foreach (Enemy elem in targets)
            {
                // This rectangle gets the rectangle location of the enemy relative to the screen
                enemyScreenRect = new Rectangle((int)(elem.ScreenLoc.X - (elem.Size / 2).X),
                    (int)(elem.ScreenLoc.Y - (elem.Size / 2).Y), (int)elem.Size.X, (int)elem.Size.Y);


                //     ---   SCAN THE PIE SLICE   ---

                //  Vector rotates clockwise starting at left side of pie slice
                for (double leftSide = -(Math.PI / 8); leftSide < (Math.PI / 8); leftSide += Math.PI / 64)
                {
                    // At each point of the vector's rotation, check every point along the vector's line
                    for (int i = 1; i <= attackRadius; i++)
                    {
                        // This vector represents every point to check within the pie slice
                        attackScanner = new Vector2(
                            (float)(ScreenLoc.X + i * Math.Sin(arcRotation + leftSide)),
                            (float)(ScreenLoc.Y - i * Math.Cos(arcRotation + leftSide)));

                        // If the point lies within the enemy's bounds, enemy takes damage from player's attack
                        if (enemyScreenRect.Contains(attackScanner))
                        {
                            elem.TakeDamage(damage);
                            //this makes sure the enemy doesn't take more than 1 damage from each attack
                            goto NextEnemy;
                        }
                    }
                }
            NextEnemy:;
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
            sb.Draw(arcImg, 
                new Rectangle((int)arcLoc.X, (int)arcLoc.Y, 80, 32),
                new Rectangle(0, 0, arcImg.Width, arcImg.Height), 
                Color.White,
                arcRotation,
                new Vector2(arcImg.Width / 2, arcImg.Height / 2), 
                SpriteEffects.None, 
                0);

            if (attack != null)
            {
                attack.Draw(sb);
                attack = null;
            }
        }

        public int Health
        {
            get { return health; }
            set 
            { 
                if(health <= value)
                {
                    health = value;
                }
                else
                {
                    TakeDamage(health - value);
                }
            }
        }

        /// <summary>
        /// the amount of damage the player does with each attack.
        /// </summary>
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public weaponType CurrentWeapon { get { return weapon; } set { weapon = value; } }
        public Texture2D WeaponImage { set { weaponImage = value; } }
    }
}
