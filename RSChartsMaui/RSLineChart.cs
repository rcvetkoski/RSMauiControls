namespace RSChartsMaui
{
    public class RSLineChartDrawable : IDrawable
    {
        private readonly RSLineChart chart;

        public RSLineChartDrawable(RSLineChart chart)
        {
            this.chart = chart;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Define margins and dimensions
            float margin = 40;
            float width = (float)dirtyRect.Width;
            float height = (float)dirtyRect.Height;
            float chartWidth = width - 2 * margin;
            float chartHeight = height - 2 * margin;

            // Set up axis lines
            canvas.StrokeColor = chart.AxisLineColor;
            canvas.StrokeSize = 1;

            // Draw X-axis
            canvas.DrawLine(margin, height - margin, width - margin, height - margin);

            // Draw Y-axis
            canvas.DrawLine(margin, margin, margin, height - margin);

            // Get data points
            var dataPoints = chart.ChartData;
            if (dataPoints == null || dataPoints.Count == 0) return;

            int dataCount = dataPoints.Count;

            // Calculate scaling factors
            float xInterval = chartWidth / (dataCount - 1);
            float maxDataValue = chart.MaxDataValue;
            float yScale = chartHeight / maxDataValue;

            if (chart.IsCurved)
            {
                // Draw smooth curves
                var path = new PathF();

                // Start the path at the first data point
                float startX = margin;
                float startY = height - margin - (dataPoints[0] * yScale);
                path.MoveTo(startX, startY);

                for (int i = 0; i < dataCount - 1; i++)
                {
                    // Get the current, next, and neighboring points
                    float x0 = i == 0 ? margin : margin + (i - 1) * xInterval;
                    float y0 = i == 0 ? height - margin - (dataPoints[0] * yScale) : height - margin - (dataPoints[i - 1] * yScale);

                    float x1 = margin + i * xInterval;
                    float y1 = height - margin - (dataPoints[i] * yScale);

                    float x2 = margin + (i + 1) * xInterval;
                    float y2 = height - margin - (dataPoints[i + 1] * yScale);

                    float x3 = i + 2 < dataCount ? margin + (i + 2) * xInterval : x2;
                    float y3 = i + 2 < dataCount ? height - margin - (dataPoints[i + 2] * yScale) : y2;

                    // Calculate control points
                    float controlX1 = x1 + (x2 - x0) / 6f;
                    float controlY1 = y1 + (y2 - y0) / 6f;

                    float controlX2 = x2 - (x3 - x1) / 6f;
                    float controlY2 = y2 - (y3 - y1) / 6f;

                    // Add a cubic curve segment to the path
                    path.CurveTo(controlX1, controlY1, controlX2, controlY2, x2, y2);
                }

                // Draw the smooth path
                canvas.StrokeColor = chart.LineColor;
                canvas.StrokeSize = 3;
                canvas.DrawPath(path);
            }
            else
            {
                // Draw straight lines
                canvas.StrokeColor = chart.LineColor;
                canvas.StrokeSize = 3;

                for (int i = 0; i < dataCount - 1; i++)
                {
                    float x1 = margin + i * xInterval;
                    float y1 = height - margin - (dataPoints[i] * yScale);
                    float x2 = margin + (i + 1) * xInterval;
                    float y2 = height - margin - (dataPoints[i + 1] * yScale);

                    canvas.DrawLine(x1, y1, x2, y2);
                }
            }

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

                canvas.DrawLine(x, y, x, y + 5);
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
    }
}
