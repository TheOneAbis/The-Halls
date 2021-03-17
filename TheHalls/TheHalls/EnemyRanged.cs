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

        public EnemyRanged(Vector2 worldLoc, Vector2 size, Texture2D image) : base (worldLoc, size, image)
        {
            moveState = MoveState.Inward;
            innerRadius = 100;
            outerRadius = 300;
            avgRadius = innerRadius + outerRadius / 2;
            movementSpeed = 2;
        }

        public override void Move(GameObject target)
        {
            Vector2 towardsPlayer = target.WorldLoc - worldLoc;

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
    }
}
