using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EngineSynth.View
{
    /// <summary>
    /// Interaction logic for Knob.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class Knob : UserControl
    {
        private bool init = true;

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                //Avoid clamping initial value before Min and Max are set up.
                if (init)
                {
                    init = false;
                    SetValue(ValueProperty, value);
                }
                else
                {
                    SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
                }
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(LabelText));
                OnPropertyChanged(nameof(Angle));
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Knob), new PropertyMetadata(0.0, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d).Value = (double)e.NewValue;
        }

        public double MarkerDiameter
        {
            get { return (double)GetValue(MarkerRadiusProperty) * 2; }
        }
        public double MarkerRadius
        {
            get { return (double)GetValue(MarkerRadiusProperty); }
            set { SetValue(MarkerRadiusProperty, value); OnPropertyChanged(nameof(MarkerDiameter)); OnPropertyChanged(nameof(MarkerRadius)); }
        }

        // Using a DependencyProperty as the backing store for MarkerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerRadiusProperty =
            DependencyProperty.Register("MarkerRadius", typeof(double), typeof(Knob), new PropertyMetadata(2.5, OnMarkerRadChanged));

        private static void OnMarkerRadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d).MarkerRadius = (double)e.NewValue;
        }

        public double InnerDiameter
        {
            get { return (double)GetValue(InnerRadiusProperty) * 2; }
        }
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); OnPropertyChanged(nameof(InnerDiameter)); OnPropertyChanged(nameof(InnerRadius)); }
        }

        // Using a DependencyProperty as the backing store for InnerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(Knob), new PropertyMetadata((double)40, OnInnerRadChanged));

        private static void OnInnerRadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d).InnerRadius = (double)e.NewValue;
        }


        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillBrushProperty =
            DependencyProperty.Register("FillBrush", typeof(Brush), typeof(Knob), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(103, 219, 134))));



        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(Knob), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(103, 173, 219))));



        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set
            {
                SetValue(MinimumProperty, value);
                OnPropertyChanged(nameof(Maximum));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(LabelText));
                OnPropertyChanged(nameof(Angle));
            }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(Knob), new PropertyMetadata(0.0, OnMinChanged));

        private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d).Minimum = (double)e.NewValue;
        }



        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set
            {
                SetValue(MaximumProperty, value);
                OnPropertyChanged(nameof(Maximum));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(LabelText));
                OnPropertyChanged(nameof(Angle));
            }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(Knob), new PropertyMetadata(1.0, OnMaxChanged));

        private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d).Maximum = (double)e.NewValue;
        }

        public string LabelText
        {
            get => string.Format(Label, Value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set { SetValue(LabelProperty, value); OnPropertyChanged(nameof(LabelText)); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(Knob), new PropertyMetadata("Value: {0:0.##}", OnLabelChanged));

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Knob)d).Label = (string)e.NewValue;
        }

        private double Normalized
        {
            get => (Value - Minimum) / (Maximum - Minimum);
            set => Value = Minimum + value * (Maximum - Minimum);
        }

        public double Angle
        {
            get => Normalized * 320 + 20;
        }

        public Knob()
        {
            InitializeComponent();
            MouseDown += Knob_MouseDown;
            MouseMove += Knob_MouseMove;
        }

        bool mouseDown = false;
        Point prevPos;

        private void Knob_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CaptureMouse();
                Point pos = e.GetPosition(null);
                Normalized += (pos.X - prevPos.X + -pos.Y + prevPos.Y) / (Keyboard.IsKeyDown(Key.LeftCtrl) ? 20000D : 1000D);

                prevPos = pos;
            }
            else
            {
                ReleaseMouseCapture();
                mouseDown = false;
            }
        }

        private void Knob_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CaptureMouse();

                mouseDown = true;
                prevPos = e.GetPosition(null);
            }
        }
    }
}
