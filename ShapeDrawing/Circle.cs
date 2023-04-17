using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDrawing
{
    class Circle : IShape
    {
        public char name { get; set; }
        public int centerX, centerY;
        public double radius;

        public Circle(int centerX,int centerY,double radius)
        {
            this.name = 'C';
            this.centerX = centerX;
            this.centerY = centerY;
            this.radius = radius;
        }

        public Circle(Circle circle)
        {
            this.name = 'C';
            centerX = circle.centerX;
            centerY = circle.centerY;
            radius = circle.radius;
        }

        public void Draw()
        {
            
        }

        public string Save()
        {
            throw new NotImplementedException();
        }

        public bool Check(int x, int y)
        {
            if (Math.Sqrt(Math.Pow(centerX - x, 2) + Math.Pow(centerY - y, 2)) <= 10)
                return true;
            return false;
        }
    }
}
