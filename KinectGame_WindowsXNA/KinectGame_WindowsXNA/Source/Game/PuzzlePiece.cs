using KinectGame_WindowsXNA.Source.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

/*CHANGELOG
 * NEIL - Created the class.
 * GAVAN - Storing texture rectangles.
 * PATRICK - Timing & movement code.
 * RICHARD - Minor bugfixes.
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
                     is_in_place = false,
                     hover = false;
        private Stopwatch timer = null;
        private float target_time = 0.0f;

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
            this.timer = new Stopwatch();
            this.hover = false;
            this.target_time = 1000.0f;
        }



        /*/////////////////////////////////////////
          * PUZZLE FUNCTION(S)
          *////////////////////////////////////////
        public void Update(Cursor p_player_cursor)
        {
            if (!this.is_in_place)
            {
                // Update the position of the puzzle-piece:
                if (this.locked_to_cursor)
                {
                    var pos = p_player_cursor.get2DPosition();
                    this.destination_rect.X = (int)pos.X - this.destination_rect.Width / 2;
                    this.destination_rect.Y = (int)pos.Y - this.destination_rect.Height / 2;

                    // Check for placement:
                    this.is_in_place = false;
                    var current_pos = p_player_cursor.get2DPosition();

                    if (this.target_rect.Contains(new Point((int)Math.Ceiling(current_pos.X),
                                                            (int)Math.Ceiling(current_pos.Y))))
                    {
                        // If the player's cursor is hovering over the piece:
                        if (!this.hover)
                        {
                            this.hover = true;
                            if (!this.timer.IsRunning) this.timer.Restart();
                        }
                    }
                    else
                    {
                        this.hover = false;
                        this.is_in_place = false;
                        this.timer.Reset();
                    }

                    // Check if timer has elapsed:
                    if (this.hover && this.timer.IsRunning)
                    {
                        long current_time = this.timer.ElapsedMilliseconds;

                        if (current_time >= this.target_time)
                        {
                            this.timer.Reset();
                            this.is_in_place = true;
                        }
                    }
                }
                else
                {
                    // Check for activation:
                    this.locked_to_cursor = false;
                    var current_pos = p_player_cursor.get2DPosition();

                    if (this.destination_rect.Contains(new Point((int)Math.Ceiling(current_pos.X),
                                                                 (int)Math.Ceiling(current_pos.Y))))
                    {
                        // If the player's cursor is hovering over the piece:
                        if (!this.hover)
                        {
                            this.hover = true;
                            if (!this.timer.IsRunning) this.timer.Restart();
                        }
                    }
                    else
                    {
                        this.hover = false;
                        this.locked_to_cursor = false;
                        this.timer.Reset();
                    }

                    // Check if timer has elapsed:
                    if (this.hover && this.timer.IsRunning)
                    {
                        long current_time = this.timer.ElapsedMilliseconds;

                        if(current_time >= this.target_time)
                        {
                            this.timer.Reset();
                            this.locked_to_cursor = true;
                        }
                    }
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
