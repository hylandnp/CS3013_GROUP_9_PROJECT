using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using KinectGame_WindowsXNA.Source.Interface;
using KinectGame_WindowsXNA.Source.KinectUtils;

/*CHANGELOG
 * NEIL - Created the class.
 * GAVAN - Implemented puzzle-piece division.
 * PATRICK - Puzzle-piece movement.
 * PATRICK - Puzzle-piece assembly.
 * RICHARD - Fixed minor bugs.
 * GAVAN - Got puzzle-assembly finished.
 */

namespace KinectGame_WindowsXNA.Source.Game
{
    // Main puzzle game class...
    public class PuzzleGame
    {
        /*/////////////////////////////////////////
          * MEMBER DATA
          *////////////////////////////////////////
        private bool is_two_player = false,
                     is_finished = false,
                     p1_new_piece = false,
                     p2_new_piece = false;
        private List<PuzzlePiece> p1_pieces = null,
                                  p2_pieces = null;
        private Texture2D outline_texture = null,
                          painted_texture = null,
                          marker_texture = null;
        private Rectangle image_rect;
        private int across = 0,
                    down = 0,
                    width = 0,
                    height = 0;
        private Button p1_next_piece = null,
                       p2_next_piece = null,
                       p1_prev_piece = null,
                       p2_prev_piece = null;
        private int p1_current_piece = 0,
                    p2_current_piece = 0;


        /*/////////////////////////////////////////
          * CONSTRUCTOR
          *////////////////////////////////////////
        public PuzzleGame()
        {
            // Initialisation...
            this.across = 4;
            this.down = 2;
            this.width = 128;
            this.height = 256;
        }



        /*/////////////////////////////////////////
          * GRAPHICAL RESOURCE LOADING
          *////////////////////////////////////////
        public void load(ContentManager p_content,
                         GraphicsDevice p_gfx_device)
        {
            // Load the puzzle game:
            this.marker_texture = p_content.Load<Texture2D>("Textures/Game/PuzzleMarkers");
            Rectangle[,] temp_rects = new Rectangle[across, down];

            // Divide image into sections:
            for (int i = 0; i < this.across; i++)
            {
                for (int j = 0; j < this.down; j++)
                {
                    temp_rects[i, j] = new Rectangle(i * this.width,
                                                     j * this.height,
                                                     this.width,
                                                     this.height);
                }
            }

            // Create puzzle-piece lists:
            if (this.is_two_player)
            {
                // Setup both lists of puzzle pieces:
                this.p1_pieces = new List<PuzzlePiece>();
                this.p2_pieces = new List<PuzzlePiece>();

                for (int i = 0; i < this.across; i++)
                {
                    for (int j = 0; j < this.down; j++)
                    {
                        var temp = temp_rects[i, j];
                        var temp2 = temp;

                        temp2.X = this.image_rect.X + i * this.width;
                        temp2.Y = this.image_rect.Y + j * this.height;

                        if(temp.X < 256)
                        {
                            // Give piece to player 1:
                            this.p1_pieces.Add(new PuzzlePiece(temp, temp2));
                        }
                        else
                        {
                            // Give piece to player 2:
                            this.p2_pieces.Add(new PuzzlePiece(temp, temp2));
                        }
                    }
                }
            }
            else
            {
                // Setup player 1's list of puzzle pieces:
                this.p1_pieces = new List<PuzzlePiece>();
                this.p2_pieces = null;

                for (int i = 0; i < this.across; i++)
                {
                    for (int j = 0; j < this.down; j++)
                    {
                        var temp = temp_rects[i, j];
                        var temp2 = temp;

                        temp2.X = this.image_rect.X + i * this.width;
                        temp2.Y = this.image_rect.Y + j * this.height;

                        // Give piece to player 1:
                        this.p1_pieces.Add(new PuzzlePiece(temp, temp2));
                    }
                }
            }

            this.p1_current_piece = 0;
            this.p2_current_piece = 0;
            this.p1_new_piece = false;
            this.p2_new_piece = false;

            // Create buttons for player 1:
            Vector2 temp_p1_pos = new Vector2(this.image_rect.X - 270.0f,
                                              (float)Math.Ceiling((p_gfx_device.Viewport.Height - 750) / 2.0f) - 10.0f);

            this.p1_next_piece = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonUp"),
                                            1.2f,
                                            temp_p1_pos,
                                            GestureType.NONE);

            temp_p1_pos.Y += 150;
            
            // Position player 1's pieces;
            foreach(var piece in this.p1_pieces)
            {
                piece.destination_rect.X = (int)temp_p1_pos.X + 50;
                piece.destination_rect.Y = (int)temp_p1_pos.Y + 100;
            }

            temp_p1_pos.Y += 150 * 3;

            this.p1_prev_piece = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonDown"),
                                            1.2f,
                                            temp_p1_pos,
                                            GestureType.NONE);

            // Create buttons for player 2:
            Vector2 temp_p2_pos = new Vector2(this.image_rect.X + this.image_rect.Width + 60.0f,
                                              (float)Math.Ceiling((p_gfx_device.Viewport.Height - 750) / 2.0f) - 10.0f);

            this.p2_next_piece = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonUp"),
                                            1.2f,
                                            temp_p2_pos,
                                            GestureType.NONE);
            temp_p2_pos.Y += 150;

            // Positions player 2's pieces:
            if (this.is_two_player &&
                this.p2_pieces != null)
            {
                foreach (var piece in this.p2_pieces)
                {
                    piece.destination_rect.X = (int)temp_p2_pos.X + 40;
                    piece.destination_rect.Y = (int)temp_p2_pos.Y + 100;
                }
            }

            temp_p2_pos.Y += 150 * 3;

            this.p2_prev_piece = new Button(p_content.Load<Texture2D>("Textures/Interface/UI_ButtonDown"),
                                            1.2f,
                                            temp_p2_pos,
                                            GestureType.NONE);
        }


        public void setupImage(Texture2D p_img_base,
                               Texture2D p_img_outline,
                               Rectangle p_rect)
        {
            // Store reference to the painted image:
            this.painted_texture = p_img_base;
            this.outline_texture = p_img_outline;
            this.image_rect = p_rect;
            this.p1_pieces = null;
            this.p2_pieces = null;
        }



        /*/////////////////////////////////////////
          * UPDATE FUNCTION
          *////////////////////////////////////////
        public void update(GameTime p_time,
                           Cursor p_player1_cursor,
                           Cursor p_player2_cursor)
        {
            // Update player 1's puzzle selection buttons:
            if (this.p1_next_piece != null)
            {
                this.p1_next_piece.Update(p_player1_cursor, p_time);

                if (this.p1_next_piece.isClicked())
                {
                    // Get next available piece:
                    for (int i = this.p1_current_piece; i < this.p1_pieces.Count; i++)
                    {
                        if (i != this.p1_current_piece &&
                            !this.p1_pieces[i].isInPlace() &&
                            !this.p1_pieces[i].lockedToCursor())
                        {
                            this.p1_current_piece = i;
                            break;
                        }
                    }
                }
            }

            if (this.p1_prev_piece != null)
            {
                this.p1_prev_piece.Update(p_player1_cursor, p_time);

                if (this.p1_prev_piece.isClicked())
                {
                    // Get next available piece:
                    for (int i = this.p1_current_piece; i >= 0; i--)
                    {
                        if (i != this.p1_current_piece &&
                            !this.p1_pieces[i].isInPlace() &&
                            !this.p1_pieces[i].lockedToCursor())
                        {
                            this.p1_current_piece = i;
                            break;
                        }
                    }
                }
            }

            // Update player 2's puzzle selection buttons:
            if(this.is_two_player)
            {
                if (this.p2_next_piece != null)
                {
                    this.p2_next_piece.Update(p_player2_cursor, p_time);

                    if (this.p2_next_piece.isClicked())
                    {
                        // Get next available piece:
                        for (int i = this.p2_current_piece; i < this.p2_pieces.Count; i++)
                        {
                            if (i != this.p2_current_piece &&
                                !this.p2_pieces[i].isInPlace() &&
                                !this.p2_pieces[i].lockedToCursor())
                            {
                                this.p2_current_piece = i;
                                break;
                            }
                        }
                    }
                }

                if (this.p2_prev_piece != null)
                {
                    this.p2_prev_piece.Update(p_player2_cursor, p_time);

                    if (this.p2_prev_piece.isClicked())
                    {
                        // Get next available piece:
                        for (int i = this.p2_current_piece; i >= 0; i--)
                        {
                            if (i != this.p2_current_piece &&
                                !this.p2_pieces[i].isInPlace() &&
                                !this.p2_pieces[i].lockedToCursor())
                            {
                                this.p2_current_piece = i;
                                break;
                            }
                        }
                    }
                }
            }

            // Check if puzzle is assembled:
            bool temp_finished = true;

            if(this.p1_pieces != null &&
                this.p1_pieces.Count > 0)
            {
                foreach(var piece in this.p1_pieces)
                {
                    if (!piece.isInPlace())
                    {
                        temp_finished = false;
                    }
                }
            }

            if (this.is_two_player &&
                this.p2_pieces != null &&
                this.p2_pieces.Count > 0)
            {
                foreach (var piece in this.p2_pieces)
                {
                    if (!piece.isInPlace())
                    {
                        temp_finished = false;
                    }
                }
            }

            this.is_finished = temp_finished;

            // Update/Set which pieces are visible:
            if (this.p1_pieces != null &&
                this.p1_pieces.Count > 0)
            {
                foreach (var piece in this.p1_pieces)
                {
                    // Update the current piece:
                    if (this.p1_pieces.IndexOf(piece) == this.p1_current_piece) piece.Update(p_player1_cursor);

                    if (piece.isInPlace())
                    {
                        piece.draw_piece = true;
                    }
                    else
                    {
                        piece.draw_piece = false;
                    }
                }
            }

            if (this.is_two_player &&
                this.p2_pieces != null &&
                this.p2_pieces.Count > 0)
            {
                foreach (var piece in this.p2_pieces)
                {
                    // Update the current piece:
                    if (this.p2_pieces.IndexOf(piece) == this.p2_current_piece) piece.Update(p_player2_cursor);

                    if (piece.isInPlace())
                    {
                        piece.draw_piece = true;
                    }
                    else
                    {
                        piece.draw_piece = false;
                    }
                }
            }

            // Check current pieces:
            if (this.p1_pieces != null &&
                this.p1_pieces.Count > 0 &&
                this.p1_current_piece < this.p1_pieces.Count)
            {
                p1_pieces[this.p1_current_piece].draw_piece = true;

                if(p1_pieces[this.p1_current_piece].isInPlace())
                {
                    // Keep at locked/solved position:
                    this.p1_pieces[this.p1_current_piece].destination_rect = this.p1_pieces[this.p1_current_piece].target_rect;

                    if(this.p1_current_piece < (this.p1_pieces.Count - 1))
                    {
                        this.p1_current_piece++;
                    }
                    else if(this.p1_current_piece > 0)
                    {
                        this.p1_current_piece--;
                    }
                }
            }

            if (this.is_two_player &&
                this.p2_pieces != null &&
                this.p2_pieces.Count > 0 &&
                this.p2_current_piece < this.p2_pieces.Count)
            {
                p2_pieces[this.p2_current_piece].draw_piece = true;

                if (p2_pieces[this.p2_current_piece].isInPlace())
                {
                    // Keep at locked/solved position:
                    this.p2_pieces[this.p2_current_piece].destination_rect = this.p2_pieces[this.p2_current_piece].target_rect;

                    if (this.p2_current_piece < (this.p2_pieces.Count - 1))
                    {
                        this.p2_current_piece++;
                    }
                    else if (this.p2_current_piece > 0)
                    {
                        this.p2_current_piece--;
                    }
                }
            }
        }



        /*/////////////////////////////////////////
          * RENDER FUNCTION
          *////////////////////////////////////////
        public void draw(GameTime p_time, SpriteBatch p_sprite_batch)
        {
            // Render the puzzle game & pieces:
            p_sprite_batch.Begin();

            if(this.marker_texture != null)
            {
                p_sprite_batch.Draw(this.marker_texture, this.image_rect, Color.White);
            }

            // Draw all of player 1's pieces:
            if (this.p1_pieces != null &&
                this.p1_pieces.Count > 0)
            {
                foreach (var piece in this.p1_pieces)
                {
                    if (piece.draw_piece)
                    {
                        p_sprite_batch.Draw(this.painted_texture, piece.destination_rect, piece.source_rect, Color.White);
                        p_sprite_batch.Draw(this.outline_texture, piece.destination_rect, piece.source_rect, Color.White);
                    }
                }
            }

            // Draw all of player 2's pieces:
            if (this.is_two_player &&
                this.p2_pieces != null &&
                this.p2_pieces.Count > 0)
            {
                foreach (var piece in this.p2_pieces)
                {
                    if (piece.draw_piece)
                    {
                        p_sprite_batch.Draw(this.painted_texture, piece.destination_rect, piece.source_rect, Color.White);
                        p_sprite_batch.Draw(this.outline_texture, piece.destination_rect, piece.source_rect, Color.White);
                    }
                }
            }

            p_sprite_batch.End();

            if (this.p1_prev_piece != null)
            {
                this.p1_prev_piece.draw(p_sprite_batch);
            }

            if (this.p2_prev_piece != null)
            {
                this.p2_prev_piece.draw(p_sprite_batch);
            }

            if (this.p1_next_piece != null)
            {
                this.p1_next_piece.draw(p_sprite_batch);
            }

            if (this.p2_next_piece != null)
            {
                this.p2_next_piece.draw(p_sprite_batch);
            }
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
