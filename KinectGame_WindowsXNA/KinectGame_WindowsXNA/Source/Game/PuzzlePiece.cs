using KinectGame_WindowsXNA.Source.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*CHANGELOG
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.Game
{
    // Individual puzzle game piece (contains smaller paint-sections)...
    public class PuzzlePiece
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        public Rectangle source_rect;
        public Rectangle destination_rect;
        public Rectangle target_rect;

        private bool locked_to_cursor = false,
                     is_in_place = false;

        public bool draw_piece { get; set; }



        /*/////////////////////////////////////////
          * CONSTRUCTOR(S)/DESTRUCTOR(S)
          *////////////////////////////////////////
        public PuzzlePiece(Rectangle p_src_rect,
                           Rectangle p_target_rect)
        {
            this.source_rect = p_src_rect;
            this.target_rect = p_target_rect;
            this.destination_rect = p_src_rect;
            this.is_in_place = false;
            this.locked_to_cursor = false;
            this.draw_piece = false;
        }



        /*/////////////////////////////////////////
          * PUZZLE FUNCTION(S)
          *////////////////////////////////////////
        public void Update(Cursor p_player_cursor)
        {
            // Update the position of the puzzle-piece:
            if (!this.is_in_place)
            {
                if (this.locked_to_cursor)
                {
                    var pos = p_player_cursor.get2DPosition();
                    this.destination_rect.X = (int)pos.X - this.destination_rect.Width / 2;
                    this.destination_rect.Y = (int)pos.Y - this.destination_rect.Height / 2;
                }
            }
        }



        /*/////////////////////////////////////////
          * GETTER FUNCTION(S)
          */ ///////////////////////////////////////
        public bool lockedToCursor()
        {
            return this.locked_to_cursor;
        }


        public bool isInPlace()
        {
            return this.is_in_place;
        }
    }
}
