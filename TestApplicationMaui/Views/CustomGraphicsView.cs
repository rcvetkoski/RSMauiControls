using Microsoft.Maui.Platform;
using System.ComponentModel;

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


        public static readonly BindableProperty TrailingIconProperty = BindableProperty.Create(nameof(TrailingIcon), typeof(Image), typeof(RSInputView), null, propertyChanged: TrailingIconChanged);
        public Image TrailingIcon
        {
            get { return (Image)GetValue(TrailingIconProperty); }
            set { SetValue(TrailingIconProperty, value); }
        }
        private static void TrailingIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).TrailingIcon.HorizontalOptions = LayoutOptions.End;
            (bindable as RSInputView).TrailingIcon.WidthRequest = 30;
            (bindable as RSInputView).TrailingIcon.HeightRequest = 30;
            (bindable as RSInputView).Add((bindable as RSInputView).TrailingIcon, 0, 0);
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(RSInputView), default, propertyChanged: PlaceholderChanged);
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        private static void PlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).graphicsDrawable.Placeholder = newValue.ToString();
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(RSInputView), Colors.LightGray, propertyChanged: PlaceholderColorChanged);
        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
        private static void PlaceholderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).graphicsDrawable.PlaceholderColor = (Color)newValue;
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RSInputView), Colors.LightGray, propertyChanged: BorderColorChanged);
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        private static void BorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).graphicsDrawable.BorderColor = (Color)newValue;
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty BorderThiknessProperty = BindableProperty.Create(nameof(BorderThikness), typeof(float), typeof(RSInputView), 1f, propertyChanged: BorderThiknessChanged);
        public float BorderThikness
        {
            get { return (float)GetValue(BorderThiknessProperty); }
            set { SetValue(BorderThiknessProperty, value); }
        }
        private static void BorderThiknessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).graphicsDrawable.BorderThikness = (float)newValue;
            (bindable as RSInputView).Graphics.Invalidate();
        }

        private GraphicsDrawable graphicsDrawable;
        public GraphicsView Graphics { get; set; }



        public RSInputView() 
        {
            // Main Grid
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            Graphics = new GraphicsView();
            graphicsDrawable = new GraphicsDrawable(Graphics);
            Graphics.Drawable = graphicsDrawable;
            this.Add(Graphics, 0, 0);
            
        }

        private void SetContent()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("BorderlessEntry", (handler, view) =>
            {
                if (view == Content)
                {
#if __ANDROID__
                handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
#elif __IOS__ || __MACCATALYST__
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                handler.PlatformView.FontWeight = Microsoft.UI.Text.FontWeights.Thin;
#endif
                }
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("BorderlessPicker", (handler, view) =>
            {
                if (view == Content)
                {
#if __ANDROID__
                    handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
#elif __IOS__ || __MACCATALYST__ 
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                handler.PlatformView.FontWeight = Microsoft.UI.Text.FontWeights.Thin;
#endif
                }
            });

            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("BorderlessDatePicker", (handler, view) =>
            {
                if (view == Content)
                {
#if __ANDROID__
                    handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
#elif __IOS__ || __MACCATALYST__
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                    handler.PlatformView.FontWeight = Microsoft.UI.Text.FontWeights.Thin;
#endif
                }
            });



            this.Add(Content, 0, 0);
            Content.Margin = new Thickness(8, 0, 8, 0);
            Content.Focused += Content_Focused;
            Content.Unfocused += Content_Unfocused;
            Content.PropertyChanged += Content_PropertyChanged;

            if (Content is Entry)
                (Content as Entry).PlaceholderColor = Colors.Transparent;

            SetDrawableProperties();
        }

        private void Content_Focused(object sender, FocusEventArgs e)
        {
            if(CheckIfShouldAnimate())
                graphicsDrawable.StartFocusedAnimation();
        }

        private void Content_Unfocused(object sender, FocusEventArgs e)
        {
            if (CheckIfShouldAnimate())
                graphicsDrawable.StartUnFocusedAnimation();
        }

        private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Microsoft.Maui.Controls.Internals.IFontElement.FontSize))
            {
                if (graphicsDrawable.FontSize != (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize)
                {
                    graphicsDrawable.InitialPlaceholderSize = (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize;
                    graphicsDrawable.SetCurrentPlaceholderSize();
                    Graphics.Invalidate();
                }
            }
        }
        private void SetDrawableProperties()
        {
            var textElement = (Content as Microsoft.Maui.Controls.Internals.IFontElement);

            graphicsDrawable.Placeholder = Placeholder;
            graphicsDrawable.PlaceholderColor = PlaceholderColor;
            graphicsDrawable.BorderColor = BorderColor;
            graphicsDrawable.BorderThikness = BorderThikness;
            graphicsDrawable.InitialPlaceholderSize = (float)textElement.FontSize;
            graphicsDrawable.SetCurrentPlaceholderSize();

            var fnt = textElement.ToFont(textElement.FontSize);
            Microsoft.Maui.Graphics.Font font = new Microsoft.Maui.Graphics.Font(fnt.Family, (int)fnt.Weight, styleType : (int)FontSlant.Default);
            graphicsDrawable.TextFont = font;
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
            Content.PropertyChanged -= Content_PropertyChanged;
        }
    }

    public class GraphicsDrawable : IDrawable
    {
        public float InitialPlaceholderSize { get; set; }   
        private float finalPlaceholderSize = 12;
        private float currentPlaceholderSize;
        private const float AnimationDuration = 200; // milliseconds
        private float currentPlaceholderX;
        private float currentPlaceholderY;
        private float startX = 12;
        private float endX = 12;
        private float startY = 0;
        private float endY = 0;
        private bool isFloating = false;
        private bool updateFloatingPosition = false;
        private DateTime animationStartTime;
        public bool GapVisible { get; set; }
        public string Placeholder { get; set; }
        public Color PlaceholderColor { get;set; }
        public Color BorderColor { get; set; }
        public float BorderThikness { get; set; }
        public float FontSize { get; set; }
        public Microsoft.Maui.Graphics.Font TextFont { get; set; }
        private Microsoft.Maui.Graphics.IImage trailingIcon;


        public GraphicsView Holder { get;set; }

        public void SetCurrentPlaceholderSize()
        {
            if(isFloating)
            {
                finalPlaceholderSize = InitialPlaceholderSize;
            }
            else
            {
                currentPlaceholderSize = InitialPlaceholderSize;
                finalPlaceholderSize = 15;
            }

            updateFloatingPosition = true;
        }

        private void setValuesBeforeAnimation()
        {
            if(isFloating)
            {
                finalPlaceholderSize = 15;
                InitialPlaceholderSize = currentPlaceholderSize;
                startX = 12;
                endX = 12;
                startY = 0;
                endY = (float)-Holder.Height / 2;
                GapVisible = true;
            }
            else
            {
                finalPlaceholderSize = InitialPlaceholderSize;
                InitialPlaceholderSize = currentPlaceholderSize;
                startX = currentPlaceholderX;
                endX = 12;
                startY = currentPlaceholderY;
                endY = 0;
                GapVisible = false;
            }
        }

        public GraphicsDrawable(GraphicsView graphicsView)
        {
            Holder = graphicsView;
            currentPlaceholderX = startX;
            currentPlaceholderY = startY;   
        }

        public bool FocusedAnimation()
        {
            float progress = (float)(DateTime.UtcNow - animationStartTime).TotalMilliseconds / AnimationDuration;
            if (progress > 1)
                progress = 1;

            // Update placeholder position and size
            currentPlaceholderX = startX + (endX - startX) * progress;
            currentPlaceholderY = startY + (endY - startY) * progress;
            currentPlaceholderSize = InitialPlaceholderSize + (finalPlaceholderSize - InitialPlaceholderSize) * progress;

            // Invalidate to redraw the control
            Holder.Invalidate();

            // Stop the animation if progress is 1 (100%)
            return progress < 1;
        }

        public bool UnfocusedAnimation()
        {
            float progress = (float)(DateTime.UtcNow - animationStartTime).TotalMilliseconds / AnimationDuration;
            if (progress > 1)
                progress = 1;

            // Update placeholder position and size
            currentPlaceholderX = startX + (endX - startX) * progress;
            currentPlaceholderY = startY + (endY - startY) * progress;
            currentPlaceholderSize = InitialPlaceholderSize + (finalPlaceholderSize - InitialPlaceholderSize) * progress;

            // Invalidate to redraw the control
            Holder.Invalidate();

            // Stop the animation if progress is 1 (100%)
            return progress < 1;
        }

        public void StartFocusedAnimation()
        {
            isFloating = true;
            setValuesBeforeAnimation();
            animationStartTime = DateTime.UtcNow;
            Holder.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        public void StartUnFocusedAnimation()
        {
            isFloating = false;
            setValuesBeforeAnimation();
            animationStartTime = DateTime.UtcNow;
            Holder.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), UnfocusedAnimation);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Update when FontSize changes at runtime
            if(updateFloatingPosition)
            {
                if (isFloating)
                    endY = (float)-Holder.Height / 2;
                else
                    endY = 0;

                currentPlaceholderY = endY;
                updateFloatingPosition = false;
            }


            canvas.StrokeSize = BorderThikness;
            canvas.StrokeColor = BorderColor;
            canvas.Antialias = true;
            canvas.FontColor = PlaceholderColor;
            canvas.Font = TextFont;
            canvas.FontSize = currentPlaceholderSize;
            canvas.DrawString(Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            float size = GapVisible ? canvas.GetStringSize(Placeholder, TextFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width : 0;
            PathF pathF = CreateEntryOutlinePath(0, 0, dirtyRect.Width, dirtyRect.Height, 12, size);
            canvas.DrawPath(pathF);

            if(trailingIcon != null)
                canvas.DrawImage(trailingIcon, dirtyRect.Width - 50, 0, 50, 50);
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
