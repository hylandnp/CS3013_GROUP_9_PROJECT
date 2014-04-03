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
 * RICHARD - Some minor bugfixes.
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
            PLAYER_MENU,
            HAND_SELECT,
            PUZZLE_PAINT,
            PUZZLE_MAKE
        }
        private GameState current_game_state = GameState.STARTUP;

        // Player hand position cursors:
        public Cursor player_1_cursor { get; private set; }
        public Cursor player_2_cursor { get; private set; }

        // Main menu buttons:
        private Button mainmenu_button_1player = null,
                       mainmenu_button_2player = null,
                       mainmenu_lefthand_1 = null,
                       mainmenu_lefthand_2 = null,
                       mainmenu_righthand_1 = null,
                       mainmenu_righthand_2 = null,
                       mainmenu_startgame = null;

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

#if DEBUG
            this.status_debug_messages = true; // set to true if you want Kinect status messages displayed in the top-left corner
            this.display_video_streams = true; // set to true if you want the Kinect video/data streams to be rendered at the right-hand side of the screen
#else
            this.status_debug_messages = false;
            this.display_video_streams = false;
#endif

            // Configure graphical settings:
            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.PreferredBackBufferHeight = 768;
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
                                              1.0f,
                                              0);
            this.player_2_cursor = new Cursor(this.Content.Load<Texture2D>("Textures/Interface/UI_CursorHand"),
                                              this.Content.Load<Texture2D>("Textures/Interface/UI_CursorColourIcon"),
                                              this.Content.Load<SpriteFont>("Fonts/Segoe16"),
                                              JointType.HandRight,
                                              1.0f,
                                              1);

            // Create main-menu/player-selection buttons (with offset):
            this.mainmenu_button_1player = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_SinglePlayer"),
                                                      1.2f,
                                                      new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width / 2.0f) - 270),
                                                                  (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height - 360) / 2.0f)),
                                                      GestureType.NONE);

            this.mainmenu_button_2player = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_MultiPlayer"),
                                                      1.2f,
                                                      new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width / 2.0f) + 10),
                                                                  (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height - 360) / 2.0f)),
                                                      GestureType.NONE);

            // Create hand selection buttons (with offset):
            this.mainmenu_lefthand_1 = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_LeftHand"),
                                                  1.2f,
                                                  new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width / 2.0f) - 270),
                                                              (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height / 2.0f) - 160)),
                                                  GestureType.NONE);

            this.mainmenu_lefthand_2 = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_LeftHand"),
                                                  1.2f,
                                                  new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width / 2.0f) + 110),
                                                              (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height / 2.0f) - 160)),
                                                  GestureType.NONE);

            this.mainmenu_righthand_1 = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_RightHand"),
                                                   1.2f,
                                                   new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width / 2.0f) - 270),
                                                               (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height / 2.0f))),
                                                   GestureType.NONE);

            this.mainmenu_righthand_2 = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_RightHand"),
                                                   1.2f,
                                                   new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width / 2.0f) + 110),
                                                               (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height / 2.0f))),
                                                   GestureType.NONE);

            // Create start game button (with offset):
            this.mainmenu_startgame = new Button(this.Content.Load<Texture2D>("Textures/Interface/UI_StartGame"),
                                                 1.2f,
                                                 new Vector2((float)Math.Ceiling((this.GraphicsDevice.Viewport.Width - 160) / 2.0f),
                                                             (float)Math.Ceiling((this.GraphicsDevice.Viewport.Height / 2.0f) + 100)),
                                                 GestureType.NONE);

            this.player_1_cursor.selected_colour = Color.Red;
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
                                                time_span,
                                                this.kinect_manager.getCurrentGesture(this.player_1_cursor.player_id));
                }
                else
                {
                    // Update position with the mouse/screen co-ordinates for testing purposes:
                    this.player_1_cursor.update(temp_pos,
                                                time_span,
                                                GestureType.NONE);
                }
            }

            if(this.player_2_cursor != null &&
               this.using_kinect_input)
            {
                // Update position with the Kinect tracking of Player 2's preferred hand:
                this.player_2_cursor.update(this.kinect_manager.getMappedJointPosition(this.player_2_cursor.getHandJoint(),
                                                                                       this.player_2_cursor.player_id),
                                            time_span,
                                            this.kinect_manager.getCurrentGesture(this.player_2_cursor.player_id));
            }
            

            // LOADING/UPDATING STATE(S):
            switch(this.current_game_state)
            {
                case GameState.STARTUP:
                    {
                        this.current_game_state = GameState.PLAYER_MENU;
                        break;
                    }
                case GameState.PLAYER_MENU:
                    {
                        // Handle one/two-player button presses:
                        if (this.mainmenu_button_1player != null)
                        {
                            this.mainmenu_button_1player.Update(this.player_1_cursor, p_game_time);
                        }

                        if (this.mainmenu_button_2player != null)
                        {
                            this.mainmenu_button_2player.Update(this.player_1_cursor, p_game_time);
                        }

                        if (this.mainmenu_button_1player != null &&
                            this.mainmenu_button_1player.isClicked())
                        {
                            this.painting_game.setTwoPlayer(false);
                            this.painting_game.setTwoPlayer(false);
                            this.current_game_state = GameState.HAND_SELECT;
                        }

                        if (this.mainmenu_button_2player != null &&
                            this.mainmenu_button_2player.isClicked())
                        {
                            this.painting_game.setTwoPlayer(true);
                            this.painting_game.setTwoPlayer(true);
                            this.current_game_state = GameState.HAND_SELECT;
                        }
                        
                        break;
                    }
                case GameState.HAND_SELECT:
                    {
                        // Handle preferred hand-selection button presses:
                        if (this.mainmenu_lefthand_1 != null)
                        {
                            this.mainmenu_lefthand_1.Update(this.player_1_cursor, p_game_time);
                        }

                        if (this.mainmenu_lefthand_2 != null &&
                            this.using_kinect_input)
                        {
                            this.mainmenu_lefthand_2.Update(this.player_2_cursor, p_game_time);
                        }

                        if (this.mainmenu_righthand_1 != null)
                        {
                            this.mainmenu_righthand_1.Update(this.player_1_cursor, p_game_time);
                        }

                        if (this.mainmenu_righthand_2 != null &&
                            this.using_kinect_input)
                        {
                            this.mainmenu_righthand_2.Update(this.player_2_cursor, p_game_time);
                        }

                        // Swap the player's preferred hands (if applicable):
                        if (this.mainmenu_lefthand_1 != null &&
                            this.mainmenu_lefthand_1.isClicked() &&
                            this.player_1_cursor.getHandJoint() == JointType.HandRight)
                        {
                            this.player_1_cursor.swapHands();
                        }

                        if (this.mainmenu_lefthand_2 != null &&
                            this.mainmenu_lefthand_2.isClicked() &&
                            this.player_2_cursor.getHandJoint() == JointType.HandRight &&
                            this.using_kinect_input)
                        {
                            this.player_2_cursor.swapHands();
                        }

                        if (this.mainmenu_righthand_1 != null &&
                            this.mainmenu_righthand_1.isClicked() &&
                            this.player_1_cursor.getHandJoint() == JointType.HandLeft)
                        {
                            this.player_1_cursor.swapHands();
                        }

                        if (this.mainmenu_righthand_2 != null &&
                            this.mainmenu_righthand_2.isClicked() &&
                            this.player_2_cursor.getHandJoint() == JointType.HandLeft &&
                            this.using_kinect_input)
                        {
                            this.player_2_cursor.swapHands();
                        }

                        // Handle start game button press (by player 1):
                        if (this.mainmenu_startgame != null)
                        {
                            this.mainmenu_startgame.Update(this.player_1_cursor, p_game_time);
                        }

                        if (this.mainmenu_startgame != null &&
                            this.mainmenu_startgame.isClicked())
                        {
                            this.painting_game.load(this.Content,
                                                    this.GraphicsDevice);
                            this.current_game_state = GameState.PUZZLE_PAINT;
                        }

                        break;
                    }
                case GameState.PUZZLE_PAINT:
                    {
                        // Handle painting game:
                        if(this.painting_game != null)
                        {
                            this.painting_game.update(p_game_time,
                                                      this.player_1_cursor,
                                                      this.player_2_cursor);

                            if (this.painting_game.isFinished())
                            {
                                this.puzzle_game.setupImage(this.painting_game.getPaintedImage(),
                                                            this.painting_game.getOutlineImage());
                                this.puzzle_game.load(this.Content);
                                this.current_game_state = GameState.PUZZLE_MAKE;
                            }
                        }

                        break;
                    }
                case GameState.PUZZLE_MAKE:
                    {
                        // Handle puzzle game:
                        if (this.puzzle_game != null)
                        {
                            this.puzzle_game.update(p_game_time);

                            if(this.puzzle_game.isFinished())
                            {
                                this.current_game_state = GameState.STARTUP;
                            }
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
            bool draw_second_cursor = this.using_kinect_input;

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
                case GameState.PLAYER_MENU:
                    {
                        // Draw the simple main menu:
                        if(this.mainmenu_button_1player != null)
                        {
                            this.mainmenu_button_1player.draw(this.sprite_batch);
                        }

                        if (this.mainmenu_button_2player != null)
                        {
                            this.mainmenu_button_2player.draw(this.sprite_batch);
                        }

                        break;
                    }
                case GameState.HAND_SELECT:
                    {
                        // Draw the hand selection buttons:
                        if (this.mainmenu_lefthand_1 != null)
                        {
                            this.mainmenu_lefthand_1.draw(this.sprite_batch);
                        }

                        if (this.mainmenu_lefthand_2 != null &&
                            draw_second_cursor)
                        {
                            this.mainmenu_lefthand_2.draw(this.sprite_batch);
                        }

                        if (this.mainmenu_righthand_1 != null)
                        {
                            this.mainmenu_righthand_1.draw(this.sprite_batch);
                        }

                        if (this.mainmenu_righthand_2 != null &&
                            draw_second_cursor)
                        {
                            this.mainmenu_righthand_2.draw(this.sprite_batch);
                        }

                        if (this.mainmenu_startgame != null)
                        {
                            this.mainmenu_startgame.draw(this.sprite_batch);
                        }

                        break;
                    }
                case GameState.PUZZLE_PAINT:
                    {
                        // Draw the puzzle-painting game:
                        if (this.painting_game != null)
                        {
                            this.painting_game.draw(p_game_time, this.sprite_batch);
                            draw_second_cursor = painting_game.isTwoPlayer();
                        }
                        break;
                    }
                case GameState.PUZZLE_MAKE:
                    {
                        // Draw the puzzle-piece assembly game:
                        if (this.puzzle_game != null)
                        {
                            this.puzzle_game.draw(p_game_time, this.sprite_batch);
                            draw_second_cursor = this.puzzle_game.isTwoPlayer();
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

            // Draw player hand cursors:
            if (this.player_1_cursor != null)
            {
                this.player_1_cursor.draw(this.sprite_batch);
            }

            if (this.player_2_cursor != null &&
                draw_second_cursor)
            {
                this.player_2_cursor.draw(this.sprite_batch);
            }

            // Draw debug Kinect status (if required):
            if(this.status_debug_messages &&
               this.kinect_manager != null)
            {
                this.kinect_manager.drawStatusMessage(this.sprite_batch);
                this.kinect_manager.drawSkeletonGesture(this.sprite_batch);
            }

            base.Draw(p_game_time);
        }
    }
}
