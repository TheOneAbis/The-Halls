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
        public List<GameObject> obstacles;
        public GameObject outDoor;
        public Direction inDirection;
        public Direction outDirection;
        public List<Vector2> enemySpawns;


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
