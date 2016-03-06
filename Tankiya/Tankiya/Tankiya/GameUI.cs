using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//custom packages used in this class
using tank_game;

namespace Tankiya
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameUI : Microsoft.Xna.Framework.Game
    {
        #region XNA Variables
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Texture2D backgroundTexture;
        Texture2D foregroundTexture;
        Texture2D tankTexture;
        Texture2D bulletTexture;
        Texture2D waterTexture;
        Texture2D brickTexture;
        Texture2D coinTexture;
        Texture2D stoneTexture;
        Texture2D healthTexture;
        SpriteFont font;
        KeyboardState keyboardState;
        int screenWidth;
        int screenHeight;
        int gridWidth;

        
        #endregion

        /**
         * Command sender and similar variables to connect with the server.
         * 
         */
        private BasicCommandSender commandSender;
        private Map map;
        private int join_count = 0;
        /// <summary>
        /// Colors array to color the tanks
        /// </summary>
        private Color[] playerColors = new Color[] { Color.LightBlue, Color.Brown, Color.Yellow, Color.Pink, Color.Red };


        public GameUI()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Mustank2016   |  Press J button to join";

            keyboardState = Keyboard.GetState();
            commandSender = new BasicCommandSender();
            map = Map.GetInstance();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;

            //screenWidth = device.PresentationParameters.BackBufferWidth;
            //screenHeight = device.PresentationParameters.BackBufferHeight;

            screenWidth = 600;
            screenHeight = 600;
            gridWidth = 60;

            backgroundTexture = Content.Load<Texture2D>("back");
            foregroundTexture = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(GenerateMap());
            tankTexture = Content.Load<Texture2D>("tank_min");
            bulletTexture=Content.Load<Texture2D>("bullet");
            waterTexture = Content.Load<Texture2D>("water_min");
            brickTexture = Content.Load<Texture2D>("brick");
            coinTexture = Content.Load<Texture2D>("coin");
            stoneTexture = Content.Load<Texture2D>("stone");
            healthTexture = Content.Load<Texture2D>("health");
            font = Content.Load<SpriteFont>("SegoeUI");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            //read the keyboard input
            GetInput();

            base.Update(gameTime);
        }



        #region Read Inputs from Keyboard
        /// <summary>
        /// Read the keyboard inputs and call the command sender class to give the corresponding response
        /// </summary>
        private void GetInput()
        {

            KeyboardState newState = Keyboard.GetState();

            //if "J" is pressed, send the command to join to the server
            if (newState.IsKeyDown(Keys.J))
            {
                if (!keyboardState.IsKeyDown(Keys.J))
                {
                    if (join_count == 0)
                    {
                        commandSender.Join();
                        join_count += 1;
                    }
                }
            }

            /*
            if (newState.IsKeyDown(Keys.Space))
            {
                if (!keyboardState.IsKeyDown(Keys.Space))
                {
                    commandSender.Shoot();
                }
            }

            

            if (newState.IsKeyDown(Keys.A))
            {
                if (!keyboardState.IsKeyDown(Keys.A))
                {
                    map.playingMethod = 0;
                }
            }

            if (newState.IsKeyDown(Keys.S))
            {
                if (!keyboardState.IsKeyDown(Keys.S))
                {
                    map.playingMethod = 1;
                }
            }

            if (newState.IsKeyDown(Keys.D))
            {
                if (!keyboardState.IsKeyDown(Keys.D))
                {
                    map.playingMethod = 2;
                }
            }

            if (newState.IsKeyDown(Keys.F))
            {
                if (!keyboardState.IsKeyDown(Keys.F))
                {
                    map.playingMethod = 3;
                }
            }

            if (newState.IsKeyDown(Keys.NumPad1))
            {
                if (!keyboardState.IsKeyDown(Keys.NumPad1))
                {
                    map.op_id=1;
                    Console.WriteLine("opponent selected :1");
                }
            }

            if (newState.IsKeyDown(Keys.NumPad2))
            {
                if (!keyboardState.IsKeyDown(Keys.NumPad2))
                {
                    map.op_id = 2;
                    Console.WriteLine("opponent selected :2");
               
                }
            }

            if (newState.IsKeyDown(Keys.NumPad3))
            {
                if (!keyboardState.IsKeyDown(Keys.NumPad3))
                {
                    map.op_id = 3;
                    Console.WriteLine("opponent selected :3");
               
                }
            }

            if (newState.IsKeyDown(Keys.NumPad0))
            {
                if (!keyboardState.IsKeyDown(Keys.NumPad0))
                {
                    map.op_id = 0;
                    Console.WriteLine("opponent selected :0");
               
                }
            }
            
            if (newState.IsKeyDown(Keys.NumPad4))
            {
                if (!keyboardState.IsKeyDown(Keys.NumPad4))
                {
                    map.op_id = 4;
                    Console.WriteLine("opponent selected :4");
               
                }
            }
             * */
        }

        #endregion


        #region Methods to Draw Map Objects

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            
            DrawScenery();
            DrawObstacles();
            DrawTanks();
            DrawBullet();
            DrawScores();

            spriteBatch.End();

            base.Draw(gameTime);
        }



        /// <summary>
        /// Draws the background and the foreground.
        /// </summary>
        private void DrawScenery()
        {

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        /*
        texture
        Type: Texture2D
        A texture.
        position
        Type: Vector2
        The location (in screen coordinates) to draw the sprite.
        sourceRectangle
        Type: Nullable<Rectangle>
        A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
        color
        Type: Color
        The color to tint a sprite. Use Color.White for full color with no tinting.
        rotation
        Type: Single
        Specifies the angle (in radians) to rotate the sprite about its center.
        origin
        Type: Vector2
        The sprite origin; the default is (0,0) which represents the upper-left corner.
        scale
        Type: Vector2
        Scale factor.
        effects
        Type: SpriteEffects
        Effects to apply.
        layerDepth
        Type: Single
        The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. 
                 * Use SpriteSortMode if you want sprites to be sorted during drawing.
         * */

        private void DrawTanks()
        {
            Player[] players = Map.GetInstance().GetPlayers();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].health > 0)
                {

                    spriteBatch.Draw(tankTexture, new Vector2(players[i].cordinateX * 60 + 30, players[i].cordinateY * 60 + 30),
                        null, playerColors[i], GetRotation(players[i].direction), new Vector2(30, 30), 1, SpriteEffects.None, 1);

                }
            }
        }
        private void DrawBullet()
        {
            List<Bullet> bullet_list = map.bullet_list;
           for (int i = 0; i < bullet_list.Count; i++)
            {
               
                if (bullet_list[i].isAlive)
                {
                    spriteBatch.Draw(bulletTexture, new Vector2(bullet_list[i].current_cordinate.x * 60 + 30, bullet_list[i].current_cordinate.y * 60 + 30),
                    null, Color.White, GetRotation(bullet_list[i].direction), new Vector2(30, 30), 1, SpriteEffects.None, 1);
                }
               
           }
        }


        /// <summary>
        /// Draws the obstacles in the map. Stone, water brick coins and health packs
        /// </summary>
        private void DrawObstacles()
        {
            
            MapItem[,] grid = Map.GetInstance().GetGrid();
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {

                    /**
                     * Draw water
                     */
                    if (grid[i, j] != null && grid[i, j].GetType() == typeof(Water))
                    {

                        spriteBatch.Draw(waterTexture, new Vector2(i * 60, j * 60),
                        null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);

                    }

                    /**
                     * Draw bricks
                     */
                    else if (grid[i, j] != null && grid[i, j].GetType() == typeof(Brick))
                    {
                        spriteBatch.Draw(brickTexture, new Vector2(i * 60, j * 60),
                        null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }


                    /**
                     * Draw coins
                     */
                    else if (grid[i, j] != null && grid[i, j].GetType() == typeof(Coin))
                    {
                        spriteBatch.Draw(coinTexture, new Vector2(i * 60, j * 60),
                        null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }

                    /**
                     * Draw HealthPack
                     */
                    else if (grid[i, j] != null && grid[i, j].GetType() == typeof(HealthPack))
                    {
                        spriteBatch.Draw(healthTexture, new Vector2(i * 60, j * 60),
                        null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }

                    /**
                     * Draw stones
                     */
                    else if (grid[i, j] != null && grid[i, j].GetType() == typeof(Stone))
                    {
                        spriteBatch.Draw(stoneTexture, new Vector2(i * 60, j * 60),
                        null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
                    }

                }

            }
        }


        /// <summary>
        /// Draw the scores of each player on the map
        /// </summary>
        private void DrawScores() {
            Player[] players = Map.GetInstance().GetPlayers();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    String description = players[i].health + " " + players[i].points + " " + players[i].coins;
                    spriteBatch.DrawString(font,description,new Vector2(600,50*i),playerColors[i]);
                }
            }
        }

        /*
                0 North
                1 East,
                2 South 
                3 West 
         */
        private float GetRotation(int direction)
        {
            switch (direction)
            {
                case 0:
                    return ToRadian(0);
                    break;
                case 1:
                    return ToRadian(90);
                    break;
                case 2:
                    return ToRadian(180);
                    break;
                case 3:
                    return ToRadian(-90);
                    break;
            }

            return 0;

        }

        private float ToRadian(int degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }

        /// <summary>
        /// Generates the grid where the game will be played. A 10x10 grid
        /// </summary>
        /// <returns></returns>
        private Color[] GenerateMap()
        {
            Color[] grid = new Color[screenHeight * screenWidth];

            for (int i = 0; i < screenWidth; i++)
            {
                for (int j = 0; j < screenHeight; j++)
                {
                    grid[i + screenWidth * j] = Color.Transparent;
                    if (i % gridWidth == 0)
                    {
                        grid[i + screenWidth * j] = Color.DarkGreen;
                    }

                    if (j % gridWidth == 0)
                    {
                        grid[i + screenWidth * j] = Color.DarkGreen;
                    }
                }
            }

            return grid;
        }
    }
}

        #endregion