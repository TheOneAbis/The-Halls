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
        private Player player;

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

            testObj = new GameObject(new Vector2(0, 0), new Vector2(80, 96), coinImg);
            player = new Player(new Vector2(_graphics.PreferredBackBufferWidth/2 -20, _graphics.PreferredBackBufferHeight/2 - 24), new Vector2(40, 48), coinImg);
            screenOffset = new Vector2(0, 0);
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

            player.Move();

            //adjusts the screenOffset to center the player.
            screenOffset = new Vector2(player.WorldLoc.X - (_graphics.PreferredBackBufferWidth - player.Size.X) / 2, player.WorldLoc.Y - (_graphics.PreferredBackBufferHeight - player.Size.Y) / 2);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            player.Draw(_spriteBatch, screenOffset);
            testObj.Draw(_spriteBatch, screenOffset);
            _spriteBatch.DrawString(arial16, "ScreenOffset: X: " + screenOffset.X + " Y: " + screenOffset.Y, new Vector2(25, 25), Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
