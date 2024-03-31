using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static Arcanoid.Measures.Converter;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Arcanoid
{
    internal class RelativeObject(float relativeX, float relativeY, float relativeWidth, float relativeHeight)
    {
        public float RelativeWidth { get; set; } = relativeWidth;

        public float RelativeHeight { get; set; } = relativeHeight;

        public float RelativeHorizontalPos { get; set; } = relativeX;

        public float RelativeVerticalPos { get; set; } = relativeY;

        public Point GetPosition(Size relativeTo)
        {
            return new Point(PercentToPixels(RelativeHorizontalPos, relativeTo.Width), PercentToPixels(RelativeVerticalPos, relativeTo.Height));
        }

        public virtual Size GetSize(Size relativeTo)
        {
            return new Size(PercentToPixels(RelativeWidth, relativeTo.Width), PercentToPixels(RelativeHeight, relativeTo.Height));
        }

        public Rectangle GetRectangle(Size relativeTo)
        {
            var pos = GetPosition(relativeTo);
            var size = GetSize(relativeTo);
            return new Rectangle(pos.X, pos.Y, size.Width, size.Height);
        }
    }
}
