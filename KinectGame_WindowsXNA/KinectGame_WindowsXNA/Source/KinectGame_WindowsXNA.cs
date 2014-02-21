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


namespace KinectGame_WindowsXNA
{
    public class KinectGame_WindowsXNA : Microsoft.Xna.Framework.Game
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private GraphicsDeviceManager graphics = null;
        private SpriteBatch spriteBatch = null;
        private KinectSensor kinect = null;
        private bool using_kinect_input = false;


        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public KinectGame_WindowsXNA()
        {
            graphics = new GraphicsDeviceManager(this);

            // Configure graphical settings:
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true; // use antialiasing
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            graphics.SynchronizeWithVerticalRetrace = true; // use vsync

            Content.RootDirectory = "Content";
        }


        /*/////////////////////////////////////////
          * NON-GRAPHICAL DATA INITIALISATION
          *////////////////////////////////////////
        protected override void Initialize()
        {
            using_kinect_input = true; // set to false if you want to use the mouse to simulate Kinect input

            base.Initialize();
        }


        /*/////////////////////////////////////////
         * GRAPHICAL DATA & RESOURCE LOADING
         */////////////////////////////////////////
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        /*/////////////////////////////////////////
          * GAME ANIMATION & RENDERING FUNCTION
          *////////////////////////////////////////
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
