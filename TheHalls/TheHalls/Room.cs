using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class Room
    {
        protected Dictionary<Direction, Room> adjacentRooms;
        protected List<GameObject> obstacles;
        protected List<GameObject> outDoor;
        protected Direction inDirection;
        protected Direction outDirection;
        protected Rectangle enemySpawnArea;
        protected Vector2 roomOffset;
        protected Texture2D tiles;

        public List<GameObject> Obstacles
        {
            get { return obstacles; }
        }

        public Vector2 RoomOffset
        {
            get { return roomOffset; }
        }

        public Direction OutDirection
        {
            get { return outDirection; }
        }

        public List<GameObject> OutDoor
        {
            get { return outDoor; }
        }

        public Rectangle EnemySpawnArea
        {
            get { return enemySpawnArea; }
        }
        /// <summary>
        /// this overload constructs the starter room
        /// </summary>
        /// <param name="room"></param>
        /// <param name="obstacle"></param>
        public Room(RoomData roomTemplate, Texture2D tiles)
        {
            obstacles = roomTemplate.Obstacles;
            outDoor = roomTemplate.OutDoor;
            inDirection = roomTemplate.InDirection;
            outDirection = roomTemplate.OutDirection;
            enemySpawnArea = roomTemplate.EnemySpawnArea;
            this.tiles = tiles;

            roomOffset = new Vector2(0, 0);
        }

        public Room(RoomData roomTemplate, Room previous, Texture2D tiles, Vector2 roomOffset)
        {
            //adjacentRooms[room.inDirection] = previous;
            obstacles = roomTemplate.Obstacles;
            outDoor = roomTemplate.OutDoor;
            inDirection = roomTemplate.InDirection;
            outDirection = roomTemplate.OutDirection; 
            enemySpawnArea = roomTemplate.EnemySpawnArea;
            this.tiles = tiles;

            this.roomOffset = roomOffset;
            foreach(GameObject obs in obstacles)
            {
                obs.WorldLoc += roomOffset;
            }

            enemySpawnArea.X += (int)roomOffset.X;
            enemySpawnArea.Y += (int)roomOffset.Y;

            foreach (GameObject doorTile in outDoor)
            {
                doorTile.WorldLoc += roomOffset;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(GameObject obstacle in obstacles)
            {
                obstacle.DrawTile(sb);
            }

            foreach(GameObject doorTile in outDoor)
            {
                doorTile.DrawTile(sb);
            }
        }
    }
}
