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

        public Projectile(Vector2 worldLoc, Vector2 size, Texture2D image, Vector2 worldVel, Player target) : base(worldLoc, size, image)
        {
            this.target = target;
            this.worldVel = worldVel;
            active = true;
            rotation = (float)Math.Atan(worldVel.Y / worldVel.X);
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
                sb.Draw(
                    image,
                    new Rectangle((int)(worldLoc.X - Game1.screenOffset.X + 12.5), (int)(worldLoc.Y - Game1.screenOffset.Y), 50, 25),
                    null,
                    Tint,
                    rotation,
                    //Vector2.Zero,
                    new Vector2(image.Width /2, image.Height /2),
                    SpriteEffects.None,
                    0f);
                //base.Draw(sb);
                
            }
        }
    }
}