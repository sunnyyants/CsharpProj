using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <remarks>
    /// A number tile
    /// </remarks>
    class NumberTile
    {
        #region Fields

        // original length of each side of the tile
        int originalSideLength;

        // whether or not this tile is the correct number
        bool isCorrectNumber;

        // drawing support
        Texture2D texture;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;
        Texture2D blinkingtexture;
        Texture2D currenttexture;

        // blinking support
        const int TOTAL_BLINK_MILLISECONDS = 4000;
        int elapsedBlinkMilliseconds = 0;
        const int FRAME_BLINK_MILLISECONDS = 1000;
        int elapsedFrameMilliseconds = 0;
        
        // set enumerate variables
        bool isVisiable = true;
        bool isShrinking = false;
        bool isBlinking = false;

        // add  clickStarted and buttonReleased fields
        bool clickStarted = false;
        bool buttonReleased = false;

        // declear minisecond variables
        const int TotalMilliseconds = 1000;
        int shrinkMilliseconds = 0;

        // declear soundbank variable
        SoundBank soundbank;




        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="center">the center of the tile</param>
        /// <param name="sideLength">the side length for the tile</param>
        /// <param name="number">the number for the tile</param>
        /// <param name="correctNumber">the correct number</param>
        /// <param name="soundBank">the sound bank for playing cues</param>
        public NumberTile(ContentManager contentManager, Vector2 center, int sideLength,
            int number, int correctNumber, SoundBank soundBank)
        {
            // set original side length field
            this.originalSideLength = sideLength;

            // set sound bank field
            soundbank = soundBank;

            // load content for the tile and create draw rectangle
            LoadContent(contentManager, number);
            drawRectangle = new Rectangle((int)center.X - sideLength / 2,
                 (int)center.Y - sideLength / 2, sideLength, sideLength);

            // set isCorrectNumber flag
            isCorrectNumber = number == correctNumber;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the tile based on game time and mouse state
        /// </summary>
        /// <param name="gameTime">the current GameTime</param>
        /// <param name="mouse">the current mouse state</param>
        /// <return>true if the correct number was guessed, false otherwise</return>
        public bool Update(GameTime gameTime, MouseState mouse)
        {

            // if we get here, return false
            if (isBlinking)
            {
                elapsedBlinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                elapsedFrameMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedBlinkMilliseconds > TOTAL_BLINK_MILLISECONDS)
                {
                    isVisiable = false;
                    return true;
                }
                else
                {
                    if (elapsedFrameMilliseconds > FRAME_BLINK_MILLISECONDS)
                    {
                        if (sourceRectangle.X == 0)
                        {
                            sourceRectangle.X = currenttexture.Width / 2;
                        }
                        else if (sourceRectangle.X == currenttexture.Width / 2)
                        {
                            sourceRectangle.X = 0;
                        }
                        elapsedFrameMilliseconds = 0;
                    }
                }

            }
            else if (isShrinking)
            {
                shrinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                float shrinkRatio = (TotalMilliseconds - shrinkMilliseconds) / (float)(TotalMilliseconds);
                int newTileSideLength = (int)(this.originalSideLength * shrinkRatio);
                if (newTileSideLength > 0)
                {
                    this.drawRectangle.Width = newTileSideLength;
                    this.drawRectangle.Height = newTileSideLength;
                    sourceRectangle.X = texture.Width / 2;
                }
                else
                {
                    isVisiable = false;
                }
            }
            else
            {
                if (drawRectangle.Contains(mouse.X, mouse.Y))
                {
                    sourceRectangle.X = texture.Width / 2;
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        clickStarted = true;
                        //buttonReleased = false;


                    }
                    else if (clickStarted && mouse.LeftButton == ButtonState.Released)
                    {
                        sourceRectangle.X = 0;
                        buttonReleased = true;
                        if (isCorrectNumber)
                        {
                            isBlinking = true;
                            soundbank.PlayCue("correctGuess");
                            currenttexture = blinkingtexture;
                            clickStarted = false;
                            // true;
                        }
                        else
                        {
                            isShrinking = true;
                            soundbank.PlayCue("incorrectGuess");
                            clickStarted = false;
                        }
                    }
                }

                else
                {
                    sourceRectangle.X = 0;
                    buttonReleased = true;
                }
            }

            buttonReleased = false;
            return false;
        }

        /// <summary>
        /// Draws the number tile
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch to use for the drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the tile
            if (isVisiable)
            {
                spriteBatch.Draw(currenttexture, drawRectangle, sourceRectangle, Color.White);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the tile
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="number">the tile number</param>
        private void LoadContent(ContentManager contentManager, int number)
        {
            // convert the number to a string
            string numberString = ConvertIntToString(number);
            string blinkingnumber = "blinking" + ConvertIntToString(number);
            // load content for the tile and set source rectangle
            texture = contentManager.Load<Texture2D>(numberString);
            blinkingtexture = contentManager.Load<Texture2D>(blinkingnumber);
            currenttexture = texture;
            sourceRectangle = new Rectangle(0, 0, texture.Width / 2, texture.Height);

        }

        /// <summary>
        /// Converts an integer to a string for the corresponding number
        /// </summary>
        /// <param name="number">the integer to convert</param>
        /// <returns>the string for the corresponding number</returns>
        private String ConvertIntToString(int number)
        {
            switch (number)
            {
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
                default:
                    throw new Exception("Unsupported number for number tile");
            }

        }

        #endregion
    }
}
