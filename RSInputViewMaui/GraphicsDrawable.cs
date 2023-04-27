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
        protected float borderGapSpacing = 8;
        public float messageSpacing = 4;
        public Thickness PlaceholderMargin { get; protected set; }
        protected DateTime animationStartTime;
        protected const float AnimationDuration = 100; // milliseconds
        protected bool isAnimating = false;
        protected ICanvas canvas;
        public RSInputView InputView { get; set; }

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

            SetPlaceholderMargin(0);
        }

        public abstract void SetPlaceholderMargin(double bottomMargin);

        public SizeF GetCanvasStringSize(string text)
        {
            if (this.canvas == null)
                return SizeF.Zero;

            return this.canvas.GetStringSize(text, textFont, fontSizeFloating, HorizontalAlignment.Left, VerticalAlignment.Center);
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

        public abstract void Draw(ICanvas canvas, RectF dirtyRect);
    }
}
