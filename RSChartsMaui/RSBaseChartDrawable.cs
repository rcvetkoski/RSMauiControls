using Microsoft.Maui.Graphics;

namespace RSChartsMaui
{
    public abstract class RSBaseChartDrawable : IDrawable
    {
        protected RSBaseChart chart;
        protected float margin;
        protected float width;
        protected float height;
        protected float chartWidth;
        protected float chartHeight;
        protected int dataCount;
        protected float xInterval;
        protected float maxDataValue;
        protected float yScale;
        protected PathF shadowPath;
        protected PathF dataLinePath;
        public double ClipProgress;

        protected List<PointF> scaledPoints;
        protected List<PointF> interpolatedPoints;


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

        protected List<PointF> InterpolatePoints(IReadOnlyList<PointF> points)
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

        protected List<PointF> GeneratePoints(IList<float> yValues, float spacing = 100f)
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

        public virtual void Draw(ICanvas canvas, RectF dirtyRect)
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

            // Get data points
            if (chart.ChartData == null || chart.ChartData.Count == 0)
            {
                // Draw No Data
                canvas.FontSize = 24;
                canvas.FontColor = chart.AxisLabelColor;

                var stringSize = canvas.GetStringSize("Na Data !", Microsoft.Maui.Graphics.Font.Default, 24);

                canvas.DrawString("Na Data !", chartWidth / 2, chartHeight / 2 - stringSize.Height / 2, stringSize.Width, stringSize.Height, HorizontalAlignment.Left, VerticalAlignment.Center);
                return;
            }

            dataCount = chart.ChartData.Count;


            // Label width calculations


            var xLabelSize = canvas.GetStringSize(chart.ChartData.Max().ToString(), Microsoft.Maui.Graphics.Font.Default, 12);



            // Draw X-axis
            canvas.DrawLine(margin, height - margin, width - margin, height - margin);

            // Draw Y-axis
            canvas.DrawLine(margin, margin, margin, height - margin);


            // Calculate scaling factors
            xInterval = chartWidth / (dataCount - 1);
            maxDataValue = chart.ChartData.Max();

            // Font size and spacing for labels
            float availableHeight = height - (2 * margin); // Space for Y-axis
            float fontHeight = 12; // Height of font in pixels
            float minLabelSpacing = fontHeight * 2; // Minimum spacing between labels
            int maxSteps = (int)(availableHeight / minLabelSpacing); // Maximum number of steps

            // Calculate step size
            float stepSize = (float)Math.Ceiling(maxDataValue / maxSteps);
            maxDataValue += stepSize;
            stepSize = (float)Math.Ceiling(maxDataValue / maxSteps);

            // Extend max Y value by one step
            maxDataValue = (float)Math.Ceiling(maxDataValue / stepSize) * stepSize;

            yScale = chartHeight / maxDataValue;

            // Add axis labels and indicators
            canvas.FontSize = 12;
            canvas.FontColor = chart.AxisLabelColor;
            canvas.StrokeColor = chart.AxisLineColor;
            canvas.StrokeSize = 1;


            SetPoints(chart.ChartData);

            // Fix bug on ios need to add some moe space to bouds to draw string
            int labelMarge = 4;

            // X-axis labels and indicators
            int labelInterval = Math.Max(1, dataCount / 12); // Show ~12 labels
            for (int i = 0; i < dataCount; i += labelInterval)
            {
                float x = margin + i * xInterval;
                float y = height - margin;

                // Draw indicator
                canvas.DrawLine(x, y, x, y + 5);

                var stringSize = canvas.GetStringSize((i + 1).ToString(), Microsoft.Maui.Graphics.Font.Default, 12);
                var stringBounds = new RectF(new PointF(x, y), stringSize);

                // Draw label
                canvas.DrawString((i + 1).ToString(), x - stringSize.Width / 2 - labelMarge / 2, y + 12, stringSize.Width + labelMarge, stringSize.Height + labelMarge, HorizontalAlignment.Center, VerticalAlignment.Top);
            }


            // Y-axis labels and indicators
            for (float i = 0; i <= maxDataValue; i += stepSize)
            {
                float x = margin;
                float y = height - margin - (i * yScale);

                // Draw indicator
                canvas.DrawLine(x - 5, y, x, y);

                var stringSize = canvas.GetStringSize(i.ToString(), Microsoft.Maui.Graphics.Font.Default, 12);

                // Draw label
                canvas.DrawString(i.ToString(), 7, y - stringSize.Height / 2, stringSize.Width + labelMarge, stringSize.Height + labelMarge, HorizontalAlignment.Left, VerticalAlignment.Top);
            }
        }
    }
}
