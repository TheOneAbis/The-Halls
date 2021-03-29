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
        protected Direction outDirection;
        protected Direction inDirection;
        protected bool set;

        public Room()
        {

        }
    }
}
