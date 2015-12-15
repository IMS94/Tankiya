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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GraphicsDevice device;

        Texture2D backgroundTexture;
        Texture2D foregroundTexture;

        KeyboardState keyboardState;


        int screenWidth;
        int screenHeight;
        int gridWidth;



        /**
         * Command sender and similar variables to connect with the server.
         * 
         */
        private BasicCommandSender commandSender;
        private MapItem[,] grid;





        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.grid = Map.grid;
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

            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "The Tankiya";

            keyboardState = Keyboard.GetState();
            commandSender = new BasicCommandSender();
            
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




        /// <summary>
        /// Read the keyboard inputs and call the command sender class to give the corresponding response
        /// </summary>
        private void GetInput() {

            KeyboardState newState = Keyboard.GetState();

            //if "J" is pressed, send the command to join to the server
            if(newState.IsKeyDown(Keys.J)){
                if(!keyboardState.IsKeyDown(Keys.J)){
                    commandSender.Join();
                }
            }

            //if "Space" is pressed, send the command to shoot
            if (newState.IsKeyDown(Keys.Space))
            {
                if (!keyboardState.IsKeyDown(Keys.Space))
                {
                    commandSender.Shoot();
                }
            }


            //if "Up" is pressed, send the command to go up
            if (newState.IsKeyDown(Keys.Up))
            {
                if (!keyboardState.IsKeyDown(Keys.Up))
                {
                    commandSender.Up();
                }
            }

            //if "Down" is pressed, send the command to go down
            if (newState.IsKeyDown(Keys.Down))
            {
                if (!keyboardState.IsKeyDown(Keys.Down))
                {
                    commandSender.Down();
                }
            }

            //if "Left" is pressed, send the command to go Left
            if (newState.IsKeyDown(Keys.Left))
            {
                if (!keyboardState.IsKeyDown(Keys.Left))
                {
                    commandSender.Left();
                }
            }
            
            //if "Right" is pressed, send the command to go right
            if (newState.IsKeyDown(Keys.Right))
            {
                if (!keyboardState.IsKeyDown(Keys.Right))
                {
                    commandSender.Right();
                }
            }

            keyboardState = newState;
        }




        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawScenery();
            spriteBatch.End();

            base.Draw(gameTime);
        }



        /// <summary>
        /// Draws the background and the foreground.
        /// </summary>
        private void DrawScenery() {

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }




        /// <summary>
        /// Generates the grid where the game will be played. A 10x10 grid
        /// </summary>
        /// <returns></returns>
        private Color[] GenerateMap() { 
            Color[] grid=new Color[screenHeight*screenWidth];

            for (int i = 0; i < screenWidth;i++ )
            {
                for (int j = 0; j < screenHeight; j++)
                {
                    grid[i+screenWidth*j]=Color.Transparent;
                    if(i%gridWidth==0){
                        grid[i + screenWidth * j] = Color.White;
                    }

                    if(j%gridWidth==0){
                        grid[i + screenWidth * j] = Color.White;
                    }
                }
            }

            return grid;
        } 
    }
}
