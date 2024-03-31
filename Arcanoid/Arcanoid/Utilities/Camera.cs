using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//Thanks to David Amador and o KB o. Static since we will only have one camera.

namespace Arcanoid
{
    static internal class Camera
    {
        private static Matrix transformMatrix; //A transformation matrix containing info on our position, how much we are rotated and zoomed etc.
        private static Vector2 position;
        public static float Rotation;
        private static float zoom;
        private static Rectangle screenRect;
        public static bool UpdateYAxis; //Should the camera move along on the y axis?
        public static bool UpdateXAxis = true; //Should the camera move along on the x axis?

        public static void Initialize()
        {
            zoom = 1.0f;
            Rotation = 0.0f;

            //Start the camera at the center of the screen:
            position = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);
        }

        /// <summary>
        /// This rectangle covers the entire screen based on where the camera is, useful if you need to determine what is currently viewable to the player, or the position of the camera.
        /// </summary>
        public static Rectangle ScreenRect => screenRect;

        public static float Zoom
        {
            get => zoom;
            set
            {
                zoom = value;
                if (zoom < 0.1f)
                {
                    zoom = 0.1f; //Negative zoom will flip image.
                }
            }
        }

        public static void Update(Vector2 follow)
        {
            UpdateMovement(follow);
            CalculateMatrixAndRectangle();
        }

        private static void UpdateMovement(Vector2 follow)
        {
            //Make the camera center on and follow the position:
            if (UpdateXAxis == true)
            {
                position.X += ((follow.X - position.X)); //Camera will follow the position passed in
            }

            if (UpdateYAxis == true)
            {
                position.Y += ((follow.Y - position.Y)); //Camera will follow the position passed in
            }
        }

        /// <summary>
        /// Immediately sets the camera to look at the position passed in.
        /// </summary>
        public static void LookAt(Vector2 lookAt)
        {
            //Immediately looks at the vector passed in:
            if (UpdateXAxis == true)
            {
                position.X = lookAt.X;
            }

            if (UpdateYAxis == true)
            {
                position.Y = lookAt.Y;
            }
        }

        private static void CalculateMatrixAndRectangle()
        {
            //The math involved with calculated our transformMatrix and screenRect is a little intense, so instead of calling the math whenever we need these variables,
            //we'll calculate them once each frame and store them... when someone needs these variables we will simply return the stored variable instead of re cauclating them every time.

            //Calculate the camera transform matrix:
            transformMatrix = Matrix.CreateTranslation(new Vector3(-position, 0)) * Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(zoom, zoom, 1)) * Matrix.CreateTranslation(new Vector3(Resolution.VirtualWidth
                            * 0.5f, Resolution.VirtualHeight * 0.5f, 0));

            //Now combine the camera's matrix with the Resolution Manager's transform matrix to get our final working matrix:
            transformMatrix *= Resolution.getTransformationMatrix();

            //Round the X and Y translation so the camera doesn't jerk as it moves:
            transformMatrix.M41 = (float)Math.Round(transformMatrix.M41, 0);
            transformMatrix.M42 = (float)Math.Round(transformMatrix.M42, 0);

            //Calculate the rectangle that represents where our camera is at in the world:
            screenRect = VisibleArea();
        }

        /// <summary>
        /// Calculates the screenRect based on where the camera currently is.
        /// </summary>
        private static Rectangle VisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(transformMatrix);
            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Resolution.VirtualWidth, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Resolution.VirtualHeight), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Resolution.VirtualWidth, Resolution.VirtualHeight), inverseViewMatrix);
            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
#pragma warning disable IDE0059 // Ненужное присваивание значения
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
#pragma warning restore IDE0059 // Ненужное присваивание значения
            return new Rectangle((int)min.X, (int)min.Y, (int)(Resolution.VirtualWidth / zoom), (int)(Resolution.VirtualHeight / zoom));
        }

        public static Matrix GetTransformMatrix()
        {
            return transformMatrix; //Return the transformMatrix we calculated earlier in this frame.
        }
    }
}
