using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid.Measures
{
    static internal class Converter
    {
        public static int PercentToPixels(double percent, int relativeTo)
        {
            return (int)Math.Round(relativeTo * percent);
        }

        public static double PixelsToPercent(int pixels, int relativeTo)
        {
            return pixels / (double)relativeTo;
        }
    }
}
