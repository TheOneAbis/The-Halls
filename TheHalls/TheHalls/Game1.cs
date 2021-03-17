using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static Vector2 screenOffset;

        private Texture2D coinImg;
        private Texture2D arcImg;
        private Texture2D whiteSquare;
        private SpriteFont arial16;

        private List<GameObject> obstacles;
        private List<Enemy> enemies;
        private Player player;

        private KeyboardState kb;
        private MouseState mouse;
        private MouseState prevMouse;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            obstacles = new List<GameObject>();

            obstacles.Add(new GameObject(new Vector2(0, 0), new Vector2(50, 300), whiteSquare));
            obstacles.Add(new GameObject(new Vector2(200, 50), new Vector2(300, 50), whiteSquare));
            obstacles.Add(new GameObject(new Vector2(50, 0), new Vector2(200, 50), whiteSquare));

            enemies = new List<Enemy>();
            enemies.Add(new EnemyRanged(new Vector2(300, 300), new Vector2(50, 50), whiteSquare));
            enemies.Add(new Enemy(new Vector2(-50, -50), new Vector2(50, 50), whiteSquare));
            
            foreach(Enemy elem in enemies)
            {
                elem.Tint = Color.Red;
                obstacles.Add(elem);
            }

            player = new Player(new Vector2(_graphics.PreferredBackBufferWidth/2 - 20, 
                _graphics.PreferredBackBufferHeight/2 - 24),
                new Vector2(50, 50), whiteSquare, arcImg);
            player.Tint = Color.Green;

            obstacles.Add(player);

            screenOffset = new Vector2(0, 0);


        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            coinImg = Content.Load<Texture2D>("coin");
            arcImg = Content.Load<Texture2D>("Arc");
            whiteSquare = Content.Load<Texture2D>("WhiteSquare");
            arial16 = Content.Load<SpriteFont>("arial16");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Get mouse and keyboard states
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();


            foreach (Enemy elem in enemies)
            {
                elem.Move(player);
                elem.ResolveCollisions(obstacles);
            }


            player.Aim(mouse);
            player.Move(kb);
            player.ResolveCollisions(obstacles);

            //adjusts the screenOffset to center the player.
            screenOffset = new Vector2(
                player.WorldLoc.X - (_graphics.PreferredBackBufferWidth - player.Size.X) / 2, 
                player.WorldLoc.Y - (_graphics.PreferredBackBufferHeight - player.Size.Y) / 2);

            prevMouse = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            foreach (Enemy elem in enemies)
            {
                elem.Draw(_spriteBatch);
            }

            foreach (GameObject elem in obstacles)
            {
                elem.Draw(_spriteBatch);
            }
            _spriteBatch.DrawString(arial16, "ScreenOffset: X: " + screenOffset.X + " Y: " + screenOffset.Y, new Vector2(25, 25), Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
