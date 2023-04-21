using Microsoft.Maui;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
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


        public static readonly BindableProperty TrailingIconProperty = BindableProperty.Create(nameof(TrailingIcon), typeof(string), typeof(RSInputView), null, propertyChanged: TrailingIconChanged);
        public string TrailingIcon
        {
            get { return (string)GetValue(TrailingIconProperty); }
            set { SetValue(TrailingIconProperty, value); }
        }
        private static void TrailingIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Image image = new Image()
            {
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 8, 0),
                WidthRequest = 25,
                HeightRequest = 25,
                Source = (bindable as RSInputView).TrailingIcon
            };
            (bindable as RSInputView).Add(image, 0, 0);
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


        public static readonly BindableProperty FilledBorderColorProperty = BindableProperty.Create(nameof(FilledBorderColor), typeof(Color), typeof(RSInputView), Colors.LightGray, propertyChanged: FilledBorderColorChanged);
        public Color FilledBorderColor
        {
            get { return (Color)GetValue(FilledBorderColorProperty); }
            set { SetValue(FilledBorderColorProperty, value); }
        }
        private static void FilledBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).graphicsDrawable.FilledBorderColor = (Color)newValue;
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


        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(RSInputView), 4f, propertyChanged: CornerRadiusChanged);
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
            graphicsDrawable = new FilledDrawable(Graphics);
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
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
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
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
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
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
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
            if (CheckIfShouldAnimate())
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
            else if (e.PropertyName == nameof(Entry.Text) || e.PropertyName == nameof(Picker.SelectedItem) || e.PropertyName == nameof(Picker.SelectedIndex))
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
            Microsoft.Maui.Graphics.Font font = new Microsoft.Maui.Graphics.Font(fnt.Family, (int)fnt.Weight, styleType: (int)FontSlant.Default);
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

    public abstract class GraphicsDrawable : IDrawable
    {
        protected float startPlaceholderSize;
        protected float endPlaceholderSize;
        protected float currentPlaceholderSize;
        protected float startX;
        protected float endX;
        protected float currentPlaceholderX;
        protected float startY;
        protected float endY;
        protected float currentPlaceholderY;
        protected float borderGapSpacing = 10;
        protected float borderPadding = 5;
        protected DateTime animationStartTime;
        protected const float AnimationDuration = 100; // milliseconds
        protected bool isAnimating = false;
        public string Placeholder { get; set; }
        public Color PlaceholderColor { get; set; }
        public Color BorderColor { get; set; }
        public Color FilledBorderColor { get; set; }

        public float BorderThikness { get; set; }
        public float CornerRadius { get; set; }
        public float FontSize { get; set; }
        public Thickness PlaceholderMargin { get; set; }
        public Microsoft.Maui.Graphics.Font TextFont { get; set; }
        public Image TrailingIcon { get; set; }
        public GraphicsView Holder { get; set; }
        public View Control { get; set; }

        public virtual void SetControl(View content)
        {
            Control = content;
            Control.VerticalOptions = LayoutOptions.Center;
        }

        protected bool IsFloating()
        {
            if (Control.IsFocused)
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

        public virtual void StartFocusedAnimation()
        {
            animationStartTime = DateTime.UtcNow;
            Holder.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
        }

        public virtual void StartUnFocusedAnimation()
        {
            animationStartTime = DateTime.UtcNow;
            Holder.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), FocusedAnimation);
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
            Holder.Invalidate();

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



    public class OutlineDrawable : GraphicsDrawable
    {
        public OutlineDrawable(GraphicsView graphicsView)
        {
            Holder = graphicsView;
            PlaceholderMargin = new Thickness(8, 0, 8, 0);
            currentPlaceholderX = (float)PlaceholderMargin.Left;
            currentPlaceholderY = 0;
            currentPlaceholderSize = FontSize;
        }

        public override void SetControl(View content)
        {
            base.SetControl(content);
            Control.Margin = new Thickness(8, borderPadding, 8, borderPadding);
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = 12;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = CornerRadius + borderGapSpacing / 2;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-Holder.Height / 2 + borderPadding;


            base.StartFocusedAnimation();
        }

        public override void StartUnFocusedAnimation()
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


            base.StartUnFocusedAnimation();
        }

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (IsFloating())
                {
                    currentPlaceholderX = CornerRadius + borderGapSpacing / 2;
                    currentPlaceholderY = (float)-Holder.Height / 2 + borderPadding;
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
            canvas.DrawString(Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            float size = IsFloating() ? canvas.GetStringSize(Placeholder, TextFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
            PathF pathF = CreateEntryOutlinePath(0, borderPadding, dirtyRect.Width, dirtyRect.Height - borderPadding * 2, CornerRadius, size);
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


    public class FilledDrawable : GraphicsDrawable
    {
        public FilledDrawable(GraphicsView graphicsView)
        {
            Holder = graphicsView;
            PlaceholderMargin = new Thickness(8, 0, 8, 0);
            currentPlaceholderX = (float)PlaceholderMargin.Left;
            currentPlaceholderY = 0;
            currentPlaceholderSize = FontSize;
        }

        public override void SetControl(View content)
        {
            base.SetControl(content);
            Control.Margin = new Thickness(8, 15, 8, 0);
        }

        public PathF CreateEntryOutlinePath(float x, float y, float width, float height, float cornerRadius, float gapWidth)
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
                if (IsFloating())
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = (float)-Holder.Height / 2 + 20;
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
            float size = IsFloating() ? canvas.GetStringSize(Placeholder, TextFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
            PathF pathF = CreateEntryOutlinePath(0, 0, dirtyRect.Width, dirtyRect.Height, CornerRadius, size);
            canvas.DrawPath(pathF);
            canvas.FillColor = BorderColor;
            canvas.FillPath(pathF, WindingMode.NonZero);
            canvas.DrawString(Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);


            canvas.StrokeColor = Colors.Blue;
            canvas.StrokeSize = 2;
            canvas.FillColor = Colors.Blue;
            canvas.DrawLine(0, dirtyRect.Height, dirtyRect.Width, dirtyRect.Height);
        }
    }
}
