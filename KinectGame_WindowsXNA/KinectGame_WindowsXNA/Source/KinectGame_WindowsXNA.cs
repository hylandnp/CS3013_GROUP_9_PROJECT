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

using Microsoft.Kinect;
using KinectGame_WindowsXNA.Source.KinectUtils;

/*CHANGELOG
 * NEIL - Created class & basic functionality.
 */

namespace KinectGame_WindowsXNA
{
    public class KinectGame_WindowsXNA : Microsoft.Xna.Framework.Game
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private GraphicsDeviceManager graphics = null;
        private SpriteBatch sprite_batch = null;
        private KinectSensor kinect = null;
        private bool using_kinect_input = false;

        private Texture2D logo = null; // game loading splash/logo
        private Vector2 logo_pos;

        // Kinect selector tool:
        KinectSelector selector = null;

        // Game states:
        private enum GameState : byte
        {
            STARTUP,
            MENU,
            PUZZLE_PAINT,
            PUZZLE_MAKE
        }
        GameState current_game_state = GameState.STARTUP;



        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public KinectGame_WindowsXNA()
        {
            // Create the game class...
            this.graphics = new GraphicsDeviceManager(this);

            // Configure graphical settings:
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferMultiSampling = true; // use antialiasing
            this.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            this.graphics.SynchronizeWithVerticalRetrace = true; // use vsync

            this.Content.RootDirectory = "Content";
        }



        /*/////////////////////////////////////////
          * NON-GRAPHICAL DATA INITIALISATION
          *////////////////////////////////////////
        protected override void Initialize()
        {
            // Initialise the game class...
            this.using_kinect_input = true; // set to false if you want to use the mouse to simulate Kinect input

            base.Initialize();
        }



        /*/////////////////////////////////////////
         * GRAPHICAL DATA & RESOURCE LOADING
         */////////////////////////////////////////
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.sprite_batch = new SpriteBatch(GraphicsDevice);

            // Load game logo:
            this.logo = this.Content.Load<Texture2D>("Textures/Kinect/UI_Logo");
            this.logo_pos = new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width - this.logo.Width) / 2.0),
                                        (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height - this.logo.Height) / 2.0));

            // Create Kinect selector:
            this.selector = new KinectSelector(ColorImageFormat.RgbResolution640x480Fps30,
                                               DepthImageFormat.Resolution640x480Fps30);
                                               //this.Content.Load<Texture2D>("Textures/Kinect/SelectorUI_Logo"),
                                               //this.Content.Load<SpriteFont>("Fonts/Segoe16"));
        }



        /*/////////////////////////////////////////
          * GRAPHICAL DATA & RESOURCE DISPOSAL
          *////////////////////////////////////////
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /*/////////////////////////////////////////
         * GAME LOGIC UPDATE FUNCTION
         */////////////////////////////////////////
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

            // LOADING/UPDATING:
            switch(current_game_state)
            {
                case GameState.STARTUP:
                    {
                        break;
                    }
                case GameState.MENU:
                    {
                        break;
                    }
                case GameState.PUZZLE_PAINT:
                    {
                        break;
                    }
                case GameState.PUZZLE_MAKE:
                    {
                        break;
                    }
                default:
                    {
                        break; // do nothing
                    }
            }

            base.Update(gameTime);
        }



        /*/////////////////////////////////////////
          * GAME ANIMATION & RENDERING FUNCTION
          *////////////////////////////////////////
        protected override void Draw(GameTime p_game_time)
        {
            this.GraphicsDevice.Clear(Color.Black);

            // RENDERING:
            this.sprite_batch.Begin(); // start the sprite batch

            switch(current_game_state)
            {
                case GameState.STARTUP:
                    {
                        // Draw the logo:
                        if (logo != null) this.sprite_batch.Draw(this.logo, this.logo_pos, Color.White);
                        break;
                    }
                case GameState.MENU:
                    {
                        // Draw the simple main menu:
                        break;
                    }
                case GameState.PUZZLE_PAINT:
                    {
                        // Draw the inidividual puzzle-painting game:
                        break;
                    }
                case GameState.PUZZLE_MAKE:
                    {
                        // Draw the final puzzle assembly game:
                        break;
                    }
                default:
                    {
                        // Draw the logo:
                        if (logo != null) this.sprite_batch.Draw(this.logo, this.logo_pos, Color.White);
                        break;
                    }
            }

            this.sprite_batch.End(); // finish the sprite batch

            base.Draw(p_game_time);
        }
    }
}
