﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid
{
    //If you use the Content Pipeline for XNA or MonoGame, textures have their alpha "pre multiplied" to prevent any weird artifacts.
    //If you're loading raw .pngs from your engine, they haven't been pre multipled so you'll likely see artifacts when drawing your images.
    //This class will automatically pre multiply textures. If you later switch to using a Content Pipeline just set the usingPipeline bool to true so we don't premultiply the alpha again.

    public static class TextureLoader
    {
        private const bool UsingPipeline = false;

        public static Texture2D Load(string filePath, ContentManager content)
        {
            Texture2D image = content.Load<Texture2D>(filePath);

            if (UsingPipeline == false)
            {
                PremultiplyTexture(image);
            }

            return image;
        }

        private static void PremultiplyTexture(Texture2D texture)
        {
            //This function pre multiplies the alpha of a texture, just like the XNA Content Pipeline does:
            var buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }
    }
}
