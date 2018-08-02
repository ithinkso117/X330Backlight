using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace X330Backlight
{

    internal class ValueToProcessConverter : IValueConverter
    {
        private const double Thickness = 20;
        private static readonly SolidColorBrush ProgressBarBackgroundBrush = new SolidColorBrush(Color.FromRgb(54, 54, 54));
        private static readonly SolidColorBrush ProgressBarValueBrush = Brushes.White;

        private Point _centerPoint;
        private double _radius;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double arg && !string.IsNullOrEmpty((string)parameter))
            {
                double diameter = double.Parse((string)parameter);
                _radius = diameter / 2;
                _centerPoint = new Point(_radius, _radius);
                return DrawBrush(arg, 15, _radius, Thickness);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }


        private Point GetPointByAngel(Point centerPoint, double r, double angel)
        {
            var p = new Point
            {
                X = Math.Sin(angel * Math.PI / 180) * r + centerPoint.X,
                Y = centerPoint.Y - Math.Cos(angel * Math.PI / 180) * r
            };

            return p;
        }


        private Geometry DrawingArcGeometry(Point bigFirstPoint, Point bigSecondPoint, Point smallFirstPoint,
            Point smallSecondPoint, double bigRadius, double smallRadius, bool isLargeArc)
        {
            var pathFigure = new PathFigure
            {
                IsClosed = true,
                StartPoint = bigFirstPoint
            };
            pathFigure.Segments.Add(
                new ArcSegment
                {
                    Point = bigSecondPoint,
                    IsLargeArc = isLargeArc,
                    Size = new Size(bigRadius, bigRadius),
                    SweepDirection = SweepDirection.Clockwise
                });
            pathFigure.Segments.Add(new LineSegment {Point = smallSecondPoint});
            pathFigure.Segments.Add(
                new ArcSegment
                {
                    Point = smallFirstPoint,
                    IsLargeArc = isLargeArc,
                    Size = new Size(smallRadius, smallRadius),
                    SweepDirection = SweepDirection.Counterclockwise
                });
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }


        private Geometry GetGeometry(double value, double maxValue, double radius, double thickness)
        {
            var percent = value / maxValue;
            var angel = percent * 360D;
            var isLargeArc = angel > 180;
            var bigR = radius;
            var smallR = radius - thickness;
            var firstpoint = GetPointByAngel(_centerPoint, bigR, 0);
            var secondpoint = GetPointByAngel(_centerPoint, bigR, angel);
            var thirdpoint = GetPointByAngel(_centerPoint, smallR, 0);
            var fourpoint = GetPointByAngel(_centerPoint, smallR, angel);
            return DrawingArcGeometry(firstpoint, secondpoint, thirdpoint, fourpoint, bigR, smallR, isLargeArc);
        }

        private void DrawingGeometry(DrawingContext drawingContext, double value, double maxValue, double radiusX,
            double radiusY, double thickness)
        {
            if (value >= maxValue)
            {
                drawingContext.DrawEllipse(null, new Pen(ProgressBarValueBrush, thickness), _centerPoint, radiusX-thickness/2, radiusY - thickness / 2);
            }
            else
            {
                drawingContext.DrawEllipse(null, new Pen(ProgressBarBackgroundBrush, thickness), _centerPoint, radiusX - thickness / 2, radiusY - thickness / 2);
                drawingContext.DrawGeometry(ProgressBarValueBrush, new Pen(), GetGeometry(value, maxValue, radiusX, thickness));
            }
            drawingContext.Close();
        }


        private Brush DrawBrush(double value, double maxValue, double radius, double thickness)
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Open();
            DrawingGeometry(drawingContext, value, maxValue, radius, radius, thickness);
            DrawingBrush brush = new DrawingBrush(drawingGroup);
            return brush;
        }

    }

    /// <summary>
    /// Interaction logic for CirculaOsdWindow.xaml
    /// </summary>
    public partial class CirculaOsdWindow
    {
        public CirculaOsdWindow()
        {
            InitializeComponent();
        }
    }
}
