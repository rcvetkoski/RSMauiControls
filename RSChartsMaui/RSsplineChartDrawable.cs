namespace RSChartsMaui
{
    public class RSsplineChartDrawable : RSBaseChartDrawable
    {
        public RSsplineChartDrawable(RSsplineChart chart) 
        {
            this.chart = chart;
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
                //for (int i = 0; i < interpolatedPoints.Count; i += 3)
                //{
                //    dataLinePath.CurveTo(interpolatedPoints[i], interpolatedPoints[i + 1], interpolatedPoints[i + 2]);

                //    if (chart.ShowShadow)
                //        shadowPath.CurveTo(interpolatedPoints[i], interpolatedPoints[i + 1], interpolatedPoints[i + 2]);
                //}

                CreateCubicSpline(scaledPoints);
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

            // Build shadow if enabled
            if (chart.ShowShadow)
            {
                shadowPath.MoveTo(points[0].X, points[0].Y);
            }

            for (int i = 0; i < n; i++)
            {
                for (float x = points[i].X; x < points[i + 1].X; x += 1)
                {
                    var dx = x - points[i].X;
                    var y = a[i] + b[i] * dx + c[i] * dx * dx + d[i] * dx * dx * dx;
                    dataLinePath.LineTo(x, y);

                    if (chart.ShowShadow)
                    {
                        shadowPath.LineTo(x, y);
                    }
                }
            }
        }
    }
}
