using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


/*CHANGELOG
 * NEIL - Created the empty class.
 * GAVAN - Button functionality added.
 */

namespace KinectGame_WindowsXNA.Source.Interface
{
    // Simple button for the Kinect game...
    public class ColourButton
    {
        Texture2D c_image;
        Rectangle c_location;
        string c_text;
        Vector2 c_textLocation;
        MouseState c_mouse;
        MouseState c_oldMouse;
        bool c_clicked;
        bool c_hover;
        float c_target_time;
        Stopwatch c_timer;
        Color color;


        public ColourButton(Texture2D texture, float startTime, int x, int y, Color p_color)
        {
            c_image = texture;
            c_location = new Rectangle(x, y, c_image.Width, c_image.Height);
            c_target_time = startTime * 1000;
            c_hover = false;
            c_timer = new Stopwatch();
            color = p_color;
        }

        public void Update(Cursor player_one_cursor, GameTime p_time)
        {
            c_mouse = Mouse.GetState();
            c_clicked = false;

            if (c_location.Contains(new Point(c_mouse.X, c_mouse.Y)))
            {
                if (!c_hover)
                {
                    c_hover = true;
                    if (!c_timer.IsRunning) c_timer.Restart();
                }
            }
            else
            {
                c_hover = false;
                c_clicked = false;
                c_timer.Reset();
            }

            if (c_hover && c_timer.IsRunning)
            {
                long current_time = c_timer.ElapsedMilliseconds;

                if (current_time >= c_target_time)
                {
                    Console.WriteLine(current_time.ToString());
                    c_timer.Reset();
                    c_clicked = true;
                    player_one_cursor.debug_message = "";
                }
                else
                {
                    player_one_cursor.debug_message = current_time.ToString();
                }
            }
        }

        public bool isClicked()
        {
            bool is_clicked = c_clicked;
            c_clicked = false;
            return is_clicked;
        }


        public void Location(int x, int y)
        {
            c_location.X = x;
            c_location.Y = y;
        }
        
        public Color button_color()
        {
            return color;
        }

        public void Draw(SpriteBatch p_spriteBatch)
        {
            p_spriteBatch.Begin();
            if (c_clicked)
            {
                p_spriteBatch.Draw(c_image, c_location, color);
            }
            else
            {
                p_spriteBatch.Draw(c_image, c_location, color);
            }
            p_spriteBatch.End();
        }

    }
}