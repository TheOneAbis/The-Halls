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
        protected GameObject outDoor;
        protected Direction inDirection;
        protected Direction outDirection;
        protected List<Vector2> enemySpawns;
        protected Vector2 roomOffset;

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

        public List<Vector2> EnemySpawns
        {
            get { return enemySpawns; }
        }
        /// <summary>
        /// this overload constructs the starter room
        /// </summary>
        /// <param name="room"></param>
        /// <param name="obstacle"></param>
        public Room(RoomData roomTemplate, Texture2D obstacle)
        {
            obstacles = roomTemplate.Obstacles;
            outDoor = roomTemplate.OutDoor;
            inDirection = roomTemplate.InDirection;
            outDirection = roomTemplate.OutDirection;
            enemySpawns = roomTemplate.EnemySpawns;

            roomOffset = new Vector2(0, 0);
        }

        public Room(RoomData roomTemplate, Room previous, Texture2D obstacle, Vector2 roomOffset)
        {
            //adjacentRooms[room.inDirection] = previous;
            obstacles = roomTemplate.Obstacles;
            outDoor = roomTemplate.OutDoor;
            inDirection = roomTemplate.InDirection;
            outDirection = roomTemplate.OutDirection; 
            enemySpawns = roomTemplate.EnemySpawns;

            this.roomOffset = roomOffset;
            foreach(GameObject obs in obstacles)
            {
                obs.WorldLoc += roomOffset;
            }
            //foreach did work on this for some reason
            for(int i = 0; i < enemySpawns.Count; i++)
            {
                enemySpawns[i] += roomOffset;
            }

            outDoor.WorldLoc += roomOffset;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(GameObject obstacle in obstacles)
            {
                obstacle.Draw(sb);
            }
        }
    }
}
