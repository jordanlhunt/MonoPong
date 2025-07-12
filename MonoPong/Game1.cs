using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoPong;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    Texture2D spritesTexture;


    const float PADDLE_STARTING_X = 300.0f;
    const float PADDLE_STARTING_Y = 300.0f;
    const float BALL_STARTING_X = 400.0f;
    const float BALL_STARTING_Y = 300.0f;
    const float PADDLE_SPEED = 0.5f;

    Vector2[] paddlePositions = new Vector2[2];
    Vector2 ballLocation = new Vector2(BALL_STARTING_X, BALL_STARTING_Y);
    Vector2 ballTrajectory = Vector2.Zero;
    Vector2 paddleLocation = new Vector2(PADDLE_STARTING_X, PADDLE_STARTING_Y);
    bool isPlaying = false;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        spritesTexture = Content.Load<Texture2D>(@"Graphics/sprites");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update both players with correct PlayerIndex cast
        for (int i = 0; i < 2; i++)
        {
            UpdatePlayer(i, gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin();
        // Update both players with correct PlayerIndex cast
        for (int i = 0; i < 2; i++)
        {
            DrawPaddle(i);
        }
        DrawBall();
        spriteBatch.End();

        base.Draw(gameTime);
    }

    #region  Private Methods
    private void UpdatePlayer(int playerIndex, GameTime gameTime)
    {
        GamePadState gamePadState = GamePad.GetState(playerIndex);
        paddlePositions[playerIndex].Y -= gamePadState.ThumbSticks.Left.Y * gameTime.ElapsedGameTime.Milliseconds * .05f;

        if (paddlePositions[playerIndex].Y < 100.0f)
        {
            paddlePositions[playerIndex].Y = 100.0f;
        }
        if (paddlePositions[playerIndex].Y > 500.0f)
        {
            paddlePositions[playerIndex].Y = 500.0f;
        }
        if (!isPlaying)
        {
            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                isPlaying = true;
                Debug.Write("[Game1.cs] - UpdatePlayer() - The " + playerIndex + " pressed the A Button");
                ballTrajectory.X = -.5f;
                ballTrajectory.Y = -.5f;
            }
        }

    }

    private void DrawPaddle(int playerIndex)
    {
        float screenPosition = 736.0f;
        int paddleHeight = 128;
        int paddleWidth = 64;
        Rectangle destinationRectangle = new Rectangle(playerIndex * (int)screenPosition, (int)paddlePositions[playerIndex].Y - paddleWidth, paddleWidth, paddleHeight);
        spriteBatch.Draw(spritesTexture, destinationRectangle, new Rectangle(playerIndex * paddleWidth, 0, paddleWidth, paddleHeight), Color.White);
    }

    private void DrawBall()
    {
        spriteBatch.Draw(spritesTexture, new Rectangle((int)ballLocation.X - 16, (int)ballLocation.Y - 16, 32, 32), new Rectangle(128, 0, 64, 64), Color.White);
    }
    #endregion
}
