using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XnaCards;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // keep track of game state and current winner
        static GameState gameState = GameState.Play;

        // hands and battle piles for the players
        WarHand player1 = new WarHand(400,100);
        WarHand player2 = new WarHand(400,500);
        WarBattlePile wbp1;
        WarBattlePile wbp2;

        // winner messages for players
        WinnerMessage wmessage1;
        WinnerMessage wmessage2;

        // menu buttons
        MenuButton quitbutton;
        MenuButton flipbutton;
        MenuButton collectwinningsbutton;

        // create the player type
        Player WhoisWinner = Player.None;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // make mouse visible and set resolution
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create the deck object and shuffle
            Deck deck = new Deck(Content, 0, 0);
            deck.Shuffle();

            // create the player hands and fully deal the deck into the hands
            for(int i = 0; i < 3;i ++ )/*while (deck.Empty != true)*/
            {
                player1.AddCard(deck.TakeTopCard());
                if (deck.Empty != true)
                {
                    player2.AddCard(deck.TakeTopCard());
                }
            }
            // create the player battle piles
            wbp1 = new WarBattlePile(400, 200);
            wbp2 = new WarBattlePile(400, 400);

            // create the player winner messages
            wmessage1 = new WinnerMessage(Content, 600, 100);
            wmessage2 = new WinnerMessage(Content,600,500);

            // create the menu buttons
            quitbutton = new MenuButton(Content, "quitbutton", 180, 450,GameState.Quit);
            flipbutton = new MenuButton(Content, "flipbutton", 180, 150,GameState.Flip);
            collectwinningsbutton = new MenuButton(Content, "collectwinningsbutton", 180, 300, GameState.CollectWinnings);

            // initialized the menu buttons
            collectwinningsbutton.Visible = false;
            flipbutton.Visible = true;
            quitbutton.Visible = true;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            // setting a MouseState variable
            MouseState mouse = Mouse.GetState();

            // update the menu buttons
            quitbutton.Update(mouse);
            flipbutton.Update(mouse);
            collectwinningsbutton.Update(mouse);

            // update based on game state
            if (gameState == GameState.Quit)
            {
                this.Exit();
            }
            if (gameState == GameState.Flip)
            {
                Card card1 = player1.TakeTopCard();
                card1.FlipOver();
                wbp1.AddCard(card1);

                Card card2 = player2.TakeTopCard();
                card2.FlipOver();
                wbp2.AddCard(card2);

                flipbutton.Visible = false;
                collectwinningsbutton.Visible = true;
                if (card1.WarValue > card2.WarValue)
                {
                    wmessage1.Visible = true;
                    WhoisWinner = Player.Player1;
                }
                else if (card1.WarValue < card2.WarValue)
                {
                    wmessage2.Visible = true;
                    WhoisWinner = Player.Player2;
                }
                else if (card1.WarValue == card2.WarValue)
                {
                    WhoisWinner = Player.None;
                }
                gameState = GameState.Play;

            }
            if (gameState == GameState.CollectWinnings)
            {
                if (WhoisWinner == Player.Player1)
                {
                    player1.AddCards(wbp1);
                    player1.AddCards(wbp2);
                    wmessage1.Visible = false;
                }
                else if (WhoisWinner == Player.Player2)
                {
                    player2.AddCards(wbp1);
                    player2.AddCards(wbp2);
                    wmessage2.Visible = false;
                }
                else if (WhoisWinner == Player.None)
                {
                    player1.AddCards(wbp1);
                    player2.AddCards(wbp2);
                }
                flipbutton.Visible = true;
                collectwinningsbutton.Visible = false;
                gameState = GameState.Play;
            }
            if (player1.Empty == true)
            {
                if (wbp1.Empty == true)
                {
                    wmessage2.Visible = true;
                    collectwinningsbutton.Visible = false;
                    flipbutton.Visible = false;
                    gameState = GameState.GameOver;
                }
            }
            else if (player2.Empty == true)
            {
                if (wbp2.Empty == true)
                {
                    wmessage1.Visible = true;
                    collectwinningsbutton.Visible = false;
                    flipbutton.Visible = false;
                    gameState = GameState.GameOver;
                }
            }
            quitbutton.Visible = true;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);

            spriteBatch.Begin();

            // draw the game objects
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            wbp1.Draw(spriteBatch);
            wbp2.Draw(spriteBatch);

            // draw the winner messages
            wmessage1.Draw(spriteBatch);
            wmessage2.Draw(spriteBatch);

            // draw the menu buttons
            quitbutton.Draw(spriteBatch);
            flipbutton.Draw(spriteBatch);
            collectwinningsbutton.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            gameState = newState;
        }
    }
}
