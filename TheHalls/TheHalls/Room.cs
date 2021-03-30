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

        public Room(RoomData room, Room previous, Texture2D obstacle)
        {
            adjacentRooms[room.inDirection] = previous;
            this.room = room;
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
