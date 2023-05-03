namespace RSInputViewMaui
{
    public abstract class GraphicsDrawable : IDrawable
    {
        public Microsoft.Maui.Graphics.Font textFont;
        public float FontSize;
        protected float fontSizeFloating;
        protected float startPlaceholderSize;
        protected float endPlaceholderSize;
        protected float currentPlaceholderSize;
        protected float startX;
        protected float endX;
        protected float currentPlaceholderX;
        protected float startY;
        protected float endY;
        protected float currentPlaceholderY;
        /// <summary>
        /// Base left and righ margin, default value is 12
        /// </summary>
        protected float baseSidesMargin = 12;
        protected float borderGapSpacing = 8;
        public float messageSpacing = 4;
        public FloatThickness BorderMargin { get; protected set; }
        public FloatThickness PlaceholderMargin { get; protected set; }
        protected DateTime animationStartTime;
        protected const float AnimationDuration = 100; // milliseconds
        protected bool isAnimating = false;
        public ICanvas Canvas { get; protected set; }
        protected Color borderColor;
        public RSInputView InputView { get; set; }

        public FloatThickness MessageMargin
        {
            get
            {
                return new FloatThickness(baseSidesMargin, 0, baseSidesMargin, BorderMargin.Bottom);
            }
        }

        protected float prefixWidth
        {
            get
            {
                if (InputView.Prefix == null)
                    return 0;

                return Canvas != null ? GetCanvasStringSize(Canvas, InputView.Prefix.ToString()).Width : 0;
            }
        }

        protected float suffixWidth
        {
            get
            {
                if (InputView.Suffix == null)
                    return 0;

                return Canvas != null ? GetCanvasStringSize(Canvas, InputView.Suffix.ToString()).Width : 0;
            }
        }

        public GraphicsDrawable(RSInputView inputView)
        {
            InputView = inputView;

            if (InputView.Content != null)
            {
                // Font
                var textElement = (InputView.Content as Microsoft.Maui.Controls.Internals.IFontElement);
                var fnt = textElement.ToFont(textElement.FontSize);
                textFont = new Microsoft.Maui.Graphics.Font(fnt.Family, (int)fnt.Weight, styleType: (int)FontSlant.Default);

                // Font size
                FontSize = (float)textElement.FontSize;
            }
            else
            {
                // Font
                textFont = new Microsoft.Maui.Graphics.Font();

                // Font size
                FontSize = endPlaceholderSize;
            }
            fontSizeFloating = 11;
            currentPlaceholderSize = FontSize;

            SetIconMargin(0);
            SetContentMargin(0);
            SetBorderMargin(0);
            SetPlaceholderMargin(0);
        }

        public abstract void SetBorderMargin(float bottomMargin);

        public abstract void SetPlaceholderMargin(float bottomMargin);

        public abstract void SetContentMargin(double bottomMargin);

        public abstract void SetIconMargin(double bottomMargin);

        public SizeF GetCanvasStringSize(ICanvas canvas, string text)
        {
            if (canvas == null)
                return SizeF.Zero;

            return canvas.GetStringSize(text, textFont, fontSizeFloating, HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        public virtual void StartFocusedAnimation()
        {
            animationStartTime = DateTime.UtcNow;
            InputView.Graphics.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        public virtual void StartUnFocusedAnimation()
        {
            animationStartTime = DateTime.UtcNow;
            InputView.Graphics.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        private bool FocusedAnimation()
        {
            isAnimating = true;

            float progress = (float)(DateTime.UtcNow - animationStartTime).TotalMilliseconds / AnimationDuration;
            if (progress > 1)
                progress = 1;

            // Update placeholder position and size
            currentPlaceholderX = startX + (endX - startX) * progress;
            currentPlaceholderY = startY + (endY - startY) * progress;
            currentPlaceholderSize = startPlaceholderSize + (endPlaceholderSize - startPlaceholderSize) * progress;

            // Invalidate to redraw the control
            InputView.Graphics.Invalidate();

            // Stop the animation if progress is 1 (100%)
            if (progress < 1)
            {
                return true;
            }
            else
            {
                isAnimating = false;
                return false;
            }
        }

        public virtual void Draw(ICanvas canvas, RectF dirtyRect)
        {
            Color placeholderColor;

            if (InputView.ErrorMessageEnabled)
            {
                placeholderColor = Colors.Red;
                borderColor = Colors.Red;
            }
            else if (InputView.IsActive)
            {
                placeholderColor = Colors.Blue;
                borderColor = Colors.Blue;
            }
            else
            {
                placeholderColor = InputView.PlaceholderColor;
                borderColor = InputView.BorderColor;
            }

            canvas.StrokeSize = InputView.IsActive ? 2 : InputView.BorderThikness;
            canvas.Antialias = true;
            canvas.FontColor = placeholderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;


            // Used to measure message label size and add margin to control accordingly
            if (this.Canvas == null)
            {
                this.Canvas = canvas;

                if (!string.IsNullOrEmpty(InputView.ErrorMessage) || !string.IsNullOrEmpty(InputView.HelperMessage) || !string.IsNullOrEmpty(InputView.characterCounterString))
                    InputView.SetBottomMessageMargin(InputView);
                else if (!string.IsNullOrEmpty(InputView.Prefix?.ToString()) || !string.IsNullOrEmpty(InputView.Suffix?.ToString()))
                    SetContentMargin(InputView.ContentMargin.Bottom);
            }
        }

        protected void DrawMessage(ICanvas canvas, RectF dirtyRect)
        {
            if (!InputView.ErrorMessageEnabled && string.IsNullOrEmpty(InputView.HelperMessage))
                return;

            string message = InputView.ErrorMessageEnabled ? InputView.ErrorMessage : InputView.HelperMessage;

            canvas.FontSize = fontSizeFloating;
            canvas.FontColor = InputView.ErrorMessageEnabled ? Colors.Red : InputView.BorderColor;
            float height = MessageMargin.Bottom >= messageSpacing ? MessageMargin.Bottom - messageSpacing : MessageMargin.Bottom;
            float characterCountSize = InputView.CharacterCounter >= 0 ? GetCanvasStringSize(canvas, InputView.characterCounterString).Width + PlaceholderMargin.Right : 0;

            canvas.DrawString(message,
                              MessageMargin.Left,
                              dirtyRect.Height - MessageMargin.Bottom + messageSpacing,
                              dirtyRect.Width - MessageMargin.Left - MessageMargin.Right - characterCountSize,
                              height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Top,
                              TextFlow.ClipBounds);
        }

        protected void DrawCharacterCounter(ICanvas canvas, RectF dirtyRect)
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
                              VerticalAlignment.Top,
                              TextFlow.ClipBounds);
        }
    }
}
