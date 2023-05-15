using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDrawing
{
    class Rectangle : IShape
    {

        public char name { get; set; }
        public List<(int x, int y)> points = new List<(int x, int y)>();
        public (int x, int y) center;
        public int left, right, up, down;

        public Rectangle()
        {
            this.name = 'R';
        }

        public Rectangle(int x1, int y1, int x2, int y2)
        {
            this.name = 'R';
            points.Add((x1, y1));
            points.Add((x2, y1));
            points.Add((x2, y2));
            points.Add((x1, y2));
            center = (x1 + Convert.ToInt32((x2-x1)/2), y1 + Convert.ToInt32((y2-y1)/2));
            left = Math.Min(x1, x2);
            right = Math.Max(x1, x2);
            up = Math.Min(y1, y2);
            down = Math.Max(y1, y2);
        }

        public Rectangle(Rectangle rectangle)
        {
            this.name = 'R';
            foreach(var point in rectangle.points)
            {
                this.points.Add(point);
            }
            center = rectangle.center;
            left = Math.Min(rectangle.points[0].x, rectangle.points[1].x);
            right = Math.Max(rectangle.points[0].x, rectangle.points[1].x);
            up = Math.Min(rectangle.points[0].y, rectangle.points[2].y);
            down = Math.Max(rectangle.points[0].y, rectangle.points[2].y);
        }

        public bool Check(int x, int y)
        {
            foreach (var point in points)
            {
                if (Math.Sqrt(Math.Pow(point.x - x, 2) + Math.Pow(point.y - y, 2)) <= 10)
                    return true;
            }
            if (Math.Sqrt(Math.Pow(center.x - x, 2) + Math.Pow(center.y - y, 2)) <= 10)
                return true;
            return false;
        }

        public string Save()
        {
            string pointStrings = "";
            foreach (var point in points)
            {
                pointStrings += ";" + point.x + ";" + point.y;
            }
            return name + pointStrings;
        }
    }
}
