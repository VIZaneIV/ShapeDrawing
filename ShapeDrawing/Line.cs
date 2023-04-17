using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDrawing
{
    class Line : IShape
    {
        public char name { get ; set ; }

        public int X1,Y1,X2,Y2;
        public int thickness = 1;

        public Line(int x1, int y1, int x2, int y2)
        {
            this.name = 'L';
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public Line(Line line)
        {
            this.name = 'L';
            X1 = line.X1;
            Y1 = line.Y1;
            X2 = line.X2;
            Y2 = line.Y2;
        }

        public string Save()
        {
            throw new NotImplementedException();
        }

        public bool Check(int x, int y)
        {
            if (Math.Sqrt(Math.Pow(X1 - x, 2) + Math.Pow(Y1 - y, 2)) <= 10 ||
                Math.Sqrt(Math.Pow(X2 - x, 2) + Math.Pow(Y2 - y, 2)) <= 10)
                return true;

            return false;
        }
    }
}
