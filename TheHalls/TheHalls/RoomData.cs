using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace TheHalls
{
    struct RoomData
    {
        private List<GameObject> obstacles;
        private List<GameObject> outDoor;
        private Direction inDirection;
        private Direction outDirection;
        private Rectangle enemySpawnArea;
        private BinaryReader reader;

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
                    copy.Add(new GameObject(obstacle.WorldLoc, obstacle.Size, obstacle.Image, obstacle.TileNum));
                }
                return copy;
            }
        }

        /// <summary>
        /// gets a copy of the exit door of this room.
        /// </summary>
        public List<GameObject> OutDoor
        {
            get
            {
                List<GameObject> copy = new List<GameObject>();
                foreach (GameObject doorTile in outDoor)
                {
                    copy.Add(new GameObject(doorTile.WorldLoc, doorTile.Size, doorTile.Image, doorTile.TileNum));
                }
                return copy;
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
        public Rectangle EnemySpawnArea
        {
            get
            {
                return new Rectangle(enemySpawnArea.Location, enemySpawnArea.Size);
            }
        }

        public RoomData(string roomFileName, Direction inDirection, Direction outDirection, Rectangle enemySpawnArea, Texture2D tileSheet)
        {
            obstacles = new List<GameObject>();
            outDoor = new List<GameObject>(); // Ideally this will be given a value if the level was designed correctly
            int tileIndex;

            reader = new BinaryReader(File.OpenRead($"Content/{roomFileName}.room"));

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    tileIndex = reader.ReadInt32();
                    // 110 is the index for the up spikes, the closed entrance and exit tile of the game.
                    // If it is this index, this is the outDoor. If not, it's just another tile.
                    if (tileIndex == 110)
                    {
                        outDoor.Add(new GameObject(new Vector2(i * 50, j * 50), new Vector2(50, 50), tileSheet, tileIndex));
                    }
                    else
                    {
                        obstacles.Add(
                        new GameObject(new Vector2(i * 50, j * 50), new Vector2(50, 50), tileSheet, tileIndex));
                    }
                }
            }
            reader.Close();
            this.inDirection = inDirection;
            this.outDirection = outDirection;
            this.enemySpawnArea = enemySpawnArea;
        }
    }
}
