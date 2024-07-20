using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcanoid.Measures;
using Microsoft.Xna.Framework;
using static Arcanoid.Measures.Converter;

namespace Arcanoid
{
    internal sealed class Ball(float size, float relativeX, float relativeY) : RelativeObject(relativeX, relativeY, size, size)
    {

        public Vector2 Speed { get; set; } = new Vector2(0.0055f, -0.0055f);

        public void Move()
        {
            RelativeHorizontalPos += Speed.X;
            RelativeVerticalPos += Speed.Y;
        }

        public override Size GetSize(Size relativeTo)
        {
            var size = PercentToPixels(RelativeWidth, relativeTo.Height);
            return new Size(size, size);
        }
    }
}
