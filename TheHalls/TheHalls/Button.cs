using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TheHalls
{
    class Button
    {
        private Rectangle rect;
        private Texture2D img;
        private Texture2D hovered;
        private string msg;
        private SpriteFont font;
        private SoundEffect[] buttonSounds;
        private bool isHovering;

        /// <summary>
        /// Creates an interactable button the player can click on
        /// </summary>
        /// <param name="X">Button X position</param>
        /// <param name="Y">Button Y position</param>
        /// <param name="Width">Button width</param>
        /// <param name="Height">Button height</param>
        /// <param name="image">Button texture iamge</param>
        /// <param name="hovered">image when button is hovered</param>
        /// <param name="message">Button message text</param>
        /// <param name="font">The font of the message</param>
        public Button(int X, int Y, int Width, int Height, Texture2D image, Texture2D hovered, string message, SpriteFont font, SoundEffect[] buttonSounds)
        {
            rect = new Rectangle(X, Y, Width, Height);
            img = image;
            msg = message;
            this.font = font;
            this.hovered = hovered;
            this.buttonSounds = buttonSounds;
            isHovering = false;
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

            if (rect.Contains(mouseP) &&
                mouse.LeftButton == ButtonState.Pressed &&
                prevMouse.LeftButton == ButtonState.Released)
            {
                buttonSounds[1].Play();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws the button to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch from Draw()</param>
        /// <param name="color">The text message color</param>
        public void Draw(SpriteBatch sb, Color color, MouseState mouse)
        {
            if(rect.Contains(mouse.Position))
            {
                if (!isHovering)
                {
                    buttonSounds[0].Play();
                    isHovering = true;
                }
                sb.Draw(hovered, rect, Color.White);
                sb.DrawString(font, msg, new Vector2(
                    rect.X + (rect.Width / 2) - (font.MeasureString(msg).X / 2),
                    rect.Y + (rect.Height / 2) - (font.MeasureString(msg).Y / 2)), color);
            }
            else
            {
                isHovering = false;
                sb.Draw(img, rect, Color.White);
                sb.DrawString(font, msg, new Vector2(
                    rect.X + (rect.Width / 2) - (font.MeasureString(msg).X / 2),
                    rect.Y + (rect.Height / 2) - (font.MeasureString(msg).Y / 2)), color);
            }
        }
    }
}
