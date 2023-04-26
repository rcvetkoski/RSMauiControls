namespace RSInputViewMaui
{
    public class FilledDrawable : GraphicsDrawable
    {
        private float PlaceholderYFloatingMargin = 16;
        public float filledBorderMargin { get; private set; } = 10;


        public FilledDrawable(RSInputView inputView) : base(inputView)
        {
            if (InputView.Content == null)
                return;

            double bottomMargin = 0;
            if (!string.IsNullOrEmpty(InputView.Helper))
                bottomMargin = 13 + messageSpacing;

            PlaceholderMargin = new Thickness(12, 0, 12, bottomMargin);
            ContentMargin = new Thickness(PlaceholderMargin.Left, filledBorderMargin, PlaceholderMargin.Right, PlaceholderMargin.Bottom);
            InputView.Content.Margin = ContentMargin;
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
            endY = (float)-InputView.Graphics.Height / 2 + PlaceholderYFloatingMargin;


            base.StartFocusedAnimation();
        }

        public override void StartUnFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSize;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)(PlaceholderMargin.Top / 2 - PlaceholderMargin.Bottom) / 2;


            base.StartUnFocusedAnimation();
        }

        public PathF CreateEntryOutlinePath(float x, float y, float width, float height, float cornerRadius)
        {
            PathF path = new PathF();

            float right = x + width;
            float bottom = y + height;

            // Start at the top-left corner
            path.MoveTo(x + cornerRadius, y);

            path.LineTo(right - cornerRadius, y);
            path.QuadTo(right, y, right, y + cornerRadius);
            path.LineTo(right, bottom);

            path.LineTo(x, bottom);
            path.LineTo(x, y + cornerRadius);

            path.QuadTo(x, y, x + cornerRadius, y);
            path.Close();

            return path;
        }

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (IsFloating())
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + PlaceholderYFloatingMargin;
                    currentPlaceholderSize = fontSizeFloating;
                }
                else
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = (float)(PlaceholderMargin.Top / 2 - PlaceholderMargin.Bottom) / 2;
                    currentPlaceholderSize = fontSize;
                }
            }


            var borderColor = InputView.IsActive ? Colors.Blue : InputView.BorderColor;
            var fillColor = InputView.IsActive ? Colors.LightGray : InputView.FilledBorderColor;
            Color placeholderColor;

            if (!string.IsNullOrEmpty(InputView.Error))
                placeholderColor = Colors.Red;
            else if (InputView.IsActive)
                placeholderColor = Colors.Blue;
            else
                placeholderColor = InputView.PlaceholderColor;


            canvas.Antialias = true;
            canvas.StrokeSize = InputView.BorderThikness;
            canvas.StrokeColor = InputView.FilledBorderColor;
            canvas.FontColor = placeholderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;
            PathF pathF = CreateEntryOutlinePath(0, (float)PlaceholderMargin.Top, dirtyRect.Width, dirtyRect.Height - (float)PlaceholderMargin.Top - (float)PlaceholderMargin.Bottom, InputView.CornerRadius);
            canvas.DrawPath(pathF);
            canvas.FillColor = fillColor;
            canvas.FillPath(pathF, WindingMode.NonZero);
            canvas.DrawString(InputView.Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right * 2, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);


            canvas.StrokeColor = borderColor;
            canvas.StrokeSize = 1;
            canvas.FillColor = borderColor;
            canvas.DrawLine(0, dirtyRect.Height - (float)PlaceholderMargin.Bottom, dirtyRect.Width, dirtyRect.Height - (float)PlaceholderMargin.Bottom);


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
                    currentPlaceholderX,
                    dirtyRect.Height / 2 + (float)(messageSize.Height * multiplier) / 2 - (float)PlaceholderMargin.Bottom + messageSpacing,
                    dirtyRect.Width - (float)PlaceholderMargin.Right * 2, dirtyRect.Height,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Center,
                    TextFlow.ClipBounds);
        }
    }
}
