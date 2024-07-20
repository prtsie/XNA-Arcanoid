using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Arcanoid
{
    static internal class Global
    {
        public static ArcanoidGame Game = null!;
        public static Random Random = new();
        public static string LevelName = null!;

        public static void Initialize(ArcanoidGame inputGame)
        {
            Game = inputGame;
        }
    }
}
