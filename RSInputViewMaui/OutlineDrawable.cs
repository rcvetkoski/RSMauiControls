namespace RSInputViewMaui
{
    public class OutlineDrawable : GraphicsDrawable
    {
        public float OutlinedBorderMargin { get; private set; } = 5;

        public OutlineDrawable(RSInputView inputView) : base(inputView)
        {
        }

        public override void SetPlaceholderMargin(double bottomMargin)
        {
            PlaceholderMargin = new Thickness(12, OutlinedBorderMargin, 12, OutlinedBorderMargin + bottomMargin);
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSizeFloating;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-InputView.Graphics.Height / 2 + (float)PlaceholderMargin.Top;


            base.StartFocusedAnimation();
        }

        public override void StartUnFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSize;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)(PlaceholderMargin.Top - PlaceholderMargin.Bottom) / 2;


            base.StartUnFocusedAnimation();
        }

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (InputView.IsFloating())
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + (float)PlaceholderMargin.Top;
                    currentPlaceholderSize = fontSizeFloating;
                }
                else
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;
                    currentPlaceholderY = (float)(PlaceholderMargin.Top - (PlaceholderMargin.Bottom - OutlinedBorderMargin)) / 2;
                    currentPlaceholderSize = fontSize;
                }
            }


            var borderColor = InputView.IsActive ? Colors.Blue : InputView.BorderColor;
            Color placeholderColor;

            if (!string.IsNullOrEmpty(InputView.Error))
                placeholderColor = Colors.Red;
            else if (InputView.IsActive)
                placeholderColor = Colors.Blue;
            else
                placeholderColor = InputView.PlaceholderColor;

            canvas.Antialias = true;
            canvas.StrokeSize = InputView.IsActive ? 2 : InputView.BorderThikness;
            canvas.StrokeColor = borderColor;
            canvas.FontColor = placeholderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;
            canvas.DrawString(InputView.Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right * 2, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            float size = InputView.IsFloating() ? canvas.GetStringSize(InputView.Placeholder, textFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
            PathF pathF = CreateEntryOutlinePath(0, (float)PlaceholderMargin.Top, dirtyRect.Width, dirtyRect.Height - (float)PlaceholderMargin.Top - ((float)PlaceholderMargin.Bottom - OutlinedBorderMargin), InputView.CornerRadius, size);
            canvas.DrawPath(pathF);


            // Error or Helper
            DrawMessage(canvas, dirtyRect);

            this.canvas = canvas;
        }

        private void DrawMessage(ICanvas canvas, RectF dirtyRect)
        {
            if (string.IsNullOrEmpty(InputView.Error) && string.IsNullOrEmpty(InputView.Helper))
                return;

            bool isError = !string.IsNullOrEmpty(InputView.Error);
            string message = isError ? InputView.Error : InputView.Helper;

            canvas.FontSize = fontSizeFloating;
            canvas.FontColor = isError ? Colors.Red : InputView.BorderColor;

            var messageSize = canvas.GetStringSize(message, textFont, fontSizeFloating, HorizontalAlignment.Left, VerticalAlignment.Center);
            var multiplier = Math.Floor(messageSize.Width / (dirtyRect.Width - PlaceholderMargin.Left - PlaceholderMargin.Right) + 1);

            canvas.DrawString(message,
                             (float)PlaceholderMargin.Left,
                             dirtyRect.Height / 2 + (float)(messageSize.Height * multiplier) / 2 - (float)PlaceholderMargin.Bottom + messageSpacing + OutlinedBorderMargin,
                             dirtyRect.Width - (float)PlaceholderMargin.Left - (float)PlaceholderMargin.Right,
                             dirtyRect.Height,
                             HorizontalAlignment.Left,
                             VerticalAlignment.Center,
                             TextFlow.ClipBounds);
        }

        public PathF CreateEntryOutlinePath(float x, float y, float width, float height, float cornerRadius, float gapWidth)
        {
            PathF path = new PathF();

            float right = x + width;
            float bottom = y + height;
            float margin = Math.Abs((float)(PlaceholderMargin.Left - cornerRadius));

            // Start at the gap on the top-left side
            path.MoveTo(x + cornerRadius + margin + gapWidth - borderGapSpacing / 2, y);

            path.LineTo(right - cornerRadius, y);
            path.QuadTo(right, y, right, y + cornerRadius);
            path.LineTo(right, bottom - cornerRadius);

            path.QuadTo(right, bottom, right - cornerRadius, bottom);
            path.LineTo(x + cornerRadius, bottom);

            path.QuadTo(x, bottom, x, bottom - cornerRadius);
            path.LineTo(x, y + cornerRadius);

            path.QuadTo(x, y, x + cornerRadius, y);
            path.LineTo(x + cornerRadius + margin - borderGapSpacing / 2, y);

            return path;
        }
    }
}
