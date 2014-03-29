using KinectGame_WindowsXNA.Source.Interface;
using KinectGame_WindowsXNA.Source.KinectUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*CHANGELOG:
 * NEIL - Created the class.
 * PATRICK - Implemented image colouring.
 * GAVAN - Added simple & colour-selection buttons.
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
        private Color[] paint_colours = null,
                        valid_button_colours = null;
        private bool update_texture = false,
                     is_two_player = false,
                     is_finished = false;
        private string[] valid_pictures = null;
        private Button finish_confirm = null,
                       p1_next_col = null,
                       p2_next_col = null,
                       p1_prev_col = null,
                       p2_prev_col = null;
        private byte[] p1_colours,
                       p2_colours;
        private ColourButton[] p1_cbuttons = null,
                               p2_cbuttons = null;



        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public PaintingGame()
        {
            // Initialisation:
            this.valid_pictures = new string[1];

            // Create paintable colour palette:
            this.valid_button_colours = new Color[8];
            this.valid_button_colours[0] = Color.White;
            this.valid_button_colours[1] = Color.Red;
            this.valid_button_colours[2] = Color.Orange;
            this.valid_button_colours[3] = Color.Yellow;
            this.valid_button_colours[4] = Color.Green;
            this.valid_button_colours[5] = Color.Blue;
            this.valid_button_colours[6] = Color.Indigo;
            this.valid_button_colours[6] = Color.Violet;

            // Player colour selections:
            this.p1_colours = new byte[3];
            this.p2_colours = new byte[3];
            this.p1_colours[0] = 0;
            this.p1_colours[1] = 1;
            this.p1_colours[2] = 2;
            this.p2_colours[0] = 0;
            this.p2_colours[1] = 1;
            this.p2_colours[2] = 2;
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

            // Load image outline, setup blank white paintable image, position with offset:
            this.image_outlines = p_content.Load<Texture2D>("Textures/Game/Picture_1_Outline");
            this.image_rect = new Rectangle((int)Math.Ceiling((p_gfx_device.Viewport.Width - image_base.Width) / 2.0f),
                                            (int)Math.Ceiling(((p_gfx_device.Viewport.Height - image_base.Height) / 2.0f) - 60),
                                            image_base.Width,
                                            image_base.Height);

            this.image_paintable = new Texture2D(p_gfx_device, image_base.Width, image_base.Height);
            this.image_paintable.SetData(image_colours);

            // Convert base image to paint sections:
            image_base.GetData(image_colours);
            Color[] unique_colours = this.findUniqueColours(image_colours);
            this.image_sections = new PaintSection[unique_colours.Length]; // create a paint section for each unique colour

            for (int i = 0; i < image_sections.Length; i++)
            {
                this.image_sections[i] = new PaintSection(base_size,
                                                          unique_colours[i],
                                                          1.2f,
                                                          GestureType.NONE);
                this.image_sections[i].createSectionMask(image_colours);
            }

            this.paint_colours = image_colours;
            for (int i = 0; i < this.paint_colours.Length; i++) this.paint_colours[i] = Color.White;

            // Create finish minigame button (positioned from the bottom of the base image with offset):
            this.finish_confirm = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonConfirm"),
                                             1.2f,
                                             new Vector2((float)Math.Ceiling((p_gfx_device.Viewport.Width - 210) / 2.0f),
                                                         this.image_rect.Y + this.image_rect.Height + 20),
                                             GestureType.NONE);

            // Create colour selection & scroll buttons for player 1:
            Vector2 temp_p1_pos = new Vector2(this.image_rect.X - 270.0f,
                                              (float)Math.Ceiling((p_gfx_device.Viewport.Height - 750) / 2.0f) - 10.0f);

            this.p1_next_col = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonUp"),
                                          1.2f,
                                          temp_p1_pos,
                                          GestureType.NONE);

            this.p1_cbuttons = new ColourButton[3];
            temp_p1_pos.Y += 150;

            for (int i = 0; i < this.p1_cbuttons.Length; i++)
            {
                this.p1_cbuttons[i] = new ColourButton(p_content.Load<Texture2D>("Textures/Interface/UI_ColourButton"),
                                                       1.2f,
                                                       temp_p1_pos,
                                                       GestureType.NONE,
                                                       this.valid_button_colours[this.p1_colours[i]]);

                // Next button position:
                temp_p1_pos.Y += 150;
            }

            this.p1_prev_col = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonDown"),
                                          1.2f,
                                          temp_p1_pos,
                                          GestureType.NONE);

            // Create colour selection & scroll buttons for player 2:
            Vector2 temp_p2_pos = new Vector2(this.image_rect.X + this.image_rect.Width + 60.0f,
                                              (float)Math.Ceiling((p_gfx_device.Viewport.Height - 750) / 2.0f) - 10.0f);

            this.p2_next_col = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonUp"),
                                          1.2f,
                                          temp_p2_pos,
                                          GestureType.NONE);

            this.p2_cbuttons = new ColourButton[3];
            temp_p2_pos.Y += 150;

            for (int i = 0; i < this.p2_cbuttons.Length; i++)
            {
                this.p2_cbuttons[i] = new ColourButton(p_content.Load<Texture2D>("Textures/Interface/UI_ColourButton"),
                                                       1.2f,
                                                       temp_p2_pos,
                                                       GestureType.NONE,
                                                       this.valid_button_colours[this.p1_colours[i]]);

                // Next button position:
                temp_p2_pos.Y += 150;
            }

            this.p2_prev_col = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonDown"),
                                          1.2f,
                                          temp_p2_pos,
                                          GestureType.NONE);
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
            Vector2 p2 = ((this.is_two_player && p_player_2 != null) ? p_player_2.get2DPosition() : Vector2.Zero);
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

            if (this.is_two_player &&
                p2 != Vector2.Zero &&
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
                    this.image_sections[i].update(p_player_1,
                                                  p1,
                                                  this.image_rect.Width);

                    if (this.image_sections[i].isClicked())
                    {
                        this.image_sections[i].changeSectionColour(ref this.paint_colours,
                                                                   p_player_1.selected_colour);
                        this.update_texture = true;
                    }
                }
            }

            if (this.is_two_player &&
                p2_can_paint)
            {
                for (int i = 0; i < this.image_sections.Length; i++)
                {
                    this.image_sections[i].update(p_player_2,
                                                  p2,
                                                  this.image_rect.Width);

                    if (this.image_sections[i].isClicked())
                    {
                        this.image_sections[i].changeSectionColour(ref this.paint_colours,
                                                                   p_player_2.selected_colour);
                        this.update_texture = true;
                    }
                }
            }

            // Handle colour-changing button presses:
            if (this.p1_next_col != null)
            {
                this.p1_next_col.Update(p_player_1, p_time);

                if (this.p1_next_col.isClicked())
                {
                    //byte temp = 0;
                    //for (byte i = 0; i < this.valid_button_colours.Length; i++)
                    //{
                    //    if (this.valid_button_colours[this.p1_colours[0]] == this.valid_button_colours[i])
                    //    {
                    //        temp = i;
                    //        break;
                    //    }
                    //}

                    //this.p1_colours[2] = this.p1_colours[1];
                    //this.p1_colours[1] = this.p1_colours[0];


                    //if (temp - 1 < 0)
                    //{
                    //    temp = (byte)(this.p1_colours.Length - 1);
                    //}
                    //else
                    //{
                    //    temp--;
                    //}

                    //this.p1_colours[0] = temp;
                }
            }

            if (this.is_two_player &&
                this.p2_next_col != null)
            {
                this.p2_next_col.Update(p_player_2, p_time);

                if (this.p2_next_col.isClicked())
                {
                    // TODO
                }
            }

            if (this.p1_prev_col != null)
            {
                this.p1_prev_col.Update(p_player_1, p_time);

                if (this.p1_prev_col.isClicked())
                {
                    // TODO
                }
            }

            if (this.is_two_player &&
                this.p2_prev_col != null)
            {
                this.p2_prev_col.Update(p_player_2, p_time);

                if (this.p2_prev_col.isClicked())
                {
                    // TODO
                }
            }

            // Handle colour button presses:
            for (int i = 0; i < this.p1_cbuttons.Length; i++)
            {
                if (this.p1_cbuttons[i] != null)
                {
                    this.p1_cbuttons[i].button_colour = this.valid_button_colours[this.p1_colours[i]];
                    this.p1_cbuttons[i].Update(p_player_1, p_time);

                    if(this.p1_cbuttons[i].isClicked())
                    {
                        p_player_1.selected_colour = this.p1_cbuttons[i].button_colour;
                    }
                }
            }

            if (this.is_two_player)
            {
                for (int i = 0; i < this.p2_cbuttons.Length; i++)
                {
                    if (this.p2_cbuttons[i] != null)
                    {
                        this.p2_cbuttons[i].Update(p_player_2, p_time);

                        if(this.p2_cbuttons[i].isClicked())
                        {
                            p_player_2.selected_colour = this.p2_cbuttons[i].button_colour;
                        }
                    }
                }
            }

            // Handle finish game press:
            if (this.finish_confirm != null)
            {
                this.finish_confirm.Update(p_player_1, p_time);

                if (this.finish_confirm.isClicked())
                {
                    this.is_finished = true;
                }
            }
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
            for (int i = 0; i < this.p1_cbuttons.Length; i++)
            {
                if (this.p1_cbuttons[i] != null)
                {
                    this.p1_cbuttons[i].draw(p_sprite_batch);
                }
            }

            if (this.is_two_player)
            {
                for (int i = 0; i < this.p2_cbuttons.Length; i++)
                {
                    if (this.p2_cbuttons[i] != null)
                    {
                        this.p2_cbuttons[i].draw(p_sprite_batch);
                    }
                }
            }

            // Render the "complete/finished" buttons:
            if (this.finish_confirm != null)
            {
                this.finish_confirm.draw(p_sprite_batch);
            }

            // Render the selection scroll buttons:
            if (this.p1_next_col != null)
            {
                this.p1_next_col.draw(p_sprite_batch);
            }

            if (this.is_two_player &&
                this.p2_next_col != null)
            {
                this.p2_next_col.draw(p_sprite_batch);
            }

            if (this.p1_prev_col != null)
            {
                this.p1_prev_col.draw(p_sprite_batch);
            }

            if (this.is_two_player &&
                this.p2_prev_col != null)
            {
                this.p2_prev_col.draw(p_sprite_batch);
            }
        }



        /*/////////////////////////////////////////
          * UTILITY FUNCTION(S)
          *////////////////////////////////////////
        private Color[] findUniqueColours(Color[] p_colour_array)
        {
            // Run through the colour array and identify/store all new colours:
            List<Color> new_list = new List<Color>();
            Color ignore = Color.White;

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


        public Texture2D getPaintedImage()
        {
            return this.image_paintable;
        }


        public Texture2D getOutlineImage()
        {
            return this.image_outlines;
        }
    }
}
