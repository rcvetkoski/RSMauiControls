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
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty HelperProperty = BindableProperty.Create(nameof(Helper), typeof(string), typeof(RSInputView), default, propertyChanged: MessageChanged);
        public string Helper
        {
            get { return (string)GetValue(HelperProperty); }
            set { SetValue(HelperProperty, value); }
        }

        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(string), typeof(RSInputView), default, propertyChanged: MessageChanged);
        public string Error
        {
            get { return (string)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }

        private static void MessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bindable as RSInputView).graphicsDrawable == null)
                return;

            var rsInput = (bindable as RSInputView);
            var Graphics = (bindable as RSInputView).Graphics;

            // Do not update if new message is null or empty but one of error or helper is still active
            if (string.IsNullOrEmpty((string)newValue))
            {
                if (string.IsNullOrEmpty(rsInput.Error) && !string.IsNullOrEmpty(rsInput.Helper))
                {
                    Graphics.Invalidate();
                    return;
                }
                else if (string.IsNullOrEmpty(rsInput.Helper) && !string.IsNullOrEmpty(rsInput.Error))
                {
                    Graphics.Invalidate();
                    return;
                }
            }

            var graphicsDrawable = (bindable as RSInputView).graphicsDrawable;
            string newHelperText = newValue != null ? newValue.ToString() : string.Empty;
            var size = graphicsDrawable.GetCanvasStringSize(newHelperText);
            var multiplier = Math.Floor(size.Width / (Graphics.Width - graphicsDrawable.PlaceholderMargin.Left - graphicsDrawable.PlaceholderMargin.Right) + 1);
            var bottomMarging = size.Width > 0 ? size.Height * multiplier : 0;

            graphicsDrawable.SetPlaceholderBottomMargin(bottomMarging);
            Graphics.Invalidate();
        }


        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RSInputView), Colors.LightGray, propertyChanged: BorderColorChanged);
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        private static void BorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Invalidate();
        }

        private bool isDesignSet;
        public static readonly BindableProperty DesignProperty = BindableProperty.Create(nameof(Design), typeof(RSInputViewDesign), typeof(RSInputView), RSInputViewDesign.Outlined, propertyChanged: DesignChanged);
        public RSInputViewDesign Design
        {
            get { return (RSInputViewDesign)GetValue(DesignProperty); }
            set { SetValue(DesignProperty, value); }
        }
        private static void DesignChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(!(bindable as RSInputView).isDesignSet)
            {
                (bindable as RSInputView).isDesignSet = true;
                return;
            }

            if ((RSInputViewDesign)newValue == RSInputViewDesign.Outlined)
            {
                (bindable as RSInputView).graphicsDrawable = new OutlineDrawable(bindable as RSInputView);
            }
            else if ((RSInputViewDesign)newValue == RSInputViewDesign.Filled)
            {
                (bindable as RSInputView).graphicsDrawable = new FilledDrawable(bindable as RSInputView);
            }

            (bindable as RSInputView).Graphics.Drawable = (bindable as RSInputView).graphicsDrawable;
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty FilledBorderColorProperty = BindableProperty.Create(nameof(FilledBorderColor), typeof(Color), typeof(RSInputView), Colors.WhiteSmoke, propertyChanged: FilledBorderColorChanged);
        public Color FilledBorderColor
        {
            get { return (Color)GetValue(FilledBorderColorProperty); }
            set { SetValue(FilledBorderColorProperty, value); }
        }
        private static void FilledBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
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
            this.Add(Graphics, 0, 0);
        }

        private void SetContent()
        {
            if (Content == null)
                return;

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
#elif __IOS__ 
                    handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
#endif
                }
            });

            // Set drawable
            graphicsDrawable = Design == RSInputViewDesign.Outlined ? new OutlineDrawable(this) : new FilledDrawable(this);
            Graphics.Drawable = graphicsDrawable;
            isDesignSet = true;

            this.Add(Content, 0, 0);
            Content.VerticalOptions = LayoutOptions.Center;
            Content.Focused += Content_Focused;
            Content.Unfocused += Content_Unfocused;
            Content.PropertyChanged += Content_PropertyChanged;

            if (Content is Entry)
                (Content as Entry).PlaceholderColor = Colors.Transparent;
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
                if (graphicsDrawable.fontSize != (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize)
                {
                    graphicsDrawable.fontSize = (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize;
                    Graphics.Invalidate();
                }
            }
            else if (e.PropertyName == nameof(Entry.Text) || e.PropertyName == nameof(Picker.SelectedItem) || e.PropertyName == nameof(Picker.SelectedIndex))
            {
                Graphics.Invalidate();
            }
            else if(e.PropertyName == nameof(View.Margin))
            {
                graphicsDrawable.SetPlaceholderMargin(Content.Margin);
                Graphics.Invalidate();
            }
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
        public float messageSpacing = 3;
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
            fontSizeFloating = 10;
            currentPlaceholderSize = fontSize;
        }

        public void SetPlaceholderMargin(Thickness thickness)
        {
            PlaceholderMargin = thickness;
        }

        public void SetPlaceholderBottomMargin(double bottom)
        {
            PlaceholderMargin = new Thickness(PlaceholderMargin.Left, PlaceholderMargin.Top, PlaceholderMargin.Right, bottom);
            InputView.Content.Margin = new Thickness(PlaceholderMargin.Left, PlaceholderMargin.Top, PlaceholderMargin.Right, PlaceholderMargin.Bottom);
        }


        public SizeF GetCanvasStringSize(string text)
        {
            return this.canvas.GetStringSize(text, textFont, fontSizeFloating, HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        protected bool IsFloating()
        {
            if(InputView.Content == null)
                return false;       

            if (InputView.Content.IsFocused)
                return true;

            if (InputView.Content is Entry || InputView.Content is Editor || InputView.Content is SearchBar)
            {
                if (!string.IsNullOrEmpty((InputView.Content as Entry).Text))
                    return true;
            }
            else if (InputView.Content is Picker)
            {
                if ((InputView.Content as Picker).SelectedItem != null || (InputView.Content as Picker).SelectedIndex >= 0)
                    return true;
            }
            else if (InputView.Content is DatePicker || InputView.Content is TimePicker)
            {
                return true;
            }

            return false;
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

    public class OutlineDrawable : GraphicsDrawable
    {
        public OutlineDrawable(RSInputView inputView) : base(inputView)
        {
            if (InputView.Content == null)
                return;

            double bottomMargin = 0;
            if(!string.IsNullOrEmpty(InputView.Helper))
                bottomMargin = 11 + messageSpacing;

            PlaceholderMargin = new Thickness(12, 5, 12, bottomMargin);

            currentPlaceholderX = (float)PlaceholderMargin.Left;
            currentPlaceholderY = (float)(PlaceholderMargin.Top - PlaceholderMargin.Bottom) / 2;

            InputView.Content.Margin = new Thickness(PlaceholderMargin.Left, PlaceholderMargin.Top, PlaceholderMargin.Right, PlaceholderMargin.Bottom);
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSizeFloating;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-InputView.Graphics.Height / 2 + (float)PlaceholderMargin.Top;


            base.StartFocusedAnimation();
        }

        public override void StartUnFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSize;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)(PlaceholderMargin.Top - PlaceholderMargin.Bottom) / 2;


            base.StartUnFocusedAnimation();
        }

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (IsFloating())
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + (float)PlaceholderMargin.Top;
                    currentPlaceholderSize = fontSizeFloating;
                }
                else
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = (float)(PlaceholderMargin.Top - PlaceholderMargin.Bottom) / 2;
                    currentPlaceholderSize = fontSize;
                }
            }

            canvas.StrokeSize = InputView.BorderThikness;
            canvas.StrokeColor = InputView.BorderColor;
            canvas.Antialias = true;
            canvas.FontColor = InputView.PlaceholderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;
            canvas.DrawString(InputView.Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right * 2, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            float size = IsFloating() ? canvas.GetStringSize(InputView.Placeholder, textFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
            PathF pathF = CreateEntryOutlinePath(0, (float)PlaceholderMargin.Top, dirtyRect.Width, dirtyRect.Height - (float)PlaceholderMargin.Top - (float)PlaceholderMargin.Bottom, InputView.CornerRadius, size);
            canvas.DrawPath(pathF);


            // Error or Helper
            DrawMessage(canvas, dirtyRect);

            this.canvas = canvas;
        }

        private void DrawMessage(ICanvas canvas, RectF dirtyRect)
        {
            if (string.IsNullOrEmpty(InputView.Error) && string.IsNullOrEmpty(InputView.Helper))
                return;

            bool isError = !string.IsNullOrEmpty(InputView.Error);
            string message = isError ? InputView.Error : InputView.Helper;

            canvas.FontSize = fontSizeFloating;
            canvas.FontColor = isError ? Colors.Red : InputView.BorderColor;

            var messageSize = canvas.GetStringSize(message, textFont, fontSizeFloating, HorizontalAlignment.Left, VerticalAlignment.Center);
            var multiplier = Math.Floor(messageSize.Width / (dirtyRect.Width - PlaceholderMargin.Left - PlaceholderMargin.Right) + 1);

            canvas.DrawString(message,
                    currentPlaceholderX,
                    dirtyRect.Height / 2 + (float)(messageSize.Height * multiplier) / 2 - (float)PlaceholderMargin.Bottom + messageSpacing,
                    dirtyRect.Width - (float)PlaceholderMargin.Right * 2, dirtyRect.Height,
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
            path.MoveTo(x + cornerRadius + margin + gapWidth - borderGapSpacing / 2, y);

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

    public class FilledDrawable : GraphicsDrawable
    {
        public FilledDrawable(RSInputView inputView) : base(inputView)
        {
            if (InputView.Content == null)
                return;

            double bottomMargin = 0;
            if (!string.IsNullOrEmpty(InputView.Helper))
                bottomMargin = 11 + messageSpacing;

            PlaceholderMargin = new Thickness(12, 10, 12, bottomMargin);

            InputView.Content.Margin = new Thickness(PlaceholderMargin.Left, PlaceholderMargin.Top, PlaceholderMargin.Right, PlaceholderMargin.Bottom);
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSizeFloating;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-InputView.Graphics.Height / 2 + 15;


            base.StartFocusedAnimation();
        }

        public override void StartUnFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = fontSize;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = (float)PlaceholderMargin.Left;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = 0;


            base.StartUnFocusedAnimation();
        }

        public PathF CreateEntryOutlinePath(float x, float y, float width, float height, float cornerRadius)
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
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + 15;
                    currentPlaceholderSize = fontSizeFloating;
                }
                else
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = 0;
                    currentPlaceholderSize = fontSize;
                }
            }

            canvas.StrokeSize = InputView.BorderThikness;
            canvas.StrokeColor = InputView.FilledBorderColor;
            canvas.Antialias = true;
            canvas.FontColor = InputView.PlaceholderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;
            PathF pathF = CreateEntryOutlinePath(0, 0, dirtyRect.Width, dirtyRect.Height, InputView.CornerRadius);
            canvas.DrawPath(pathF);
            canvas.FillColor = InputView.FilledBorderColor;
            canvas.FillPath(pathF, WindingMode.NonZero);
            canvas.DrawString(InputView.Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right * 2, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);


            canvas.StrokeColor = InputView.BorderColor;
            canvas.StrokeSize = 1;
            canvas.FillColor = InputView.BorderColor;
            canvas.DrawLine(0, dirtyRect.Height, dirtyRect.Width, dirtyRect.Height);
        }
    }

    public enum RSInputViewDesign
    {
        Outlined,
        Filled
    }
}
