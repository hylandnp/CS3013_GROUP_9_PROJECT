using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*CHANGELOG
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.Game
{
    // Main puzzle game class...
    public class PuzzleGame
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          */
        ///////////////////////////////////////
        private bool is_two_player = false,
                     is_finished = false;



        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public PuzzleGame()
        {
            // TODO
        }



        /*/////////////////////////////////////////
          * GRAPHICAL RESOURCE LOADING
          *////////////////////////////////////////
        public void load(ContentManager p_content)
        {
            // TODO
        }


        public void setupImage(Texture2D p_img_base,
                               Texture2D p_img_outline)
        {
            // TODO
        }



        /*/////////////////////////////////////////
          * UPDATE FUNCTION
          *////////////////////////////////////////
        public void update(GameTime p_time)
        {
            // TODO
        }



        /*/////////////////////////////////////////
          * RENDER FUNCTION
          *////////////////////////////////////////
        public void draw(GameTime p_time, SpriteBatch p_sprite_batch)
        {
            // TODO
        }



        /*/////////////////////////////////////////
          * UTILITY FUNCTION(S)
          *////////////////////////////////////////
        public void setTwoPlayer(bool p_two_player)
        {
            this.is_two_player = p_two_player;
        }


        public bool isTwoPlayer()
        {
            return this.is_two_player;
        }


        public bool isFinished()
        {
            return this.is_finished;
        }
    }
}
