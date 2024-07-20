using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Arcanoid
{
    internal sealed class Block(float relativeX, float relativeY, float relativeWidth, float relativeHeight) : RelativeObject(relativeX, relativeY, relativeWidth, relativeHeight)
    {
        public bool IsBroken { get; set; }

        public Color Color { get; set; } = Color.White;
    }
}
