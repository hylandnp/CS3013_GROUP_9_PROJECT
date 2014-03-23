using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        GameTime time;

        public Button(Texture2D texture, SpriteFont font)
        {
            image = texture;
            this.font = font;
            location = new Rectangle(100, 100, image.Width, image.Height);
            time = null;
        }

        public void Update(Cursor player_one_cursor, GameTime time)
        {
            mouse = Mouse.GetState();
            clicked = false;

           // if (mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
           // {
            if (location.Contains(new Point(mouse.X, mouse.Y)))
            {
                hover = true;
                this.time = time;
            }
            else
            {
                hover = false;
            }

            if(hover && time != null)
            {
                int prev_time = this.time.ElapsedGameTime.Seconds;
                int new_time = time.ElapsedGameTime.Seconds;

                if(new_time - prev_time > 1)
                {
                    clicked = true;
                }
            }
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
            if(hover)
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
