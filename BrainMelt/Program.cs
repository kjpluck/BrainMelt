using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BrainMelt
{
    class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public double angle { get; set; }
        public double radius { get; set; }
        public int Rpm { get; set; }
        public override string ToString()
        {
            return $"[X={X}, Y={Y}, a={angle}, r={radius}, rpm={Rpm}]";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const int Width = 5;
            const int Height = 5;
            const int Size = 200;
            var fpp = new Point[Width,Height];

            var rnd = new Random();

            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                    fpp[x, y] = new Point { X = x*Size, Y = y*Size, radius = 1, angle = rnd.NextDouble()*2*Math.PI, Rpm = (int)(rnd.NextDouble()*5)+1 };

            var bmp = new Bitmap(Width * Size, Height * Size, PixelFormat.Format24bppRgb);
            var graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;


            for (var x = 0; x < Width - 1; x++)
                for (var y = 0; y < Height - 1; y++)
                {
                    if((x+ y)%2==0)
                        spiral(graphics, fpp[x, y], fpp[x + 1, y], fpp[x + 1, y + 1], fpp[x, y + 1]);
                    else
                        spiral(graphics, fpp[x, y], fpp[x, y + 1], fpp[x + 1, y + 1], fpp[x + 1, y]);
                }
            
            bmp.Save("foo.bmp");

        }

        static void spiral(Graphics g, params Point[] points)
        {
            List<Point> foo = new List<Point>(points);
            var index = 0;
            while (index < 30)
            {
                var thisPoint = MakePointF(foo[index]);
                var thatPoint = MakePointF(foo[index + 1]);
                foo.Add(MakeNewPoint(thisPoint, thatPoint));

                var p = new Pen(Color.FromArgb(255-index*6, 255, 255, 255), 2);
                g.DrawLine(p, thisPoint, thatPoint);
                index++;
            }
                        
        }

        private static Point MakeNewPoint(PointF thisPoint, PointF thatPoint)
        {
            var t = 0.1f;
            return new Point
            {
                X = ((1 - t) * thisPoint.X + t * thatPoint.X),
                Y = ((1 - t) * thisPoint.Y + t * thatPoint.Y)
            };
        }

        private static PointF MakePointF(Point point)
        {
            return new PointF { X = point.X, Y = point.Y };
        }
    }
}
