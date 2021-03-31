using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TheHalls
{
    /// <summary>
    /// gameObject only takes a world location and size, and draw will adapt that to screen coordinates. 
    /// </summary>
    class GameObject
    {
        //Fields
        protected Vector2 worldLoc;
        protected Vector2 size;
        protected Texture2D image;
        protected Color tint;

        //Properties
        public Vector2 WorldLoc
        {
            get { return worldLoc; }
            set { worldLoc = value; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Texture2D Image
        {
            get { return image; }
        }

        public Color Tint
        {
            get { return tint; }
            set { tint = value; }
        }

        /// <summary>
        /// creates a new gameObject at the given location with the given size and image.
        /// </summary>
        /// <param name="worldLoc"></param>
        /// <param name="size"></param>
        /// <param name="image"></param>
        public GameObject(Vector2 worldLoc, Vector2 size, Texture2D image)
        {
            this.worldLoc = worldLoc;
            this.size = size;
            this.image = image;
            tint = Color.White;
        }

        /// <summary>
        /// draws the object, adjusted to the screenOffset.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="screenOffset"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle((int)(worldLoc.X - Game1.screenOffset.X), (int)(worldLoc.Y - Game1.screenOffset.Y), (int)size.X, (int)size.Y), tint);
        }

        /// <summary>
        /// returns true if the two objects are colliding.
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public bool Collides(GameObject toCheck)
        {
            if(this == toCheck)
            {
                return false;
            }
            return GetRect().Intersects(toCheck.GetRect());
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)worldLoc.X, (int)worldLoc.Y, (int)size.X, (int)size.Y);
        }

        /// <summary>
        /// Gets the location of the Game object relative to the screen, rather than the world
        /// </summary>
        public Vector2 ScreenLoc
        {
            get { return (worldLoc - Game1.screenOffset) + (Size / 2); }
        }

        /// <summary>
        /// Checks the player's location against all of the game objects passed in, and stops the player if they are overlapping.
        /// </summary>
        /// <param name="obstacles">what game objects to check collision against</param>
        public bool ResolveCollisions(List<GameObject> obstacles)
        {
            bool collides = false;
            Rectangle rect = GetRect();

            foreach (GameObject elem in obstacles)
            {
                if (!(elem == this))
                {
                    Rectangle obstacle = elem.GetRect();
                    if (obstacle.Intersects(rect))
                    {
                        collides = true;
                        Rectangle overlap = Rectangle.Intersect(obstacle, rect);
                        if (overlap.Width <= overlap.Height)
                        {
                            //X adjustment
                            if (obstacle.X > rect.X)
                            {
                                //obstacle is to the right of player 
                                rect.X -= overlap.Width;
                            }
                            else
                            {
                                //obstacle is to the left of the player
                                rect.X += overlap.Width;
                            }
                        }
                        else
                        {
                            //Y adjustment
                            if (obstacle.Y > rect.Y)
                            {
                                //obstacle is above the player
                                rect.Y -= overlap.Height;
                            }
                            else
                            {
                                //obstacle is below the player
                                rect.Y += overlap.Height;
                            }
                        }
                    }
                }
            }

            //sets the player location to the updated location
            worldLoc.X = (int)rect.X;
            worldLoc.Y = (int)rect.Y;
            if(collides && this is EnemyRanged)
            {
                ((EnemyRanged)this).Bounce();
            }
            return collides;
        }
    }
}
