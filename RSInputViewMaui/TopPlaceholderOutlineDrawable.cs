using Microsoft.Maui.Graphics;

namespace RSInputViewMaui
{
    public class TopPlaceholderOutlineDrawable : GraphicsDrawable
    {
        /// <summary>
        /// Value = 5
        /// </summary>
        public float OutlinedBorderMargin { get; private set; } = 5;

        private float placeholderSpacing = 5;

        public TopPlaceholderOutlineDrawable(RSInputView inputView) : base(inputView)
        {

        }

        public override void SetBorderMargin(float bottomMargin)
        {
            BorderMargin = new FloatThickness(0, OutlinedBorderMargin, 0, bottomMargin);
        }

        public override void SetPlaceholderMargin(float bottomMargin)
        {
            PlaceholderMargin = new FloatThickness(baseSidesMargin, BorderMargin.Top, baseSidesMargin, bottomMargin);
        }

        public override void SetIconMargin(double bottomMargin)
        {
            float placeholderTotalHeight = 0;
            if (Canvas != null && InputView.PlaceholderOnTop)
            {
                var placeholderSize = Canvas.GetStringSize(InputView.Placeholder, TextFont, FontSize, HorizontalAlignment.Left, VerticalAlignment.Center);
                placeholderTotalHeight = placeholderSize.Height / 2 + placeholderSpacing;
            }

            if (InputView.LeadingIcon != null)
                InputView.LeadingIconImage.Margin = new Thickness(baseSidesMargin, 
                                                                  OutlinedBorderMargin + OutlinedBorderMargin / 2 + placeholderTotalHeight,
                                                                  0,
                                                                  OutlinedBorderMargin / 2 + bottomMargin);

            if (InputView.TrailingIconImage != null)
                InputView.TrailingIconImage.Margin = new Thickness(0, 
                                                                   OutlinedBorderMargin + OutlinedBorderMargin / 2 + placeholderTotalHeight,
                                                                   baseSidesMargin,
                                                                   OutlinedBorderMargin / 2 + bottomMargin);
        }

        public override void SetContentMargin(double bottomMargin)
        {
            float placeholderTotalHeight = 0;
            if (Canvas != null && InputView.PlaceholderOnTop)
            {
                var placeholderSize = Canvas.GetStringSize(InputView.Placeholder, TextFont, FontSize, HorizontalAlignment.Left, VerticalAlignment.Center);
                placeholderTotalHeight = placeholderSize.Height / 2 + placeholderSpacing;
            }

            InputView.ContentMargin = new Thickness(baseSidesMargin + InputView.LeadingIconTotalWidth + prefixWidth,
                                                    OutlinedBorderMargin + OutlinedBorderMargin / 2 + placeholderTotalHeight,
                                                    baseSidesMargin + InputView.TrailingIconTotalWidth + suffixWidth,
                                                    OutlinedBorderMargin / 2 + bottomMargin);

            InputView.Content.Margin = InputView.ContentMargin;
        }


        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (InputView.PlaceholderOnTop)
            {
                currentPlaceholderX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;
                currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + BorderMargin.Top;
                currentPlaceholderSize = FontSize;
            }
            else
            {
                float placeholderHeight = Canvas != null ? Canvas.GetStringSize(InputView.Placeholder, TextFont, FontSize, HorizontalAlignment.Left, VerticalAlignment.Center).Height : 0f;

                currentPlaceholderX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;
                currentPlaceholderY = (BorderMargin.Top - BorderMargin.Bottom) / 2;
                currentPlaceholderSize = FontSize;
            }

            base.Draw(canvas, dirtyRect);

            // Draw Border
            DrawBorder(canvas, dirtyRect);

            // Clear icon
            CreateClearIcon(x: dirtyRect.Width - PlaceholderMargin.Right - (float)InputView.IconWidthRequest,
                            y: (float)InputView.Content.Bounds.Center.Y - (float)InputView.IconHeightRequest / 2,
                            width: (float)InputView.IconWidthRequest,
                            height: (float)InputView.IconHeightRequest,
                            canvas);

            // Drop down icon
            CreateDropDownIcon(dirtyRect.Width - PlaceholderMargin.Right - (float)InputView.IconWidthRequest,
                               y: (float)InputView.Content.Bounds.Center.Y - (float)InputView.IconHeightRequest / 2,
                               (float)InputView.IconWidthRequest,
                               (float)InputView.IconHeightRequest,
                               canvas);
            
            // Draw Placeholder
            DrawPlaceholder(canvas, dirtyRect);

            // Error or Helper
            DrawMessage(canvas, dirtyRect);

            // Draw Counter
            DrawCharacterCounter(canvas, dirtyRect);

            // Draw Prefix
            DrawPrefix(canvas, dirtyRect);

            // Draw PrefilValue
            DrawPrefilValue(canvas, dirtyRect);

            // Draw Suffix
            DrawSuffix(canvas, dirtyRect);
        }

        private void DrawBorder(ICanvas canvas, RectF dirtyRect)
        {
            // Placeholder size
            float placeholderTotalHeight = 0;
            if (InputView.PlaceholderOnTop)
            {
                var placeholderSize = canvas.GetStringSize(InputView.Placeholder, TextFont, FontSize, HorizontalAlignment.Left, VerticalAlignment.Center);
                placeholderTotalHeight = placeholderSize.Height / 2 + placeholderSpacing;
            }



            // Calculate gap size
            PathF pathF = CreateEntryOutlinePath(x: BorderMargin.Left + InputView.BorderThikness,
                                                 y: BorderMargin.Top + placeholderTotalHeight + InputView.BorderThikness,
                                                 width: dirtyRect.Width - BorderMargin.Right - InputView.BorderThikness * 2,
                                                 height: dirtyRect.Height - BorderMargin.Top - placeholderTotalHeight - BorderMargin.Bottom - InputView.BorderThikness * 2,
                                                 cornerRadius: InputView.CornerRadius,
                                                 gapWidth: 0);

            canvas.StrokeColor = borderColor;
            canvas.FillColor = InputView.FilledBorderColor;
            canvas.FillPath(pathF, WindingMode.NonZero);
            canvas.DrawPath(pathF);
        }

        private void DrawPlaceholder(ICanvas canvas, RectF dirtyRect)
        {
            if (!InputView.PlaceholderOnTop && (InputView.IsActive || InputView.ControlHasValue()))
                return;

            var size = canvas.GetStringSize(InputView.Placeholder, TextFont, FontSize, HorizontalAlignment.Left, VerticalAlignment.Center);

            canvas.DrawString(value: InputView.Placeholder,
                              x: currentPlaceholderX,
                              y: currentPlaceholderY,
                              width: dirtyRect.Width - (float)PlaceholderMargin.Left - (float)PlaceholderMargin.Right,
                              height: dirtyRect.Height,
                              HorizontalAlignment.Left,
                              VerticalAlignment.Center,
                              TextFlow.ClipBounds);
        }

        public PathF CreateEntryOutlinePath(float x, float y, float width, float height, float cornerRadius, float gapWidth)
        {
            PathF path = new PathF();

            float right = x + width;
            float bottom = y + height;

            // Start at the top-left corner with the initial move
            path.MoveTo(x + cornerRadius, y);

            // Top-right corner
            path.LineTo(right - cornerRadius, y);
            path.QuadTo(right, y, right, y + cornerRadius);

            // Bottom-right corner
            path.LineTo(right, bottom - cornerRadius);
            path.QuadTo(right, bottom, right - cornerRadius, bottom);

            // Bottom-left corner
            path.LineTo(x + cornerRadius, bottom);
            path.QuadTo(x, bottom, x, bottom - cornerRadius);

            // Top-left corner
            path.LineTo(x, y + cornerRadius);
            path.QuadTo(x, y, x + cornerRadius, y);

            return path;
        }
    }
}
