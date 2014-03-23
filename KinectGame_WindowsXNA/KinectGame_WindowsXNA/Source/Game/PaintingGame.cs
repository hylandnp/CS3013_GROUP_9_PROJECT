using KinectGame_WindowsXNA.Source.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*CHANGELOG:
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.Game
{
    // Game class to run the picture-painting game...
    public class PaintingGame
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private Texture2D image_paintable = null,
                          image_outlines = null;
        private PaintSection[] image_sections = null;
        private Rectangle image_rect;
        private Color[] paint_colours = null;
        private bool update_texture = false;



        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public PaintingGame()
        {
            // Initialisation:
        }


        /*/////////////////////////////////////////
          * GRAPHICAL RESOURCE LOADING
          *////////////////////////////////////////
        public void load(ContentManager p_content,
                         GraphicsDevice p_gfx_device)
        {
            // DEBUG: Load the test image:
            Texture2D image_base = p_content.Load<Texture2D>("Textures/Game/Picture_1_Sections");
            int base_size = image_base.Width * image_base.Height;

            Color[] image_colours = new Color[base_size];
            for (int i = 0; i < image_colours.Length; i++) image_colours[i] = Color.White;

            // Load image outline & setup blank white paintable image:
            this.image_outlines = p_content.Load<Texture2D>("Textures/Game/Picture_1_Outline");
            this.image_rect = new Rectangle(50, 50, image_base.Width, image_base.Height);

            this.image_paintable = new Texture2D(p_gfx_device, image_base.Width, image_base.Height);
            this.image_paintable.SetData(image_colours);

            // Convert base image to paint sections:
            image_base.GetData(image_colours);
            Color[] unique_colours = this.findUniqueColours(image_colours);
            this.image_sections = new PaintSection[unique_colours.Length]; // create a paint section for each unique colour

            for (int i = 0; i < image_sections.Length; i++)
            {
                Console.WriteLine(unique_colours[i].ToString());

                this.image_sections[i] = new PaintSection(base_size, unique_colours[i]);
                this.image_sections[i].createSectionMask(image_colours);
            }

            this.paint_colours = image_colours;
            for (int i = 0; i < this.paint_colours.Length; i++) this.paint_colours[i] = Color.White;
        }



        /*/////////////////////////////////////////
          * UPDATE FUNCTION
          *////////////////////////////////////////
        public void update(GameTime p_time,
                           Cursor p_player_1,
                           Cursor p_player_2)
        { 
            // Process cursor positions for paint image:
            Vector2 p1 = ((p_player_1 != null) ? p_player_1.get2DPosition() : Vector2.Zero);
            Vector2 p2 = ((p_player_2 != null) ? p_player_2.get2DPosition() : Vector2.Zero);
            bool p1_can_paint = false,
                 p2_can_paint = false;

            // Adjust co-ordinates to paintable image rect:
            if(p1 != Vector2.Zero &&
               this.image_rect.Contains(new Point((int)p1.X,
                                                  (int)p1.Y)))
            {
                p1_can_paint = true;
                p1.X -= this.image_rect.X;
                p1.Y -= this.image_rect.Y;
            }

            if (p2 != Vector2.Zero &&
               this.image_rect.Contains(new Point((int)p2.X,
                                                  (int)p2.Y)))
            {
                p2_can_paint = true;
                p2.X -= this.image_rect.X;
                p2.Y -= this.image_rect.Y;
            }

            // Check paint sections:
            if (p1_can_paint)
            {
                for (int i = 0; i < this.image_sections.Length; i++)
                {
                    if (this.image_sections[i].isOverSection(p1, this.image_rect.Width))
                    {
                        this.image_sections[i].changeSectionColour(ref this.paint_colours,
                                                                   p_player_1.selected_colour);
                        this.update_texture = true;
                    }
                }
            }

            // TODO player 2 & timing stuff

        }



        /*/////////////////////////////////////////
          * RENDER FUNCTION
          *////////////////////////////////////////
        public void draw(GameTime p_time, SpriteBatch p_sprite_batch)
        {
            // Render the paintable image:
            p_sprite_batch.Begin();

            if(this.image_paintable != null &&
               this.image_rect != null)
            {
                if(this.update_texture) this.image_paintable.SetData(this.paint_colours);
                p_sprite_batch.Draw(this.image_paintable, this.image_rect, Color.White);
            }

            if (this.image_outlines != null &&
                this.image_rect != null)
            {
                p_sprite_batch.Draw(this.image_outlines, this.image_rect, Color.White);
            }

            p_sprite_batch.End();

            // Render the colour selection buttons:
            // TODO

            // Render the "complete/finished" buttons:
            // TODO
        }



        /*/////////////////////////////////////////
          * UTILITY FUNCTION(S)
          *////////////////////////////////////////
        private Color[] findUniqueColours(Color[] p_colour_array)
        {
            // Run through the colour array and identify/store all new colours:
            List<Color> new_list = new List<Color>();
            Color ignore = new Color(255, 255, 255);

            for (int i = 0; i < p_colour_array.Length; i++)
            {
                // If not white (ignorable colour in base image) and not found currently in the list then store new colour:
                if(p_colour_array[i] != ignore &&
                   !new_list.Contains(p_colour_array[i]))
                {
                    new_list.Add(p_colour_array[i]);
                }
            }

            return new_list.ToArray();
        }
    }
}
