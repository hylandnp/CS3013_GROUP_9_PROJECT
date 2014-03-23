using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*CHANGELOG
 * NEIL - Created the empty class.
 * PATRICK - Added colour-changing functionality.
 * NEIL - Setup bit-array masks.
 */

// TODO - button-timing stuff

namespace KinectGame_WindowsXNA.Source.Game
{
    // The indiviudal paintable sections that the picture is subdivided into.
    public class PaintSection
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private Color unique_colour;
        private BitArray valid_section = null; // used to check if the given array position



        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public PaintSection(int p_array_size,
                            Color p_colour)
        {
            // Initialisation...
            this.unique_colour = p_colour;
            this.valid_section = new BitArray(p_array_size);
            this.valid_section.SetAll(false);
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
            return (int)(p_coords.X + p_coords.Y * p_rect_width);
        }
    }
}
