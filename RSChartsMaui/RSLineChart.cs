namespace RSChartsMaui
{
    public class RSLineChartDrawable : IDrawable
    {
        private readonly RSLineChart chart;
        private float margin;
        private float width;
        private float height;
        private float chartWidth;
        private float chartHeight;
        private int dataCount;
        private float xInterval;
        private float maxDataValue;
        private float yScale;
        private float startX;
        private float startY;
        float x0;
        float y0;
        float x1;
        float y1;
        float x2;
        float y2;
        float x3;
        float y3;
        float controlX1;
        float controlY1;
        float controlX2;
        float controlY2;
        private PathF shadowPath;
        private PathF dataLinePath;

        public RSLineChartDrawable(RSLineChart chart)
        {
            this.chart = chart;
        }

        private void DrawDataLine(ICanvas canvas, float startX, float startY, float controlX1, float controlY1, float controlX2, float controlY2, float x2, float y2)
        {
            // Create dataLinePath
            if(dataLinePath == null)
            {
                dataLinePath = new PathF();

                // Start the path at the first data point
                dataLinePath.MoveTo(startX, startY);
            }
            
            
            // Add a cubic curve segment to the path
            dataLinePath.CurveTo(controlX1, controlY1, controlX2, controlY2, x2, y2);

            // Draw the smooth path
            canvas.StrokeColor = chart.LineColor;
            canvas.StrokeSize = 3;

            canvas.DrawPath(dataLinePath);
        }

        private void DrawShadow(ICanvas canvas, float startX, float startY, float controlX1, float controlY1, float controlX2, float controlY2, float x2, float y2, bool closePath)
        {
            // Create shadowPath
            if(shadowPath == null)
            {
                shadowPath = new PathF();

                // Start the shadow at the first data point
                shadowPath.MoveTo(startX, startY);
            }

            // Add a cubic curve segment to the shadow path
            shadowPath.CurveTo(controlX1, controlY1, controlX2, controlY2, x2, y2);

            // Close the shadow path
            if (closePath)
            {
                // Close the shadow path to the X-axis
                shadowPath.LineTo(margin + (dataCount - 1) * xInterval, height - margin);
                shadowPath.LineTo(margin, height - margin);
                shadowPath.Close();

                // Use GradientPaint to fill the shadow with a vertical gradient effect
                var gradientPaint = new LinearGradientPaint
                {
                    StartPoint = new PointF(0, 0),
                    EndPoint = new PointF(0, 1),
                    GradientStops = new[]
                    {
                        new PaintGradientStop(0.0f, chart.ShadowColor.WithAlpha(0.8f)), // Fully visible at the top
                        new PaintGradientStop(1.0f, chart.ShadowColor.WithAlpha(0.0f))  // Transparent at the bottom
                    }
                };

                // Save the canvas state before applying gradient paint
                canvas.SaveState();

                canvas.SetFillPaint(gradientPaint, shadowPath.Bounds);
                canvas.FillPath(shadowPath);

                // Restore the canvas
                canvas.RestoreState();
            }
        }

        private void DrawDataPoints(ICanvas canvas, float x1, float y1)
        {
            canvas.FillColor = chart.DataPointColor;
            canvas.FillCircle(x1, y1, 3.5f); // Draw a circle with radius 3.5f
        }


        public async void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Define margins and dimensions
            margin = 40;
            width = (float)dirtyRect.Width;
            height = (float)dirtyRect.Height;
            chartWidth = width - 2 * margin;
            chartHeight = height - 2 * margin;

            // Set up axis lines
            canvas.StrokeColor = chart.AxisLineColor;
            canvas.StrokeSize = 1;

            // Draw X-axis
            canvas.DrawLine(margin, height - margin, width - margin, height - margin);

            // Draw Y-axis
            canvas.DrawLine(margin, margin, margin, height - margin);

            // Get data points
            if (chart.ChartData == null || chart.ChartData.Count == 0) return;

            dataCount = chart.ChartData.Count;

            // Calculate scaling factors
            xInterval = chartWidth / (dataCount - 1);
            maxDataValue = chart.MaxDataValue;
            yScale = chartHeight / maxDataValue;

            // Add axis labels and indicators
            canvas.FontSize = 12;
            canvas.FontColor = chart.AxisLabelColor;
            canvas.StrokeColor = chart.AxisLineColor;
            canvas.StrokeSize = 1;

            // X-axis labels and indicators
            int labelInterval = Math.Max(1, dataCount / 12); // Show ~12 labels
            for (int i = 0; i < dataCount; i += labelInterval)
            {
                float x = margin + i * xInterval;
                float y = height - margin;

                // Draw indicator
                canvas.DrawLine(x, y, x, y + 5);

                // Draw label
                canvas.DrawString((i + 1).ToString(), x, y + 20, HorizontalAlignment.Center);
            }

            // Y-axis labels and indicators
            for (int i = 0; i <= maxDataValue; i += 10)
            {
                float x = margin;
                float y = height - margin - (i * yScale);

                // Draw indicator
                canvas.DrawLine(x - 5, y, x, y);

                // Draw label
                canvas.DrawString(i.ToString(), x - 20, y + 5, HorizontalAlignment.Center);
            }

            

            // Start the path at the first data point
            startX = margin;
            startY = height - margin - (chart.ChartData[0] * yScale);

            if (chart.IsCurved)
            {
                for (int i = 0; i < dataCount - 1; i++)
                {
                    // Get the current, next, and neighboring points
                    x0 = i == 0 ? margin : margin + (i - 1) * xInterval;
                    y0 = i == 0 ? height - margin - (chart.ChartData[0] * yScale) : height - margin - (chart.ChartData[i - 1] * yScale);

                    x1 = margin + i * xInterval;
                    y1 = height - margin - (chart.ChartData[i] * yScale);

                    x2 = margin + (i + 1) * xInterval;
                    y2 = height - margin - (chart.ChartData[i + 1] * yScale);

                    x3 = i + 2 < dataCount ? margin + (i + 2) * xInterval : x2;
                    y3 = i + 2 < dataCount ? height - margin - (chart.ChartData[i + 2] * yScale) : y2;

                    // Calculate control points
                    controlX1 = x1 + (x2 - x0) / 6f;
                    controlY1 = y1 + (y2 - y0) / 6f;

                    controlX2 = x2 - (x3 - x1) / 6f;
                    controlY2 = y2 - (y3 - y1) / 6f;

                    // Data Line
                    DrawDataLine(canvas, startX, startY, controlX1, controlY1, controlX2, controlY2, x2, y2);

                    // Shadow
                    if (chart.ShowShadow)
                        DrawShadow(canvas, startX, startY, controlX1, controlY1, controlX2, controlY2, x2, y2, (i == (dataCount - 2)));

                    // Data Points
                    if (chart.ShowDataPoints)
                        DrawDataPoints(canvas, x1, y1);
                }

                dataLinePath = null;
            }
        }
    }

    public class RSLineChart : GraphicsView
    {
        public RSLineChart()
        {
            Drawable = new RSLineChartDrawable(this);
        }

        public static readonly BindableProperty ChartDataProperty = BindableProperty.Create(
            nameof(ChartData),
            typeof(IList<float>),
            typeof(RSLineChart),
            new List<float>(),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty MaxDataValueProperty = BindableProperty.Create(
            nameof(MaxDataValue),
            typeof(float),
            typeof(RSLineChart),
            100f,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty IsCurvedProperty = BindableProperty.Create(
            nameof(IsCurved),
            typeof(bool),
            typeof(RSLineChart),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
            nameof(LineColor),
            typeof(Color),
            typeof(RSLineChart),
            Colors.Blue,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty AxisLineColorProperty = BindableProperty.Create(
            nameof(AxisLineColor),
            typeof(Color),
            typeof(RSLineChart),
            Colors.LightGray,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty AxisLabelColorProperty = BindableProperty.Create(
            nameof(AxisLabelColor),
            typeof(Color),
            typeof(RSLineChart),
            Colors.Gray,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty ShowDataPointsProperty = BindableProperty.Create(
            nameof(ShowDataPoints),
            typeof(bool),
            typeof(RSLineChart),
            true,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty DataPointColorProperty = BindableProperty.Create(
            nameof(DataPointColor),
            typeof(Color),
            typeof(RSLineChart),
            Colors.Gray,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty ShowShadowProperty = BindableProperty.Create(
            nameof(ShowShadow),
            typeof(bool),
            typeof(RSLineChart),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
            });

        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
            nameof(ShadowColor),
            typeof(Color),
            typeof(RSLineChart),
            Colors.Blue,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((RSLineChart)bindable).Invalidate();
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

        public bool IsCurved
        {
            get => (bool)GetValue(IsCurvedProperty);
            set => SetValue(IsCurvedProperty, value);
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
    }
}
