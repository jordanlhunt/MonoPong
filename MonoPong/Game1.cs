using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoPong;

public class Game1 : Game
{
    #region Constants
    const float BALL_STARTING_X = 400.0f;
    const float BALL_STARTING_Y = 300.0f;
    const float PADDLE_SPEED = 0.5f;
    const float PADDLE_STARTING_X = 300.0f;
    const float PADDLE_STARTING_Y = 300.0f;
    const int BACKGROUND_TEXTURE_HEIGHT = 600;
    const int BACKGROUND_TEXTURE_WIDTH = 800;
    const int PADDLE_HEIGHT = 128;
    const int PADDLE_WIDTH = 64;
    const int WINDOW_HEIGHT = 600;
    const int WINDOW_WIDTH = 800;
    #endregion
    #region Member Variables
    bool isPlaying;
    float[] forceFeedback = { 0.0f, 0.0f };
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    Texture2D backgroundTexture;
    Texture2D spritesTexture;
    Vector2 ballLocation = new Vector2(BALL_STARTING_X, BALL_STARTING_Y);
    Vector2 ballTrajectory = Vector2.Zero;
    Vector2 paddleLocation = Vector2.Zero;
    Vector2[] paddlePositions = new Vector2[2];
    SoundEffect zapSoundEffect;

    #endregion
    #region Public Methods
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
        graphics.ApplyChanges();
        paddlePositions[0] = new Vector2(0, WINDOW_HEIGHT / 2f);
        paddlePositions[1] = new Vector2(0, WINDOW_HEIGHT / 2f);
        isPlaying = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        spritesTexture = Content.Load<Texture2D>(@"Graphics/sprites");
        backgroundTexture = Content.Load<Texture2D>(@"Graphics/background");
        zapSoundEffect = Content.Load<SoundEffect>(@"SoundEffects/zap");
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
        {
            Exit();
        }
        // Update both players with correct PlayerIndex cast
        for (int i = 0; i < 2; i++)
        {
            UpdatePlayer(i, gameTime);
        }
        UpdateBall(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin();
        DrawBackground();
        // Update both players with correct PlayerIndex cast
        for (int i = 0; i < 2; i++)
        {
            DrawPaddle(i);
        }
        DrawBall();
        spriteBatch.End();
        base.Draw(gameTime);
    }
    #endregion
    #region Private Methods
    private void UpdatePlayer(int playerIndex, GameTime gameTime)
    {
        GamePadState gamePadState = GamePad.GetState(playerIndex);
        paddlePositions[playerIndex].Y -=
            gamePadState.ThumbSticks.Left.Y * gameTime.ElapsedGameTime.Milliseconds * .5f;
        float minimumY = PADDLE_HEIGHT;
        float maxY = WINDOW_HEIGHT - PADDLE_HEIGHT;
        paddlePositions[playerIndex].Y = MathHelper.Clamp(
            paddlePositions[playerIndex].Y,
            minimumY,
            maxY
        );
        if (!isPlaying)
        {
            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                isPlaying = true;
                ballTrajectory.X = -.5f;
                ballTrajectory.Y = -.5f;
            }
        }
        if (forceFeedback[playerIndex] > 0.0f)
        {
            forceFeedback[playerIndex] -= gameTime.ElapsedGameTime.Milliseconds;
        }
        float forceFeedbackTime = forceFeedback[playerIndex] / 50.0f;
        if (forceFeedbackTime > 1.0f)
        {
            forceFeedbackTime = 1.0f;
        }
        if (forceFeedbackTime < 0.0f)
        {
            forceFeedbackTime = 0.0f;
        }
        GamePad.SetVibration(playerIndex, forceFeedbackTime, forceFeedbackTime);
    }

    private void UpdateBall(GameTime gameTime)
    {
        float previousXLocation = ballLocation.X;
        const float RIGHT_BOUND = 800.0f;
        const float UPPER_BOUND = 50.0f;
        const float LOWER_BOUND = 550.0f;
        const float LEFT_REVERSE = 64.0f;
        const float RIGHT_REVERSE = 736.0f;
        ballLocation += ballTrajectory * gameTime.ElapsedGameTime.Milliseconds;
        if (ballLocation.X > WINDOW_WIDTH)
        {
            isPlaying = false;
        }
        if (ballLocation.X < RIGHT_BOUND)
        {
            isPlaying = false;
        }
        if (ballLocation.Y < UPPER_BOUND)
        {
            ballLocation.Y = UPPER_BOUND;
            ballTrajectory.Y *= -1;
        }
        if (ballLocation.Y > LOWER_BOUND)
        {
            ballLocation.Y = LOWER_BOUND;
            ballTrajectory.Y *= -1;
        }
        if (ballLocation.X < LEFT_REVERSE)
        {
            BallCollision(0, previousXLocation >= LEFT_REVERSE);
        }
        if (ballLocation.X > RIGHT_REVERSE)
        {
            BallCollision(1, previousXLocation <= RIGHT_REVERSE);
        }
    }

    private void DrawPaddle(int playerIndex)
    {
        int screenPosition = 736;
        Rectangle destinationRectangle = new Rectangle(
            playerIndex * screenPosition,
            (int)paddlePositions[playerIndex].Y - (PADDLE_HEIGHT / 2),
            PADDLE_WIDTH,
            PADDLE_HEIGHT
        );
        spriteBatch.Draw(
            spritesTexture,
            destinationRectangle,
            new Rectangle(playerIndex * PADDLE_WIDTH, 0, PADDLE_WIDTH, PADDLE_HEIGHT),
            Color.White
        );
    }

    private void DrawBall()
    {
        spriteBatch.Draw(
            spritesTexture,
            new Rectangle((int)ballLocation.X - 16, (int)ballLocation.Y - 16, 32, 32),
            new Rectangle(PADDLE_HEIGHT, 0, PADDLE_WIDTH, PADDLE_WIDTH),
            Color.White
        );
    }

    private void DrawBackground()
    {
        spriteBatch.Draw(
            backgroundTexture,
            new Rectangle(0, 0, BACKGROUND_TEXTURE_WIDTH, BACKGROUND_TEXTURE_HEIGHT),
            Color.White
        );
    }

    private void BallCollision(int playerIndex, bool shouldReverse)
    {
        if (
            ballLocation.Y < paddlePositions[playerIndex].Y + PADDLE_WIDTH
            && ballLocation.Y > paddlePositions[playerIndex].Y - PADDLE_WIDTH
        )
        {
            if (shouldReverse)
            {
                ballTrajectory.X *= -1;
            }
            ballTrajectory.Y = (ballTrajectory.Y - paddlePositions[playerIndex].Y) * .001f;
            forceFeedback[playerIndex] = 100.0f;
            zapSoundEffect.Play();
        }
    }
    #endregion
}
