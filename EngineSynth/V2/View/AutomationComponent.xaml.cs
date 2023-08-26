using EngineSynth.Common.Util;
using SkiaSharp;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = EngineSynth.Common.Util.Point;

namespace EngineSynth.V2.View
{
    /// <summary>
    /// Interaction logic for AutomationComponent.xaml
    /// </summary>
    public partial class AutomationComponent : UserControl
    {
        public static readonly DependencyProperty AutomationProperty = DependencyProperty.Register("Automation", typeof(Automation), typeof(AutomationComponent));

        private readonly SKPaint textPaint = new SKPaint() { Color = new SKColor(0xFF747D8C), Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKPaint backgroundPaint = new SKPaint() { Color = new SKColor(0xFFF1F2F6), Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKPaint headPaint = new SKPaint() { Color = new SKColor(0xFF2ED573), Style = SKPaintStyle.Fill, IsAntialias = true, StrokeWidth = 2 };
        private readonly SKFont interRegularFont = new SKFont(SKTypeface.FromStream(Application.GetResourceStream(new Uri("Resources/Fonts/Inter-Regular.ttf", UriKind.Relative)).Stream));

        private bool isMouseCaptured = false;

        public Automation Automation
        {
            get => (Automation)GetValue(AutomationProperty);
            set
            {
                SetValue(AutomationProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(AutomationProperty, value, value));
                CanvasView.InvalidateVisual();
            }
        }

        public AutomationComponent()
        {
            InitializeComponent();
            CanvasView.InvalidateVisual();
            CanvasView.MouseDown += CanvasView_MouseDown;
            CanvasView.MouseUp += CanvasView_MouseUp;
            CanvasView.MouseMove += CanvasView_MouseMove;
        }

        private Point pointToCanvas(Point point)
        {
            return new Point((float)(point.x / Automation.ScaleX * CanvasView.ActualWidth), (float)((Automation.ScaleY - point.y) / Automation.ScaleY * CanvasView.ActualHeight));
        }

        private Point canvasToPoint(Point point)
        {
            float x = (float)(point.x * Automation.ScaleX / CanvasView.ActualWidth);
            float y = (float)(Automation.ScaleY - point.y * Automation.ScaleY / CanvasView.ActualHeight);

            x = Math.Clamp(x, 0, Automation.ScaleX);
            y = Math.Clamp(y, 0, Automation.ScaleY);

            return new Point(x, y);
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            SKCanvas ctx = e.Surface.Canvas;
            float w = CanvasView.CanvasSize.Width;
            float h = CanvasView.CanvasSize.Height;

            ctx.Clear();
            ctx.DrawRoundRect(0, 0, w, h, 10, 10, backgroundPaint);
            if (Automation == null) return;

            ImmutableList<Point> points = Automation.GetPoints();
            if (points.Count > 0)
            {
                Point firstPoint = pointToCanvas(points.First());
                Point lastPoint = pointToCanvas(points.Last());

                ctx.DrawLine(0, firstPoint.y, firstPoint.x, firstPoint.y, headPaint);
                ctx.DrawLine(lastPoint.x, lastPoint.y, w, lastPoint.y, headPaint);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Point p0 = pointToCanvas(points[i]);
                    Point p1 = pointToCanvas(points[i + 1]);

                    ctx.DrawLine(p0.x, p0.y, p1.x, p1.y, headPaint);
                }

                foreach (var point in points)
                {
                    Point screenPoint = pointToCanvas(point);

                    ctx.DrawCircle(screenPoint.x, screenPoint.y, 8, headPaint);
                }
            }

            string text = Automation.Name;
            Span<ushort> span = new ushort[text.Length];
            interRegularFont.Size = h * .1f;
            interRegularFont.GetGlyphs(text, span);
            interRegularFont.MeasureText(span, out SKRect bounds);

            ctx.DrawText(text, w / 2 - bounds.Width / 2, h * .1f + bounds.Height, interRegularFont, textPaint);
        }

        private void CanvasView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseCaptured) return;
            System.Windows.Point position = e.GetPosition(this);

            Point p = canvasToPoint(new Point((float)position.X, (float)position.Y));
            point.x = p.x;
            point.y = p.y;

            Automation.Sort();
            CanvasView.InvalidateVisual();
        }

        private void CanvasView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseCaptured = false;
            Mouse.Capture(null);
        }

        private Point point;
        private long lastClickTime = 0;
        private void CanvasView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CanvasView.InvalidateVisual();
            System.Windows.Point position = e.GetPosition(this);

            foreach (var item in Automation.GetPoints())
            {
                Point canvasPoint = pointToCanvas(item);

                float dx = (float)(canvasPoint.x - position.X);
                float dy = (float)(canvasPoint.y - position.Y);

                if (Math.Sqrt(dx * dx + dy * dy) < 16)
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        Automation.RemovePoint(item);
                    }
                    else
                    {
                        point = item;
                        Mouse.Capture(CanvasView);
                        isMouseCaptured = true;
                    }
                    return;
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                long unixTimeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (unixTimeNow - lastClickTime < 500) // is double click
                {
                    Point p = canvasToPoint(new Point((float)position.X, (float)position.Y));

                    point = Automation.AddPoint(p.x, p.y);
                    Mouse.Capture(CanvasView);
                    isMouseCaptured = true;
                    lastClickTime = 0;
                }
                else
                {
                    lastClickTime = unixTimeNow;
                }
            }
        }
    }
}
