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
        Rectangle[,] temp_rects;


        public Color selected_colour { get; set; }
        public string debug_message { get; set; }

        private Texture2D outline_texture,
                          painted_texture;
        private Rectangle image_dest_rect,
                          image_source_rect,
                          colour_rect;


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
            int across = 8;
            int down = 2;

            temp_rects = new Rectangle[across, down];

            int temp_x = 0,
                temp_y = 0;

            for (int i = 0; i < across; i++)
            {
                temp_x = i * 64;

                for (int j = 0; j < down; j++)
                {
                    temp_y = j * 256;

                    temp_rects[i, j] = new Rectangle(temp_x, temp_y, 64, 256);
                }
            }
        }


        public void setupImage(Texture2D p_img_base,
                               Texture2D p_img_outline)
        {
            // Store reference to the painted image:
            this.painted_texture = p_img_base;
            this.outline_texture = p_img_outline;
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

            p_sprite_batch.Begin();

            p_sprite_batch.Draw(this.painted_texture, new Rectangle(10, 100, 64, 256), this.temp_rects[3,0], Color.White);

            p_sprite_batch.End();
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
