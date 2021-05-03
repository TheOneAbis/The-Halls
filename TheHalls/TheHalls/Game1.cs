using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace TheHalls
{
    /// <summary>
    /// Controls the state of the overall game
    /// </summary>
    public enum GameState
    {
        Menu,
        Controls,
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

        //Tracks if the player entered the most recently added room
        private bool EnteredLastRoom; 

        //loaded content
        private Texture2D arcImgSword;
        private Texture2D arcImgSpear;
        private Texture2D whiteSquare;
        public static Texture2D sword;
        public static Texture2D spear;
        public static Texture2D potion;
        private Texture2D hearts;
        private Texture2D titleBG;
        private Texture2D directionPointer;

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
        private Texture2D playerLowHealthBG;

        // Dungeon Tilesprites
        private Texture2D tiles;

        // Fonts
        public static SpriteFont arial16;
        private SpriteFont fffforward20;
        private SpriteFont fffforwardSmall;

        // Audio
        private Song gameMusic;
        private SoundEffect[] playerAttackSFX;
        private SoundEffect[] enemyRangedSFX;

        //seperate lists for each direction
        private Dictionary<Direction, List<RoomData>> roomTemplates;

        //game objects
        private List<GameObject> obstacles;
        private List<Enemy> enemies;
        private Player player;
        private List<Weapon> weapons;
        private List<Potion> potions;

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

        private int levelUpFrames; // how many more frames with level up text show?

        // Menu buttons
        List<Button> buttons;
        // Control button
        Button beginGameButton;

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
            EnteredLastRoom = true;

            //    -- Menu Buttons --

            // Play button
            buttons.Add(new Button(_graphics.PreferredBackBufferWidth /2 - 38, _graphics.PreferredBackBufferHeight /2 - 25, 80, 50, whiteSquare, "Play", fffforward20));

            // God mode
            buttons.Add(new Button(_graphics.PreferredBackBufferWidth / 2 - 85, _graphics.PreferredBackBufferHeight / 2 + 75, 180, 50, whiteSquare, "God Mode", fffforward20));

            // Controls Continue Button
            beginGameButton = new Button(_graphics.PreferredBackBufferWidth / 2 - 38, _graphics.PreferredBackBufferHeight / 2 + 200, 105, 50, whiteSquare, "Begin", fffforward20);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            arcImgSword = Content.Load<Texture2D>("atkIndicatorSword");
            arcImgSpear = Content.Load<Texture2D>("atkIndicatorSpear");

            whiteSquare = Content.Load<Texture2D>("WhiteSquare");

            arial16 = Content.Load<SpriteFont>("arial16");
            fffforward20 = Content.Load<SpriteFont>("FFF Forward20");
            fffforwardSmall = Content.Load<SpriteFont>("FFF ForwardSmallText");

            sword = Content.Load<Texture2D>("SwordNoBackground");
            spear = Content.Load<Texture2D>("SpearNoBackground");
            potion = Content.Load<Texture2D>("potions");
            hearts = Content.Load<Texture2D>("hearts");
            titleBG = Content.Load<Texture2D>("TitleBG");
            directionPointer = Content.Load<Texture2D>("Arrow");

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

            playerIdleR = Content.Load<Texture2D>("CharIdleR");
            playerWalkR = Content.Load<Texture2D>("CharWalkR");
            playerWalkL = Content.Load<Texture2D>("CharWalkL");
            playerWalkUp = Content.Load<Texture2D>("CharWalkU");
            playerWalkDown = Content.Load<Texture2D>("CharWalkD");
            playerLowHealthBG = Content.Load<Texture2D>("PlayerLowHealthWarning (1)");

            // Load tileset
            tiles = Content.Load<Texture2D>("dungeon_");

            // Load Audio
            gameMusic = Content.Load<Song>("GameMusic");
            playerAttackSFX = new SoundEffect[2];
            playerAttackSFX[0] = Content.Load<SoundEffect>("Sword_Slash");
            playerAttackSFX[1] = Content.Load<SoundEffect>("Spear_Thrust");
            enemyRangedSFX = new SoundEffect[2];
            enemyRangedSFX[0] = Content.Load<SoundEffect>("RangedEnemy_Death");

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

                    MediaPlayer.Stop();

                    // Exit the game
                    if (kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                        Exit();

                    // Was the play button clicked?
                    if (buttons[0].Clicked(mouse, prevMouse))
                    {
                        gameState = GameState.Controls;
                    }
                    // was god mode button clicked?
                    else if(buttons[1].Clicked(mouse, prevMouse))
                    {
                        GameStart(true);
                    }
                    break;

                // Controls State
                case GameState.Controls:
                    if (beginGameButton.Clicked(mouse, prevMouse))
                    {
                        GameStart(false);
                    }
                    break;

                // Game State
                case GameState.Game:

                    //clearout dead enemies, spawn drops and open the door if applicable
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (!enemies[i].Alive)
                        {
                            int itemDrop = rng.Next(5);
                            if (itemDrop == 0)
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X, 
                                    (int)enemies[i].WorldLoc.Y, 50, 50), sword, 
                                    rng.Next(enemies[i].MaxHealth / 4, enemies[i].MaxHealth * 3 /4) + 1,
                                    weaponType.Sword, fffforwardSmall));
                            }
                            else if(itemDrop == 1)
                            {
                                weapons.Add(new Weapon(new Rectangle((int)enemies[i].WorldLoc.X,
                                    (int)enemies[i].WorldLoc.Y, 50, 50), spear, 
                                    rng.Next(enemies[i].MaxHealth / 4, enemies[i].MaxHealth * 3 / 4) + 1, 
                                    weaponType.Spear, fffforwardSmall));
                            }
                            else if(itemDrop == 2)
                            {
                                potions.Add(new Potion(new Rectangle((int)enemies[i].WorldLoc.X, (int)enemies[i].WorldLoc.Y, 50, 50), potion, 1));
                            }
                            obstacles.Remove(enemies[i]);
                            enemies.RemoveAt(i);
                            if(enemies.Count == 0)
                            {
                                EnteredLastRoom = false;
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
                    player.SetIsInteracting(kb, prevkb);

                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        player.Attack(enemies, playerAttackSFX);
                    }

                    //weapon pickups
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapons[i].PickUp(player))
                        {
                            weapons.RemoveAt(i);
                        }
                    }

                    //Potion pickups
                    for(int i = 0; i<potions.Count; i++)
                    {
                        if (potions[i].PickUp(player))
                        {
                            potions.RemoveAt(i);
                        }
                    }

                    //adjusts the screenOffset to center the player relative to both center of screen and mouse's position
                    screenOffset = new Vector2(
                        player.WorldLoc.X - (_graphics.PreferredBackBufferWidth - player.Size.X) / 2 + ((mouse.X - _graphics.PreferredBackBufferWidth / 2) / 10),
                        player.WorldLoc.Y - (_graphics.PreferredBackBufferHeight - player.Size.Y) / 2 + ((mouse.Y - _graphics.PreferredBackBufferHeight / 2) / 10));

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
                    MediaPlayer.Stop();
                    // Reset back to menu
                    if (kb.IsKeyDown(Keys.Escape) && prevkb.IsKeyUp(Keys.Escape))
                    {
                        gameState = GameState.Menu;
                    }
                    break;
            }
            if (levelUpFrames > 0)
            {
                levelUpFrames--;
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

                case GameState.Controls:
                    // Draw start button
                    beginGameButton.Draw(_spriteBatch, Color.Black);

                    // Draw tutorial stuff
                    _spriteBatch.DrawString(fffforward20, "Use [W A S D] to move around!", new Vector2(400, 100), Color.White);
                    _spriteBatch.DrawString(fffforward20, "Use [Mouse1] to attack!", new Vector2(450, 150), Color.White);

                    _spriteBatch.DrawString(fffforwardSmall, "When the attack indicator is LIT UP, \n\nenemies are in range of your attack!", new Vector2(100, 250), Color.White);
                    _spriteBatch.Draw(arcImgSword, new Rectangle(125, 300, 150, 100), Color.White);
                    _spriteBatch.Draw(arcImgSpear, new Rectangle(275, 300, 150, 100), Color.White);

                    _spriteBatch.DrawString(fffforwardSmall, "When the attack indicator is RED, \n\nyour attack is on cooldown!", new Vector2(900, 250), Color.White);
                    _spriteBatch.Draw(arcImgSword, new Rectangle(925, 300, 150, 100), new Color(255, 155, 155));
                    _spriteBatch.Draw(arcImgSpear, new Rectangle(1075, 300, 150, 100), new Color(255, 155, 155));
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

            MediaPlayer.Play(gameMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .15f;
            

            rooms = new List<Room>();
            obstacles = new List<GameObject>();
            enemies = new List<Enemy>();
            weapons = new List<Weapon>();
            potions = new List<Potion>();

            //starter room
            rooms.Add(new Room(
                new RoomData(
                        "room2", Direction.Down, Direction.Up,
                        new Rectangle(0, 0, 50, 50), 
                        tiles),
                tiles));

            //add starter room to obstacles
            foreach (Room elem in rooms)
            {
                foreach(GameObject obstacle in elem.Obstacles)
                {
                    obstacles.Add(obstacle);
                }
            }

            //init player            
            player = new Player(
                new Vector2(475,
                475),
                new Vector2(50, 50),
                new Texture2D[]
                {
                    playerIdleR,
                    playerWalkR,
                    playerWalkL,
                    playerWalkDown,
                    playerWalkUp,
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
            
            //Open the door
            foreach(GameObject doorTile in enterFrom.OutDoor)
            {
                doorTile.IsCollidable = false;
                doorTile.TileNum = 105;
            }

            if(rooms.Count % 5 == 0)
            {
                player.Health++;
                player.Damage++;
                levelUpFrames = 60;
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
                    roomOffset.Y += 1000;
                    inDirection = Direction.Up;
                    break;

                case Direction.Up:
                    roomOffset.Y -= 1000;
                    inDirection = Direction.Down;
                    break;

                case Direction.Left:
                    roomOffset.X -= 1000;
                    inDirection = Direction.Right;
                    break;

                case Direction.Right:
                    roomOffset.X += 1000;
                    inDirection = Direction.Left;
                    break;
            }

            //this variable is always the last room in rooms, but it will be accessed a lot so this makes it easier. 
            //create the room
            lastRoom = new Room(
                roomTemplates[inDirection][rng.Next(0, roomTemplates[inDirection].Count)],
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
            }

            //Spawn enemies
            for (int i = 0; i < numEnemies; i++)
            {
                // Create a random spawn location for the enemy
                Vector2 enemySpawn = new Vector2(
                    rng.Next(lastRoom.EnemySpawnArea.X, lastRoom.EnemySpawnArea.X + lastRoom.EnemySpawnArea.Width),
                    rng.Next(lastRoom.EnemySpawnArea.Y, lastRoom.EnemySpawnArea.Y + lastRoom.EnemySpawnArea.Height));
                
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
                        rangedProjectile, enemyRangedSFX));
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
                        whiteSquare, enemyRangedSFX));
                }
            }

            //creates the exit door, which opens when the enemies are defeated.
            foreach(GameObject doorTile in lastRoom.OutDoor)
            {
                obstacles.Add(doorTile);
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
                        "DtoU_Octogon", Direction.Down, Direction.Up,
                        new Rectangle(150, 150, 700, 700),
                        tiles),

                    new RoomData(
                        "DtoR_LShape", Direction.Down, Direction.Right,
                        new Rectangle(475, 475, 450, 50),
                        tiles)
                });

            rooms.Add(Direction.Left,
                new List<RoomData>
                {
                    new RoomData(
                        "LtoU_LShape", Direction.Left, Direction.Up,
                        new Rectangle(475, 0, 50, 450),
                        tiles)
                });


            return rooms;
        }


        /// <summary>
        /// draws the game
        /// </summary>
        public void GameDraw()
        {
            foreach (Room room in rooms)
            {
                room.Draw(_spriteBatch);
                
            }

            // testing
            //_spriteBatch.DrawString(arial16, screenOffset.X.ToString(), new Vector2(300, 300), Color.White);
            //_spriteBatch.DrawString(arial16, screenOffset.Y.ToString(), new Vector2(300, 330), Color.White);
            //_spriteBatch.DrawString(arial16, player.IsInteracting.ToString(), new Vector2(300, 300), Color.White);

            foreach (Weapon elem in weapons)
            {
                elem.Draw(_spriteBatch);
            }

            foreach(Potion elem in potions)
            {
                elem.Draw(_spriteBatch);
            }

            foreach (Enemy elem in enemies)
            {
                elem.Draw(_spriteBatch);
            }


            player.Draw(_spriteBatch);

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

                // If player is at critical health, draw low health background effect
                if (player.Health <= 1)
                {
                    _spriteBatch.Draw(playerLowHealthBG, new Rectangle(-300, -300, _graphics.PreferredBackBufferWidth + 600, _graphics.PreferredBackBufferHeight + 600), Color.White);
                }
            }

            // If player kills all enemies, notify them to move forward
            if (!EnteredLastRoom)
            {
                _spriteBatch.DrawString(fffforward20, "Room cleared! Proceed to next room.", new Vector2(300, 25), Color.White);

                // Draw direction arrow to direct player to next room
                _spriteBatch.Draw(directionPointer, new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 50, 110, 100, 75), 
                    null, Color.White, -MathF.Acos(
                    (rooms[rooms.Count - 2].OutDoor[0].ScreenLoc - player.ScreenLoc).X /
                    (rooms[rooms.Count - 2].OutDoor[0].ScreenLoc - player.ScreenLoc).Length()), 
                    new Vector2(directionPointer.Width / 2, directionPointer.Height / 2), SpriteEffects.None, 0);
            }
            // Stop displaying message when player enters the latest room
            foreach (GameObject enterObs in rooms[rooms.Count - 2].OutDoor)
            {
                if (player.GetRect().Intersects(enterObs.GetRect()))
                {
                    EnteredLastRoom = true;
                }
            }

            //weapon info (bottom left)
            switch (player.CurrentWeapon)
            {
                case weaponType.Spear:
                    _spriteBatch.Draw(spear, new Rectangle(10, _graphics.PreferredBackBufferHeight - 60, 50, 50), Color.White);
                    break;

                case weaponType.Sword:
                    _spriteBatch.Draw(sword, new Rectangle(10, _graphics.PreferredBackBufferHeight - 60, 50, 50), Color.White);
                    break;
            }
            
            _spriteBatch.DrawString(arial16, player.Damage.ToString(), new Vector2(10, _graphics.PreferredBackBufferHeight - 60), Color.Black);

            //"level up" text

            if (levelUpFrames > 0)
            {
                _spriteBatch.DrawString(
                    fffforward20, "Attack +1! Health +1!",
                    new Vector2(
                        _graphics.PreferredBackBufferWidth / 2 - (fffforward20.MeasureString("Attack +1! Health +1!").X / 2),
                        _graphics.PreferredBackBufferHeight / 2 - (fffforward20.MeasureString("Attack +1! Health +1!").Y / 2)),
                    Color.Yellow
                    );
            }
        }
    }
}

