using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abalone
{
    class myPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Ind { get; set; }

        public myPoint(int x, int y, int ind)
        {
            X = x;
            Y = y;
            Ind = ind;
        }
    }
}
