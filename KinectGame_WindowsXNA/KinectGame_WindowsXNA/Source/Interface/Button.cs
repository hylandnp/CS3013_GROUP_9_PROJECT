using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 


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
        SpriteFont font;
        Rectangle location;
        string text;
        Vector2 textLocation;
        MouseState mouse;
        MouseState oldMouse;
        bool clicked;
        bool hover;
        bool started;
        bool finished;
        GameTime time;
        bool timeElapsed;
        float MouseTime;
        Timer timer;


        public Button(Texture2D texture, SpriteFont font, float startTime)
        {
            image = texture;
            this.font = font;
            location = new Rectangle(100, 100, image.Width, image.Height);
            MouseTime = startTime;
        }

        public void Update(Cursor player_one_cursor, GameTime p_time)
        {
            mouse = Mouse.GetState();
            clicked = false;

            // if (mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
            // {
            if (location.Contains(new Point(mouse.X, mouse.Y)))
            {
                hover = true;
                this.time = p_time;
            }
            else
            {
                hover = false;
            }

            if (hover && p_time != null)
            {
                //               float prev_time = (float)this.time.ElapsedGameTime.Seconds;
                float new_time = (float)p_time.ElapsedGameTime.Seconds;

                if (MouseTime > 0)
                {
                    MouseTime -= new_time;
                }
                else
                {
                    finished = true;
                }
            }
            finished = false;
        }

        public bool isClicked()
        {
            bool is_clicked = clicked;
            clicked = false;
            return is_clicked;
        }


        public String Text
        {
            get { return text; }

            set
            {
                text = value;
                Vector2 size = font.MeasureString(text);
                textLocation = new Vector2();
                textLocation.Y = location.Y + ((image.Height / 2) - (size.Y / 2));
                textLocation.X = location.X + ((image.Width / 2) - (size.X / 2));

            }
        }

        public void Location(int x, int y)
        {
            location.X = x;
            location.Y = y;
        }

        public void Draw(SpriteBatch p_spriteBatch)
        {
            p_spriteBatch.Begin();
            if(finished)
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
