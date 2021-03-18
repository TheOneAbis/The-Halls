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
        Player target;
        Vector2 worldVel;
        bool active;

        public Projectile(Vector2 worldLoc, Vector2 size, Texture2D image, Vector2 worldVel, Player target) : base(worldLoc, size, image)
        {
            this.target = target;
            this.worldVel = worldVel;
            active = true;
        }


        public void Update()
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
        }

        public override void Draw(SpriteBatch sb)
        {
            if (active)
            {
                //sb.Draw();
                base.Draw(sb);
            }
        }
    }
}