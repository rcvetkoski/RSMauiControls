namespace RSCircleCountdownMaui
{
    public class CircularCountdownDrawable : IDrawable
    {
        public float Progress { get; set; } = 1.0f; // Starts at 100%
        public Color ProgressColor { get; set; }
        public Color CircleColor { get; set; }
        public float StrokeZize { get; set; }


        public CircularCountdownDrawable(Color progressColor, Color circleColor, float strokeSize)
        {
            ProgressColor = progressColor;
            CircleColor = circleColor;
            StrokeZize = strokeSize;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float centerX = width / 2;
            float centerY = height / 2;
            float radius = Math.Min(width, height) / 2 * 0.8f; // Leave some padding

            float startAngle = 90; // Start at the top (12 o'clock)
            float sweepAngle = startAngle + (360 * -Progress); // Negative for clockwise

            // Draw background circle
            canvas.StrokeColor = CircleColor;
            canvas.StrokeSize = StrokeZize;
            canvas.Antialias = true;
            canvas.DrawCircle(centerX, centerY, radius);

            // Draw progress arc
            canvas.StrokeColor = ProgressColor;
            canvas.StrokeSize = StrokeZize;
            canvas.Antialias = true;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawArc(
                centerX - radius, centerY - radius, radius * 2, radius * 2,
                startAngle, sweepAngle, true, false);
        }
    }
}
