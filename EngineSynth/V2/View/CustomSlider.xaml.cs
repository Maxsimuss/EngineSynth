using SkiaSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EngineSynth.V2.View
{
    /// <summary>
    /// Interaction logic for CustomSlider.xaml
    /// </summary>
    public partial class CustomSlider : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(CustomSlider));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(float), typeof(CustomSlider), new PropertyMetadata(0f));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(float), typeof(CustomSlider), new PropertyMetadata(100f));
        public static readonly DependencyProperty TextFormatProperty = DependencyProperty.Register("TextFormat", typeof(string), typeof(CustomSlider), new PropertyMetadata("{0}"));

        private readonly SKPaint textPaint = new SKPaint() { Color = new SKColor(0xFFf1f2f6), Style = SKPaintStyle.Fill, IsAntialias = true, TextAlign = SKTextAlign.Center };
        private readonly SKPaint trackPaint = new SKPaint() { Color = new SKColor(0xFF2f3542), Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKPaint headPaint = new SKPaint() { Color = new SKColor(0xFF2ED573), Style = SKPaintStyle.Fill, IsAntialias = true };
        private readonly SKFont interRegularFont = new SKFont(SKTypeface.FromStream(Application.GetResourceStream(new Uri("Resources/Fonts/Inter-Regular.ttf", UriKind.Relative)).Stream));

        private float headMargin = 4;
        private bool isMouseCaptured = false;

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(ValueProperty, value, value));
                CanvasView.InvalidateVisual();
            }
        }

        public float Minimum
        {
            get => (float)GetValue(MinimumProperty);
            set
            {
                SetValue(MinimumProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(MinimumProperty, value, value));
            }
        }

        public float Maximum
        {
            get => (float)GetValue(MaximumProperty);
            set
            {
                SetValue(MaximumProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(MaximumProperty, value, value));
            }
        }

        public string TextFormat
        {
            get => (string)GetValue(TextFormatProperty);
            set
            {
                SetValue(TextFormatProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(TextFormatProperty, value, value));
            }
        }

        private float ValueNormalized
        {
            get => (Value - Minimum) / (Maximum - Minimum);
            set
            {
                Value = value * (Maximum - Minimum) + Minimum;
            }
        }

        private float headRadius => CanvasView.CanvasSize.Height / 2 - headMargin;
        private float headX
        {
            get => headRadius + headMargin + (CanvasView.CanvasSize.Width - headMargin * 2 - headRadius * 2) * ValueNormalized;
            set
            {
                ValueNormalized = Math.Clamp((value - headRadius - headMargin) / (CanvasView.CanvasSize.Width - headMargin * 2 - headRadius * 2), 0, 1);
            }
        }

        public CustomSlider()
        {
            InitializeComponent();
            CanvasView.InvalidateVisual();
            CanvasView.MouseDown += CanvasView_MouseDown;
            CanvasView.MouseUp += CanvasView_MouseUp;
            CanvasView.MouseMove += CanvasView_MouseMove;
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            SKCanvas ctx = e.Surface.Canvas;
            float w = CanvasView.CanvasSize.Width;
            float h = CanvasView.CanvasSize.Height;

            ctx.Clear();
            ctx.DrawRoundRect(0, 0, w, h, h / 2, h / 2, trackPaint);
            interRegularFont.Size = h / 2;

            string text = string.Format(TextFormat, Value);
            Span<ushort> span = new ushort[text.Length];
            interRegularFont.GetGlyphs(text, span);
            interRegularFont.MeasureText(span, out SKRect bounds, textPaint);

            ctx.DrawText(text, w / 2, h / 2 + bounds.Height / 2 / 1.5f, interRegularFont, textPaint);
            ctx.DrawCircle(headX, h / 2, headRadius, headPaint);
        }

        private void CanvasView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseCaptured) return;

            headX = (float)e.GetPosition(this).X;
        }

        private void CanvasView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseCaptured = false;
            Mouse.Capture(null);
        }

        private void CanvasView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseCaptured = true;
            Mouse.Capture(CanvasView);
        }
    }
}
