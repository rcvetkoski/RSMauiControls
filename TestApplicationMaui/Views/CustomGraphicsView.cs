﻿using Microsoft.Maui;
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


        public static readonly BindableProperty DesignProperty = BindableProperty.Create(nameof(Design), typeof(RSInputViewDesign), typeof(RSInputView), RSInputViewDesign.Outlined, propertyChanged: DesignChanged);
        public RSInputViewDesign Design
        {
            get { return (RSInputViewDesign)GetValue(DesignProperty); }
            set { SetValue(DesignProperty, value); }
        }
        private static void DesignChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if((RSInputViewDesign)newValue == RSInputViewDesign.Outlined)
            {
                (bindable as RSInputView).graphicsDrawable = new OutlineDrawable(bindable as RSInputView);
            }
            else if((RSInputViewDesign)newValue == RSInputViewDesign.Filled)
            {
                (bindable as RSInputView).graphicsDrawable = new FilledDrawable(bindable as RSInputView);
            }

            (bindable as RSInputView).Graphics.Drawable = (bindable as RSInputView).graphicsDrawable;
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
#elif __IOS__ || __MACCATALYST__
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
        protected Thickness PlaceholderMargin;
        protected DateTime animationStartTime;
        protected const float AnimationDuration = 100; // milliseconds
        protected bool isAnimating = false;
        public RSInputView InputView { get; set; }  

        public GraphicsDrawable(RSInputView inputView)
        {
            InputView = inputView;
            PlaceholderMargin = new Thickness(16, 0, 8, 0);
            currentPlaceholderX = (float)PlaceholderMargin.Left;
            currentPlaceholderY = 0;

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

            currentPlaceholderSize = fontSize;
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
            if (InputView == null)
                return;

            InputView.Content.Margin = new Thickness(PlaceholderMargin.Left, borderPadding, PlaceholderMargin.Right, borderPadding);
        }

        public override void StartFocusedAnimation()
        {
            // Set font size 
            startPlaceholderSize = currentPlaceholderSize;
            endPlaceholderSize = 12;

            // Set X start and end position
            startX = currentPlaceholderX;
            endX = InputView.CornerRadius + borderGapSpacing / 2;

            // Set Y start and end position
            startY = currentPlaceholderY;
            endY = (float)-InputView.Graphics.Height / 2 + borderPadding;


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

        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // If it's not animating set placement here where all the measurement has been finished and ready to draw
            if (!isAnimating)
            {
                if (IsFloating())
                {
                    currentPlaceholderX = InputView.CornerRadius + borderGapSpacing / 2;
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + borderPadding;
                    currentPlaceholderSize = 12;
                }
                else
                {
                    currentPlaceholderX = (float)PlaceholderMargin.Left;
                    currentPlaceholderY = 0;
                    currentPlaceholderSize = fontSize;
                }
            }

            canvas.StrokeSize = InputView.BorderThikness;
            canvas.StrokeColor = InputView.BorderColor;
            canvas.Antialias = true;
            canvas.FontColor = InputView.PlaceholderColor;
            canvas.Font = textFont;
            canvas.FontSize = currentPlaceholderSize;
            canvas.DrawString(InputView.Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);
            float size = IsFloating() ? canvas.GetStringSize(InputView.Placeholder, textFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
            PathF pathF = CreateEntryOutlinePath(0, borderPadding, dirtyRect.Width, dirtyRect.Height - borderPadding * 2, InputView.CornerRadius, size);
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
        public FilledDrawable(RSInputView inputView) : base(inputView)
        {
            if (InputView == null)
                return;

            InputView.Content.Margin = new Thickness(PlaceholderMargin.Left, 15, PlaceholderMargin.Right, 0);
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
                    currentPlaceholderY = (float)-InputView.Graphics.Height / 2 + 15;
                    currentPlaceholderSize = 12;
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
            float size = IsFloating() ? canvas.GetStringSize(InputView.Placeholder, textFont, currentPlaceholderSize, HorizontalAlignment.Left, VerticalAlignment.Center).Width + borderGapSpacing : 0;
            PathF pathF = CreateEntryOutlinePath(0, 0, dirtyRect.Width, dirtyRect.Height, InputView.CornerRadius, size);
            canvas.DrawPath(pathF);
            canvas.FillColor = InputView.FilledBorderColor;
            canvas.FillPath(pathF, WindingMode.NonZero);
            canvas.DrawString(InputView.Placeholder, currentPlaceholderX, currentPlaceholderY, dirtyRect.Width - (float)PlaceholderMargin.Right, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Center, TextFlow.ClipBounds);


            canvas.StrokeColor = InputView.BorderColor;
            canvas.StrokeSize = 2;
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