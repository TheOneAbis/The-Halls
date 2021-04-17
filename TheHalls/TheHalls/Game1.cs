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

        //loaded content
        private Texture2D arcImg;
        private Texture2D whiteSquare;
        private Texture2D sword;
        private Texture2D spear;

        // Character images
        private Texture2D rangedWalkSheet;
        private Texture2D rangedHurtSheet;
        private Texture2D rangedAttackSheet;
        private Texture2D rangedDeathSheet;

        private Texture2D meleeWalkSheet;
        private Texture2D meleeHurtSheet;
        private Texture2D meleeAttackSheet;
        private Texture2D meleeDeathSheet;

        private SpriteFont arial16;
        //seperate lists for each direction
        private Dictionary<Direction, List<RoomData>> roomTemplates;

        //game objects
        private List<GameObject> obstacles;
        private List<Enemy> enemies;
        private Player player;
        private List<Weapon> weapons;

        //rooms
        private List<Room> rooms;
        private Room lastRoom;

        //input
        private KeyboardState kb;
        private KeyboardState prevkb;
        private MouseState mouse;
        private MouseState prevMouse;

        private GameState gameState;


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
            base.Initialize();

            //startup code
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            gameState = GameState.Menu;
            buttons = new List<Button>();
            rng = new Random();

            //    -- Menu Buttons --

            // Play button
            buttons.Add(new Button(_graphics.PreferredBackBufferWidth /2 - 38, _graphics.PreferredBackBufferHeight /2 - 25, 75, 50, whiteSquare, "Play", arial16));

            // God mode
            buttons.Add(new Button(_graphics.PreferredBackBufferWidth / 2 - 55, _graphics.PreferredBackBufferHeight / 2 + 75, 110, 50, whiteSquare, "God Mode", arial16));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            arcImg = Content.Load<Texture2D>("Arc");
            whiteSquare = Content.Load<Texture2D>("WhiteSquare");
            arial16 = Content.Load<SpriteFont>("arial16");
            sword = Content.Load<Texture2D>("sword");
            spear = Content.Load<Texture2D>("spear");

            // Load character sprites
            rangedWalkSheet = Content.Load<Texture2D>("RangedWalk");
            rangedHurtSheet = Content.Load<Texture2D>("RangedHurt");
            rangedAttackSheet = Content.Load<Texture2D>("RangedAttack");
            rangedDeathSheet = Content.Load<Texture2D>("RangedDeath");

            meleeWalkSheet = Content.Load<Texture2D>("SkeletonWalkCropped");
            meleeHurtSheet = Content.Load<Texture2D>("SkeletonHurt");
            meleeAttackSheet = Content.Load<Texture2D>("SkeletonAttack");
            meleeDeathSheet = Content.Load<Texture2D>("SkeletonDeath");

            roomTemplates = LoadRooms();
        }

        protected override void Update(GameTime gameTime)
        {
            // Press F10 for Emergency Exit
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
                    // was god mode button clicked?
                    else if(buttons[1].Clicked(mouse, prevMouse))
                    {
                        GameStart(true);
                    }
                    break;

                // Game State
                case GameState.Game:

                    //clearout dead enemies, spawn drops and open the door if applicable
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (!enemies[i].Alive)
                        {
                            if (rng.Next(100) < 50)
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), sword, 3, weaponType.Sword, arial16));
                            }
                            else
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), spear, 2, weaponType.Spear, arial16));
                            }
                            obstacles.Remove(enemies[i]);
                            enemies.RemoveAt(i);
                            if(enemies.Count == 0)
                            {
                                obstacles.Remove(lastRoom.OutDoor);
                                NextRoom();
                            }
                            i--;
                        }
                        else
                        {
                            //if the enemy is still alive, update its AI and collisions
                            enemies[i].TryAttack(player, gameTime);
                            enemies[i].Move(player, obstacles);
                            enemies[i].ResolveCollisions(obstacles);
                        }
                    }

                    //Update the player by input
                    player.Aim(mouse);
                    player.Move(kb);
                    player.ResolveCollisions(obstacles);

                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        player.Attack(enemies);
                    }

                    //weapon pickups
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapons[i].PickUp(player))
                        {
                            weapons.RemoveAt(i);
                        }
                    }

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
                    if (kb.IsKeyDown(Keys.Space) && prevkb.IsKeyUp(Keys.Space))
                    {
                        gameState = GameState.Game;
                    }
                    else if(kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                    {
                        gameState = GameState.Menu;
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
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Menu:

                    // Draw each menu button to the screen
                    foreach (Button button in buttons)
                    {
                        button.Draw(_spriteBatch, Color.Black);
                    }
                    _spriteBatch.DrawString(
                        arial16, "THE HALLS", 
                        new Vector2(_graphics.PreferredBackBufferWidth /2 - (arial16.MeasureString("THE HALLS").X/2), 
                        _graphics.PreferredBackBufferHeight /2 - 150), 
                        Color.Red);

                    break;

                case GameState.Game:


                    foreach (GameObject elem in obstacles)
                    {
                        elem.Draw(_spriteBatch);
                    }

                    foreach (Weapon elem in weapons)
                    {
                        elem.Draw(_spriteBatch);
                    }

                    foreach (Enemy elem in enemies)
                    {
                        elem.Draw(_spriteBatch);
                    }

                    //HUD
                    //health
                    for(int i = 0; i < player.Health; i++)
                    {
                        _spriteBatch.Draw(whiteSquare, new Rectangle(10 + (60 * i), 10, 50, 50), Color.IndianRed);
                    }

                    //weapon
                    switch(player.CurrentWeapon)
                    {
                        case weaponType.Spear:
                            _spriteBatch.Draw(spear, new Rectangle(10, _graphics.PreferredBackBufferHeight - 60, 50, 50), Color.White);
                            break;

                        case weaponType.Sword:
                            _spriteBatch.Draw(sword, new Rectangle(10, _graphics.PreferredBackBufferHeight - 60, 50, 50), Color.White);
                            break;
                    }
                    // weapon damage
                    _spriteBatch.DrawString(arial16, player.Damage.ToString(), new Vector2(10, _graphics.PreferredBackBufferHeight - 60), Color.Black);


                    player.Draw(_spriteBatch);
                    break;

                case GameState.Pause:

                    // Draw Pause Text
                    _spriteBatch.DrawString(arial16, 
                        "GAME PAUSED", 
                        new Vector2(_graphics.PreferredBackBufferWidth /2 - (arial16.MeasureString("GAME PAUSED").X /2), 100), 
                        Color.Yellow);

                    _spriteBatch.DrawString(arial16, 
                        "\nPress [Space] to continue",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString("Press [Space] to continue").X / 2), 100), 
                        Color.Yellow);

                    _spriteBatch.DrawString(arial16, 
                        "\n\nPress [Esc] to return to menu", 
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString("Press [Esc] to return to menu").X / 2), 100), 
                        Color.Yellow);

                    break;

                case GameState.GameOver:

                    // Draw Game over text
                    _spriteBatch.DrawString(arial16, 
                        "GAME OVER", 
                        new Vector2(_graphics.PreferredBackBufferWidth /2 - (arial16.MeasureString("GAME OVER").X /2), 100), 
                        Color.Red);
                     _spriteBatch.DrawString(arial16, 
                        "\nPress [Esc] to return to menu", 
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString("Press [Esc] to return to menu").X / 2), 100), 
                        Color.Red);
                    _spriteBatch.DrawString(arial16,
                        $"\n\nRoom #{rooms.Count -1}",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString($"\n\nRoom #{rooms.Count -1}").X / 2), 100),
                        Color.Red);
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
            enemies = new List<Enemy>();
            weapons = new List<Weapon>();

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

            //add starter room to obstacles
            foreach(Room elem in rooms)
            {
                foreach(GameObject obstacle in elem.Obstacles)
                {
                    obstacles.Add(obstacle);
                    obstacle.Tint = Color.DarkGray;
                }
            }

            //init player            
            player = new Player(
                new Vector2(250,
                250),
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

            //determine what direction the room will be facing, and adjust room offset accordingly
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

            //this variable is always the last room in rooms, but it will be accessed a lot so this makes it easier. 
            //create the room
            lastRoom = new Room(
                roomTemplates[inDirection][0],
                enterFrom,
                whiteSquare,
                roomOffset
                );

            //add the room
            rooms.Add(lastRoom);

            //add to obstacles
            foreach (GameObject obstacle in lastRoom.Obstacles)
            {
                obstacles.Add(obstacle);
                obstacle.Tint = Color.DarkGray;
            }

            //Spawn enemies
            foreach (Vector2 elem in lastRoom.EnemySpawns)
            {
                if (rng.Next(2) == 0)
                {
                    enemies.Add(new EnemyRanged(elem, new Vector2(50, 50), 
                        new Texture2D[] { 
                            rangedWalkSheet, 
                            rangedAttackSheet, 
                            rangedHurtSheet, 
                            rangedDeathSheet,
                            whiteSquare}, 
                        1.5,
                        whiteSquare));
                }
                else
                {
                    enemies.Add(new Enemy(elem, new Vector2(50, 50),
                        new Texture2D[] {
                            meleeWalkSheet,
                            meleeAttackSheet,
                            meleeHurtSheet,
                            meleeDeathSheet,
                            whiteSquare}, 
                        1.5,
                        whiteSquare));
                }
            }

            //creates the exit door, which opens when the enemies are defeated.
            obstacles.Add(lastRoom.OutDoor);
            lastRoom.OutDoor.Tint = Color.Brown;
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
                            new List<GameObject> //obstacles
                            {
                                //top wall
                                new GameObject(new Vector2(0, 0), new Vector2(200, 50), whiteSquare),
                                new GameObject(new Vector2(300, 0), new Vector2(200, 50), whiteSquare),

                                //left wall
                                new GameObject(new Vector2(0, 0), new Vector2(50, 500), whiteSquare),

                                //right wall
                                new GameObject(new Vector2(450, 0), new Vector2(50, 500), whiteSquare),

                                //bottom wall
                                new GameObject(new Vector2(0, 450), new Vector2(200, 50), whiteSquare),
                                new GameObject(new Vector2(300, 450), new Vector2(200, 50), whiteSquare),
                            },
                            new GameObject(new Vector2(200, 0), new Vector2(100, 50), whiteSquare), //outDoor
                            Direction.Down, //inDirection
                            Direction.Up, //outDirection
                            new List<Vector2> //enemySpawns
                            {
                                new Vector2(100, 100),
                                new Vector2(300, 400),
                            })

                });
            /*
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
            */
            return rooms;
        }
    }
}

