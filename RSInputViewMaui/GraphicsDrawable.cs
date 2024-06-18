using Microsoft.Maui.Graphics;

namespace RSInputViewMaui
{
    public abstract class GraphicsDrawable : IDrawable
    {
        public Microsoft.Maui.Graphics.Font TextFont;
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

                float maxWidth = (float)(InputView.Graphics.Width -
                                         PlaceholderMargin.Left -
                                         InputView.ContentMargin.Right -
                                         InputView.LeadingIconTotalWidth -
                                         InputView.TrailingIconTotalWidth -
                                         InputView.Content.MinimumWidthRequest);

                float width = Canvas != null ? GetCanvasStringSize(Canvas, InputView.Prefix.ToString(), TextFont, FontSize).Width + InputView.PrefixSpacing : 0;
                return width <= maxWidth ? width : maxWidth;
            }
        }

        protected float suffixWidth
        {
            get
            {
                if (InputView.Suffix == null)
                    return 0;

                float maxWidth = (float)(InputView.Graphics.Width -
                                         InputView.ContentMargin.Left - 
                                         PlaceholderMargin.Right - 
                                         InputView.LeadingIconTotalWidth - 
                                         InputView.TrailingIconTotalWidth -
                                         InputView.Content.MinimumWidthRequest);

                float width = Canvas != null ? GetCanvasStringSize(Canvas, InputView.Suffix.ToString(), TextFont, FontSize).Width + InputView.SuffixSpacing : 0;
                return width <= maxWidth ? width : maxWidth;
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
                TextFont = new Microsoft.Maui.Graphics.Font(fnt.Family, (int)fnt.Weight, styleType: (int)FontSlant.Default);

                // Font size
                FontSize = (float)textElement.FontSize;
            }
            else
            {
                // Font
                TextFont = new Microsoft.Maui.Graphics.Font();

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

        public SizeF GetCanvasStringSize(ICanvas canvas, string text, IFont textFont, float fontSize)
        {
            if (canvas == null)
                return SizeF.Zero;

            return canvas.GetStringSize(text, textFont, fontSize, HorizontalAlignment.Left, VerticalAlignment.Center);
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
            canvas.Font = TextFont;
            canvas.FontSize = currentPlaceholderSize;


            // Used to measure message label size and add margin to control accordingly
            if (this.Canvas == null)
            {
                this.Canvas = canvas;

                if (!string.IsNullOrEmpty(InputView.ErrorMessage) || !string.IsNullOrEmpty(InputView.HelperMessage) || !string.IsNullOrEmpty(InputView.characterCounterString))
                    InputView.SetBottomMessageMargin(InputView);
                else if (!string.IsNullOrEmpty(InputView.Prefix?.ToString()) || !string.IsNullOrEmpty(InputView.Suffix?.ToString()))
                    SetContentMargin(BorderMargin.Bottom);
            }

            // Clear icon
            if(InputView.IsClearIconVisible)
            {
                CreateClearIcon(dirtyRect.Width - PlaceholderMargin.Right - (float)InputView.IconWidthRequest,
                                dirtyRect.Height / 2 + (BorderMargin.Top - BorderMargin.Bottom) / 2 - (float)InputView.IconHeightRequest / 2,
                                (float)InputView.IconWidthRequest,
                                (float)InputView.IconHeightRequest,
                                canvas);
            }
            // Drop down icon
            else if (InputView.HasDropDownIcon && string.IsNullOrEmpty(InputView.TrailingIcon) && !InputView.IsClearIconVisible)
            {
                CreateDropDownIcon(dirtyRect.Width - PlaceholderMargin.Right - (float)InputView.IconWidthRequest,
                                   dirtyRect.Height / 2 + (BorderMargin.Top - BorderMargin.Bottom) / 2 - (float)InputView.IconHeightRequest / 2,
                                   (float)InputView.IconWidthRequest,
                                   (float)InputView.IconHeightRequest,
                                   canvas);
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
            float characterCountSize = InputView.CharacterCounter >= 0 ? GetCanvasStringSize(canvas, InputView.characterCounterString, TextFont, fontSizeFloating).Width + PlaceholderMargin.Right : 0;

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
            if (InputView.characterCounterString == null)
                return;

            float height = MessageMargin.Bottom >= messageSpacing ? MessageMargin.Bottom - messageSpacing : MessageMargin.Bottom;
            var size = GetCanvasStringSize(canvas, InputView.characterCounterString, TextFont, fontSizeFloating);

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

        protected void DrawPrefilValue(ICanvas canvas, RectF dirtyRect)
        {
            if (!InputView.IsPrefilValueVisible())
                return;

            canvas.FontSize = FontSize;
            canvas.DrawString(value: InputView.PrefilValue.ToString(),
                              x: PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth + prefixWidth,
                              y: (float)(InputView.ContentMargin.Top - InputView.ContentMargin.Bottom) / 2,
                              width: dirtyRect.Width - PlaceholderMargin.Left - PlaceholderMargin.Right - (float)InputView.LeadingIconTotalWidth - (float)InputView.ContentMargin.Right - (float)InputView.TrailingIconTotalWidth - prefixWidth - suffixWidth,
                              height: dirtyRect.Height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }

        protected void DrawPrefix(ICanvas canvas, RectF dirtyRect)
        {
            if (!InputView.IsFloating() || string.IsNullOrEmpty(InputView.Prefix?.ToString()))
                return;

            canvas.FontSize = FontSize;
            canvas.DrawString(value: InputView.Prefix.ToString(),
                              x: PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth,
                              y: (float)(InputView.ContentMargin.Top - InputView.ContentMargin.Bottom) / 2,
                              width: dirtyRect.Width - PlaceholderMargin.Left - PlaceholderMargin.Right - (float)InputView.LeadingIconTotalWidth - (float)InputView.ContentMargin.Right - (float)InputView.TrailingIconTotalWidth - (float)InputView.Content.MinimumWidthRequest,
                              height: dirtyRect.Height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }

        protected void DrawSuffix(ICanvas canvas, RectF dirtyRect)
        {
            if (!InputView.IsFloating() || string.IsNullOrEmpty(InputView.Suffix?.ToString()))
                return;

            canvas.FontSize = FontSize;
            canvas.DrawString(value: InputView.Suffix.ToString(),
                              x: PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth + (float)InputView.Content.MinimumWidthRequest + (float)InputView.ContentMargin.Left,
                              y: (float)(InputView.ContentMargin.Top - InputView.ContentMargin.Bottom) / 2,
                              width: dirtyRect.Width - PlaceholderMargin.Left - PlaceholderMargin.Right - (float)InputView.LeadingIconTotalWidth - (float)InputView.TrailingIconTotalWidth - (float)InputView.ContentMargin.Left - (float)InputView.Content.MinimumWidthRequest,
                              height: dirtyRect.Height,
                              HorizontalAlignment.Right,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }

        protected void CreateDropDownIcon(float x, float y, float width, float height, ICanvas canvas)
        {
            // Set the dimensions and location for the dropdown icon
            float iconWidth = width;
            float iconHeight = height;

            // Create the path
            PathF path = new PathF();

            // Define the points for the dropdown icon
            float startX = x + iconWidth / 4;
            float startY = y + iconHeight / 3;
            float endX = x + iconWidth - iconWidth / 4;
            float endY = y + iconHeight - iconHeight / 3;
            float centerX = x + iconWidth / 2;

            // Draw the dropdown icon
            path.MoveTo(startX, startY);
            path.LineTo(centerX, endY);
            path.LineTo(endX, startY);
            path.Close();

            canvas.StrokeColor = borderColor;
            canvas.FillColor = borderColor;
            canvas.FillPath(path);
            canvas.DrawPath(path);
        }

        protected void CreateClearIcon(float x, float y, float width, float height, ICanvas canvas)
        {
            // Set the dimensions and location for the dropdown icon
            float iconWidth = width;
            float iconHeight = height;

            // Create the path
            PathF path = new PathF();

            // Define the points for the dropdown icon
            float startX = x + iconWidth / 3;
            float startY = y + iconHeight / 3;
            float endX = x + iconWidth - iconWidth / 3;
            float endY = y + iconHeight - iconHeight / 3;

            // Draw the clear icon
            path.MoveTo(startX, startY);
            path.LineTo(endX, endY);
            path.MoveTo(endX, startY);
            path.LineTo(startX, endY);
            path.Close();

            canvas.StrokeColor = borderColor;
            canvas.FillColor = borderColor;
            canvas.FillPath(path);
            canvas.DrawPath(path);
        }
    }
}
