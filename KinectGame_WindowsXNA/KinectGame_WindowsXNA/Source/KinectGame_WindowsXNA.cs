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
        private SpriteFont ui_font;

        private Texture2D logo = null; // game loading splash/logo
        private Vector2 logo_pos;

        // Kinect tools:
        public KinectSelector selector = null;
        public ColourStreamVideo colour_video = null;

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
            this.logo = this.Content.Load<Texture2D>("Textures/Kinect/UI_Logo");
            this.logo_pos = new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width - this.logo.Width) / 2.0),
                                        (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height - this.logo.Height) / 2.0));

            // Create Kinect selector:
            this.selector = new KinectSelector(ColorImageFormat.RgbResolution640x480Fps30,
                                               DepthImageFormat.Resolution640x480Fps30,
                                               this);
            ui_font = this.Content.Load<SpriteFont>("Fonts/Segoe16");

            
            if(this.display_video_streams)
            {
                // Create debug colour stream video:
                colour_video = new ColourStreamVideo(new Rectangle(this.GraphicsDevice.Viewport.Width - 128,
                                                                   0, 128, 96),
                                                     this.Content.Load<Effect>("Effects_Shaders/KinectColorVisualizer"),
                                                     this.selector,
                                                     this.GraphicsDevice);
            }
            
        }
     


        /*/////////////////////////////////////////
          * GRAPHICAL DATA & RESOURCE DISPOSAL
          *////////////////////////////////////////
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            if(this.selector != null) this.selector.close(); // close the kinect sensor
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
            if(display_video_streams &&
               colour_video != null &&
               this.selector.kinect_sensor != null &&
               this.selector.kinect_sensor.Status == KinectStatus.Connected)
            {
                colour_video.draw(this.sprite_batch, this.selector);
            }

            // Draw debug Kinect status:
            if(status_debug_messages && selector != null)
            {
                this.sprite_batch.Begin();
                this.sprite_batch.DrawString(this.ui_font,
                                             this.selector.last_status.ToString(),
                                             new Vector2(4.0f, 2.0f),
                                             Color.White);
                this.sprite_batch.End();
            }

            base.Draw(p_game_time);
        }
    }
}
