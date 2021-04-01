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
        protected int health;
        protected float movementSpeed;
        protected bool alive;
        protected int attackCooldown;
        GameObject attack;

        public bool Alive
        {
            get { return alive; }
        }
        
        //Constructor for enemy
        public Enemy(Vector2 worldLoc, Vector2 size, Texture2D image) : base(worldLoc, size, image)
        {
            movementSpeed = 2.5f;
            health = 3;
            alive = true;
            attackCooldown = 90;
            Tint = Color.Red;
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

        }

        /// <summary>
        /// reduces the enemies health by the amount of damage. if health is <= to 0, they die. 
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            if(health <= 0)
            {
                alive = false;
            }
        }

        /// <summary>
        /// Tries to attack player
        /// </summary>
        /// <param name="player">player being attack</param>
        public void TryAttack(Player player)
        {
            attackCooldown--;
            if (attackCooldown <= 0)
            {
                Attack(player);
                attackCooldown = 90;
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

            attack = new GameObject(atkDirection * 50 + worldLoc, new Vector2(50, 50), image);
            attack.Tint = Color.Orange;
            if(attack.Collides(player))
            {
                player.TakeDamage(1);
            }
        }

        /// <summary>
        /// Draws enemys
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            // HP Bar
            sb.Draw(image, new Rectangle(
                (int)(ScreenLoc.X - (size.X / 2) - 5), 
                (int)(ScreenLoc.Y - (size.Y / 2) - 15), 
                (int)((size.X + 10) / 3 * health), 
                10), 
                Color.Red);

            base.Draw(sb);

            if(attack != null)
            {
                attack.Draw(sb);
                attack = null;
            }
        }
    }
}
