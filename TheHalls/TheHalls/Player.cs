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
        private Texture2D arcImgSword;
        private Texture2D arcImgSpear;
        private Texture2D weaponImage;
        private float arcRotation;
        private float movementSpeed;
        private int health;
        private int damage;
        private int attackRadius;
        private weaponType weapon;
        private GameOver gameOver;
        private Color arcOpacity;
        private double attackSpeed;

        // Did the playe press the interact key?
        private bool interacting;

        private float weaponDrawOffset;

        private Vector2 prevMoveDirection;

        private int invulnerableFrames;

        /// <summary>
        /// Creates the player object
        /// </summary>
        /// <param name="worldLoc">location to spawn the player in</param>
        /// <param name="size">size of the player</param>
        /// <param name="image">image to display for the player</param>
        /// <param name="arcImage">image to display for the arc of the players attacks</param>
        /// <param name="gameOver">method to be called when the player dies</param>
        public Player(Vector2 worldLoc, Vector2 size, Texture2D[] spriteSheets, Texture2D arcImageSword, Texture2D arcImageSpear,Texture2D weaponImage, GameOver gameOver) :
            base(worldLoc, size, spriteSheets, 6, new Vector2(18, 18), new Vector2(7, 7), 32)
        {
            arcImgSword = arcImageSword;
            arcImgSpear = arcImageSpear;

            arcRotation = 0;
            arcOpacity = new Color(155, 155, 155, 255);
            movementSpeed = 4.0f;
            this.gameOver = gameOver;
            health = 5;
            attackRadius = 75;
            this.weaponImage = weaponImage;
            damage = 1;
            weapon = weaponType.Sword;
            attackSpeed = 36;
            prevMoveDirection = Vector2.Zero;
            weaponDrawOffset = (float)Math.Sqrt(2) / 4;
            interacting = false;
        }

        /// <summary>
        /// Moves the character based on WASD
        /// </summary>
        public void Move(KeyboardState kb)
        {
            // Move player
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

            if (prevMoveDirection != moveDirection)
            {
                if(moveDirection.X > .2)
                {
                    PlayAnimation(1, true);
                }
                else if(moveDirection.X < -.2)
                {
                    PlayAnimation(2, true);
                }
                else
                {
                    if(moveDirection.Y > 0)
                    {
                        PlayAnimation(3, true);
                    }
                    else if(moveDirection.Y < 0)
                    {
                        PlayAnimation(4, true);
                    }
                    else
                    {
                        PlayAnimation(0, true);
                    }
                }
            }

            worldLoc += (moveDirection * movementSpeed);
            prevMoveDirection = moveDirection;
        }

        /// <summary>
        /// Updates player's weapon slash arc vector and arc rotation
        /// </summary>
        /// <param name="mouse">Mouse cursor location</param>
        public void Aim(MouseState mouse, List<Enemy> targets)
        {
            // Decrement attack cooldown
            attackSpeed--;
            if (invulnerableFrames != 0)
            {
                invulnerableFrames--;
                if(invulnerableFrames == 0)
                {
                    tint = Color.White;
                }
            }

            //this resets the sword image back to a 'normal' offset, after a few in between frames
            if(attackSpeed == 32)
            {
                weaponDrawOffset *= -2;
            }

            arcLoc = ScreenLoc + (Vector2.Normalize(new Vector2(mouse.X, mouse.Y) - ScreenLoc) * attackRadius);

            arcRotation = mouse.Y - ScreenLoc.Y > 0 ?
                (float)Math.Acos((arcLoc.X - ScreenLoc.X) / (arcLoc - ScreenLoc).Length()) + (float)(Math.PI / 2) :
                -1 * (float)Math.Acos((arcLoc.X - ScreenLoc.X) / (arcLoc - ScreenLoc).Length()) + (float)(Math.PI / 2);

            // By default, there are no enemies in range yet
            

            // Are any enemies in attack range?
            if (attackSpeed <= 0)
            {
                arcOpacity = Color.DarkGray;
                foreach (Enemy elem in targets)
                {
                    if (ScanAttackArc(elem))
                    {
                        arcOpacity = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// spawns a gameObject 50 units away from the player in the direction of arcRotation. checks collision against passed in enemies, dealing damage if they overlap.
        /// </summary>
        /// <param name="targets"></param>
        public void Attack(List<Enemy> targets)
        {
            if (attackSpeed <= 0)
            {
                arcOpacity = new Color(255, 155, 155, 255);
                // Iterate for each enemy
                foreach (Enemy elem in targets)
                {
                    
                    if (ScanAttackArc(elem))
                    {
                        elem.TakeDamage(damage, this);
                        arcOpacity = Color.IndianRed;
                    }
                }
                // Set attack cooldown based on weapon type
                switch (weapon)
                {
                    case weaponType.Sword:
                        attackSpeed = 36;
                        break;

                    case weaponType.Spear:
                        attackSpeed = 55;
                        break;
                }

                //start rotating the weapon image, only matters with swords
                weaponDrawOffset /= 2;
            }
        }

        private bool ScanAttackArc(Enemy target)
        {
            Vector2 attackScanner;
            Rectangle enemyScreenRect;

            // This rectangle gets the rectangle location of the enemy relative to the screen
            enemyScreenRect = new Rectangle((int)(target.ScreenLoc.X - (target.Size / 2).X),
                (int)(target.ScreenLoc.Y - (target.Size / 2).Y), (int)target.Size.X, (int)target.Size.Y);

            switch (weapon)
            {
                // SWORD ATTACKING ALGORITHM
                case weaponType.Sword:

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
                                return true;
                            }
                        }
                    }
                    break;

                // SPEAR ATTACKING ALGORITHM
                case weaponType.Spear:

                    for (double leftSide = -(Math.PI / 20); leftSide < (Math.PI / 20); leftSide += Math.PI / 64)
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
                                return true;
                            }
                        }
                    }
                    break;
            }

            // If nothing detected
            return false;
        }

        /// <summary>
        /// subtracts from the players health. if their health is 0, call gameOver.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            if (invulnerableFrames <= 0)
            {
                invulnerableFrames = 10;
                tint = Color.Red;
                health -= damage;
                if (health <= 0)
                {
                    gameOver();
                }
            }
        }

        /// <summary>
        /// Sets player's interact field to if the player's interact key (E) was pressed
        /// </summary>
        /// <param name="kb">Current frame keyboard state</param>
        /// <param name="prevkb">previous frame keyboard state</param>
        public void SetIsInteracting(KeyboardState kb, KeyboardState prevkb)
        {
            interacting =  kb.IsKeyDown(Keys.E) && prevkb.IsKeyUp(Keys.E);
        }

        /// <summary>
        /// Draws the player and the slash arc to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch from Game1.Draw()</param>
        public override void Draw(SpriteBatch sb)
        {
            //Draw the weapon
            switch (weapon)
            {
                case weaponType.Sword:
                    sb.Draw(Game1.sword,
                        new Rectangle((int)ScreenLoc.X, (int)ScreenLoc.Y, 50, 50),
                        null,
                        Color.White,
                        arcRotation - (float)Math.Sqrt(2) / 2 + weaponDrawOffset,
                        new Vector2(0, 27),
                        SpriteEffects.None,
                        0);
                    break;

                case weaponType.Spear:
                    sb.Draw(Game1.spear,
                        new Rectangle((int)ScreenLoc.X, (int)ScreenLoc.Y, 75, 75),
                        null,
                        Color.White,
                        arcRotation - (float)Math.Sqrt(2) / 2 - .15f,
                        new Vector2(0, 175),
                        SpriteEffects.None,
                        0);
                    break;
            }

            // Draw player
            base.Draw(sb);
            Texture2D arcImg = arcImgSword;
            if(weapon == weaponType.Spear)
            {
                arcImg = arcImgSpear;
            }
            // Draw player weapon slash arc
            sb.Draw(arcImg, 
                new Rectangle((int)arcLoc.X, (int)arcLoc.Y, 80, 32),
                new Rectangle(0, 0, arcImg.Width, arcImg.Height), 
                arcOpacity,
                arcRotation,
                new Vector2(arcImg.Width / 2, arcImg.Height / 2), 
                SpriteEffects.None, 
                0);
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

        public weaponType CurrentWeapon 
        { 
            get { return weapon; } 
            set 
            { 
                weapon = value;

                // Set attack radius based on what weapon is equipped
                switch (weapon)
                {
                    case weaponType.Sword:
                        attackRadius = 75;
                        break;
                    case weaponType.Spear:
                        attackRadius = 125; 
                        break;
                }
            } 
        }
        public Texture2D WeaponImage { set { weaponImage = value; } }

        /// <summary>
        /// Gets if the player pressed their interact key
        /// </summary>
        public bool IsInteracting
        {
            get { return interacting; }
        }
    }
}
