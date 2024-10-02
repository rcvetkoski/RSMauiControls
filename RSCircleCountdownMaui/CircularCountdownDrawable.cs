using Microsoft.Maui.Graphics.Text;
using Font = Microsoft.Maui.Graphics.Font;

namespace RSCircleCountdownMaui
{
    public class CircularCountdownDrawable : IDrawable
    {
        public float Progress { get; set; } = 1.0f; // Starts at 100%
        public Color ProgressColor { get; set; }
        public Color CircleColor { get; set; }
        public float StrokeZize { get; set; }
        public bool IsTextVisible { get; set; }
        public float TextFontSize { get; set; }
        public TimeSpan Time { get; set; }


        public CircularCountdownDrawable(Color progressColor, Color circleColor, float strokeSize, bool isTextVisible)
        {
            ProgressColor = progressColor;
            CircleColor = circleColor;
            StrokeZize = strokeSize;
            IsTextVisible = isTextVisible;
            TextFontSize = 20;
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



            // Draw the time text in the middle
            if (!IsTextVisible)
                return;

            string timeText = Time.ToString(@"mm\:ss"); // Assuming Time is a TimeSpan or formatted time string
            canvas.FontColor = ProgressColor; // Set the font color
            canvas.FontSize = TextFontSize; // Adjust font size as needed

            // Set the font (default system font or specify a custom font if available)
            canvas.Font = new Font("Arial", (int)FontWeight.Bold ); // You can adjust the font family as needed

            // Measure the text size
            SizeF textSize = canvas.GetStringSize(timeText, new Font("Arial", (int)FontWeight.Bold), TextFontSize);

            // Calculate the position to center the text in the middle of the circle
            float textX = centerX;
            float textY = centerY + (textSize.Height / 2);

            // Draw the text centered in the circle
            canvas.DrawString(timeText, textX, textY, HorizontalAlignment.Center);
        }
    }
}
