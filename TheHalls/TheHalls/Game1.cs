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

        private Vector2 screenOffset;
        private KeyboardState kb;

        private Texture2D coinImg;
        private Texture2D arcImg;
        private Texture2D whiteSquare;
        private SpriteFont arial16;

        private List<GameObject> obstacles;
        private Player player;

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

            player = new Player(new Vector2(_graphics.PreferredBackBufferWidth/2 - 20, 
                _graphics.PreferredBackBufferHeight/2 - 24), screenOffset,
                new Vector2(40, 48), coinImg, arcImg);

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

            player.Move(kb);
            player.Aim(mouse);

            //adjusts the screenOffset to center the player.
            screenOffset = new Vector2(player.WorldLoc.X - (_graphics.PreferredBackBufferWidth - player.Size.X) / 2, player.WorldLoc.Y - (_graphics.PreferredBackBufferHeight - player.Size.Y) / 2);

            prevMouse = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            foreach (GameObject elem in obstacles)
            {
                elem.Draw(_spriteBatch, screenOffset);
            }
            _spriteBatch.DrawString(arial16, "ScreenOffset: X: " + screenOffset.X + " Y: " + screenOffset.Y, new Vector2(25, 25), Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
