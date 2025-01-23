using System;
using System.Collections.Generic;

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
        private PathF shadowPath;
        public PathF dataLinePath;
        public double ClipProgress;

        List<PointF> scaledPoints;
        List<PointF> interpolatedPoints;

        public RSLineChartDrawable(RSLineChart chart)
        {
            this.chart = chart;
        }

        public void SetPoints(IList<float> points)
        {
            scaledPoints = GeneratePoints(points);
            interpolatedPoints = InterpolatePoints(scaledPoints);
        }

        public void ResetPaths()
        {
            dataLinePath = null;
            shadowPath = null;
        }

        private void DrawDataPoints(ICanvas canvas, float x1, float y1)
        {
            canvas.FillColor = chart.DataPointColor;
            canvas.FillCircle(x1, y1, 3.5f); // Draw a circle with radius 3.5f
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
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


            SetPoints(chart.ChartData);

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


            // Build path for the partial curve
            if(dataLinePath == null)
            {
                dataLinePath = new PathF();
                dataLinePath.MoveTo(interpolatedPoints[0].X, interpolatedPoints[0].Y);
            }


            // Build path for shadow
            if (chart.ShowShadow && shadowPath == null)
            {
                shadowPath = new PathF();
                shadowPath.MoveTo(interpolatedPoints[0].X, interpolatedPoints[0].Y);
            }

            if(dataLinePath.Count <= 1)
            {
                if(chart.IsCurved)
                {
                    for (int i = 0; i < interpolatedPoints.Count; i += 3)
                    {
                        dataLinePath.CurveTo(interpolatedPoints[i], interpolatedPoints[i + 1], interpolatedPoints[i + 2]);

                        if (chart.ShowShadow)
                            shadowPath.CurveTo(interpolatedPoints[i], interpolatedPoints[i + 1], interpolatedPoints[i + 2]);
                    }

                    //CreateSmoothCurve(scaledPoints);
                    //CreateCubicSpline(scaledPoints);
                }
                else
                {
                    for (int i = 0; i < scaledPoints.Count; i++)
                    {
                        dataLinePath.LineTo(scaledPoints[i]);

                        if (chart.ShowShadow)
                            shadowPath.LineTo(scaledPoints[i]);
                    }
                }
            }


            // Set up clipping region
            float clipWidth = (float)(ClipProgress * dirtyRect.Width);
            canvas.ClipRectangle(0, 0, clipWidth, dirtyRect.Height);


            // Data Line
            canvas.StrokeColor = chart.LineColor;
            canvas.StrokeSize = 3;
            canvas.DrawPath(dataLinePath);


            // Shadow
            // Close the shadow path to the X-axis
            if (chart.ShowShadow)
            {
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


            // Data Points
            if(chart.ShowDataPoints)
            {
                for (int y = 0; y < scaledPoints.Count; y++)
                {
                    DrawDataPoints(canvas, scaledPoints[y].X, scaledPoints[y].Y);
                }
            }
        }

        private void CreateCubicSpline(List<PointF> points)
        {
            if (points == null || points.Count < 2)
                return;

            // Solve for cubic spline coefficients
            var n = points.Count - 1;
            var a = points.Select(p => p.Y).ToArray();
            var b = new float[n];
            var d = new float[n];
            var h = new float[n];

            for (int i = 0; i < n; i++)
                h[i] = points[i + 1].X - points[i].X;

            var alpha = new float[n];
            for (int i = 1; i < n; i++)
                alpha[i] = (3 / h[i]) * (a[i + 1] - a[i]) - (3 / h[i - 1]) * (a[i] - a[i - 1]);

            var c = new float[n + 1];
            var l = new float[n + 1];
            var mu = new float[n + 1];
            var z = new float[n + 1];
            l[0] = 1;

            for (int i = 1; i < n; i++)
            {
                l[i] = 2 * (points[i + 1].X - points[i - 1].X) - h[i - 1] * mu[i - 1];
                mu[i] = h[i] / l[i];
                z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
            }

            l[n] = 1;

            for (int j = n - 1; j >= 0; j--)
            {
                c[j] = z[j] - mu[j] * c[j + 1];
                b[j] = (a[j + 1] - a[j]) / h[j] - h[j] * (c[j + 1] + 2 * c[j]) / 3;
                d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
            }

            // Build the spline
            dataLinePath.MoveTo(points[0].X, points[0].Y);

            for (int i = 0; i < n; i++)
            {
                for (float x = points[i].X; x < points[i + 1].X; x += 1)
                {
                    var dx = x - points[i].X;
                    var y = a[i] + b[i] * dx + c[i] * dx * dx + d[i] * dx * dx * dx;
                    dataLinePath.LineTo(x, y);
                }
            }
        }

        private void CreateSmoothCurve(List<PointF> points)
        {
            if (points == null || points.Count < 2)
                return;

            // Start at the first point
            dataLinePath.MoveTo(points[0].X, points[0].Y);

            for (int i = 0; i < points.Count - 1; i++)
            {
                // Current point
                var p0 = points[i];

                // Next point
                var p1 = points[i + 1];

                // Control points for smooth curve (adjust these as needed)
                var cp1 = new PointF((p0.X + p1.X) / 2, p0.Y);
                var cp2 = new PointF((p0.X + p1.X) / 2, p1.Y);

                // Add cubic Bezier curve segment
                dataLinePath.CurveTo(cp1.X, cp1.Y, cp2.X, cp2.Y, p1.X, p1.Y);
            }
        }

        public List<PointF> InterpolatePoints(IReadOnlyList<PointF> points)
        {
            List<PointF> list = new List<PointF>();

            // We’ll treat each pair (Pi, Pi+1) as a cubic segment
            // and compute C1, C2 from neighbors.
            for (int i = 0; i < points.Count - 1; i++)
            {
                // Current point
                PointF p0 = points[i];
                // Next point
                PointF p1 = points[i + 1];

                // Previous point (for i=0, use p0)
                PointF pm = (i - 1 >= 0) ? points[i - 1] : p0;
                // Next-next point (for i+2 >= Count, use p1)
                PointF pp = (i + 2 < points.Count) ? points[i + 2] : p1;

                // Catmull–Rom-like parameters
                float tension = 0.1f;
                // The factor is tension / 2 = 0.5 / 2 = 0.25 if you prefer that style.
                // Or equivalently  (1/6) if tension=0.5 in a standard CR formula.

                // You can adjust the factor to see which style you like better.
                float factor = (1f - tension) / 6f; // This matches the “1/6” formula at tension=0.5.

                // C1 = P_i + (P_{i+1} - P_{i-1}) * factor
                float c1x = p0.X + (p1.X - pm.X) * factor;
                float c1y = p0.Y + (p1.Y - pm.Y) * factor;

                // C2 = P_{i+1} - (P_{i+2} - P_{i}) * factor
                float c2x = p1.X - (pp.X - p0.X) * factor;
                float c2y = p1.Y - (pp.Y - p0.Y) * factor;


                list.Add(new PointF(c1x, c1y));
                list.Add(new PointF(c2x, c2y));
                list.Add(new PointF(p1.X, p1.Y));
            }

            return list;    
        }

        private List<PointF> GeneratePoints(IList<float> yValues, float spacing = 100f)
        {
            var pts = new List<PointF>();
            for (int i = 0; i < yValues.Count; i++)
            {
                float x = i * xInterval + margin;
                float y = height - margin - (yValues[i] * yScale);
                pts.Add(new PointF(x, y));
            }
            return pts;
        } 
    }

    public class RSLineChart : GraphicsView
    {
        public RSLineChart()
        {
            Drawable = new RSLineChartDrawable(this);
            progress = 0;
        }

        private double progress;

        public static readonly BindableProperty ChartDataProperty = BindableProperty.Create(
            nameof(ChartData),
            typeof(IList<float>),
            typeof(RSLineChart),
            new List<float>(),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable == null || newValue == null)
                    return;

                (((RSLineChart)bindable).Drawable as RSLineChartDrawable).SetPoints(newValue as IList<float>);
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
                (((RSLineChart)bindable).Drawable as RSLineChartDrawable).ResetPaths();
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
                (((RSLineChart)bindable).Drawable as RSLineChartDrawable).ResetPaths();
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

        // Duration in seconds
        public async void StartAnimation(double duration = 2)
        {
            RSLineChartDrawable drawable = (Drawable as RSLineChartDrawable);

            // Reset values
            progress = 0;
            double increment = 1;

            // Cancel the previous timer if it is still running
            _animationTokenSource?.Cancel();
            _animationTokenSource = new CancellationTokenSource();
            var token = _animationTokenSource.Token;

            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                if (token.IsCancellationRequested)
                    return false; // Stop the timer if the token is canceled

                progress = progress + increment / (60 * duration);
                drawable.ClipProgress = progress;
                Invalidate();

                return progress < 1; // Stop the timer when progress reaches 1
            });
        }

        private CancellationTokenSource _animationTokenSource;

    }
}
