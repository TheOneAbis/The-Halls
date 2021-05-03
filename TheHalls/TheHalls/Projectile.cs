using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    class Projectile : GameObject
    {
        private Player target;
        private Vector2 worldVel;
        private bool active;
        private float rotation;

        public Projectile(Vector2 worldLoc, Vector2 size, Texture2D image, Vector2 worldVel, Player target) : 
            base(worldLoc, size, new Texture2D[] { image } , 6, new Vector2(48, 48), new Vector2(0, 0), 48)
        {
            this.target = target;
            this.worldVel = worldVel;
            active = true;
            rotation = (float)Math.Atan(worldVel.Y / worldVel.X);
            Tint = Color.Orange;
        }


        public void Update(List<GameObject> obstacles, EnemyRanged shooter)
        {
            if (active)
            {
                worldLoc += worldVel;

                if (Collides(target))
                {
                    target.TakeDamage(1);
                    active = false;
                }
            }

            foreach(GameObject elem in obstacles)
            {
                if(Collides(elem) && elem != shooter)
                {
                    active = false;
                }
            }
        }
        
        public override void Draw(SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(sb);
            }
        }
    }
}