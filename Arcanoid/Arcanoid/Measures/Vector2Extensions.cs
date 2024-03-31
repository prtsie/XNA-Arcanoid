using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Arcanoid.Measures
{
    static internal class Vector2Extensions
    {
        public static float Angle(this Vector2 vector) => MathF.Atan2(vector.Y, vector.X);

        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            return new Vector2(vector.X * MathF.Cos(angle) - vector.Y * MathF.Sin(angle), vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle));
        }
    }
}
