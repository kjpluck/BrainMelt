using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Vidja;

namespace BrainMelt
{
    class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public double angle { get; set; }
        public double radius { get; set; }
        public int direction { get; set; }
        public int RevPerDuration { get; set; }
        public override string ToString()
        {
            return $"[X={X}, Y={Y}, a={angle}, r={radius}, rpm={RevPerDuration}]";
        }
    }

    public class Program
    {
        const int BoardWidth = 5;
        const int BoardHeight = 5;
        const int Size = 200;

        static Point[,] board = new Point[BoardWidth,BoardHeight];
              

        static void Main(string[] args)
        {
            Vidja.Vidja.Generate(new BrainMelt());
        }
        
        
    }

    public class BrainMelt : IVidja
    {

        const int BoardWidth = 5;
        const int BoardHeight = 5;
        const int Size = 200;
        const int Radius = 25;

        public int Width { get; set; } = (BoardWidth * Size) - Size;
        public int Height { get; set; } = (BoardHeight * Size) - Size;
        public int Fps { get; set; } = 25;
        public double Duration { get; set; } = 15-(1/25);

        static Point[,] board = new Point[BoardWidth, BoardHeight];
        private double _t;
        private double _durationProportion;
        private Random _rnd;

        public BrainMelt()
        {
            _rnd = new Random();

            for (var x = 0; x < BoardWidth; x++)
                for (var y = 0; y < BoardHeight; y++)
                    board[x, y] = new Point
                    {
                        X = (x * Size) + Radius,
                        Y = (y * Size) + Radius,
                        radius = Radius,
                        angle = _rnd.NextDouble() * 2 * Math.PI,
                        RevPerDuration = (int)(_rnd.NextDouble() * 5) + 1,
                        direction = PickRandomDirection()
                    };

        }

        private int PickRandomDirection()
        {
            var x = _rnd.Next(2);
            if (x == 0) return -1;
            else return 1;
        }

        public Bitmap RenderFrame(double t)
        {
            _t = t;
            _durationProportion = t / (Duration+1/25);

            var bmp = new Bitmap(Width+2*Radius, Height+2*Radius, PixelFormat.Format24bppRgb);
            var graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;


            for (var x = 0; x < BoardWidth - 1; x++)
                for (var y = 0; y < BoardHeight - 1; y++)
                {
                    if ((x + y) % 2 == 0)
                        spiral(
                            graphics,
                            rotatePoint(board[x, y]),
                            rotatePoint(board[x + 1, y]),
                            rotatePoint(board[x + 1, y + 1]),
                            rotatePoint(board[x, y + 1])
                        );
                    else
                        spiral(
                            graphics,
                            rotatePoint(board[x, y]),
                            rotatePoint(board[x, y + 1]),
                            rotatePoint(board[x + 1, y + 1]),
                            rotatePoint(board[x + 1, y])
                        );
                }

            graphics.DrawString("@kevpluck", new Font("Arial", 16), new SolidBrush(Color.FromArgb(127, 255, 255, 255)), (Width / 2)-40, Height - 100);

            return bmp;
        }

        private Point rotatePoint(Point p)
        {
            var newAngle = (_durationProportion * p.RevPerDuration * 2 * Math.PI * p.direction) + (p.angle * p.direction);
            var x = (float)(p.radius * Math.Cos(newAngle));
            var y = (float)(p.radius * Math.Sin(newAngle));
            return new Point { X = p.X + x, Y = p.Y + y };
        }

        static void spiral(Graphics g, params Point[] points)
        {
            List<Point> foo = new List<Point>(points);

            g.DrawLine(new Pen(Color.White, 2), MakePointF(foo.Last()), MakePointF(foo.First()));

            var index = 0;
            while (index < 50)
            {
                var thisPoint = MakePointF(foo[index]);
                var thatPoint = MakePointF(foo[index + 1]);
                foo.Add(MakeNewPoint(thisPoint, thatPoint));

                var p = new Pen(Color.White, 2);
                g.DrawLine(p, thisPoint, thatPoint);
                index++;
            }

        }

        private static Point MakeNewPoint(PointF thisPoint, PointF thatPoint)
        {
            var d = 0.2f;
            return new Point
            {
                X = ((1 - d) * thisPoint.X + d * thatPoint.X),
                Y = ((1 - d) * thisPoint.Y + d * thatPoint.Y)
            };
        }

        private static PointF MakePointF(Point point)
        {
            
            return new PointF { X = point.X, Y = point.Y };
        }
    }
}
