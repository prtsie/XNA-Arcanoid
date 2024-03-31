using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using static Arcanoid.Measures.Converter;
using Arcanoid.Measures;
using Point = Microsoft.Xna.Framework.Point;

namespace Arcanoid
{
    public class ArcanoidGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch = null!;
        private const int Rows = 3;
        private const int Cols = 3;
        private const float PlatformSpeed = 0.005f;
        private const float MinPlatformSize = 0.05f;
        private const float MinBallAngle = (float)(Math.PI / 4);
        private const float MaxBallAngle = (float)(MinBallAngle + Math.PI / 2);
        private const float RotateAngle = (float)(Math.PI / 8);
        private const float MaxRandomAngle = RotateAngle / 10;
        private float blocksSpacing;
        private (float Width, float Height) blockSize;
        private Size blockRectSize;
        private readonly RelativeObject platform = new(0.5f - 0.15f / 2, 0.88f, 0.15f, 0.03f);
        private Texture2D blockSprite = null!;
        private Texture2D ballSprite = null!;
        private Ball ball = null!;
        private Size windowSize;
        private readonly Color leftSideOfPlatformColor = Color.Blue;
        private readonly Color rightSideOfPlatformColor = Color.Green;
        private readonly Color platformColor = Color.Bisque;
        private readonly Color[] blockColors =
        [
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Brown,
            Color.Purple,
            Color.Bisque,
            Color.Orange
        ];
        private readonly Block[,] blocks = new Block[Rows, Cols];
        private readonly Random random = new();
        private bool isGameOver;
        private Rectangle leftSide;
        private Rectangle rightSide;
        private Rectangle platformRect;
        private int sideWidth;
        private SpriteFont font = null!;

        public ArcanoidGame() //This is the constructor, this function is called whenever the game class is created.
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            ball = new Ball(0.023f, platform.RelativeHorizontalPos, platform.RelativeVerticalPos - 0.1f);
            windowSize = new Size(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col] = new Block(0, 0, 0, 0) { Color = blockColors[random.Next(blockColors.Length)] };
                }
            }
            RelocateBlocks();
            base.Initialize();
        }

        /// <summary>
        /// Automatically called when your game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blockSprite = TextureLoader.Load("block", Content);
            ballSprite = TextureLoader.Load("ball", Content);
            font = Content.Load<SpriteFont>("algerianFont");
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            ReducePlatformSize();
            CalculateCollides();
            if (Input.IsKeyDown(Keys.Left))
            {
                MovePlatform(Keys.Left);
            }
            if (Input.IsKeyDown(Keys.Right))
            {
                MovePlatform(Keys.Right);
            }
            ball.Move();
            sideWidth = PercentToPixels(MinPlatformSize, windowSize.Width) / 2;
            platformRect = platform.GetRectangle(windowSize);
            leftSide = new Rectangle(platformRect.Left, platformRect.Top, sideWidth, platformRect.Height);
            rightSide = new Rectangle(platformRect.Right - sideWidth, platformRect.Top, sideWidth, platformRect.Height);

            //Update the things FNA handles for us underneath the hood:
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            //This will clear what's on the screen each frame, if we don't clear the screen will look like a mess:
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            foreach (var block in blocks)
            {
                if (!block.IsBroken)
                {
                    spriteBatch.Draw(blockSprite, block.GetRectangle(windowSize), block.Color);
                }
            }

            spriteBatch.Draw(blockSprite, platformRect, platformColor);
            spriteBatch.Draw(blockSprite, leftSide, leftSideOfPlatformColor);
            spriteBatch.Draw(blockSprite, rightSide, rightSideOfPlatformColor);
            spriteBatch.Draw(ballSprite, ball.GetRectangle(windowSize), Color.Bisque);
            if (isGameOver)
            {
                var onGameOverMessage = "Game over";
                var stringSize = font.MeasureString(onGameOverMessage);
                var stringPos = new Vector2(windowSize.Width / 2, windowSize.Height / 2) - stringSize / 2f;
                spriteBatch.DrawString(font, onGameOverMessage, stringPos, Color.Black);
                //var location = new Point((DisplayRectangle.Width - stringSize.Width) / 2, (DisplayRectangle.Height - stringSize.Height) / 2);
                //buffer.Graphics.DrawString(onGameOverMessage, DefaultFont, fillBrush, location);
            }
            spriteBatch.End();
            //Draw the things FNA handles for us underneath the hood:
            base.Draw(gameTime);
        }

        private void RelocateBlocks()
        {
            CalculateElementsSize();
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    blocks[row, col].RelativeHorizontalPos = blockSize.Width * col + blocksSpacing * (col + 1);
                    blocks[row, col].RelativeVerticalPos = blockSize.Height * row + blocksSpacing * (row + 1);
                    blocks[row, col].RelativeWidth = blockSize.Width;
                    blocks[row, col].RelativeHeight = blockSize.Height;
                }
            }
        }

        private void CalculateElementsSize()
        {
            blocksSpacing = 0.015f;
            var blockWidth = (1.0f - blocksSpacing * (Cols + 1)) / Cols;
            blockSize = (blockWidth, 0.02f);
            blockRectSize = new(PercentToPixels(blockWidth, windowSize.Width), PercentToPixels(0.02, windowSize.Height));
        }

        private void CalculateCollides()
        {
            var ballRect = ball.GetRectangle(windowSize);
            isGameOver = ballRect.Bottom > windowSize.Height;
            if (isGameOver)
            {
                return;
            }
            var platformRect = platform.GetRectangle(windowSize);
            var horizontalColliding = ballRect.Right > windowSize.Width || ballRect.Left < 0;
            var verticalColliding = ballRect.Top < 0;
            var platformIntersect = Rectangle.Intersect(ballRect, platformRect);
            if (platformIntersect != Rectangle.Empty)
            {
                ball.RelativeVerticalPos = platform.RelativeVerticalPos - ball.RelativeHeight;
                if (platformIntersect.Width < platformIntersect.Height)
                {
                    horizontalColliding = true;
                }
                else
                {
                    verticalColliding = true;
                }
                var centerOfBall = CenterOfRect(ballRect);
                var minPlatformSize = PercentToPixels(MinPlatformSize, windowSize.Width);
                if (centerOfBall.X < platformRect.Left + minPlatformSize / 2)
                {
                    var maxAngle = MaxBallAngle - ball.Speed.Angle();
                    var rotateAngle = RotateAngle + random.NextSingle() * MaxRandomAngle;
                    ball.Speed = ball.Speed.Rotate(rotateAngle > maxAngle ? maxAngle : rotateAngle);
                }
                else if (centerOfBall.X > platformRect.Right - minPlatformSize / 2)
                {
                    var maxAngle = ball.Speed.Angle() - MinBallAngle;
                    var rotateAngle = RotateAngle + random.NextSingle() * MaxRandomAngle;
                    ball.Speed = ball.Speed.Rotate(rotateAngle > maxAngle ? -maxAngle : -rotateAngle);
                }
            }
            else
            {
                foreach (var block in blocks)
                {
                    if (block.IsBroken)
                    {
                        continue;
                    }
                    var blockRect = block.GetRectangle(windowSize);
                    var ballCenter = CenterOfRect(ballRect);
                    var closestPoint = new Point(Math.Max(blockRect.Left, Math.Min(ballCenter.X, blockRect.Right)),
                                                 Math.Max(blockRect.Top, Math.Min(ballCenter.Y, blockRect.Bottom)));
                    if (Math.Pow(closestPoint.X - ballCenter.X, 2) + Math.Pow(closestPoint.Y - ballCenter.Y, 2) > Math.Pow(ballRect.Height / 2, 2))
                    {
                        continue;
                    }
                    block.IsBroken = true;
                    horizontalColliding = (closestPoint.X == blockRect.Left && ball.Speed.X > 0)
                                       || (closestPoint.X == blockRect.Right && ball.Speed.X < 0);
                    verticalColliding = (closestPoint.Y == blockRect.Top && ball.Speed.Y > 0)
                                       || (closestPoint.Y == blockRect.Bottom && ball.Speed.Y < 0);
                }
            }
            if (horizontalColliding)
            {
                ball.Speed = ball.Speed with { X = -ball.Speed.X };
            }
            if (verticalColliding)
            {
                ball.Speed = ball.Speed with { Y = -ball.Speed.Y };
            }
            foreach (var block in blocks)
            {
                if (!block.IsBroken)
                {
                    return;
                }
            }
            isGameOver = true;
        }

        private static Point CenterOfRect(Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        private void MovePlatform(Keys key)
        {
            if (key == Keys.Left && platform.GetPosition(windowSize).X - PlatformSpeed > 0)
            {
                platform.RelativeHorizontalPos -= PlatformSpeed;
            }
            else if (key == Keys.Right && platform.GetRectangle(windowSize).Right + PlatformSpeed < windowSize.Width)
            {
                platform.RelativeHorizontalPos += PlatformSpeed;
            }
        }

        private void ReducePlatformSize()
        {
            if (platform.RelativeWidth > MinPlatformSize)
            {
                var sizeReductionSpeed = 0.00001f;
                platform.RelativeWidth -= sizeReductionSpeed;
                platform.RelativeHorizontalPos += sizeReductionSpeed / 2;
            }
        }
    }
}
