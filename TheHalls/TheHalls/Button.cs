using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheHalls
{
    class Button
    {
        private Rectangle rect;
        private Texture2D img;
        private string msg;
        private SpriteFont font;

        /// <summary>
        /// Creates an interactable button the player can click on
        /// </summary>
        /// <param name="X">Button X position</param>
        /// <param name="Y">Button Y position</param>
        /// <param name="Width">Button width</param>
        /// <param name="Height">Button height</param>
        /// <param name="image">Button texture iamge</param>
        /// <param name="message">Button message text</param>
        /// <param name="font">The font of the message</param>
        public Button(int X, int Y, int Width, int Height, Texture2D image, string message, SpriteFont font)
        {
            rect = new Rectangle(X, Y, Width, Height);
            img = image;
            msg = message;
            this.font = font;
        }

        /// <summary>
        /// Checks if the button was clicked by the user
        /// </summary>
        /// <param name="mouse">The Mouse State to get the cursor position and button state</param>
        /// /// <param name="prevMouse">The Mouse state in the previous frame to test single click</param>
        /// <returns>If the button was successfully clicked</returns>
        public bool Clicked(MouseState mouse, MouseState prevMouse)
        {
            Point mouseP = new Point(mouse.X, mouse.Y);

            return rect.Contains(mouseP) && 
                mouse.LeftButton == ButtonState.Pressed &&
                prevMouse.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Draws the button to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch from Draw()</param>
        /// <param name="color">The text message color</param>
        public void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(img, rect, Color.White);
            sb.DrawString(font, msg, new Vector2(rect.X + 5, rect.Y + 5), color);
        }
    }
}
