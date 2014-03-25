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
using KinectGame_WindowsXNA.Source.Interface;
using KinectGame_WindowsXNA.Source.Game;

/*CHANGELOG
 * NEIL - Created class & basic functionality.
 * NEIL - Slight modifications & added Kinect status debug messages that can be displayed on screen.
 * NEIL - Moved the colour stream manager to the Kinect manager class.
 * PATRICK - Added debug mouse tracking functionality & cursor updates/rendering.
 * GAVAN - Added buttons.
 * NEIL/PATRICK - Implemented basic painting/colouring game.
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

        private Texture2D logo = null; // game loading splash/logo
        private Vector2 logo_pos;

        // Kinect manager:
        public KinectManager kinect_manager { get; private set; }

        // Game states:
        private enum GameState : byte
        {
            STARTUP,
            MENU,
            PUZZLE_PAINT,
            PUZZLE_MAKE
        }
        private GameState current_game_state = GameState.STARTUP;

        // Player hand position cursors:
        public Cursor player_1_cursor { get; private set; }
        public Cursor player_2_cursor { get; private set; }

        private Button test_button;

        // Game stages:
        private PaintingGame painting_game = null;
        private PuzzleGame puzzle_game = null;


        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public KinectGame_WindowsXNA()
        {
            // Create the game class...
            this.graphics = new GraphicsDeviceManager(this);

            // Set game flags:
            this.using_kinect_input = false; // set to false if you want to use the mouse to simulate Kinect input
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

            // Create the minigame classes:
            this.painting_game = new PaintingGame();
            this.puzzle_game = new PuzzleGame();

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

            // Create player cursors:
            this.player_1_cursor = new Cursor(this.Content.Load<Texture2D>("Textures/Interface/UI_CursorHand"),
                                              this.Content.Load<Texture2D>("Textures/Interface/UI_CursorColourIcon"),
                                              this.Content.Load<SpriteFont>("Fonts/Segoe16"),
                                              JointType.HandLeft,
                                              0.3f,
                                              0);
            this.player_2_cursor = new Cursor(this.Content.Load<Texture2D>("Textures/Interface/UI_CursorHand"),
                                              this.Content.Load<Texture2D>("Textures/Interface/UI_CursorColourIcon"),
                                              this.Content.Load<SpriteFont>("Fonts/Segoe16"),
                                              JointType.HandRight,
                                              0.3f,
                                              1);

            this.test_button = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_Logo"),
                                          2.0f);
            // Load minigames:
            this.painting_game.load(this.Content,
                                    this.GraphicsDevice);

            // DEBUG: test colours
            this.player_1_cursor.selected_colour = Color.Red;
            this.player_2_cursor.selected_colour = Color.Red;
        }
     


        /*/////////////////////////////////////////
          * GRAPHICAL DATA & RESOURCE DISPOSAL
          *////////////////////////////////////////
        protected override void UnloadContent()
        {
            if(this.kinect_manager != null) this.kinect_manager.close(); // close the kinect sensor
        }



        /*/////////////////////////////////////////
         * GAME LOGIC UPDATE FUNCTION
         */////////////////////////////////////////
        protected override void Update(GameTime p_game_time)
        {
            // Update the game before rendering...
            Vector3 temp_pos = Vector3.Zero;
            TimeSpan time_span = p_game_time.ElapsedGameTime;

            // DEBUG - using mouse input if not testing with Kinect:
            if(!this.using_kinect_input)
            {
                // Get the position of the mouse:
                var mouse_info = Mouse.GetState();
                temp_pos.X = mouse_info.X;
                temp_pos.Y = mouse_info.Y;
            }


            // Update the player cursors:
            if(this.player_1_cursor != null)
            {
                if(this.using_kinect_input)
                {
                    // Update position with the Kinect tracking of Player 1's preferred hand:
                    this.player_1_cursor.update(this.kinect_manager.getMappedJointPosition(this.player_1_cursor.getHandJoint(),
                                                                                           this.player_1_cursor.player_id),
                                                time_span);
                }
                else
                {
                    // Update position with the mouse/screen co-ordinates for testing purposes:
                    this.player_1_cursor.update(temp_pos,
                                                time_span);
                }
            }

            if(this.player_2_cursor != null &&
               this.using_kinect_input)
            {
                // Update position with the Kinect tracking of Player 2's preferred hand:
                this.player_2_cursor.update(this.kinect_manager.getMappedJointPosition(this.player_2_cursor.getHandJoint(),
                                                                                       this.player_2_cursor.player_id),
                                            time_span);
            }
            

            // LOADING/UPDATING STATE(S):
            switch(this.current_game_state)
            {
                case GameState.STARTUP:
                    {
                        this.current_game_state = GameState.MENU;
                        break;
                    }
                case GameState.MENU:
                    {
                        // Handle one/two-player button presses:
                        // TODO
                        this.test_button.Update(player_1_cursor, p_game_time);

                        if(this.test_button.isClicked())
                        {
                            Console.WriteLine("hello autistic world!");
                            this.current_game_state = GameState.PUZZLE_PAINT;
                        }
                        //if(this.test_button.isClicked())
                        //{
                        //     DO SOMETHING WITH BUTTON
                        //    Console.WriteLine("hello");
                        //}
                        break;
                    }
                case GameState.PUZZLE_PAINT:
                    {
                        // Handle painting game:
                        if(painting_game != null)
                        {
                            painting_game.update(p_game_time,
                                                 this.player_1_cursor,
                                                 this.player_2_cursor);
                        }
                        break;
                    }
                case GameState.PUZZLE_MAKE:
                    {
                        // Handle puzzle game:
                        if (puzzle_game != null)
                        {
                            puzzle_game.update(p_game_time);
                        }
                        break;
                    }
                default:
                    {
                        break; // do nothing
                    }
            }

            base.Update(p_game_time);
        }



        /*/////////////////////////////////////////
          * GAME ANIMATION & RENDERING FUNCTION
          *////////////////////////////////////////
        protected override void Draw(GameTime p_game_time)
        {
            // Pre-render the debug skeleton stream video to a texture:
            if (this.display_video_streams &&
                this.kinect_manager != null &&
                this.sprite_batch != null)
            {
                this.kinect_manager.skeletonStreamPreRender(this.sprite_batch);
            }

            this.GraphicsDevice.Clear(Color.Black);

            // RENDERING GAME STATE(S):
            switch(this.current_game_state)
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
                        this.test_button.Draw(this.sprite_batch);
                        break;
                    }
                case GameState.PUZZLE_PAINT:
                    {
                        // Draw the puzzle-painting game:
                        if (painting_game != null)
                        {
                            painting_game.draw(p_game_time, this.sprite_batch);
                        }
                        break;
                    }
                case GameState.PUZZLE_MAKE:
                    {
                        // Draw the puzzle-piece assembly game:
                        if (puzzle_game != null)
                        {
                            puzzle_game.draw(p_game_time, this.sprite_batch);
                        }
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


            // Draw debug video streams (if required):
            if(this.display_video_streams &&
               this.kinect_manager != null)
            {
                this.kinect_manager.drawStreamManagers(this.sprite_batch);
            }

            // Draw debug Kinect status (if required):
            if(this.status_debug_messages &&
               this.kinect_manager != null)
            {
                this.kinect_manager.drawStatusMessage(this.sprite_batch);
                this.kinect_manager.drawSkeletonGesture(this.sprite_batch);
            }

            // Draw player hand cursors:
            if (this.player_1_cursor != null)
            {
                this.player_1_cursor.draw(this.sprite_batch);
            }

            if (this.player_2_cursor != null &&
                this.using_kinect_input)
            {
                this.player_2_cursor.draw(this.sprite_batch);
            }

            base.Draw(p_game_time);
        }
    }
}
