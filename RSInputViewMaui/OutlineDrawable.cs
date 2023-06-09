using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Drawing;

namespace RSInputViewMaui
{
    public class OutlineDrawable : GraphicsDrawable
    {
        /// <summary>
        /// Value = 5
        /// </summary>
        public float OutlinedBorderMargin { get; private set; } = 5;

        public OutlineDrawable(RSInputView inputView) : base(inputView)
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
            if (InputView.LeadingIcon != null)
                InputView.LeadingIconImage.Margin = new Thickness(baseSidesMargin, OutlinedBorderMargin + OutlinedBorderMargin / 2, 0, OutlinedBorderMargin / 2 + bottomMargin);

            if (InputView.TrailingIconImage != null)
                InputView.TrailingIconImage.Margin = new Thickness(0, OutlinedBorderMargin + OutlinedBorderMargin / 2, baseSidesMargin, OutlinedBorderMargin / 2 + bottomMargin);
        }

        public override void SetContentMargin(double bottomMargin)
        {
            InputView.ContentMargin = new Thickness(baseSidesMargin + InputView.LeadingIconTotalWidth + prefixWidth, OutlinedBorderMargin + OutlinedBorderMargin / 2, baseSidesMargin + InputView.TrailingIconTotalWidth + suffixWidth, OutlinedBorderMargin / 2 + bottomMargin);
            InputView.Content.Margin = InputView.ContentMargin;
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSizeFloating;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-InputView.Graphics.Height / 2 + BorderMargin.Top;


            base.StartFocusedAnimation();
        }

        public override void StartUnFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = FontSize;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (BorderMargin.Top - BorderMargin.Bottom) / 2;


            base.StartUnFocusedAnimation();
        }

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (InputView.IsFloating())
                {
                    currentPlaceholderX = PlaceholderMargin.Left;
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + BorderMargin.Top;
                    currentPlaceholderSize = fontSizeFloating;
                }
                else
                {
                    currentPlaceholderX = PlaceholderMargin.Left + (float)InputView.LeadingIconTotalWidth;
                    currentPlaceholderY = (BorderMargin.Top - BorderMargin.Bottom) / 2;
                    currentPlaceholderSize = FontSize;
                }
            }

            base.Draw(canvas, dirtyRect);

            // Draw Placeholder
            DrawPlaceholder(canvas, dirtyRect);

            // Draw Border
            DrawBorder(canvas, dirtyRect);

            // Error or Helper
            DrawMessage(canvas, dirtyRect);

            // Draw Counter
            DrawCharacterCounter(canvas, dirtyRect);

            // Draw Prefix
            DrawPrefix(canvas, dirtyRect);

            // Draw Suffix
            DrawSuffix(canvas, dirtyRect);

            //// Draw icon
            //CreateIcon(dirtyRect.Width - PlaceholderMargin.Right - (float)InputView.IconWidthRequest,
            //           dirtyRect.Height / 2 + (BorderMargin.Top - BorderMargin.Bottom) / 2 - (float)InputView.IconHeightRequest / 2,
            //           (float)InputView.IconWidthRequest,
            //           (float)InputView.IconHeightRequest,
            //           canvas);

            this.Canvas = canvas;
        }

        private void DrawBorder(ICanvas canvas, RectF dirtyRect)
        {
            // Calculate gap size
            float size = InputView.IsFloating() ? canvas.GetStringSize(InputView.Placeholder, TextFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;

            PathF pathF = CreateEntryOutlinePath(x : BorderMargin.Left + InputView.BorderThikness, 
                                                 y : BorderMargin.Top + InputView.BorderThikness, 
                                                 width : dirtyRect.Width - BorderMargin.Right - InputView.BorderThikness * 2, 
                                                 height : dirtyRect.Height - BorderMargin.Top - BorderMargin.Bottom - InputView.BorderThikness * 2,
                                                 cornerRadius : InputView.CornerRadius,
                                                 gapWidth :  size);

            canvas.StrokeColor = borderColor;
            canvas.DrawPath(pathF);
        }

        private void DrawPlaceholder(ICanvas canvas, RectF dirtyRect)
        {
            canvas.DrawString(value : InputView.Placeholder, 
                              x: currentPlaceholderX, 
                              y : currentPlaceholderY, 
                              width : dirtyRect.Width - (float)PlaceholderMargin.Left - (float)PlaceholderMargin.Right, 
                              height : dirtyRect.Height, 
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
            path.MoveTo(x + cornerRadius + margin + gapWidth - InputView.BorderThikness * 2 - borderGapSpacing / 2, y);

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
