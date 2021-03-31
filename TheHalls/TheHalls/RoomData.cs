using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    struct RoomData
    {
        private List<GameObject> obstacles;
        private GameObject outDoor;
        private Direction inDirection;
        private Direction outDirection;
        private List<Vector2> enemySpawns;

        /// <summary>
        /// gets a copy of the obstacles.
        /// </summary>
        public List<GameObject> Obstacles
        {
            get 
            {
                List<GameObject> copy = new List<GameObject>();
                foreach(GameObject obstacle in obstacles)
                {
                    copy.Add(new GameObject(obstacle.WorldLoc, obstacle.Size, obstacle.Image));
                }
                return copy;
            }
        }

        /// <summary>
        /// gets a copy of the exit door of this room.
        /// </summary>
        public GameObject OutDoor
        {
            get
            {
                return new GameObject(outDoor.WorldLoc, outDoor.Size, outDoor.Image);
            }
        }

        public Direction InDirection
        {
            get { return inDirection; }
        }

        public Direction OutDirection
        {
            get { return outDirection; }
        }

        /// <summary>
        /// returns a copy of the enemy spawns.
        /// </summary>
        public List<Vector2> EnemySpawns
        {
            get
            {
                List<Vector2> copy = new List<Vector2>();
                foreach (Vector2 enemySpawn in enemySpawns)
                {
                    copy.Add(new Vector2(enemySpawn.X, enemySpawn.Y));
                }
                return copy;
            }
        }



        public RoomData(List<GameObject> obstacles, GameObject outDoor, Direction inDirection, Direction outDirection, List<Vector2> enemySpawns)
        {
            this.obstacles = obstacles;
            this.outDoor = outDoor;
            this.inDirection = inDirection;
            this.outDirection = outDirection;
            this.enemySpawns = enemySpawns;
        }
    }
}
