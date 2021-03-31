using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    /// <summary>
    /// Controls the state of the overall game
    /// </summary>
    public enum GameState
    {
        Menu,
        Game,
        Pause,
        GameOver
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static Vector2 screenOffset;
        public Random rng;

        private Texture2D arcImg;
        private Texture2D whiteSquare;
        private Texture2D sword;
        private Texture2D spear;
        private SpriteFont arial16;

        private List<GameObject> obstacles;
        private List<Room> rooms;
        private Room currentRoom;
        private List<Enemy> enemies;
        private Player player;
        private List<Weapon> weapons;

        private KeyboardState kb;
        private KeyboardState prevkb;
        private MouseState mouse;
        private MouseState prevMouse;

        private GameState gameState;

        //sperate lists for each direction
        private Dictionary<Direction, List<RoomData>> roomTemplates;

        // Menu buttons
        List<Button> buttons;

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
            gameState = GameState.Menu;

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            arcImg = Content.Load<Texture2D>("Arc");
            whiteSquare = Content.Load<Texture2D>("WhiteSquare");
            arial16 = Content.Load<SpriteFont>("arial16");
            sword = Content.Load<Texture2D>("sword");
            spear = Content.Load<Texture2D>("spear");
            roomTemplates = LoadRooms();

            //    -- Menu Buttons --

            // Play button
            buttons = new List<Button>();
            buttons.Add(new Button(350, 200, 75, 50, whiteSquare, "Play", arial16));
            buttons.Add(new Button(350, 300, 110, 50, whiteSquare, "God Mode", arial16));
        }

        protected override void Update(GameTime gameTime)
        {
            // Press F10 fo Emergency Exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F10))
                Exit();

            // Get mouse and keyboard states
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();
            Point mouseP = new Point(mouse.X, mouse.Y);

            switch (gameState)
            {
                // Menu State
                case GameState.Menu:

                    // Exit the game
                    if (kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                        Exit();

                    // Was the play button clicked?
                    if (buttons[0].Clicked(mouse, prevMouse))
                    {
                        GameStart(false);
                    }
                    else if(buttons[1].Clicked(mouse, prevMouse))
                    {
                        GameStart(true);
                    }
                    break;

                // Game State
                case GameState.Game:
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (!enemies[i].Alive)
                        {
                            if (rng.Next(100) < 50)
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), sword, 3, weaponType.Sword));
                            }
                            else
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), spear, 2, weaponType.Spear));
                            }
                            obstacles.Remove(enemies[i]);
                            enemies.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            enemies[i].TryAttack(player);
                            enemies[i].Move(player, obstacles);
                            enemies[i].ResolveCollisions(obstacles);
                        }
                    }

                    player.Aim(mouse);
                    player.Move(kb);

                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapons[i].PickUp(player))
                        {
                            weapons.RemoveAt(i);
                        }
                    }

                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        player.Attack(enemies);
                    }

                    player.ResolveCollisions(obstacles);

                    //adjusts the screenOffset to center the player.
                    screenOffset = new Vector2(
                        player.WorldLoc.X - (_graphics.PreferredBackBufferWidth - player.Size.X) / 2,
                        player.WorldLoc.Y - (_graphics.PreferredBackBufferHeight - player.Size.Y) / 2);

                    // Pause the game
                    if (kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                    {
                        gameState = GameState.Pause;
                    }
                    break;

                // Pause State
                case GameState.Pause:
                    // Resume the game
                    if (kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                    {
                        gameState = GameState.Game;
                    }
                    break;

                // GameOver State
                case GameState.GameOver:
                    // Reset back to menu
                    if (kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                    {
                        gameState = GameState.Menu;
                    }
                    break;
            }
            
            prevMouse = Mouse.GetState();
            prevkb = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Menu:

                    // Draw each menu button to the screen
                    foreach (Button button in buttons)
                    {
                        button.Draw(_spriteBatch, Color.Black);
                    }
                    _spriteBatch.DrawString(arial16, "THE HALLS", new Vector2(350, 75), Color.Red);
                    break;

                case GameState.Game:

                    player.Draw(_spriteBatch);
                    foreach (Enemy elem in enemies)
                    {
                        elem.Draw(_spriteBatch);
                    }

                    foreach (GameObject elem in obstacles)
                    {
                        elem.Draw(_spriteBatch);
                    }
                    //HUD
                    _spriteBatch.DrawString(arial16, "Health: " + player.Health, new Vector2(25, 25), Color.Black);
                    _spriteBatch.DrawString(arial16, "Weapon: " + player.CurrentWeapon.ToString(), new Vector2(25, 50), Color.Black);
                    _spriteBatch.DrawString(arial16, "Damage: " + player.Damage, new Vector2(25, 75), Color.Black);

                    foreach (Weapon elem in weapons)
                    {
                        elem.Draw(_spriteBatch);
                    }
                    break;

                case GameState.Pause:

                    // Draw Pause Text
                    _spriteBatch.DrawString(arial16, "GAME PAUSED \nPress [Esc] to resume", new Vector2(300, 100), Color.Yellow);
                    break;

                case GameState.GameOver:

                    // Draw Game over text
                    _spriteBatch.DrawString(arial16, "GAME OVER \nPress [Esc] to return to menu", new Vector2(300, 100), Color.Red);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// called when the game ends. sets the game state to game over. 
        /// </summary>
        public void GameOver()
        {
            gameState = GameState.GameOver;
        }

        /// <summary>
        /// changes the gamestate to game. resets obstacles, enemies, weapons and player.
        /// </summary>
        private void GameStart(bool easyMode)
        {
            gameState = GameState.Game;
            rooms = new List<Room>();
            obstacles = new List<GameObject>();
            rng = new Random();

            //starter room
            rooms.Add(new Room(
                new RoomData(
                        new List<GameObject>
                        {
                                new GameObject(new Vector2(0, 0), new Vector2(200, 50), whiteSquare),
                                new GameObject(new Vector2(0, 0), new Vector2(50, 500), whiteSquare),
                                new GameObject(new Vector2(300, 0), new Vector2(200, 50), whiteSquare),
                                new GameObject(new Vector2(450, 0), new Vector2(50, 500), whiteSquare),
                                new GameObject(new Vector2(0, 450), new Vector2(500, 50), whiteSquare)
                        },
                        new GameObject(new Vector2(0, 0), new Vector2(50, 50), whiteSquare),
                        Direction.Down,
                        Direction.Up,
                        new List<Vector2>
                        {
                            new Vector2(400, 400),
                            new Vector2(400, 0),
                        }),
                whiteSquare));

            
            foreach(Room elem in rooms)
            {
                foreach(GameObject obstacle in elem.Obstacles)
                {
                    obstacles.Add(obstacle);
                }
            }


            //obstacles.Add(new GameObject(new Vector2(0, 0), new Vector2(50, 300), whiteSquare));
            //obstacles.Add(new GameObject(new Vector2(200, 50), new Vector2(300, 50), whiteSquare));
            //obstacles.Add(new GameObject(new Vector2(50, 0), new Vector2(200, 50), whiteSquare));

            enemies = new List<Enemy>();
            //enemies.Add(new EnemyRanged(new Vector2(300, 300), new Vector2(50, 50), whiteSquare));
            //enemies.Add(new Enemy(new Vector2(-50, -50), new Vector2(50, 50), whiteSquare));

            weapons = new List<Weapon>();
            weapons.Add(new Weapon(new Rectangle(100, -100, 50, 50), sword, 1, weaponType.Sword));
            weapons.Add(new Weapon(new Rectangle(-100, 25, 50, 50), spear, 1, weaponType.Spear));

            foreach (Enemy elem in enemies)
            {
                elem.Tint = Color.Red;
                if (elem is EnemyRanged)
                {
                    elem.Tint = Color.DarkRed;
                }
                obstacles.Add(elem);
            }

            player = new Player(
                new Vector2(_graphics.PreferredBackBufferWidth / 2 - 20,
                _graphics.PreferredBackBufferHeight / 2 - 24),
                new Vector2(50, 50),
                whiteSquare,
                arcImg,
                sword,
                GameOver);
            player.Tint = Color.Green;
            if (easyMode)
            {
                player.Health = int.MaxValue;
            }

            obstacles.Add(player);

            screenOffset = new Vector2(0, 0);
            
            //currently just to test the the method. this will be called when the player collides with the outdoor of the current room.
            NextRoom();
        }

        /// <summary>
        /// generates a new room at the end of the chain. 
        /// </summary>
        public void NextRoom()
        {
            Room enterFrom = rooms[rooms.Count - 1];
            Vector2 roomOffset = enterFrom.RoomOffset;
            Direction inDirection = Direction.Up;

            //determine what direction the room will be facing, and adjust offset accordingly
            switch(enterFrom.OutDirection)
            {
                case Direction.Down:
                    roomOffset.Y += 500;
                    inDirection = Direction.Up;
                    break;

                case Direction.Up:
                    roomOffset.Y -= 500;
                    inDirection = Direction.Down;
                    break;

                case Direction.Left:
                    roomOffset.X -= 500;
                    inDirection = Direction.Right;
                    break;

                case Direction.Right:
                    roomOffset.X += 500;
                    inDirection = Direction.Left;
                    break;
            }

            //create the room
            rooms.Add(new Room(
                roomTemplates[inDirection][0],
                enterFrom,
                whiteSquare,
                roomOffset
                ));

            //add to obstacles
            foreach (GameObject obstacle in rooms[rooms.Count- 1].Obstacles)
            {
                obstacles.Add(obstacle);
            }

            //Spawn enemies
            foreach (Vector2 elem in rooms[rooms.Count - 1].EnemySpawns)
            {
                if (rng.Next(2) == 0)
                {
                    enemies.Add(new EnemyRanged(elem, new Vector2(50, 50), whiteSquare));
                    enemies[enemies.Count - 1].Tint = Color.DarkRed;
                }
                else
                {
                    enemies.Add(new Enemy(elem, new Vector2(50, 50), whiteSquare));
                    enemies[enemies.Count - 1].Tint = Color.Red;
                }
            }
        }

        /// <summary>
        /// in the future this will load the rooms from .room files, and return a list of all the rooms. 
        /// </summary>
        /// <returns></returns>
        private Dictionary<Direction, List<RoomData>> LoadRooms()
        {
            Dictionary<Direction, List<RoomData>> rooms = new Dictionary<Direction, List<RoomData>>();

            rooms.Add(Direction.Down,
                new List<RoomData>
                {

                    new RoomData(
                            new List<GameObject>
                            {
                                new GameObject(new Vector2(0, 0), new Vector2(500, 50), whiteSquare),
                                new GameObject(new Vector2(0, 0), new Vector2(50, 200), whiteSquare),
                                new GameObject(new Vector2(0, 300), new Vector2(50, 200), whiteSquare),
                                new GameObject(new Vector2(450, 0), new Vector2(50, 500), whiteSquare),
                                new GameObject(new Vector2(0, 450), new Vector2(200, 50), whiteSquare),
                                new GameObject(new Vector2(300, 450), new Vector2(200, 50), whiteSquare),
                            },
                            new GameObject(new Vector2(0, 0), new Vector2(50, 50), whiteSquare),
                            Direction.Down,
                            Direction.Left,
                            new List<Vector2>
                            {
                                new Vector2(100, 100),
                                new Vector2(300, 400),
                            })

                });

            rooms.Add(Direction.Up,
                new List<RoomData>
                {
                    new RoomData(
                            new List<GameObject>
                            {
                                new GameObject(new Vector2(0, 0), new Vector2(50, 300), whiteSquare),
                                new GameObject(new Vector2(300, 50), new Vector2(200, 50), whiteSquare),
                                new GameObject(new Vector2(50, 0), new Vector2(200, 50), whiteSquare)
                            },
                            new GameObject(new Vector2(0, 0), new Vector2(50, 50), whiteSquare),
                            Direction.Up,
                            Direction.Right,
                            new List<Vector2>
                            {
                                new Vector2(400, 400),
                                new Vector2(400, 0),
                            })
                });

            return rooms;
        }
    }
}

