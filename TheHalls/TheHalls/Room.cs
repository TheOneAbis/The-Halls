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
        protected RoomData room;
        protected Vector2 roomOffset;

        public RoomData RoomInfo
        {
            get { return room; }
        }

        //this overload constructs the starter room
        public Room(RoomData room, Texture2D obstacle)
        {
            this.room = room;
            roomOffset = new Vector2(0, 0);
        }

        public Room(RoomData room, Room previous, Texture2D obstacle, Vector2 roomOffset)
        {
            //adjacentRooms[room.inDirection] = previous;
            this.room = room;
            this.roomOffset = roomOffset;
            foreach(GameObject obs in room.obstacles)
            {
                obs.WorldLoc += roomOffset;
            }
            //foreach did work on this for some reason
            for(int i = 0; i < room.enemySpawns.Count; i++)
            {
                room.enemySpawns[i] += roomOffset;
            }

            room.outDoor.WorldLoc += roomOffset;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(GameObject obstacle in room.obstacles)
            {
                obstacle.Draw(sb);
            }
        }
    }
}
