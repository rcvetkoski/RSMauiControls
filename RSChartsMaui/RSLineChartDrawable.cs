namespace RSChartsMaui
{
    public class RSLineChartDrawable : RSBaseChartDrawable
    {
        public RSLineChartDrawable(RSLineChart chart)
        {
            this.chart = chart;
        }

        private void DrawDataPoints(ICanvas canvas, float x1, float y1)
        {
            canvas.FillColor = chart.DataPointColor;
            canvas.FillCircle(x1, y1, 3.5f); // Draw a circle with radius 3.5f
        }

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            base.Draw(canvas, dirtyRect);


            // Build path for the partial curve
            if (dataLinePath == null)
            {
                dataLinePath = new PathF();
                //dataLinePath.MoveTo(interpolatedPoints[0].X, interpolatedPoints[0].Y);
            }
            dataLinePath = new PathF();


            // Build path for shadow
            if (chart.ShowShadow && shadowPath == null)
            {
                shadowPath = new PathF();
                //shadowPath.MoveTo(interpolatedPoints[0].X, interpolatedPoints[0].Y);
            }
            shadowPath = new PathF();


            if (dataLinePath.Count <= 1)
            {
                if ((chart as RSLineChart).IsCurved)
                {
                    CreateSmoothCurve(scaledPoints);
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
            if (chart.ShowDataPoints)
            {
                for (int y = 0; y < scaledPoints.Count; y++)
                {
                    DrawDataPoints(canvas, scaledPoints[y].X, scaledPoints[y].Y);
                }
            }
        }

        private void CreateSmoothCurve(List<PointF> points)
        {
            if (points == null || points.Count < 2)
                return;

            // Start at the first point
            dataLinePath.MoveTo(points[0].X, points[0].Y);

            if (chart.ShowShadow)
            {
                shadowPath.MoveTo(points[0].X, points[0].Y);
            }

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

                if (chart.ShowShadow)
                {
                    shadowPath.CurveTo(cp1.X, cp1.Y, cp2.X, cp2.Y, p1.X, p1.Y);
                }
            }
        }
    }
}
