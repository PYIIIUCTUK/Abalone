using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abalone
{
    class Cell
    {
        public List<Point> Cells { get; set; } = new List<Point>();
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; } = 0;

        public Cell(int value, int curX, int curY, int type)
        {
            Value = value;
            X = curX;
            Y = curY;

            // начало с верхнего левого угла
            if (type == 3)
            {
                Cells.Add(new Point(curX, curY - 1));
                Cells.Add(new Point(curX + 1, curY - 1));

                Cells.Add(new Point(curX + 1, curY));

                Cells.Add(new Point(curX, curY + 1));
                Cells.Add(new Point(curX - 1, curY + 1));

                Cells.Add(new Point(curX - 1, curY));
            }
            else if (type == 2)
            {
                Cells.Add(new Point(curX - 1, curY - 1));
                Cells.Add(new Point(curX, curY - 1));

                Cells.Add(new Point(curX + 1, curY));

                Cells.Add(new Point(curX, curY + 1));
                Cells.Add(new Point(curX - 1, curY + 1));

                Cells.Add(new Point(curX - 1, curY));
            }
            else
            {
                Cells.Add(new Point(curX -1, curY -1));
                Cells.Add(new Point(curX, curY -1));

                Cells.Add(new Point(curX + 1, curY));

                Cells.Add(new Point(curX + 1, curY + 1));
                Cells.Add(new Point(curX, curY + 1));

                Cells.Add(new Point(curX -1, curY));
            }
        }

        public int ChoiceOption(int x, int y)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                if (x == Cells[i].X && y == Cells[i].Y)
                {
                    return i;
                }
            }
            return -1;
        }
        public Point GetNexCell(int i)
        {
            return Cells[i];
        }
    }
}
