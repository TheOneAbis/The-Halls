using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TheHalls
{
    class Player : GameObject
    {
        public delegate void GameOver();

        private Vector2 arcLoc;
        private Texture2D arcImgSword;
        private Texture2D arcImgSpear;
        private Texture2D swordImage;
        private Texture2D spearImage;
        private float arcRotation;
        private float movementSpeed;
        private int health;
        private int damage;
        private int attackRadius;
        private double arcLength;
        private weaponType weapon;
        private GameOver gameOver;
        private Color arcOpacity;
        private double attackSpeed;

        private Vector2 DodgeVector;
        private int dodgeCooldown;
        private int dodgeTime;
        private bool isDodging;
        private int dodgeSpeed;

        // Did the player press the interact key?
        private bool interacting;

        private float weaponDrawOffsetAngular;
        private Vector2 weaponDrawOffsetTrans;

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
        public Player(Vector2 worldLoc, Vector2 size, Texture2D[] spriteSheets, Texture2D arcImageSword, Texture2D arcImageSpear,Texture2D swordImage, Texture2D spearImage, GameOver gameOver) :
            base(worldLoc, size, spriteSheets, 6, new Vector2(18, 18), new Vector2(7, 7), 32)
        {
            arcImgSword = arcImageSword;
            arcImgSpear = arcImageSpear;

            arcRotation = 0;
            arcOpacity = new Color(155, 155, 155, 255);
            movementSpeed = 4.5f;
            this.gameOver = gameOver;
            health = 5;
            attackRadius = 75;
            this.swordImage = swordImage;
            this.spearImage = spearImage;
            damage = 1;
            weapon = weaponType.Sword;
            attackSpeed = 28;
            arcLength = Math.PI / 8;
            prevMoveDirection = Vector2.Zero;
            weaponDrawOffsetAngular = (float)Math.Sqrt(2) / 4;
            //after game start, this value gets reduced once (so its values are increased from 0, 175 so it ends in its default position
            weaponDrawOffsetTrans = new Vector2(-50, 200);

            interacting = false;
            dodgeCooldown = 1;
            dodgeTime = 15; //player dodges for 15 frames, or .25s
            DodgeVector = Vector2.Zero;
            dodgeSpeed = 14;
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

            dodgeCooldown--;

            // Dodging mechanics will go here
            if (isDodging)
            {
                worldLoc += DodgeVector;
                dodgeTime--;
                if (dodgeTime <= 0)
                {
                    isDodging = false;
                    dodgeTime = 15;
                }
            }
            else
            {
                worldLoc += (moveDirection * movementSpeed);
            }
            
            prevMoveDirection = moveDirection;
        }

        /// <summary>
        /// Dodges the player, giving them a sudden boost of speed in their current movement direction.
        /// </summary>
        public void Dodge()
        {
            if (dodgeCooldown <= 1)
            {
                isDodging = true;

                // If player isn't moving, they will by default dodge up
                if (prevMoveDirection.Length() == 0)
                {
                    DodgeVector = new Vector2(0, -dodgeSpeed);
                }
                else
                {
                    DodgeVector = new Vector2(prevMoveDirection.X / prevMoveDirection.Length(), prevMoveDirection.Y / prevMoveDirection.Length()) * dodgeSpeed;
                }
                dodgeCooldown = 130; // = 2.15 second cooldown
            }
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
            if(attackSpeed == 25)
            {
                weaponDrawOffsetAngular *= -2;
                
            }

            if(attackSpeed == 35 || attackSpeed == 25)
            {
                weaponDrawOffsetTrans += new Vector2(50, -25);
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
        public void Attack(List<Enemy> targets, SoundEffect[] attackSFX)
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
                        attackSFX[0].Play(); // Sword slash sfx
                        attackSpeed = 28;
                        break;

                    case weaponType.Spear:
                        attackSFX[1].Play(); // Spear thrust sfx
                        attackSpeed = 43;
                        break;
                }

                //start rotating the weapon image, only matters with swords
                weaponDrawOffsetAngular /= 2;

                //move the spear (if its a spear)
                weaponDrawOffsetTrans = new Vector2(-100, 225);
            }
        }

        private bool ScanAttackArc(Enemy target)
        {
            Vector2 attackScanner;
            Rectangle enemyScreenRect;

            // This rectangle gets the rectangle location of the enemy relative to the screen
            enemyScreenRect = new Rectangle((int)(target.ScreenLoc.X - (target.Size / 2).X),
                (int)(target.ScreenLoc.Y - (target.Size / 2).Y), (int)target.Size.X, (int)target.Size.Y);

            //     ---   SCAN THE PIE SLICE   ---
            
            //  Vector rotates clockwise starting at left side of pie slice
            for (double leftSide = -arcLength; leftSide < arcLength; leftSide += Math.PI / 64)
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
                invulnerableFrames = 15;
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
                    sb.Draw(swordImage,
                        new Rectangle((int)ScreenLoc.X, (int)ScreenLoc.Y, 50, 50),
                        null,
                        Color.White,
                        arcRotation - (float)Math.Sqrt(2) / 2 + weaponDrawOffsetAngular,
                        new Vector2(0, 27),
                        SpriteEffects.None,
                        0);
                    break;

                case weaponType.Spear:
                    sb.Draw(spearImage,
                        new Rectangle((int)ScreenLoc.X, (int)ScreenLoc.Y, 75, 75),
                        null,
                        Color.White,
                        arcRotation - (float)Math.Sqrt(2) / 2 - .15f,
                        weaponDrawOffsetTrans,
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

            // Draw dodge cooldown bar
            if (dodgeCooldown > 1)
            {
                sb.Draw(Game1.debugSquare, new Rectangle((int)ScreenLoc.X - 50, (int)ScreenLoc.Y + 40, 
                    GetRect().Width + 50, 10), new Color(50, 50, 50, 50));

                sb.Draw(Game1.debugSquare, new Rectangle((int)ScreenLoc.X - 50, (int)ScreenLoc.Y + 40, 
                    (int)((GetRect().Width + 50.0) * ((130.0 - dodgeCooldown) / 130.0)), 10), new Color(200, 200, 200));
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
                        arcLength = Math.PI / 8;
                        break;
                    case weaponType.Spear:
                        attackRadius = 125;
                        arcLength = Math.PI / 22;
                        break;
                }
            } 
        }
        //public Texture2D WeaponImage { set { weaponImage = value; } }

        /// <summary>
        /// Gets if the player pressed their interact key
        /// </summary>
        public bool IsInteracting
        {
            get { return interacting; }
        }
    }
}
