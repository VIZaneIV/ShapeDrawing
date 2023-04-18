using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShapeDrawing
{
    public partial class Form1 : Form
    {
        int x1, y1; // Drawing starting points
        int mode = 0; // Selected mode
                      // 0 - Grab
                      // 1 - Line
                      // 2 - Brush
                      // 3 - Circle
                      // 4 - Polygon
                      // 5 - Capsule

        bool drawing = false;
        Bitmap image;
        Bitmap imagePreview;
        List<IShape> shapes = new List<IShape>();

        Polygon tempPoly;
        IShape LastEdited;
        int polygonIndexEdited;

        public Form1()
        {
            InitializeComponent();

            thicknessBox.DataBindings.Add("Text", thicknessBar, "Value");

            Bitmap bmp = new Bitmap(Canvas.Width, Canvas.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            }
            Canvas.Image = bmp;
            image = bmp;

        }

        public void DrawLine(int x1, int y1, int x2,int y2)
        {
            // We'll draw left to right
            int startX = 0, startY = 0;
            int finishX = 0, finishY = 0;
            // Check if the points are directly above eachother 
            if (x1 == x2)
            {
                if (y1 < y2) // P1 is above P2
                    startY = y1;
                else         // P@ is above P1
                    startY = y2;

                for (int i = 0; i < Math.Abs(y2 - y1); i++)
                {
                    imagePreview.SetPixel(x1, startY + i, Color.Black);
                }
                return;
            }
            // Check order of the Points (is P1 more left to P2?). We'll draw left to right
            if (x1 < x2) //P1 is to the left of P2
            {
                startX = x1;
                startY = y1;
                finishX = x2;
                finishY = y2;
            }
            else         //P2 is to the left of P1
            {
                startX = x2;
                startY = y2;
                finishX = x1;
                finishY = y1;
            }
            // See the direction of the line by calculating Tan(alpha): (N-NE, NE-E, E-SE, SE-S)
            double angle = Convert.ToDouble(finishY - startY) / Convert.ToDouble(finishX - startX);
            double bCoeff = startY - angle * startX;
            int LastX = startX, LastY = startY;
            imagePreview.SetPixel(startX, startY, Color.Black);
            if (angle <= -1) // N-NE
            {
                for (int i = 0; i < Math.Abs(finishY - startY); i++) 
                {
                    //Set new midpoint
                    double MidY = LastY - 1;
                    double MidX = LastX + 0.5;

                    //Is the line above or below it?
                    if((MidY-bCoeff)/angle < MidX)
                    {
                        imagePreview.SetPixel(LastX, LastY - 1, Color.Black);
                        LastY -= 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY - 1, Color.Black);
                        LastX += 1;
                        LastY -= 1;
                    }
                }
                Canvas.Image = imagePreview;

                return;
            }
            if (angle < 0) // NE-E
            {
                for(int i = 0; i < Math.Abs(finishX - startX); i++)
                {
                    //Set new midpoint
                    double MidY = LastY - 0.5;
                    double MidX = LastX + 1;

                    //Is the line above or below it?
                    if (angle*MidX + bCoeff > MidY)
                    {
                        imagePreview.SetPixel(LastX + 1, LastY, Color.Black);
                        LastX += 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY - 1, Color.Black);
                        LastX += 1;
                        LastY -= 1;
                    }
                }
                Canvas.Image = imagePreview;
                return;
            }
            if (angle <= 1) // E-SE
            {
                for(int i = 0; i < Math.Abs(finishX - startX); i++)
                {
                    //Set new midpoint
                    double MidY = LastY + 0.5;
                    double MidX = LastX + 1;

                    //Is the line above or below it?
                    if (angle * MidX + bCoeff < MidY)
                    {
                        imagePreview.SetPixel(LastX + 1, LastY, Color.Black);
                        LastX += 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY + 1, Color.Black);
                        LastX += 1;
                        LastY += 1;
                    }
                }
                Canvas.Image = imagePreview;
                return;
            }
            if (angle > 1) // SE-S
            {
                for (int i = 0; i < Math.Abs(finishY - startY); i++)
                {
                    //Set new midpoint
                    double MidY = LastY + 1;
                    double MidX = LastX + 0.5;

                    //Is the line above or below it?
                    if ((MidY - bCoeff) / angle < MidX)
                    {
                        imagePreview.SetPixel(LastX, LastY + 1, Color.Black);
                        LastY += 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY + 1, Color.Black);
                        LastX += 1;
                        LastY += 1;
                    }
                }
                Canvas.Image = imagePreview;

                return;
            }

        }

        public void DrawCircle(int x1, int y1, int x2, int y2,Color color)
        {

            double radius = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            int x = 0, y = (int)radius;
            int d = 1 - (int)radius;

            while (x <= y)
            {
                imagePreview.SetPixel(x1 + x, y1 + y, color);
                imagePreview.SetPixel(x1 - x, y1 + y, color);
                imagePreview.SetPixel(x1 + x, y1 - y, color);
                imagePreview.SetPixel(x1 - x, y1 - y, color);
                imagePreview.SetPixel(x1 + y, y1 + x, color);
                imagePreview.SetPixel(x1 - y, y1 + x, color);
                imagePreview.SetPixel(x1 + y, y1 - x, color);
                imagePreview.SetPixel(x1 - y, y1 - x, color);

                x++;
                if (d < 0)
                    d += 2 * x + 1;
                else
                {
                    y--;
                    d += 2 * (x - y) + 1;
                }
            }

            Canvas.Image = imagePreview;
        }

        public void DrawHalfCircle(int x1,int y1,int tangentX1, int tangentY1, int tangentX2, int tangentY2, double radius, string direction, Color color)
        {
            int x = 0, y = (int)radius;
            int d = 1 - (int)radius;
            // If capsule is drawn horizontaly
            if(tangentX1 == tangentX2)
            {
                while (x <= y)
                {
                    if (direction == "Above")
                    {
                        if (x1 + x <= tangentX1)
                            imagePreview.SetPixel(x1 + x, y1 + y, color);
                        if (x1 - x <= tangentX1)
                            imagePreview.SetPixel(x1 - x, y1 + y, color);
                        if (x1 + x <= tangentX1)
                            imagePreview.SetPixel(x1 + x, y1 - y, color);
                        if (x1 - x <= tangentX1)
                            imagePreview.SetPixel(x1 - x, y1 - y, color);
                        if (x1 + y <= tangentX1)
                            imagePreview.SetPixel(x1 + y, y1 + x, color);
                        if (x1 - y <= tangentX1)
                            imagePreview.SetPixel(x1 - y, y1 + x, color);
                        if (x1 + y <= tangentX1)
                            imagePreview.SetPixel(x1 + y, y1 - x, color);
                        if (x1 - y <= tangentX1)
                            imagePreview.SetPixel(x1 - y, y1 - x, color);
                    }
                    else
                    {
                        if (x1 + x >= tangentX1)
                            imagePreview.SetPixel(x1 + x, y1 + y, color);
                        if (x1 - x <= tangentX1)
                            imagePreview.SetPixel(x1 - x, y1 + y, color);
                        if (x1 + x <= tangentX1)
                            imagePreview.SetPixel(x1 + x, y1 - y, color);
                        if (x1 - x <= tangentX1)
                            imagePreview.SetPixel(x1 - x, y1 - y, color);
                        if (x1 + y <= tangentX1)
                            imagePreview.SetPixel(x1 + y, y1 + x, color);
                        if (x1 - y <= tangentX1)
                            imagePreview.SetPixel(x1 - y, y1 + x, color);
                        if (x1 + y <= tangentX1)
                            imagePreview.SetPixel(x1 + y, y1 - x, color);
                        if (x1 - y <= tangentX1)
                            imagePreview.SetPixel(x1 - y, y1 - x, color);
                    }

                    x++;
                    if (d < 0)
                        d += 2 * x + 1;
                    else
                    {
                        y--;
                        d += 2 * (x - y) + 1;
                    }
                }
                return;
            }

            // Calculate line equation
                double angle = Convert.ToDouble(tangentY2 - tangentY1) / Convert.ToDouble(tangentX2 - tangentX1);
                double bCoeff = tangentY1 - angle * tangentX1;

            while (x <= y)
            {   
                if (direction == "Above")
                {
                    if (angle * (x1 + x) + bCoeff >= y1 + y)
                        imagePreview.SetPixel(x1 + x, y1 + y, color);
                    if (angle * (x1 - x) + bCoeff >= y1 + y)
                        imagePreview.SetPixel(x1 - x, y1 + y, color);
                    if (angle * (x1 + x) + bCoeff >= y1 - y)
                        imagePreview.SetPixel(x1 + x, y1 - y, color);
                    if (angle * (x1 - x) + bCoeff >= y1 - y)
                        imagePreview.SetPixel(x1 - x, y1 - y, color);
                    if (angle * (x1 + y) + bCoeff >= y1 + x)
                        imagePreview.SetPixel(x1 + y, y1 + x, color);
                    if (angle * (x1 - y) + bCoeff >= y1 + x)
                        imagePreview.SetPixel(x1 - y, y1 + x, color);
                    if (angle * (x1 + y) + bCoeff >= y1 - x)
                        imagePreview.SetPixel(x1 + y, y1 - x, color);
                    if (angle * (x1 - y) + bCoeff >= y1 - x)
                        imagePreview.SetPixel(x1 - y, y1 - x, color);
                }
                else
                {
                    if (angle * (x1 + x) + bCoeff <= y1 + y)
                        imagePreview.SetPixel(x1 + x, y1 + y, color);
                    if (angle * (x1 - x) + bCoeff <= y1 + y)
                        imagePreview.SetPixel(x1 - x, y1 + y, color);
                    if (angle * (x1 + x) + bCoeff <= y1 - y)
                        imagePreview.SetPixel(x1 + x, y1 - y, color);
                    if (angle * (x1 - x) + bCoeff <= y1 - y)
                        imagePreview.SetPixel(x1 - x, y1 - y, color);
                    if (angle * (x1 + y) + bCoeff <= y1 + x)
                        imagePreview.SetPixel(x1 + y, y1 + x, color);
                    if (angle * (x1 - y) + bCoeff <= y1 + x)
                        imagePreview.SetPixel(x1 - y, y1 + x, color);
                    if (angle * (x1 + y) + bCoeff <= y1 - x)
                        imagePreview.SetPixel(x1 + y, y1 - x, color);
                    if (angle * (x1 - y) + bCoeff <= y1 - x)
                        imagePreview.SetPixel(x1 - y, y1 - x, color);
                }

                x++;
                if (d < 0)
                    d += 2 * x + 1;
                else
                {
                    y--;
                    d += 2 * (x - y) + 1;
                }
            }

            Canvas.Image = imagePreview;
        }

        public bool DrawPolygon(int x2, int y2)
        {
            // Stop drawing polygon? Click next to the start point
            if (Math.Sqrt((x2 - tempPoly.startX) * (x2 - tempPoly.startX) + (y2 - tempPoly.startY) * (y2 - tempPoly.startY)) <= 10)
            {
                for (int i = 0; i < tempPoly.points.Count - 1; i++)
                {
                    DrawLine(tempPoly.points[i].x, tempPoly.points[i].y,
                                tempPoly.points[i + 1].x, tempPoly.points[i + 1].y);
                }
                DrawLine(tempPoly.points[tempPoly.points.Count - 1].x, tempPoly.points[tempPoly.points.Count - 1].y,
                            tempPoly.points[0].x, tempPoly.points[0].y);
                return true;
            }
            // Draw "End Circle"
            DrawCircle(x1, y1, tempPoly.startX, tempPoly.startY + 10, Color.Red);

            // Redraw all edges
            for (int i = 0; i <tempPoly.points.Count - 1;i++)
            {
                DrawLine(tempPoly.points[i].x, tempPoly.points[i].y,
                            tempPoly.points[i + 1].x, tempPoly.points[i + 1].y);
            }
            // Last edge is drawn to the cursor
            DrawLine(tempPoly.points[tempPoly.points.Count - 1].x, tempPoly.points[tempPoly.points.Count - 1].y,
                        x2, y2);
            return false;
        }

        private void DrawPolygon(Polygon polygon)
        {
            for (int i = 0; i < polygon.points.Count - 1; i++)
            {
                DrawLine(polygon.points[i].x, polygon.points[i].y,
                            polygon.points[i + 1].x, polygon.points[i + 1].y);
            }
            DrawLine(polygon.points[polygon.points.Count - 1].x, polygon.points[polygon.points.Count - 1].y,
                        polygon.points[0].x, polygon.points[0].y);
        }

        private void Redraw()
        {
            // Fill image with white 
            Bitmap bmp = new Bitmap(Canvas.Width, Canvas.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            }
            Canvas.Image = bmp;
            image = bmp;
            imagePreview = (Bitmap)bmp.Clone();

            foreach (var shape in shapes)
            {
                switch (shape.name)
                {
                    case 'L':
                        Line tmpLine = shape as Line;
                        DrawLine(tmpLine.X1, tmpLine.Y1, tmpLine.X2, tmpLine.Y2);
                        break;
                    case 'C':
                        Circle tmpCircle = shape as Circle;
                        DrawCircle(tmpCircle.centerX, tmpCircle.centerY, tmpCircle.centerX, (int)(tmpCircle.centerY + tmpCircle.radius), Color.Black);
                        break;
                    case 'P':
                        Polygon tmpPolygon = shape as Polygon;
                        DrawPolygon(tmpPolygon.startX, tmpPolygon.startY);
                        break;
                }
            }
            image = (Bitmap)imagePreview.Clone();

        }

        private void DrawCapsule(int x, int y)
        {
            int radius = 20;
            double dx = x1 - x;
            double dy = y1 - y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            dx /= dist;
            dy /= dist;

            int tangentAX1 = (int)(x1 + radius * dy), tangentAY1 = (int)(y1 - radius * dx), 
                tangentAX2 = (int)(x1 - radius * dy), tangentAY2 = (int)(y1 + radius * dx);
            int tangentBX1 = (int)(x + radius * dy), tangentBY1 = (int)(y - radius * dx),
                tangentBX2 = (int)(x - radius * dy), tangentBY2 = (int)(y + radius * dx);

            if (x1 < x)
            {
                if (y1 <= y)
                {
                    DrawHalfCircle(x1, y1, tangentAX1, tangentAY1, tangentAX2, tangentAY2, radius, "Above", Color.Black);
                    DrawHalfCircle(x, y, tangentBX1, tangentBY1, tangentBX2, tangentBY2, radius, "Below", Color.Black);
                }
                else
                {
                    DrawHalfCircle(x1, y1, tangentAX1, tangentAY1, tangentAX2, tangentAY2, radius, "Below", Color.Black);
                    DrawHalfCircle(x, y, tangentBX1, tangentBY1, tangentBX2, tangentBY2, radius, "Above", Color.Black);
                }
            }
            else
            {
                if(y1 < y)
                {
                    DrawHalfCircle(x1, y1, tangentAX1, tangentAY1, tangentAX2, tangentAY2, radius, "Above", Color.Black);
                    DrawHalfCircle(x, y, tangentBX1, tangentBY1, tangentBX2, tangentBY2, radius, "Below", Color.Black);
                }
                else
                {
                    DrawHalfCircle(x1, y1, tangentAX1, tangentAY1, tangentAX2, tangentAY2, radius, "Below", Color.Black);
                    DrawHalfCircle(x, y, tangentBX1, tangentBY1, tangentBX2, tangentBY2, radius, "Above", Color.Black);
                }
            }
            
            //DrawCircle(x1, y1, x1, y1 + radius, Color.Black);
            //DrawCircle(x, y, x, y + radius, Color.Black);



            DrawLine(tangentAX1, tangentAY1,
                        tangentBX1, tangentBY1);
            DrawLine(tangentAX2, tangentAY2,
                        tangentBX2, tangentBY2);

        }

        private void grabButton_Click(object sender, EventArgs e)
        {
            mode = 0;
            this.Text = "Grab";
        }

        private void thicknessButton_Click(object sender, EventArgs e)
        {
            mode = 1;
            this.Text = "Line";
        }

        private void brushButton_Click(object sender, EventArgs e)
        {
            mode = 2;
            this.Text = "Brush";
        }

        private void circleButton_Click(object sender, EventArgs e)
        {
            mode = 3;
            this.Text = "Circle";
        }

        private void polygonButton_Click(object sender, EventArgs e)
        {
            mode = 4;
            this.Text = "Polygon";
        }

        private void capsuleButton_Click(object sender, EventArgs e)
        {
            mode = 5;
            this.Text = "Capsule";
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (!drawing)
                imagePreview = (Bitmap)image.Clone();
            switch (mode)
            {
                case 0:
                    // Iterate over all shapes to find one selecter
                    for (int i = 0; i<shapes.Count;i++)
                    {
                        if (shapes[i].Check(e.X, e.Y))
                        {
                            switch (shapes[i].name)
                            {
                                case 'L':
                                    Line tmpLine = new Line(shapes[i] as Line);
                                    LastEdited = tmpLine;
                                    shapes.RemoveAt(i);
                                    if (Math.Sqrt(Math.Pow(tmpLine.X1 - e.X, 2) + Math.Pow(tmpLine.Y1 - e.Y, 2)) <= 10)
                                    {
                                        x1 = tmpLine.X2;
                                        y1 = tmpLine.Y2;
                                    }
                                    else
                                    {
                                        x1 = tmpLine.X1;
                                        y1 = tmpLine.Y1;
                                    }
                                    drawing = true;
                                    break;
                                case 'C':
                                    Circle tmpCircle = new Circle(shapes[i] as Circle);
                                    LastEdited = tmpCircle;
                                    shapes.RemoveAt(i);
                                    x1 = tmpCircle.centerX;
                                    y1 = tmpCircle.centerY;
                                    drawing = true;
                                    break;
                                case 'P':
                                    Polygon tmpPolygon = new Polygon(shapes[i] as Polygon);
                                    LastEdited = tmpPolygon;
                                    shapes.RemoveAt(i);
                                    foreach (var point in tmpPolygon.points)
                                    {
                                        if (Math.Sqrt(Math.Pow(point.x - e.X, 2) + Math.Pow(point.y - e.Y, 2)) <= 10)
                                        {
                                            polygonIndexEdited = tmpPolygon.points.IndexOf(point);
                                            break;
                                        }
                                    }
                                    drawing = true;
                                    break;
                            }
                            Redraw();
                            break;
                        }
                    }
                    break;
                case 1:
                    x1 = e.X;
                    y1 = e.Y;
                    drawing = true;
                    break;
                case 2:
                    x1 = e.X;
                    y1 = e.Y;
                    drawing = true;
                    break;
                case 3:
                    x1 = e.X;
                    y1 = e.Y;
                    drawing = true;
                    break;
                case 4:
                    if(drawing == false)
                    {
                        x1 = e.X;
                        y1 = e.Y;
                        drawing = true;
                        tempPoly = new Polygon();
                        tempPoly.Add(x1, y1);
                    }
                    else
                    {
                        if (!DrawPolygon(e.X, e.Y))
                            tempPoly.Add(e.X, e.Y);
                        else
                        {
                            drawing = false;
                            shapes.Add(tempPoly);
                            image = (Bitmap)imagePreview.Clone();
                        }
                    }
                    break;
                case 5:
                    x1 = e.X;
                    y1 = e.Y;
                    drawing = true;
                    break;

            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                imagePreview = (Bitmap)image.Clone();
                switch (mode)
                {
                    case 0:
                        if (LastEdited != null && drawing)
                            switch (LastEdited.name)
                            {
                                case 'L':
                                    DrawLine(x1, y1, e.X, e.Y);
                                    break;
                                case 'C':
                                    DrawCircle(e.X, e.Y, e.X, (int)(e.Y + (LastEdited as Circle).radius), Color.Black);
                                    break;
                                case 'P':
                                    (LastEdited as Polygon).points.RemoveAt(polygonIndexEdited);
                                    (LastEdited as Polygon).points.Insert(polygonIndexEdited, (e.X, e.Y));
                                    DrawPolygon(LastEdited as Polygon);
                                    break;
                            }
                            break;
                    case 1:
                        DrawLine(x1, y1, e.X, e.Y);
                        break;
                    case 2:
                        DrawLine(x1, y1, e.X, e.Y);
                        x1 = e.X;
                        y1 = e.Y;
                        image = (Bitmap)imagePreview.Clone();
                        break;
                    case 3:
                        DrawCircle(x1, y1, e.X, e.Y, Color.Black);
                        break;
                    case 4:
                        DrawPolygon(e.X, e.Y);
                        break;
                    case 5:
                        DrawCapsule(e.X, e.Y);
                        break;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case 0:
                    if (LastEdited != null && drawing)
                        switch (LastEdited.name)
                        {
                            case 'L':
                                drawing = false;
                                shapes.Add(new Line(x1, y1, e.X, e.Y));
                                image = (Bitmap)imagePreview.Clone();
                                break;
                            case 'C':
                                drawing = false;
                                shapes.Add(new Circle(e.X, e.Y, (LastEdited as Circle).radius));
                                image = (Bitmap)imagePreview.Clone();
                                break;
                            case 'P':
                                drawing = false;
                                shapes.Add(LastEdited as Polygon);
                                image = (Bitmap)imagePreview.Clone();
                                break;
                        }
                    break;
                case 1:
                    drawing = false;
                    shapes.Add(new Line(x1, y1, e.X, e.Y));
                    image = (Bitmap)imagePreview.Clone();
                    break;
                case 2:
                    drawing = false;
                    break;
                case 3:
                    drawing = false;
                    shapes.Add(new Circle(x1, y1, Math.Sqrt((e.X - x1) * (e.X - x1) + (e.Y - y1) * (e.Y - y1))));
                    image = (Bitmap)imagePreview.Clone();
                    break;
                case 5:
                    drawing = false;
                    image = (Bitmap)imagePreview.Clone();
                    break;
            }
            
        }
    }
}
