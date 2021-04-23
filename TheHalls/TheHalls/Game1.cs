﻿using System;
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
        public static Texture2D debugSquare;
        public Random rng;

        //loaded content
        private Texture2D arcImgSword;
        private Texture2D arcImgSpear;
        private Texture2D whiteSquare;
        public static Texture2D sword;
        public static Texture2D spear;
        private Texture2D hearts;
        private Texture2D titleBG;

        // Character images
        private Texture2D rangedWalkSheet;
        private Texture2D rangedHurtSheet;
        private Texture2D rangedAttackSheet;
        private Texture2D rangedDeathSheet;
        private Texture2D rangedProjectile;

        private Texture2D meleeWalkSheet;
        private Texture2D meleeHurtSheet;
        private Texture2D meleeAttackSheet;
        private Texture2D meleeDeathSheet;

        private Texture2D playerIdleR;
        private Texture2D playerWalkR;
        private Texture2D playerWalkL;
        private Texture2D playerWalkUp;
        private Texture2D playerWalkDown;

        // Dungeon Tilesprites
        private Texture2D tiles;

        public static SpriteFont arial16;
        private SpriteFont fffforward20;
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

        public static GameState gameState;

        private int enemyHealth;  // Enemy Health
        private int numEnemies;   // Number of enemies in a room
        private int nextEnemIncrease; // Tracks how many more rooms until the number of enemies that spawn increases

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
            arcImgSword = Content.Load<Texture2D>("atkIndicatorSword");
            arcImgSpear = Content.Load<Texture2D>("atkIndicatorSpear");

            whiteSquare = Content.Load<Texture2D>("WhiteSquare");
            arial16 = Content.Load<SpriteFont>("arial16");
            fffforward20 = Content.Load<SpriteFont>("FFF Forward20");
            sword = Content.Load<Texture2D>("SwordNoBackground");
            spear = Content.Load<Texture2D>("SpearNoBackground");
            hearts = Content.Load<Texture2D>("hearts");
            titleBG = Content.Load<Texture2D>("TitleBG");

            debugSquare = whiteSquare;

            // Load character sprites
            rangedWalkSheet = Content.Load<Texture2D>("RangedWalk");
            rangedHurtSheet = Content.Load<Texture2D>("RangedHurt");
            rangedAttackSheet = Content.Load<Texture2D>("RangedAttack");
            rangedDeathSheet = Content.Load<Texture2D>("RangedDeath");
            rangedProjectile = Content.Load<Texture2D>("projectile_sprite");

            meleeWalkSheet = Content.Load<Texture2D>("SkeletonWalk");
            meleeHurtSheet = Content.Load<Texture2D>("SkeletonHurt");
            meleeAttackSheet = Content.Load<Texture2D>("SkeletonAttack");
            meleeDeathSheet = Content.Load<Texture2D>("SkeletonDeath");

            playerIdleR = Content.Load<Texture2D>("PlayerIdleNew");
            playerWalkR = Content.Load<Texture2D>("PlayerWalkRightNew");
            playerWalkL = Content.Load<Texture2D>("PlayerWalkLeftNew");
            playerWalkUp = Content.Load<Texture2D>("PlayerWalkUpNew");
            playerWalkDown = Content.Load<Texture2D>("PlayerWalkDownNew");

            // Load tileset
            tiles = Content.Load<Texture2D>("dungeon_");

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
                            if (rng.Next(2) == 0)
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), sword, rng.Next(enemies[i].MaxHealth / 4, enemies[i].MaxHealth * 3 /4) + 1, weaponType.Sword, arial16));
                            }
                            else
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), spear, rng.Next(enemies[i].MaxHealth / 4, enemies[i].MaxHealth * 3 / 4) + 1, weaponType.Spear, arial16));
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
                            // Check if player is in enemy attack range (For Melee enemies only)
                            if (enemies[i] is EnemyRanged)
                            {
                                enemies[i].TryAttack(player, gameTime);
                            }
                            else
                            {
                                if ((enemies[i].ScreenLoc - player.ScreenLoc).Length() < 200)
                                {
                                    enemies[i].TryAttack(player, gameTime);
                                }
                            }
                            enemies[i].Move(player, obstacles);
                            enemies[i].ResolveCollisions(obstacles);
                        }
                    }

                    //Update the player by input
                    player.Aim(mouse, enemies);
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
                    _spriteBatch.Draw(titleBG, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                    // Draw each menu button to the screen
                    foreach (Button button in buttons)
                    {
                        button.Draw(_spriteBatch, Color.Black);
                    }
                    _spriteBatch.DrawString(
                        fffforward20, "THE HALLS", 
                        new Vector2(_graphics.PreferredBackBufferWidth /2 - (fffforward20.MeasureString("THE HALLS").X/2), 
                        _graphics.PreferredBackBufferHeight /2 - 200), 
                        Color.Red);

                    break;

                case GameState.Pause:
                    GameDraw();
                    _spriteBatch.Draw(whiteSquare, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), new Color(0, 0, 0, .4f));
                    // Draw Pause Text
                    _spriteBatch.DrawString(fffforward20, 
                        "GAME PAUSED", 
                        new Vector2(_graphics.PreferredBackBufferWidth /2 - (fffforward20.MeasureString("GAME PAUSED").X /2), 100), 
                        Color.Yellow);

                    _spriteBatch.DrawString(arial16, 
                        "\nPress [Space] to continue",
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString("Press [Space] to continue").X / 2), 300), 
                        Color.Yellow);

                    _spriteBatch.DrawString(arial16, 
                        "\n\nPress [Esc] to return to menu", 
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString("Press [Esc] to return to menu").X / 2), 300), 
                        Color.Yellow);

                    break;


                case GameState.Game:
                    GameDraw();
                    break;


                case GameState.GameOver:
                    GameDraw();
                    _spriteBatch.Draw(whiteSquare, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), new Color(0, 0, 0, .4f));
                    // Draw Game over text
                    _spriteBatch.DrawString(fffforward20, 
                        "GAME OVER", 
                        new Vector2(_graphics.PreferredBackBufferWidth /2 - (fffforward20.MeasureString("GAME OVER").X /2), 100), 
                        Color.Red);
                     _spriteBatch.DrawString(arial16, 
                        "\nPress [Esc] to return to menu", 
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - (arial16.MeasureString("Press [Esc] to return to menu").X / 2), 300), 
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
            enemyHealth = 2;
            numEnemies = 2;
            nextEnemIncrease = rng.Next(3, 7);

            rooms = new List<Room>();
            obstacles = new List<GameObject>();
            enemies = new List<Enemy>();
            weapons = new List<Weapon>();

            //starter room
            rooms.Add(new Room(
                new RoomData(
                        new List<GameObject>
                        {
                                // Top left wall
                                new GameObject(new Vector2(0, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(50, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(100, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(150, 0), new Vector2(50, 50), tiles),

                                // left wall
                                new GameObject(new Vector2(0, 50), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 100), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 150), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 200), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 250), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 300), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 350), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 400), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 450), new Vector2(50, 50), tiles),

                                // top right wall
                                new GameObject(new Vector2(300, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(350, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(400, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 0), new Vector2(50, 50), tiles),

                                // right wall
                                new GameObject(new Vector2(450, 50), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 100), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 150), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 200), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 250), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 300), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 350), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 400), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 450), new Vector2(50, 50), tiles),

                                // bottom wall
                                new GameObject(new Vector2(50, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(100, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(150, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(200, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(250, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(300, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(350, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(400, 450), new Vector2(50, 50), tiles)
                        },
                        new GameObject(new Vector2(0, 0), new Vector2(50, 50), whiteSquare),
                        Direction.Down,
                        Direction.Up,
                        new List<Vector2>
                        {
                            new Vector2(400, 400),
                            new Vector2(400, 0),
                        }),
                tiles));

            //add starter room to obstacles
            foreach(Room elem in rooms)
            {
                foreach(GameObject obstacle in elem.Obstacles)
                {
                    obstacles.Add(obstacle);
                    //obstacle.Tint = Color.DarkGray;
                }
            }

            //init player            
            player = new Player(
                new Vector2(250,
                250),
                new Vector2(50, 50),
                new Texture2D[]
                {
                    playerIdleR,
                    playerWalkR,
                    playerWalkL,
                    playerWalkDown,
                    playerWalkUp
                },
                arcImgSword,
                arcImgSpear,
                sword,
                GameOver);
            if (easyMode)
            {
                player.Health = 9999;
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

            if(rooms.Count % 5 == 0)
            {
                player.Health++;
                player.Damage++;
            }

            // Decrease number of rooms until increase enemy count
            nextEnemIncrease--;

            // Enemy health increases every 3 rooms
            if ((rooms.Count - 1) % 3 == 0)
            {
                enemyHealth++;
            }

            // Increase number of enemy spawns if player reached the room where this will happen
            if (nextEnemIncrease <= 0) 
            {
                numEnemies++;
                nextEnemIncrease = rng.Next(2, 6);
            }

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
                tiles,
                roomOffset
                );

            //add the room
            rooms.Add(lastRoom);

            //add to obstacles
            foreach (GameObject obstacle in lastRoom.Obstacles)
            {
                obstacles.Add(obstacle);
                //obstacle.Tint = Color.DarkGray;
            }

            //Spawn enemies
            for (int i = 0; i < numEnemies; i++)
            {
                // Create a random spawn location for the enemy
                Vector2 enemySpawn = new Vector2(rng.Next(50, 400), rng.Next(50, 400)) + roomOffset;

                // Make sure the enemy spawn location is not inside a wall, we don't want that
                foreach (GameObject obstacle in enterFrom.Obstacles)
                {
                    while (obstacle.GetRect().Contains(enemySpawn))
                    {
                        enemySpawn = new Vector2(rng.Next(50, 400), rng.Next(50, 400)) + roomOffset;
                    }
                }
                
                if (rng.Next(2) == 0)
                {
                    enemies.Add(new EnemyRanged(enemySpawn, new Vector2(50, 50), enemyHealth,
                        new Texture2D[] { 
                            rangedWalkSheet, 
                            rangedAttackSheet, 
                            rangedHurtSheet, 
                            rangedDeathSheet,
                            whiteSquare}, 
                        3,
                        rangedProjectile));
                }
                else
                {
                    enemies.Add(new Enemy(enemySpawn, new Vector2(50, 50), enemyHealth + 1,
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
                                // Top left wall
                                new GameObject(new Vector2(0, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(50, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(100, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(150, 0), new Vector2(50, 50), tiles),

                                // left wall
                                new GameObject(new Vector2(0, 50), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 100), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 150), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 200), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 250), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 300), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 350), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 400), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(0, 450), new Vector2(50, 50), tiles),

                                // top right wall
                                new GameObject(new Vector2(300, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(350, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(400, 0), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 0), new Vector2(50, 50), tiles),

                                // right wall
                                new GameObject(new Vector2(450, 50), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 100), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 150), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 200), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 250), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 300), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 350), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 400), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(450, 450), new Vector2(50, 50), tiles),

                                // bottom wall
                                new GameObject(new Vector2(50, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(100, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(150, 450), new Vector2(50, 50), tiles),

                                new GameObject(new Vector2(300, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(350, 450), new Vector2(50, 50), tiles),
                                new GameObject(new Vector2(400, 450), new Vector2(50, 50), tiles)
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


        /// <summary>
        /// draws the game
        /// </summary>
        public void GameDraw()
        {

            // Draw obstacles and tiles
            foreach (GameObject elem in obstacles)
            {
                if (!elem.Animated)
                {
                    elem.Draw(_spriteBatch, new Rectangle(0, 64, 16, 16));
                }

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
            if (player.Health > 10)
            {
                _spriteBatch.Draw(hearts, new Rectangle(10, 10, 60, 60), new Rectangle(0, 17, 16, 15), Color.White);
                _spriteBatch.DrawString(fffforward20, " X " + player.Health, new Vector2(80, 25), Color.Black);
            }
            else
            {
                for (int i = 0; i < player.Health; i++)
                {
                    _spriteBatch.Draw(hearts, new Rectangle(10 + (70 * i), 10, 60, 60), new Rectangle(0, 17, 16, 15), Color.White);
                }
            }

            //weapon
            switch (player.CurrentWeapon)
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
        }
    }
}

