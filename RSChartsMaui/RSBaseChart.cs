namespace RSChartsMaui
{
    public abstract class RSBaseChart : GraphicsView
    {
        public double Progress { get; set; } = 0;

        public static readonly BindableProperty ChartDataProperty = BindableProperty.Create(
            nameof(ChartData),
            typeof(IList<float>),
            typeof(RSBaseChart),
            new List<float>(),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable == null || newValue == null)
                    return;

                (((RSBaseChart)bindable).Drawable as RSBaseChartDrawable).SetPoints(newValue as IList<float>);
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty MaxDataValueProperty = BindableProperty.Create(
            nameof(MaxDataValue),
            typeof(float),
            typeof(RSBaseChart),
            100f,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
            nameof(LineColor),
            typeof(Color),
            typeof(RSBaseChart),
            Colors.Blue,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty AxisLineColorProperty = BindableProperty.Create(
            nameof(AxisLineColor),
            typeof(Color),
            typeof(RSBaseChart),
            Colors.LightGray,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty AxisLabelColorProperty = BindableProperty.Create(
            nameof(AxisLabelColor),
            typeof(Color),
            typeof(RSBaseChart),
            Colors.Gray,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty ShowDataPointsProperty = BindableProperty.Create(
            nameof(ShowDataPoints),
            typeof(bool),
            typeof(RSBaseChart),
            true,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty DataPointColorProperty = BindableProperty.Create(
            nameof(DataPointColor),
            typeof(Color),
            typeof(RSBaseChart),
            Colors.Gray,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty ShowShadowProperty = BindableProperty.Create(
            nameof(ShowShadow),
            typeof(bool),
            typeof(RSBaseChart),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (((RSBaseChart)bindable).Drawable as RSBaseChartDrawable).ResetPaths();
                ((RSBaseChart)bindable).Invalidate();
            });

        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
            nameof(ShadowColor),
            typeof(Color),
            typeof(RSBaseChart),
            Colors.Blue,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSBaseChart)bindable).Invalidate();
            });

        public IList<float> ChartData
        {
            get => (IList<float>)GetValue(ChartDataProperty);
            set => SetValue(ChartDataProperty, value);
        }

        public float MaxDataValue
        {
            get => (float)GetValue(MaxDataValueProperty);
            set => SetValue(MaxDataValueProperty, value);
        }

        public Color LineColor
        {
            get => (Color)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public Color AxisLineColor
        {
            get => (Color)GetValue(AxisLineColorProperty);
            set => SetValue(AxisLineColorProperty, value);
        }

        public Color AxisLabelColor
        {
            get => (Color)GetValue(AxisLabelColorProperty);
            set => SetValue(AxisLabelColorProperty, value);
        }

        public bool ShowDataPoints
        {
            get => (bool)GetValue(ShowDataPointsProperty);
            set => SetValue(ShowDataPointsProperty, value);
        }

        public Color DataPointColor
        {
            get => (Color)GetValue(DataPointColorProperty);
            set => SetValue(DataPointColorProperty, value);
        }

        public bool ShowShadow
        {
            get => (bool)GetValue(ShowShadowProperty);
            set => SetValue(ShowShadowProperty, value);
        }

        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }

        // Duration in seconds
        public async void StartAnimation(double duration = 2)
        {
            RSBaseChartDrawable drawable = (Drawable as RSBaseChartDrawable);

            // Reset values
            Progress = 0;
            double increment = 1;

            // Cancel the previous timer if it is still running
            _animationTokenSource?.Cancel();
            _animationTokenSource = new CancellationTokenSource();
            var token = _animationTokenSource.Token;

            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                if (token.IsCancellationRequested)
                    return false; // Stop the timer if the token is canceled

                Progress = Progress + increment / (60 * duration);
                drawable.ClipProgress = Progress;
                Invalidate();

                return Progress < 1; // Stop the timer when progress reaches 1
            });
        }

        private CancellationTokenSource _animationTokenSource;
    }
}
