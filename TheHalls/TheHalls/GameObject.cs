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

        protected bool animated;
        protected Texture2D[] spriteSheets;
        protected int currentAnim;
        protected int returnAnim;
        protected int currentFrameX;
        protected Vector2 frameOffset;

        protected float animFps;
        protected Vector2 frameSize;
        protected int animFramesElapsed;
        protected int interframeDistX;

        // Integer array of wall tiles
        protected List<int> wallIndices;
        // Integer array of floor tiles
        protected List<int> floorIndices;
        protected bool collidable;    // Is this a floor or wall tile?
        protected int tileIndex;


        //Properties
        public int TileNum
        {
            get { return tileIndex; }
            set { tileIndex = value; }
        }

        public bool IsCollidable
        {
            get { return collidable; }
            set { collidable = value; }
        }

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

        public bool Animated
        {
            get { return animated; }
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
            animated = false;
            collidable = true;
        }

        /// <summary>
        /// creates a new gameObject at the given location with the given size and image (FOR TILE OBJECTS).
        /// </summary>
        /// <param name="worldLoc"></param>
        /// <param name="size"></param>
        /// <param name="image"></param>
        /// <param name="TileIndex">The index of the tile sprite in the spritesheet</param>
        public GameObject(Vector2 worldLoc, Vector2 size, Texture2D image, int TileIndex)
        {
            this.worldLoc = worldLoc;
            this.size = size;
            this.image = image;
            tint = Color.White;
            animated = false;
            collidable = false;
            tileIndex = TileIndex;

            floorIndices = new List<int> 
            { 0, 1, 2, 3, 4, 5, 8, 9, 10, 11, 12, 15, 16, 17,
                18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 30, 31, 32, 33, 35, 36, 37,
                38, 39, 40, 41, 42, 45, 46, 47, 48, 50, 51, 52, 53, 54, 64, 66, 69,
                105, 125, 139, 150, 151, 152, 165, 166, 167, 168 };

            wallIndices = new List<int> 
            { 60, 61, 62, 63, 75, 76, 77, 78, 90, 91, 92, 93,
                106, 107, 108, 109, 110, 120, 121, 122, 123, 124, 135, 136, 137, 153 };

            // Is this object tile a wall?
            if (wallIndices.Contains(tileIndex))
            {
                collidable = true;
            }
        }

        /// <summary>
        /// creates a new animated gameObj
        /// </summary>
        /// <param name="worldLoc"></param>
        /// <param name="size"></param>
        /// <param name="spriteSheets"></param>
        /// <param name="animFps"></param>
        /// <param name="frameSizeX"></param>
        public GameObject(Vector2 worldLoc, Vector2 size, Texture2D[] spriteSheets, float animFps, Vector2 frameSize, Vector2 frameOffset, int interframeDistX)
        {
            this.worldLoc = worldLoc;
            this.size = size;
            this.spriteSheets = spriteSheets;
            this.animFps = animFps;
            this.frameSize = frameSize;
            this.frameOffset = frameOffset;
            tint = Color.White;
            animated = true;
            collidable = true;

            currentFrameX = (int)frameOffset.X;
            currentAnim = 0;
            this.interframeDistX = interframeDistX;
        }

        /// <summary>
        /// draws the object, adjusted to the screenOffset.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="screenOffset"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (animated)
            {
                if(Game1.gameState == GameState.Game)
                {
                    animFramesElapsed++;
                }
                
                if (animFramesElapsed > animFps)
                {
                    animFramesElapsed = 0;
                    currentFrameX += interframeDistX;
                    if(currentFrameX > currentFrameX % spriteSheets[currentAnim].Width)
                    {
                        currentFrameX = (int)frameOffset.X;
                        currentAnim = returnAnim;
                    }
                }

                sb.Draw(spriteSheets[currentAnim],
                    new Rectangle((int)(worldLoc.X - Game1.screenOffset.X), (int)(worldLoc.Y - Game1.screenOffset.Y), (int)size.X, (int)size.Y),
                    new Rectangle(currentFrameX, (int)frameOffset.Y, (int)frameSize.X, (int)frameSize.Y), Tint);
            }
            else
            {
                sb.Draw(image, 
                    new Rectangle((int)(worldLoc.X - Game1.screenOffset.X), 
                    (int)(worldLoc.Y - Game1.screenOffset.Y), 
                    (int)size.X, (int)size.Y), tint);
            }
        }

        /// <summary>
        /// Overload of original Draw method that should only take non-animated objects,
        /// Utilizes a source rect for drawing from tile sprite sheets
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="srcRect"></param>
        public virtual void Draw(SpriteBatch sb, Rectangle srcRect)
        {
            sb.Draw(image,
                    new Rectangle((int)(worldLoc.X - Game1.screenOffset.X),
                    (int)(worldLoc.Y - Game1.screenOffset.Y),
                    (int)size.X, (int)size.Y), srcRect, tint);
        }

        /// <summary>
        /// If the game object is a tile, use this draw method to draw the specified tile
        /// </summary>
        /// <param name="sb"></param>
        public void DrawTile(SpriteBatch sb)
        {
            sb.Draw(image,
                    new Rectangle((int)(worldLoc.X - Game1.screenOffset.X),
                    (int)(worldLoc.Y - Game1.screenOffset.Y),
                    (int)size.X, (int)size.Y), new Rectangle(
                        tileIndex % 15 * 16, tileIndex / 15 * 16, 16, 16), 
                    tint);
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
            return toCheck.IsCollidable && GetRect().Intersects(toCheck.GetRect());
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
                if (!(elem == this) && elem.IsCollidable)
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
            if (collides && this is EnemyRanged)
            {
                ((EnemyRanged)this).Bounce();
            }
        
            return false;
        }

        public void PlayAnimation(int animIndex, bool looping)
        {
            if(!looping)
            {
                returnAnim = currentAnim;
            }
            else
            {
                returnAnim = animIndex;
            }
            currentAnim = animIndex;
            currentFrameX = (int)frameOffset.X;
            animFramesElapsed = 0; 
        }
    }
}
