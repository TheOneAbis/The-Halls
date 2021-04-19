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
        //Fields
        protected int maxHealth;
        protected int currentHealth;
        protected float movementSpeed;
        protected bool alive;
        protected double attackCooldown;
        protected double attackInterval;
        GameObject attack;

        protected Texture2D[] animSheets;
        protected Texture2D attackImg;
        private int animTimer;
        private int animationFPS;
        private int srcRectX;

        protected Vector2 knockback;

        public bool Alive
        {
            get { return alive; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
        }
        
        //Constructor for enemy
        public Enemy(Vector2 worldLoc, Vector2 size, int Health, Texture2D[] spriteSheets, double attackInterval, Texture2D attackImg) : base(worldLoc, size, spriteSheets, 6, 150)
        {
            movementSpeed = 2.5f;
            maxHealth = Health;
            currentHealth = Health;
            alive = true;
            attackCooldown = attackInterval;
            this.attackInterval = attackInterval;
            Tint = Color.Red;
            animSheets = spriteSheets;
            animTimer = 0;
            srcRectX = 55;
            animationFPS = 6;
            this.attackImg = attackImg;
            knockback = Vector2.Zero;
        }

        /// <summary>
        /// This has the enemy move directly tsdowards the target - generally the player.
        /// </summary>
        /// <param name="target"></param>
        public virtual void Move(Player target, List<GameObject> obstacles)
        {
            Vector2 moveDirection = target.WorldLoc - worldLoc;

            if (!(moveDirection.X == 0 && moveDirection.Y == 0))
            {
                moveDirection.Normalize();
            }

            worldLoc += (moveDirection * movementSpeed);

            // If enemy was knocked back by a player hit
            if (knockback.Length() > 0)
            {
                worldLoc += knockback;
                knockback *= .92f; // Reduce knockback vector
            }
        }

        /// <summary>
        /// reduces the enemies health by the amount of damage, also knocking them back. if health is <= to 0, they die. 
        /// </summary>
        /// <param name="damage">The amount of damage to do to the enemy</param>
        /// <param name="dmgSource"> The source of the damage being dealt to this enemy</param>
        public void TakeDamage(int damage, Player dmgSource)
        {
            currentHealth -= damage;

            // Set knockback vector 
            knockback = (ScreenLoc - dmgSource.ScreenLoc) / 7;

            if(currentHealth <= 0)
            {
                alive = false;
            }
        }

        /// <summary>
        /// Tries to attack player
        /// </summary>
        /// <param name="player">player being attack</param>
        public void TryAttack(Player player, GameTime gameTime)
        {
            attackCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
            if (attackCooldown <= 0)
            {
                Attack(player);
                attackCooldown = attackInterval;
            }
            else if(attackCooldown <= .8 && base.currentAnim != 1)
            {
                PlayAnimation(1, false);
            }
        }

        /// <summary>
        /// Attacks player
        /// </summary>
        /// <param name="player">player being attack</param>
        public virtual void Attack(Player player)
        {
            Vector2 atkDirection = player.WorldLoc - worldLoc;

            if (!(atkDirection.X == 0 && atkDirection.Y == 0))
            {
                atkDirection.Normalize();
            }
            else
            {
                return;
            }

            attack = new GameObject(atkDirection * 50 + worldLoc, new Vector2(50, 50), attackImg);
            attack.Tint = Color.Orange;
            if(attack.Collides(player))
            {
                player.TakeDamage(1);
            }

            //PlayAnimation(1, false);
        }

        /// <summary>
        /// Draws enemys
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            // HP rectangle
            sb.Draw(animSheets[animSheets.Length - 1], new Rectangle(
                (int)(ScreenLoc.X - (size.X / 2) - 5),
                (int)(ScreenLoc.Y - (size.Y / 2) - 15),
                (int)((size.X + 10) / maxHealth * currentHealth),
                10),
                Color.Red);

            // Draw enemy sprite animation frames
            base.Draw(sb);
            /*
            if (animTimer % animationFPS == 0)
            {
                if (srcRectX >= 450)
                {
                    srcRectX = 55;
                }
                else
                {
                    srcRectX += 150;
                }
            }
            animTimer++;
            
            sb.Draw(animSheets[0],
               new Rectangle((int)(worldLoc.X - Game1.screenOffset.X), (int)(worldLoc.Y - Game1.screenOffset.Y), (int)size.X, (int)size.Y),
               new Rectangle(srcRectX, 50, 45, 52), Tint);
            */
            if(attack != null)
            {
                attack.Draw(sb);
                attack = null;
            }
        }
    }
}
