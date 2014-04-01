using KinectGame_WindowsXNA.Source.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*CHANGELOG
 * NEIL - Created the class.
 */

namespace KinectGame_WindowsXNA.Source.Game
{
    // Individual puzzle game piece (contains smaller paint-sections)...
    public class PuzzlePiece
    {
        // TODO
        private Rectangle source_rect,
                          destination_rect;
        bool locked_to_cursor = false;

        public PuzzlePiece(Rectangle p_src_rect)
        {
            this.source_rect = p_src_rect;
            this.destination_rect = p_src_rect;
        }

        public void Update(Cursor p_player_cursor)
        {
            if(this.locked_to_cursor)
            {
                var pos = p_player_cursor.get2DPosition();
                this.destination_rect.X = (int)pos.X - this.destination_rect.Width / 2;
                this.destination_rect.Y = (int)pos.Y - this.destination_rect.Height / 2;
            }
        }
    }
}
