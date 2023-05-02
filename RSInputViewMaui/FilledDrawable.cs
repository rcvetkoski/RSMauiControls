﻿namespace RSInputViewMaui
{
    public class FilledDrawable : GraphicsDrawable
    {
        private float PlaceholderYFloatingMargin = 16;
        public float filledBorderMargin { get; private set; } = 10;


        public FilledDrawable(RSInputView inputView) : base(inputView)
        {
        }

        public override void SetBorderMargin(float bottomMargin)
        {
            BorderMargin = new FloatThickness(0, 0, 0, bottomMargin);
        }

        public override void SetPlaceholderMargin(float bottomMargin)
        {
            PlaceholderMargin = new FloatThickness(baseSidesMargin, BorderMargin.Top, baseSidesMargin, bottomMargin);
        }

        public override void SetIconMargin(double bottomMargin)
        {
            if (InputView.LeadingIcon != null)
                InputView.LeadingIconImage.Margin = new Thickness(baseSidesMargin, 0, 0, bottomMargin);

            if (InputView.TrailingIconImage != null)
                InputView.TrailingIconImage.Margin = new Thickness(0, 0, baseSidesMargin, bottomMargin);
        }

        public override void SetContentMargin(double bottomMargin)
        {
            InputView.ContentMargin = new Thickness(baseSidesMargin + InputView.LeadingIconTotalWidth, filledBorderMargin, baseSidesMargin + InputView.TrailingIconTotalWidth, bottomMargin);
            InputView.Content.Margin = InputView.ContentMargin;
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSizeFloating;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;

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
            endX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = -(PlaceholderMargin.Bottom / 2);


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
                if (InputView.IsFloating())
                {
                    currentPlaceholderX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + PlaceholderYFloatingMargin;
                    currentPlaceholderSize = fontSizeFloating;
                }
                else
                {
                    currentPlaceholderX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;
                    currentPlaceholderY = -BorderMargin.Bottom / 2;
                    currentPlaceholderSize = fontSize;
                }
            }

            base.Draw(canvas, dirtyRect);

            // Draw Border
            DrawBorder(canvas, dirtyRect);

            // Draw Placeholder
            DrawPlaceholder(canvas, dirtyRect);

            // Error or Helper
            DrawMessage(canvas, dirtyRect);

            // Draw Counter
            DrawCharacterCounter(canvas, dirtyRect);    

            this.Canvas = canvas;
        }

        private void DrawBorder(ICanvas canvas, RectF dirtyRect)
        {
            // Border
            PathF pathF = CreateEntryOutlinePath(x: BorderMargin.Left + InputView.BorderThikness,
                                                 y: BorderMargin.Top + InputView.BorderThikness,
                                                 width: dirtyRect.Width - BorderMargin.Right - InputView.BorderThikness * 2,
                                                 height: dirtyRect.Height - BorderMargin.Top - BorderMargin.Bottom - InputView.BorderThikness * 2,
                                                 cornerRadius: InputView.CornerRadius);

            var fillColor = InputView.IsActive ? Colors.LightGray : InputView.FilledBorderColor;
            canvas.StrokeColor = fillColor;
            canvas.FillColor = fillColor;
            canvas.FillPath(pathF, WindingMode.NonZero);
            canvas.DrawPath(pathF);


            // Underline
            canvas.StrokeColor = borderColor;
            canvas.FillColor = borderColor;

            canvas.DrawLine(x1: BorderMargin.Left + InputView.BorderThikness / 2,
                            y1: dirtyRect.Height - BorderMargin.Bottom - InputView.BorderThikness,
                            x2: dirtyRect.Width - BorderMargin.Right - InputView.BorderThikness / 2,
                            y2: dirtyRect.Height - BorderMargin.Bottom - InputView.BorderThikness);
        }

        private void DrawPlaceholder(ICanvas canvas, RectF dirtyRect)
        {
            canvas.DrawString(value: InputView.Placeholder,
                              x: currentPlaceholderX,
                              y: currentPlaceholderY,
                              width: dirtyRect.Width - PlaceholderMargin.Left - PlaceholderMargin.Right,
                              height: dirtyRect.Height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }

        private void DrawMessage(ICanvas canvas, RectF dirtyRect)
        {
            if (string.IsNullOrEmpty(InputView.ErrorMessage) && string.IsNullOrEmpty(InputView.HelperMessage))
                return;

            string message = InputView.ErrorMessageEnabled ? InputView.ErrorMessage : InputView.HelperMessage;

            canvas.FontSize = fontSizeFloating;
            canvas.FontColor = InputView.ErrorMessageEnabled ? Colors.Red : InputView.BorderColor;
            float height = MessageMargin.Bottom >= messageSpacing ? MessageMargin.Bottom - messageSpacing : MessageMargin.Bottom;

            canvas.DrawString(message,
                              MessageMargin.Left,
                              dirtyRect.Height - MessageMargin.Bottom + messageSpacing,
                              dirtyRect.Width - MessageMargin.Left - MessageMargin.Right,
                              height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }

        private void DrawCharacterCounter(ICanvas canvas, RectF dirtyRect)
        {
            float height = MessageMargin.Bottom >= messageSpacing ? MessageMargin.Bottom - messageSpacing : MessageMargin.Bottom;
            var size = GetCanvasStringSize(canvas, InputView.characterCounterString);

            canvas.FontColor = InputView.ErrorMessageEnabled ? Colors.Red : InputView.BorderColor;
            canvas.FontSize = fontSizeFloating;
            canvas.DrawString(InputView.characterCounterString,
                              dirtyRect.Width - MessageMargin.Left - size.Width,
                              dirtyRect.Height - MessageMargin.Bottom + messageSpacing,
                              size.Width,
                              height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }
    }
}
