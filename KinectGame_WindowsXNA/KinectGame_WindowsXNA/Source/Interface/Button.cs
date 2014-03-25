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
    public class Button
    {
        Texture2D image;
        Rectangle location;
        string text;
        Vector2 textLocation;
        MouseState mouse;
        MouseState oldMouse;
        bool clicked;
        bool hover;
        float target_time;
        Stopwatch timer;


        public Button(Texture2D texture, float startTime)
        {
            image = texture;
            location = new Rectangle(100, 350, image.Width, image.Height);
            target_time = startTime  * 1000;
            hover = false;
            timer = new Stopwatch();
        }

        public void Update(Cursor player_one_cursor, GameTime p_time)
        {
            mouse = Mouse.GetState();
            clicked = false;

            if (location.Contains(new Point(mouse.X, mouse.Y)))
            {
                if (!hover)
                {
                    hover = true;
                    if (!timer.IsRunning) timer.Restart();
                }
            }
            else
            {
                hover = false;
                clicked = false;
                timer.Reset();
            }

            if (hover && timer.IsRunning)
            {
                long current_time = timer.ElapsedMilliseconds;

                if (current_time >= target_time)
                {
                    Console.WriteLine(current_time.ToString());
                    timer.Reset();
                    clicked = true;
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
            bool is_clicked = clicked;
            clicked = false;
            return is_clicked;
        }


        public void Location(int x, int y)
        {
            location.X = x;
            location.Y = y;
        }

        public void Draw(SpriteBatch p_spriteBatch)
        {
            p_spriteBatch.Begin();
            if(clicked)
            { 
                    p_spriteBatch.Draw(image, location, Color.Green);
            }
            else
            {
                    p_spriteBatch.Draw(image, location, Color.White);
            }
            p_spriteBatch.End();
        }

    }
}