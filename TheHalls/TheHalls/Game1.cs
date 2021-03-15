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
        private SpriteFont arial16;

        private GameObject testObj;

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

            testObj = new GameObject(new Vector2(50, 50), new Vector2(80, 96), coinImg);
            screenOffset = new Vector2(75, 75);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            coinImg = Content.Load<Texture2D>("coin");
            arial16 = Content.Load<SpriteFont>("arial16");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            kb = Keyboard.GetState();

            //Currently, wasd moves the screenoffset.
            if(kb.IsKeyDown(Keys.W))
            {
                screenOffset.Y--;
            }

            if(kb.IsKeyDown(Keys.S))
            {
                screenOffset.Y++;
            }

            if(kb.IsKeyDown(Keys.A))
            {
                screenOffset.X--;
            }

            if(kb.IsKeyDown(Keys.D))
            {
                screenOffset.X++;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            testObj.Draw(_spriteBatch, screenOffset);
            _spriteBatch.DrawString(arial16, "ScreenOffset: X: " + screenOffset.X + " Y: " + screenOffset.Y, new Vector2(25, 25), Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
