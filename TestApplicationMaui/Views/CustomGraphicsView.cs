namespace TestApplicationMaui.Views
{
    [ContentProperty("Content")]
    public class RSInputView : Grid, IDisposable
    {
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(RSInputView), default, propertyChanged: ContentChanged);
        public View Content
        {
            get { return (View)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        private static void ContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).SetContent();
        }


        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(RSInputView), default, propertyChanged: PlaceholderChanged);
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        private static void PlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Placeholder = newValue.ToString();
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(RSInputView), default, propertyChanged: PlaceholderColorChanged);
        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
        private static void PlaceholderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RSInputView), default, propertyChanged: BorderColorChanged);
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        private static void BorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public CustomGraphicsView Graphics { get; set; }



        public RSInputView() 
        {
            // Main Grid
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            Graphics = new CustomGraphicsView();
            this.Add(Graphics, 0, 0);
        }

        private void SetContent()
        {
            this.Add(Content, 0, 0);
            Content.Focused += Content_Focused;
            Content.Unfocused += Content_Unfocused;

            //if (Content is Entry)
            //    (Content as Entry).PlaceholderColor = Colors.Transparent;
        }

        private void Content_Focused(object sender, FocusEventArgs e)
        {
            if(CheckIfShouldAnimate())
                Graphics.StartFocusedAnimation();
        }

        private void Content_Unfocused(object sender, FocusEventArgs e)
        {
            if (CheckIfShouldAnimate())
                Graphics.StartUnFocusedAnimation();
        }

        private bool CheckIfShouldAnimate()
        {
            if (Content is Entry)
            {
                if (!string.IsNullOrEmpty((Content as Entry).Text))
                    return false;
            }
            else if (Content is Picker)
            {
                if ((Content as Picker).SelectedItem != null)
                    return false;
            }
            else if (Content is SearchBar)
            {
                if (!string.IsNullOrEmpty((Content as SearchBar).Text))
                    return false;
            }
            else if (Content is DatePicker)
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            Content.Focused -= Content_Focused;
            Content.Unfocused -= Content_Unfocused;
        }
    }

    public class CustomGraphicsView : GraphicsView
    {
        private const double InitialPlaceholderSize = 15;
        private const double FinalPlaceholderSize = 12;
        private const double AnimationDuration = 200; // milliseconds
        public double currentPlaceholderX;
        public double currentPlaceholderY;
        private double startX = 0;
        private double endX = 0;
        private double startY = 0;
        private double endY = 0;

        public double currentPlaceholderSize = 14;
        private DateTime _animationStartTime;
        public bool GapVisible { get; set; }
        public string Placeholder { get; set; }


        public CustomGraphicsView() 
        {
            Drawable = new GraphicsDrawable(this);
        }

        public bool FocusedAnimation()
        {
            double progress = (double)(DateTime.UtcNow - _animationStartTime).TotalMilliseconds / AnimationDuration;
            if (progress > 1)
                progress = 1;

            // Update placeholder position and size
            currentPlaceholderX = startX + (endX - startX) * progress;
            currentPlaceholderY = startY + (endY - startY) * progress;
            currentPlaceholderSize = InitialPlaceholderSize + (FinalPlaceholderSize - InitialPlaceholderSize) * progress;

            // Invalidate to redraw the control
            this.Invalidate();

            // Stop the animation if progress is 1 (100%)
            return progress < 1;
        }

        public bool UnfocusedAnimation()
        {
            double progress = (double)(DateTime.UtcNow - _animationStartTime).TotalMilliseconds / AnimationDuration;
            if (progress > 1)
                progress = 1;

            // Update placeholder position and size
            currentPlaceholderX = startX + (endX - startX) * progress;
            currentPlaceholderY = startY + (endY - startY) * progress;
            currentPlaceholderSize = InitialPlaceholderSize + (FinalPlaceholderSize - InitialPlaceholderSize) * progress;

            // Invalidate to redraw the control
            this.Invalidate();

            // Stop the animation if progress is 1 (100%)
            return progress < 1;
        }

        public void StartFocusedAnimation()
        {
            //var animation = new Animation(v => { currentPlaceholderY = v; this.Invalidate(); }, currentPlaceholderY, -10);
            //animation.Commit(this, "lol");

            startX = 0;
            endX = 12;
            startY = 0;
            endY = -this.Height / 2;
            GapVisible = true;
            _animationStartTime = DateTime.UtcNow;
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        public void StartUnFocusedAnimation()
        {
            startX = currentPlaceholderX;
            endX = 0;
            startY = currentPlaceholderY;
            endY = 0;
            GapVisible = false;
            _animationStartTime = DateTime.UtcNow;
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), UnfocusedAnimation);
        }
    }

    public class GraphicsDrawable : IDrawable
    {
        public CustomGraphicsView CustomGraphicsView { get; set; } 

        public GraphicsDrawable(CustomGraphicsView CustomGraphicsView) 
        {
            this.CustomGraphicsView = CustomGraphicsView;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeSize = 1;
            canvas.StrokeColor = Colors.White;
            canvas.Antialias = true;
            canvas.FontColor = Colors.LightGray;
            canvas.FontSize = (float)CustomGraphicsView.currentPlaceholderSize;
            string text = CustomGraphicsView.Placeholder;
            canvas.DrawString(text, (float)CustomGraphicsView.currentPlaceholderX, (float)CustomGraphicsView.currentPlaceholderY, dirtyRect.Width, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);

            float size = CustomGraphicsView.GapVisible ? canvas.GetStringSize(text, Microsoft.Maui.Graphics.Font.Default, (float)CustomGraphicsView.currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width : 0;
            PathF pathF = CreateEntryOutlinePath(0, 0, dirtyRect.Width, dirtyRect.Height, 12, size);
            canvas.DrawPath(pathF);
        }

        public PathF CreateEntryOutlinePath(float x, float y, float width, float height, float cornerRadius, float gapWidth)
        {
            PathF path = new PathF();

            float right = x + width;
            float bottom = y + height;

            // Start at the gap on the top-left side
            path.MoveTo(x + cornerRadius + gapWidth, y);

            path.LineTo(right - cornerRadius, y);
            path.QuadTo(right, y, right, y + cornerRadius);
            path.LineTo(right, bottom - cornerRadius);

            path.QuadTo(right, bottom, right - cornerRadius, bottom);
            path.LineTo(x + cornerRadius, bottom);

            path.QuadTo(x, bottom, x, bottom - cornerRadius);
            path.LineTo(x, y + cornerRadius);

            path.QuadTo(x, y, x + cornerRadius, y);

            return path;
        }
    }
}
