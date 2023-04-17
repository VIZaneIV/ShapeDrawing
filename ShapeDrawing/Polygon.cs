using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDrawing
{
    class Polygon : IShape
    {
        public char name { get; set; }
        public int startX = -1, startY = -1;
        public List<(int x, int y)> points = new List<(int x, int y)>();

        public Polygon()
        {
            this.name = 'P';
        }

        public Polygon(List<(int x, int y)> points)
        {
            this.name = 'P';
            foreach( var point in points)
            {
                this.points.Add(point);
            }
        }

        public Polygon(Polygon polygon)
        {
            this.name = 'P';
            startX = polygon.points[0].x;
            startY = polygon.points[0].y;
            foreach(var point in polygon.points)
            {
                points.Add(point);
            }
        }

        public void Add(int x, int y)
        {
            if(startX == -1 || startY == -1)
            {
                startX = x;
                startY = y;
            }
            points.Add((x, y));
        }
        public void Draw()
        {
            throw new NotImplementedException();
        }

        public string Save()
        {
            throw new NotImplementedException();
        }

        public bool Check(int x, int y)
        {
            foreach (var point in points)
            {
                if (Math.Sqrt(Math.Pow(point.x - x, 2) + Math.Pow(point.y - y, 2)) <= 10)
                    return true;
            }
            return false;
        }
    }
}
