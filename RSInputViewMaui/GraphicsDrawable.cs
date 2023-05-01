using Microsoft.Maui.Graphics;

namespace RSInputViewMaui
{
    public abstract class GraphicsDrawable : IDrawable
    {
        public Microsoft.Maui.Graphics.Font textFont;
        public float fontSize;
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
                fontSize = (float)textElement.FontSize;
            }
            else
            {
                // Font
                textFont = new Microsoft.Maui.Graphics.Font();

                // Font size
                fontSize = endPlaceholderSize;
            }
            fontSizeFloating = 11;
            currentPlaceholderSize = fontSize;

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

            if (!string.IsNullOrEmpty(InputView.Error))
                placeholderColor = Colors.Red;
            else if (InputView.IsActive)
                placeholderColor = Colors.Blue;
            else
                placeholderColor = InputView.PlaceholderColor;

            canvas.Antialias = true;
            canvas.StrokeSize = InputView.IsActive ? 2 : InputView.BorderThikness;
            canvas.FontColor = placeholderColor;
            borderColor = InputView.IsActive ? Colors.Blue : InputView.BorderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;

            // Used to measure message label size and add margin to control accordingly
            SetMessageMargin(canvas, dirtyRect.Width);

            this.Canvas = canvas;
        }

        private void SetMessageMargin(ICanvas canvas, float widthAvailable)
        {
            if (this.Canvas != null)
                return;

            if (string.IsNullOrEmpty(InputView.Error) && string.IsNullOrEmpty(InputView.Helper))
                return;

            string message = !string.IsNullOrEmpty(InputView.Error) ? InputView.Error : InputView.Helper;
            var size = GetCanvasStringSize(canvas, message);
            var multiplier = Math.Floor(size.Width / (widthAvailable - PlaceholderMargin.Left - PlaceholderMargin.Right) + 1);
            var bottomMarging = size.Width > 0 ? size.Height * multiplier + messageSpacing : 0;

            SetIconMargin(bottomMarging);
            SetContentMargin(bottomMarging);
            SetBorderMargin((float)bottomMarging);
            SetPlaceholderMargin((float)bottomMarging);
        }
    }
}
