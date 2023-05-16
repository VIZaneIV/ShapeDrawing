using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
                      // 6 - Rectangle
                      // 7 - Box Select
                      // 8 - Bucket

        bool drawing = false;
        bool move = false;
        Bitmap image;
        Bitmap imagePreview;
        List<IShape> shapes = new List<IShape>();


        Polygon tempPoly;
        IShape LastEdited;
        int polygonIndexEdited;
        int lastIndexEdited;

        Rectangle selection = new Rectangle(0,0,0,0);

        Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Violet };

        public enum Outcode
        {
            Inside = 0,
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8
        }

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

            InitializeComboBox();
        }

        public void InitializeComboBox()
        {
            colorComboBox.Items.Add(Color.Red);
            colorComboBox.Items.Add(Color.Orange);
            colorComboBox.Items.Add(Color.Yellow);
            colorComboBox.Items.Add(Color.Green);
            colorComboBox.Items.Add(Color.Blue);
            colorComboBox.Items.Add(Color.Violet);
            //colorComboBox.DrawItem += ColorComboBox_DrawItem;
        }

        //private void ColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    if (e.Index < 0)
        //        return;

        //    // Retrieve the ComboBoxItem
        //    ComboBoxItem item = (ComboBoxItem)colorComboBox.Items[e.Index];

        //    // Set the background color and text color based on the item's state
        //    Brush backgroundBrush = (e.State & DrawItemState.Selected) == DrawItemState.Selected
        //        ? SystemBrushes.Highlight
        //        : new SolidBrush(item.BackgroundColor);
        //    Brush textBrush = (e.State & DrawItemState.Selected) == DrawItemState.Selected
        //        ? SystemBrushes.HighlightText
        //        : SystemBrushes.WindowText;

        //    // Clear the area
        //    e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

        //    // Draw the color name text
        //    System.Drawing.Rectangle textRect = new System.Drawing.Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
        //    e.Graphics.DrawString(item.Text, e.Font, textBrush, textRect);
        //}

        public void DrawLine(int x1, int y1, int x2, int y2, Color color = default)
        {
            if (color == Color.Empty)
                color = Color.Black;
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
                    imagePreview.SetPixel(x1, startY + i, color);
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
            imagePreview.SetPixel(startX, startY, color);
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
                        imagePreview.SetPixel(LastX, LastY - 1, color);
                        LastY -= 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY - 1, color);
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
                        imagePreview.SetPixel(LastX + 1, LastY, color);
                        LastX += 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY - 1, color);
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
                        imagePreview.SetPixel(LastX + 1, LastY, color);
                        LastX += 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY + 1, color);
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
                        imagePreview.SetPixel(LastX, LastY + 1, color);
                        LastY += 1;
                    }
                    else
                    {
                        imagePreview.SetPixel(LastX + 1, LastY + 1, color);
                        LastX += 1;
                        LastY += 1;
                    }
                }
                Canvas.Image = imagePreview;

                return;
            }

        }

        public void DrawThickLine(int x1, int y1, int x2, int y2)
        {
            double radius = Convert.ToDouble(thicknessBox.Text) + 0.5;
            int x = 0, y = (int)radius;
            int d = 1 - (int)radius;

            while (x <= y)
            {
                DrawLine(x1 + x, y1 + y, x2 + x, y2 + y);
                DrawLine(x1 - x, y1 + y, x2 - x, y2 + y);
                DrawLine(x1 + x, y1 - y, x2 + x, y2 - y);
                DrawLine(x1 - x, y1 - y, x2 - x, y2 - y);
                DrawLine(x1 + y, y1 + x, x2 + y, y2 + x);
                DrawLine(x1 - y, y1 + x, x2 - y, y2 + x);
                DrawLine(x1 + y, y1 - x, x2 + y, y2 - x);
                DrawLine(x1 - y, y1 - x, x2 - y, y2 - x);

                x++;
                if (d < 0)
                    d += 2 * x + 1;
                else
                {
                    y--;
                    d += 2 * (x - y) + 1;
                }
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
                    Cohen_Sutherland(tempPoly.points[i].x, tempPoly.points[i].y,
                                tempPoly.points[i + 1].x, tempPoly.points[i + 1].y);

                }
                DrawLine(tempPoly.points[tempPoly.points.Count - 1].x, tempPoly.points[tempPoly.points.Count - 1].y,
                            tempPoly.points[0].x, tempPoly.points[0].y);
                Cohen_Sutherland(tempPoly.points[tempPoly.points.Count - 1].x, tempPoly.points[tempPoly.points.Count - 1].y,
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
                Cohen_Sutherland(tempPoly.points[i].x, tempPoly.points[i].y,
            tempPoly.points[i + 1].x, tempPoly.points[i + 1].y);
            }
            // Last edge is drawn to the cursor
            DrawLine(tempPoly.points[tempPoly.points.Count - 1].x, tempPoly.points[tempPoly.points.Count - 1].y,
                        x2, y2);
            Cohen_Sutherland(tempPoly.points[tempPoly.points.Count - 1].x, tempPoly.points[tempPoly.points.Count - 1].y,
                        x2, y2);
            return false;
        }

        private void DrawPolygon(Polygon polygon)
        {
            for (int i = 0; i < polygon.points.Count - 1; i++)
            {
                DrawLine(polygon.points[i].x, polygon.points[i].y,
                            polygon.points[i + 1].x, polygon.points[i + 1].y);
                Cohen_Sutherland(polygon.points[i].x, polygon.points[i].y,
                            polygon.points[i + 1].x, polygon.points[i + 1].y);
            }
            DrawLine(polygon.points[polygon.points.Count - 1].x, polygon.points[polygon.points.Count - 1].y,
                        polygon.points[0].x, polygon.points[0].y);
            Cohen_Sutherland(polygon.points[polygon.points.Count - 1].x, polygon.points[polygon.points.Count - 1].y,
                        polygon.points[0].x, polygon.points[0].y);
        }

        private void DrawRectangle(int x2, int y2, Color color = default)
        {
            if (color == Color.Empty)
                color = Color.Black;
            DrawLine(x1, y1, x2, y1, color);
            DrawLine(x2, y1, x2, y2, color);
            DrawLine(x2, y2, x1, y2, color);
            DrawLine(x1, y2, x1, y1, color);
        }

        private void DrawRectangle(int x1, int y1, int x2, int y2, Color color = default)
        {
            if (color == Color.Empty)
                color = Color.Black;
            DrawLine(x1, y1, x2, y1, color);
            DrawLine(x2, y1, x2, y2, color);
            DrawLine(x2, y2, x1, y2, color);
            DrawLine(x1, y2, x1, y1, color);

            Cohen_Sutherland(x1, y1, x2, y1);
            Cohen_Sutherland(x2, y1, x2, y2);
            Cohen_Sutherland(x2, y2, x1, y2);
            Cohen_Sutherland(x1, y2, x1, y1);
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
                        Cohen_Sutherland(tmpLine.X1, tmpLine.Y1, tmpLine.X2, tmpLine.Y2);
                        break;
                    case 'C':
                        Circle tmpCircle = shape as Circle;
                        DrawCircle(tmpCircle.centerX, tmpCircle.centerY, tmpCircle.centerX, (int)(tmpCircle.centerY + tmpCircle.radius), Color.Black);
                        break;
                    case 'P':
                        Polygon tmpPolygon = shape as Polygon;
                        tempPoly = tmpPolygon;
                        DrawPolygon(tmpPolygon.startX, tmpPolygon.startY);
                        break;
                    case 'R':
                        Rectangle tmprectangle = shape as Rectangle;
                        DrawRectangle(tmprectangle.points[0].x, tmprectangle.points[0].y,
                                        tmprectangle.points[2].x, tmprectangle.points[2].y);
                        break;
                }
            }
            image = (Bitmap)imagePreview.Clone();

        }

        private Outcode CalculateOutcode(int x, int y)
        {
            Outcode outcode = Outcode.Inside;

            if (x < selection.left)
                outcode |= Outcode.Left;
            else if (x > selection.right)
                outcode |= Outcode.Right;

            if (y < selection.up)
                outcode |= Outcode.Top;
            else if (y > selection.down)
                outcode |= Outcode.Bottom;

            return outcode;
        }

        private void Cohen_Sutherland(int x1, int y1, int x2, int y2)
        {
            Outcode outcode1 = CalculateOutcode(x1, y1);
            Outcode outcode2 = CalculateOutcode(x2, y2);
            bool accepted = false;

            while (true)
            {
                // Completely inside
                if ((outcode1 | outcode2) == Outcode.Inside)
                {
                    accepted = true;
                    break;
                }
                // Completely outside
                if ((outcode1 & outcode2) != Outcode.Inside)
                {
                    break;
                }
                // The line may intersect selection box
                Outcode outcodeOut = outcode1 != Outcode.Inside ? outcode1 : outcode2;

                // Calculate intersection point
                Point intersectionPoint = new Point();
                if ((outcodeOut & Outcode.Top) == Outcode.Top)
                {
                    intersectionPoint.X = (int)(x1 + (float)((x2 - x1) * (selection.up - y1)) / (y2 - y1));
                    intersectionPoint.Y = selection.up;
                }
                else if ((outcodeOut & Outcode.Bottom) == Outcode.Bottom)
                {
                    intersectionPoint.X = (int)(x1 + (float)((x2 - x1) * (selection.down - y1)) / (y2 - y1));
                    intersectionPoint.Y = selection.down;
                }
                else if ((outcodeOut & Outcode.Right) == Outcode.Right)
                {
                    intersectionPoint.Y = (int)(y1 + (float)((y2 - y1) * (selection.right - x1)) / (x2 - x1));
                    intersectionPoint.X = selection.right;
                }
                else if ((outcodeOut & Outcode.Left) == Outcode.Left)
                {
                    intersectionPoint.Y = (int)(y1 + (float)((y2 - y1) * (selection.left - x1)) / (x2 - x1));
                    intersectionPoint.X = selection.left;
                }

                if (outcodeOut == outcode1)
                {
                    x1 = intersectionPoint.X;
                    y1 = intersectionPoint.Y;
                    outcode1 = CalculateOutcode(x1, y1);
                }
                else
                {
                    x2 = intersectionPoint.X;
                    y2 = intersectionPoint.Y;
                    outcode2 = CalculateOutcode(x2, y2);
                }
            }
            if (accepted)
                DrawLine(x1, y1, x2, y2, Color.Red);
        }

        //private (int x, int y) Subdivide(int x1, int y1, int x2, int y2,Outcode outcode)
        //{

        //}

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

        private void FloodFillInit(int x,int y, Color initColor)
        {
            initColor = imagePreview.GetPixel(x, y);
            Stack<(int x, int y)> points = new Stack<(int x, int y)>();
            points.Push((x, y));

            while(points.Count != 0)
            {
                (int x, int y) point = points.Pop();
                if (!points.Contains((point.x, point.y - 1)) && imagePreview.GetPixel(point.x, point.y - 1) == initColor)
                    points.Push((point.x, point.y - 1));
                if (!points.Contains((point.x + 1, point.y)) && imagePreview.GetPixel(point.x + 1, point.y) == initColor)
                    points.Push((point.x + 1, point.y));
                if (!points.Contains((point.x, point.y + 1)) && imagePreview.GetPixel(point.x, point.y + 1) == initColor)
                    points.Push((point.x, point.y + 1));
                if (!points.Contains((point.x - 1, point.y)) && imagePreview.GetPixel(point.x - 1, point.y) == initColor)
                    points.Push((point.x - 1, point.y));

                
                imagePreview.SetPixel(point.x, point.y, colors[colorComboBox.SelectedIndex]);
            }

            Canvas.Image = imagePreview;
        }

        //private void FloodFill(int x, int y, Color initColor, Stack<(int x,int y)> points)
        //{
        //    if (imagePreview.GetPixel(x, y) != initColor)
        //        return;
        //    points.Push((x, y));
        //    if (!points.Contains((x    , y - 1)))
        //        FloodFill(x    , y - 1, initColor, points);
        //    if (!points.Contains((x + 1, y)))
        //        FloodFill(x + 1, y    , initColor, points);
        //    if (!points.Contains((x    , y + 1)))
        //        FloodFill(x    , y + 1, initColor, points);
        //    if (!points.Contains((x - 1, y)))
        //        FloodFill(x - 1, y    , initColor, points);
        //}

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

        private void rectangleButton_Click(object sender, EventArgs e)
        {
            mode = 6;
            this.Text = "Rectangle";
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            mode = 7;
            this.Text = "Select";
        }

        private void bucketButton_Click(object sender, EventArgs e)
        {
            mode = 8;
            this.Text = "Bucket";
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (!drawing)
                imagePreview = (Bitmap)image.Clone();
            switch (mode)
            {
                case 0:
                    // Iterate over all shapes to find one selected
                    for (int i = 0; i<shapes.Count;i++)
                    {
                        if (shapes[i].Check(e.X, e.Y))
                        {
                            lastIndexEdited = i;
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
                                case 'R':
                                    Rectangle tmpRectangle = new Rectangle(shapes[i] as Rectangle);
                                    LastEdited = tmpRectangle;
                                    shapes.RemoveAt(i);
                                    if (Math.Sqrt(Math.Pow(tmpRectangle.center.x - e.X, 2) + Math.Pow(tmpRectangle.center.y - e.Y, 2)) > 10)
                                        foreach (var point in tmpRectangle.points)
                                        {
                                            if (Math.Sqrt(Math.Pow(point.x - e.X, 2) + Math.Pow(point.y - e.Y, 2)) <= 10)
                                            {
                                                polygonIndexEdited = tmpRectangle.points.IndexOf(point);
                                                x1 = tmpRectangle.points[((polygonIndexEdited + 2) % 4)].x;
                                                y1 = tmpRectangle.points[((polygonIndexEdited + 2) % 4)].y;
                                                break;
                                            }
                                        }
                                    else
                                        move = true;
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
                case 6:
                    x1 = e.X;
                    y1 = e.Y;
                    drawing = true;
                    break;
                case 7:
                    x1 = e.X;
                    y1 = e.Y;
                    drawing = true;
                    break;
                case 8:
                    FloodFillInit(e.X, e.Y, imagePreview.GetPixel(e.X, e.Y));
                    image = (Bitmap)imagePreview.Clone();
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
                                case 'R':
                                    if (!move)
                                        DrawRectangle(e.X, e.Y);
                                    else
                                    {
                                        int width = Math.Abs((LastEdited as Rectangle).points[1].x - (LastEdited as Rectangle).points[0].x);
                                        int height = Math.Abs((LastEdited as Rectangle).points[2].y - (LastEdited as Rectangle).points[1].y);
                                        DrawRectangle(e.X - (width / 2), e.Y - (height / 2),
                                                        e.X + (width / 2), e.Y + (height / 2));
                                    }
                                    break;
                            }
                            break;
                    case 1:
                        DrawLine(x1, y1, e.X, e.Y);
                        break;
                    case 2:
                        DrawLine(x1, y1, e.X, e.Y);
                        shapes.Add(new Line(x1, y1, e.X, e.Y, Convert.ToInt32(thicknessBox.Text)));
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
                    case 6:
                        DrawRectangle(e.X, e.Y);
                        break;
                    case 7:
                        DrawRectangle(e.X, e.Y,Color.Purple);
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
                            case 'R':
                                drawing = false;
                                if(!move)
                                    shapes.Add(new Rectangle(x1, y1, e.X, e.Y));
                                else
                                {
                                    int width = Math.Abs((LastEdited as Rectangle).points[1].x - (LastEdited as Rectangle).points[0].x);
                                    int height = Math.Abs((LastEdited as Rectangle).points[2].y - (LastEdited as Rectangle).points[1].y);
                                    shapes.Add(new Rectangle(e.X - (width / 2), e.Y - (height / 2),
                                                                e.X + (width / 2), e.Y + (height / 2)));
                                }
                                break;
                        }
                    break;
                case 1:
                    drawing = false;
                    shapes.Add(new Line(x1, y1, e.X, e.Y,Convert.ToInt32(thicknessBox.Text)));
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
                case 6:
                    drawing = false;
                    shapes.Add(new Rectangle(x1, y1, e.X, e.Y));
                    image = (Bitmap)imagePreview.Clone();
                    break;
                case 7:
                    drawing = false;
                    selection = new Rectangle(x1, y1, e.X, e.Y);
                    image = (Bitmap)imagePreview.Clone();
                    Redraw();
                    break;
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                if(LastEdited != null)
                {
                    shapes.RemoveAt(lastIndexEdited);
                    LastEdited = null;
                    Redraw();
                }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Unnamed.txt";
            save.Filter = "Text File | *.txt";

            if (save.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                foreach (var shape in shapes)
                {
                    writer.WriteLine(shape.Save());
                }

                writer.Dispose();
                writer.Close();
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\txt";
            openFile.Title = "Browse Text Files Only";
            openFile.Filter = "Text Files Only (*.txt) | *.txt";
            openFile.DefaultExt = "txt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                shapes.Clear();
                var lines = File.ReadLines(openFile.FileName);
                foreach(var line in lines)
                {
                    string[] details = line.Split(';');
                    switch (line[0])
                    {
                        case 'L':
                            shapes.Add(new Line(Convert.ToInt32(details[2]), Convert.ToInt32(details[3]),
                                                Convert.ToInt32(details[4]), Convert.ToInt32(details[5]), 
                                                Convert.ToInt32(details[1])));
                            break;
                        case 'C':
                            shapes.Add(new Circle(Convert.ToInt32(details[1]), Convert.ToInt32(details[2]), Convert.ToDouble(details[3])));
                            break;
                        case 'P':
                            List<(int x, int y)> points = new List<(int x, int y)>();
                            for(int i = 1; i < details.Length; i += 2)
                                points.Add((Convert.ToInt32(details[i]), Convert.ToInt32(details[i + 1])));
                            shapes.Add(new Polygon(points));
                            break;
                    }
                }
                Redraw();
            }
        }
    }
}
