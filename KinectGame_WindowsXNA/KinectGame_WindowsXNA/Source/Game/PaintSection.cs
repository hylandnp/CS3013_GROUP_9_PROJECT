using KinectGame_WindowsXNA.Source.Interface;
using KinectGame_WindowsXNA.Source.KinectUtils;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


/*CHANGELOG
 * NEIL - Created the empty class.
 * PATRICK - Added colour-changing functionality.
 * NEIL - Setup bit-array masks.
 * GAVAN - Incorporated button timing code.
 */

namespace KinectGame_WindowsXNA.Source.Game
{
    // The indiviudal paintable sections that the picture is subdivided into.
    public class PaintSection
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private Color unique_colour = Color.White;
        private BitArray valid_section = null; // used to check if the given array position is valid
        private float target_time;
        private Stopwatch timer;
        private GestureType target_gesture;
        private bool clicked = false,
                     hover = false;


        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public PaintSection(int p_array_size,
                            Color p_colour,
                            float p_target_time,
                            GestureType p_gesture)
        {
            // Initialisation...
            this.unique_colour = p_colour;

            this.valid_section = new BitArray(p_array_size);
            this.valid_section.SetAll(false);

            this.target_time = p_target_time * 1000;
            this.target_gesture = p_gesture;
            this.clicked = false;
            this.hover = false;

            this.timer = new Stopwatch();
        }



        /*/////////////////////////////////////////
          * UTILITY FUNCTION(S)
          *////////////////////////////////////////
        public void update(Cursor p_player_cursor,
                           Vector2 p_modified_pos,
                           int p_rect_width)
        {
            // Update the paint section:
            this.clicked = false;

            if (this.isOverSection(p_modified_pos, p_rect_width))
            {
                // If the player's cursor is over the paint section's bounds:
                if (!this.hover)
                {
                    this.hover = true;
                    if (!this.timer.IsRunning) this.timer.Restart();
                }
            }
            else
            {
                this.hover = false;
                this.clicked = false;
                this.timer.Reset();
            }


            // Check if target hover time has elapsed:
            if (this.hover && this.timer.IsRunning)
            {
                long current_time = this.timer.ElapsedMilliseconds;

                if (current_time >= this.target_time)
                {
                    this.timer.Reset();

                    if (this.target_gesture == GestureType.NONE || p_player_cursor.gesture == this.target_gesture)
                    {
                        this.clicked = true;
                    }
                }
            }
        }


        public bool isClicked()
        {
            // Check for clicked callback, resets the clicked callback boolean...
            bool is_clicked = this.clicked;
            this.clicked = false;
            return is_clicked;
        }



        /*/////////////////////////////////////////
          * UTILITY FUNCTION(S)
          *////////////////////////////////////////
        public Color getUniqueColour()
        {
            return this.unique_colour;
        }


        public void createSectionMask(Color[] p_colour_array)
        {
            // Use the bit-array to store valid 1D positions that the section uses (from the paintable image):
            for (int i = 0; i < p_colour_array.Length; i++)
            {
                if(p_colour_array[i] == this.unique_colour)
                {
                    this.valid_section.Set(i, true);
                }
            }
        }


        public bool isOverSection(Vector2 p_coords,
                                  int p_rect_width)
        {
            // Check if the given co-ordinates (relative to the paintable image) are valid for the paint section:
            int temp_coords = this.convertCoords(p_coords, p_rect_width);

            if (this.valid_section != null &&
                temp_coords >= 0 &&
                temp_coords < this.valid_section.Length &&
                this.valid_section.Get(temp_coords))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void changeSectionColour(ref Color[] p_colour_array,
                                        Color p_paint_colour)
        {
            // Change the colour of the valid section to the paint colour:
            if (p_colour_array != null &&
                this.valid_section != null &&
                p_colour_array.Length == this.valid_section.Length)
            {
                for (int i = 0; i < p_colour_array.Length; i++)
                {
                    if(this.valid_section.Get(i))
                    {
                        p_colour_array[i] = p_paint_colour;
                    }
                }
            }
        }


        private int convertCoords(Vector2 p_coords,
                                  int p_rect_width)
        {
            // Convert Vector2 co-ordinates to 1D array positions:
            return (int)Math.Ceiling(p_coords.X + p_coords.Y * p_rect_width);
        }
    }
}
