using Microsoft.Maui.Platform;
using System;
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


        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(RSInputView), 12f, propertyChanged: CornerRadiusChanged);
        public float CornerRadius
        {
            get { return (float)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        private static void CornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).graphicsDrawable.CornerRadius = (float)newValue;
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
            graphicsDrawable.SetControl(Content);
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
                    graphicsDrawable.FontSize = (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize;
                    Graphics.Invalidate();
                }
            }
            else if(e.PropertyName == nameof(Entry.Text) || e.PropertyName == nameof(Picker.SelectedItem) || e.PropertyName == nameof(Picker.SelectedIndex))
            {
                Graphics.Invalidate();
            }
        }

        private void SetDrawableProperties()
        {
            var textElement = (Content as Microsoft.Maui.Controls.Internals.IFontElement);

            graphicsDrawable.Placeholder = Placeholder;
            graphicsDrawable.PlaceholderColor = PlaceholderColor;
            graphicsDrawable.BorderColor = BorderColor;
            graphicsDrawable.BorderThikness = BorderThikness;
            graphicsDrawable.CornerRadius = CornerRadius;   
            graphicsDrawable.FontSize = (float)textElement.FontSize;

            var fnt = textElement.ToFont(textElement.FontSize);
            Microsoft.Maui.Graphics.Font font = new Microsoft.Maui.Graphics.Font(fnt.Family, (int)fnt.Weight, styleType : (int)FontSlant.Default);
            graphicsDrawable.TextFont = font;
        }

        private bool CheckIfShouldAnimate()
        {
            if (Content is Entry || Content is Editor || Content is SearchBar)
            {
                if (!string.IsNullOrEmpty((Content as Entry).Text))
                    return false;
            }
            else if (Content is Picker)
            {
                if ((Content as Picker).SelectedItem != null || (Content as Picker).SelectedIndex >= 0)
                    return false;
            }
            else if (Content is DatePicker || Content is TimePicker)
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
        private float startPlaceholderSize;  
        private float endPlaceholderSize;
        private float currentPlaceholderSize;
        private float startX;
        private float endX;
        private float currentPlaceholderX;
        private float startY;
        private float endY;
        private float currentPlaceholderY;
        private float borderGapSpacing = 10;
        private float borderPadding = 5;   
        private DateTime animationStartTime;
        private const float AnimationDuration = 200; // milliseconds
        private bool isAnimating = false;
        public string Placeholder { get; set; }
        public Color PlaceholderColor { get;set; }
        public Color BorderColor { get; set; }
        public float BorderThikness { get; set; }
        public float CornerRadius { get; set; }
        public float FontSize { get; set; }
        public Thickness PlaceholderMargin { get; set; }    
        public Microsoft.Maui.Graphics.Font TextFont { get; set; }
        private Microsoft.Maui.Graphics.IImage trailingIcon;
        public GraphicsView Holder { get;set; }
        public View Control { get; set; }    
        public void SetControl(View content)
        {
            Control = content;
            Control.Margin = new Thickness(8, 0, 8, 0);
        }

        public GraphicsDrawable(GraphicsView graphicsView)
        {
            Holder = graphicsView;
            PlaceholderMargin = new Thickness(8, 0, 8, 0);
            currentPlaceholderX = (float)PlaceholderMargin.Left;
            currentPlaceholderY = 0;
            currentPlaceholderSize = FontSize;
        }

        public bool IsFloating()
        {
            if(Control.IsFocused)
                return true;    

            if (Control is Entry || Control is Editor || Control is SearchBar)
            {
                if (!string.IsNullOrEmpty((Control as Entry).Text))
                    return true;
            }
            else if (Control is Picker)
            {
                if ((Control as Picker).SelectedItem != null || (Control as Picker).SelectedIndex >= 0)
                    return true;
            }
            else if (Control is DatePicker || Control is TimePicker)
            {
                return true;
            }

            return false;
        }

        public void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = 12;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = CornerRadius + borderGapSpacing / 2;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-Holder.Height / 2;

            animationStartTime = DateTime.UtcNow;
            Holder.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        public void StartUnFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = FontSize;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = 0;

            animationStartTime = DateTime.UtcNow;
            Holder.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        public bool FocusedAnimation()
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
            Holder.Invalidate();

            // Stop the animation if progress is 1 (100%)
            if(progress < 1)
            {
                return true;
            }
            else
            {
                isAnimating = false;
                return false;      
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (IsFloating())
                {
                    currentPlaceholderX = CornerRadius + borderGapSpacing / 2;
                    currentPlaceholderY = (float)-Holder.Height / 2;
                    currentPlaceholderSize = 12;
                }
                else
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = 0;
                    currentPlaceholderSize = FontSize;
                }
            }

            canvas.StrokeSize = BorderThikness;
            canvas.StrokeColor = BorderColor;
            canvas.Antialias = true;
            canvas.FontColor = PlaceholderColor;
            canvas.Font = TextFont;
            canvas.FontSize = currentPlaceholderSize;
            canvas.DrawString(Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            float size = IsFloating() ? canvas.GetStringSize(Placeholder, TextFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
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
