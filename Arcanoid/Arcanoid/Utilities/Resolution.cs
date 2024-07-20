//////////////////////////////////////////////////////////////////////////
////License:  The MIT License (MIT)
////Copyright (c) 2010 David Amador (http://www.david-amador.com)
////
////Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
////
////The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
////
////THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid
{
    static internal class Resolution
    {
        private static GraphicsDeviceManager device = null!;

        private static int width = 800;
        private static int height = 600;
        private static int vWidth = 1024;
        private static int vHeight = 768;
        private static Matrix scaleMatrix;
        private static bool fullScreen;
        private static bool dirtyMatrix = true;
        private static int virtualViewportX;
        private static int virtualViewportY;

        public static int VirtualViewportX => virtualViewportX;

        public static int VirtualViewportY => virtualViewportY;

        public static int VirtualWidth => vWidth;

        public static int VirtualHeight => vHeight;

        public static void Init(ref GraphicsDeviceManager device)
        {
            width = device.PreferredBackBufferWidth;
            height = device.PreferredBackBufferHeight;
            Resolution.device = device;
            dirtyMatrix = true;
            ApplyResolutionSettings();
        }

        public static Matrix getTransformationMatrix()
        {
            if (dirtyMatrix)
            {
                RecreateScaleMatrix();
            }

            return scaleMatrix;
        }

        public static void SetResolution(int Width, int Height, bool FullScreen)
        {
            width = Width;
            height = Height;

            fullScreen = FullScreen;

            ApplyResolutionSettings();
        }

        public static void SetVirtualResolution(int Width, int Height)
        {
            vWidth = Width;
            vHeight = Height;

            dirtyMatrix = true;
        }

        private static void ApplyResolutionSettings()
        {

#if XBOX360
           _FullScreen = true;
#endif

            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (fullScreen == false)
            {
                if ((width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    device.PreferredBackBufferWidth = width;
                    device.PreferredBackBufferHeight = height;
                    device.IsFullScreen = fullScreen;
                    device.PreferMultiSampling = true;
                    device.ApplyChanges();
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate through the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == width) && (dm.Height == height))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        device.PreferredBackBufferWidth = width;
                        device.PreferredBackBufferHeight = height;
                        device.IsFullScreen = fullScreen;
                        device.PreferMultiSampling = true;
                        device.ApplyChanges();
                    }
                }
            }

            dirtyMatrix = true;

            width = device.PreferredBackBufferWidth;
            height = device.PreferredBackBufferHeight;
        }

        /// <summary>
        /// Sets the device to use the draw pump
        /// Sets correct aspect ratio
        /// </summary>
        public static void BeginDraw()
        {
            // Start by reseting viewport to (0,0,1,1)
            FullViewport();
            // Clear to Black
            device.GraphicsDevice.Clear(Color.Black);
            // Calculate Proper Viewport according to Aspect Ratio
            ResetViewport();
            // and clear that
            // This way we are gonna have black bars if aspect ratio requires it and
            // the clear color on the rest
            device.GraphicsDevice.Clear(Color.Black);
        }

        private static void RecreateScaleMatrix()
        {
            dirtyMatrix = false;
            scaleMatrix = Matrix.CreateScale(
                           (float)device.GraphicsDevice.Viewport.Width / vWidth,
                           (float)device.GraphicsDevice.Viewport.Width / vWidth,
                           1f);
        }


        public static void FullViewport()
        {
            var vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = width;
            vp.Height = height;
            device.GraphicsDevice.Viewport = vp;
        }

        /// <summary>
        /// Get virtual Mode Aspect Ratio
        /// </summary>
        /// <returns>aspect ratio</returns>
        public static float getVirtualAspectRatio()
        {
            return (float)vWidth / (float)vHeight;
        }

        public static void ResetViewport()
        {
            var targetAspectRatio = getVirtualAspectRatio();
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            var width = device.PreferredBackBufferWidth;
            var height = (int)(width / targetAspectRatio + .5f);
            var changed = false;

            if (height > device.PreferredBackBufferHeight)
            {
                height = device.PreferredBackBufferHeight;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
                changed = true;
            }

            // set up the new viewport centered in the backbuffer
            var viewport = new Viewport
            {
                X = (device.PreferredBackBufferWidth / 2) - (width / 2),
                Y = (device.PreferredBackBufferHeight / 2) - (height / 2)
            };
            virtualViewportX = viewport.X;
            virtualViewportY = viewport.Y;
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (changed)
            {
                dirtyMatrix = true;
            }

            device.GraphicsDevice.Viewport = viewport;
        }
    }
}
