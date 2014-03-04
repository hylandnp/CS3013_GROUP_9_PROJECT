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
 * NEIL - Slight modifications & added Kinect status debug messages that can be displayed on screen.
 * NEIL - Moved the colour stream manager to the Kinect manager class.
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
        private bool using_kinect_input = false,
                     status_debug_messages = false,
                     display_video_streams = false;
        //private SpriteFont ui_font;

        private Texture2D logo = null; // game loading splash/logo
        private Vector2 logo_pos;

        // Kinect manager:
        public KinectManager kinect_manager = null;

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

            // Set game flags:
            this.using_kinect_input = true; // set to false if you want to use the mouse to simulate Kinect input
            this.status_debug_messages = true; // set to true if you want Kinect status messages displayed in the top-left corner
            this.display_video_streams = true; // set to true if you want the Kinect video/data streams to be rendered at the right-hand side of the screen (windowed mode only)

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
            this.current_game_state = GameState.STARTUP;

            // Configure window:
            this.Window.Title = "Kinect Puzzle Game";
            this.Window.AllowUserResizing = false;

            base.Initialize();
        }



        /*/////////////////////////////////////////
         * GRAPHICAL DATA & RESOURCE LOADING
         */////////////////////////////////////////
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.sprite_batch = new SpriteBatch(this.GraphicsDevice);

            // Load game logo:
            this.logo = this.Content.Load<Texture2D>("Textures/Interface/UI_Logo");
            this.logo_pos = new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width - this.logo.Width) / 2.0),
                                        (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height - this.logo.Height) / 2.0));

            // Create Kinect manager:
            this.kinect_manager = new KinectManager(ColorImageFormat.RgbResolution640x480Fps30,
                                                    DepthImageFormat.Resolution640x480Fps30,
                                                    this);
        }
     


        /*/////////////////////////////////////////
          * GRAPHICAL DATA & RESOURCE DISPOSAL
          *////////////////////////////////////////
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            if(this.kinect_manager != null) this.kinect_manager.close(); // close the kinect sensor
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
            // Pre-render the debug skeleton stream video to a texture:
            //if (this.display_video_streams &&
            //    this.kinect_manager != null &&
            //    this.sprite_batch != null)
            //{
            //    this.kinect_manager.skeletonStreamPreRender(this.sprite_batch);
            //}

            this.GraphicsDevice.Clear(Color.Black);

            // RENDERING:
            switch(current_game_state)
            {
                case GameState.STARTUP:
                    {
                        // Draw the logo:
                        this.sprite_batch.Begin();
                        if (logo != null) this.sprite_batch.Draw(this.logo, this.logo_pos, Color.White);
                        this.sprite_batch.End();

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
                        this.sprite_batch.Begin();
                        if (logo != null) this.sprite_batch.Draw(this.logo, this.logo_pos, Color.White);
                        this.sprite_batch.End();

                        break;
                    }
            }


            // Draw video debug streams
            if(this.display_video_streams &&
               this.kinect_manager != null)
            {
                this.kinect_manager.drawStreamManagers(this.sprite_batch);
            }

            // Draw debug Kinect status:
            if(this.status_debug_messages &&
               this.kinect_manager != null)
            {
                this.kinect_manager.drawStatusMessage(this.sprite_batch);
            }

            base.Draw(p_game_time);
        }
    }
}
