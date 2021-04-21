﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    public enum MoveState
    {
        Inward,
        Outward,
        Clockwise,
        CounterClockwise
    }

    class EnemyRanged : Enemy
    {
        private MoveState moveState;
        private int innerRadius;
        private int outerRadius;
        private int avgRadius;
        private Projectile projectile;
        private Texture2D[] animSheets;
        private int animTimer;
        private int animationFPS;
        private int srcRectX;
        private Texture2D projectileImage;

        public EnemyRanged(Vector2 worldLoc, Vector2 size, int Health, Texture2D[] spriteSheets, double attackInterval, Texture2D projectile) : base (worldLoc, size, Health, spriteSheets, attackInterval, projectile)
        {
            moveState = MoveState.Inward;
            innerRadius = 100;
            outerRadius = 300;
            avgRadius = innerRadius + outerRadius / 2;
            movementSpeed = 2;
            Tint = Color.DarkRed;
            animSheets = spriteSheets;
            animTimer = 0;
            srcRectX = 55;
            animationFPS = 6;
            projectileImage = projectile;
        }

        /// <summary>
        /// This override of Move makes the enemy move in different directions based on where they are in relation to the player.
        /// </summary>
        /// <param name="target">the object which the enemy will circle - should be the player</param>
        public override void Move(Player target, List<GameObject> obstacles)
        {
            //if theres a projectile, move it
            if(projectile != null)
            {
                projectile.Update(obstacles);
            }
            Vector2 towardsPlayer = target.WorldLoc - worldLoc;

            // If enemy was knocked back by a player hit
            if (knockback.Length() > 0)
            {
                worldLoc += knockback;
                knockback *= .8f; // Reduce knockback vector
            }

            switch (moveState)
            {
                case MoveState.Inward:
                    if (towardsPlayer.Length() < innerRadius)
                    {
                        moveState = MoveState.Outward;
                    }
                    else if(towardsPlayer.Length() < avgRadius)
                    {
                        moveState = MoveState.Clockwise;
                    }
                    else
                    {
                        if (!(towardsPlayer.X == 0 && towardsPlayer.Y == 0))
                        {
                            towardsPlayer.Normalize();
                        }

                        worldLoc += (towardsPlayer * movementSpeed);
                    }
                    break;

                case MoveState.Outward:
                    if (towardsPlayer.Length() > outerRadius)
                    {
                        moveState = MoveState.Inward;
                    }
                    else if(towardsPlayer.Length() > avgRadius)
                    {
                        moveState = MoveState.Clockwise;
                    }
                    else
                    {

                        if (!(towardsPlayer.X == 0 && towardsPlayer.Y == 0))
                        {
                            towardsPlayer.Normalize();
                        }

                        worldLoc -= (towardsPlayer * movementSpeed);
                    }
                    break;

                case MoveState.Clockwise:
                    if (towardsPlayer.Length() > outerRadius)
                    {
                        moveState = MoveState.Inward;
                    }
                    else if (towardsPlayer.Length() < innerRadius)
                    {
                        moveState = MoveState.Outward;
                    }
                    else
                    {
                        towardsPlayer = new Vector2(-towardsPlayer.Y, towardsPlayer.X);

                        if (!(towardsPlayer.X == 0 && towardsPlayer.Y == 0))
                        {
                            towardsPlayer.Normalize();
                        }

                        worldLoc -= (towardsPlayer * movementSpeed);
                    }
                    break;

                case MoveState.CounterClockwise:
                    if (towardsPlayer.Length() > outerRadius)
                    {
                        moveState = MoveState.Inward;
                    }
                    else if (towardsPlayer.Length() < innerRadius)
                    {
                        moveState = MoveState.Outward;
                    }
                    else
                    {
                        towardsPlayer = new Vector2(towardsPlayer.Y, -towardsPlayer.X);

                        if (!(towardsPlayer.X == 0 && towardsPlayer.Y == 0))
                        {
                            towardsPlayer.Normalize();
                        }

                        worldLoc -= (towardsPlayer * movementSpeed);
                    }
                    break;
            }
        }

        /// <summary>
        /// if this enemy is circling the player, this reverses their spin (if they are going clockwise, they start going counterclockwise)
        /// </summary>
        public void Bounce()
        {
            switch(moveState)
            {
                case MoveState.CounterClockwise:
                    moveState = MoveState.Clockwise;
                    break;

                case MoveState.Clockwise:
                    moveState = MoveState.CounterClockwise;
                    break;
            }
        }

        /// <summary>s
        /// spawns a new projectile
        /// </summary>
        /// <param name="player"></param>
        public override void Attack(Player player)
        {
            projectile = new Projectile(worldLoc + (Size/2), new Vector2(30, 30), projectileImage, Vector2.Normalize((player.WorldLoc - worldLoc))*10, player);
        }

        /// <summary>
        /// draws the ranged enemy and their projectile
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
            /*
            if (animTimer % animationFPS == 0)
            {
                if (srcRectX >= 1105)
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
               new Rectangle(srcRectX, 60, 45, 45), Tint);
            */
            base.Draw(sb);

            if (projectile != null)
            {
                projectile.Draw(sb);

            }
        }
    }
}
