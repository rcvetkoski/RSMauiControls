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
            // Define margins using Thickness
            Thickness margin = new Thickness(40, 10, 10, 40);

            // Calculate dimensions
            float width = (float)dirtyRect.Width;
            float height = (float)dirtyRect.Height;
            float chartWidth = width - (float)(margin.Left + margin.Right);
            float chartHeight = height - (float)(margin.Top + margin.Bottom);

            // Set up axis lines
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;

            // Draw X-axis
            canvas.DrawLine((float)margin.Left, height - (float)margin.Bottom, width - (float)margin.Right, height - (float)margin.Bottom);

            // Draw Y-axis
            canvas.DrawLine((float)margin.Left, (float)margin.Top, (float)margin.Left, height - (float)margin.Bottom);

            // Get data points
            var dataPoints = chart.ChartData;
            if (dataPoints == null || dataPoints.Count == 0) return;

            int dataCount = dataPoints.Count;

            // Calculate scaling factors
            float xInterval = chartWidth / (dataCount - 1);
            float maxDataValue = chart.MaxDataValue;
            float yScale = chartHeight / maxDataValue;

            // Draw data points as a line chart
            canvas.StrokeColor = Colors.Blue;
            canvas.StrokeSize = 3;
            for (int i = 0; i < dataCount - 1; i++)
            {
                float x1 = (float)margin.Left + i * xInterval;
                float y1 = height - (float)margin.Bottom - (dataPoints[i] * yScale);
                float x2 = (float)margin.Left + (i + 1) * xInterval;
                float y2 = height - (float)margin.Bottom - (dataPoints[i + 1] * yScale);

                canvas.DrawLine(x1, y1, x2, y2);
            }

            // Add axis labels and indicators
            canvas.FontSize = 12;
            canvas.FontColor = Colors.Gray;
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;

            // X-axis labels and indicators
            int labelInterval = Math.Max(1, dataCount / 12); // Show ~12 labels
            for (int i = 0; i < dataCount; i += labelInterval)
            {
                float x = (float)margin.Left + i * xInterval;
                float y = height - (float)margin.Bottom;

                canvas.DrawLine(x, y, x, y + 5);
                canvas.DrawString((i + 1).ToString(), x, y + 20, HorizontalAlignment.Center);
            }

            // Y-axis labels and indicators
            for (int i = 0; i <= maxDataValue; i += 10)
            {
                float x = (float)margin.Left;
                float y = height - (float)margin.Bottom - (i * yScale);

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
    }
}
