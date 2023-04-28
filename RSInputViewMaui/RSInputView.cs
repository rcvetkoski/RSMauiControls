using Microsoft.Maui.Platform;
using System.ComponentModel;

namespace RSInputViewMaui
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


        public static readonly BindableProperty LeadingIconProperty = BindableProperty.Create(nameof(LeadingIcon), typeof(string), typeof(RSInputView), null, propertyChanged: LeadingIconChanged);
        public string LeadingIcon
        {
            get { return (string)GetValue(LeadingIconProperty); }
            set { SetValue(LeadingIconProperty, value); }
        }
        private static void LeadingIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = bindable as RSInputView;

            if (rsInput.leadingIconImage != null)
            {
                rsInput.Remove(rsInput.leadingIconImage);
                rsInput.leadingIconImage = null;
            }

            if (string.IsNullOrEmpty((string)newValue))
            {
                rsInput.Graphics.Invalidate();
                return;
            }

            rsInput.leadingIconImage = new Image()
            {
                BackgroundColor = Colors.Red,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                Source = rsInput.LeadingIcon
            };

            rsInput.leadingIconImage.SetBinding(Image.WidthRequestProperty, new Binding("IconWidthRequest", source: rsInput));
            rsInput.leadingIconImage.SetBinding(Image.HeightRequestProperty, new Binding("IconHeightRequest", source: rsInput));
            if (rsInput.graphicsDrawable != null)
            {
                rsInput.SetLeadingIconMargin(rsInput.ContentMargin.Top, rsInput.ContentMargin.Bottom);
                rsInput.SetTrailingIconMargin(rsInput.ContentMargin.Top, rsInput.ContentMargin.Bottom);
            }

            rsInput.Add(rsInput.leadingIconImage, 0, 0);

            rsInput.Graphics.Invalidate();
        }
        private Image leadingIconImage { get; set; }


        public static readonly BindableProperty TrailingIconProperty = BindableProperty.Create(nameof(TrailingIcon), typeof(string), typeof(RSInputView), null, propertyChanged: TrailingIconChanged);
        public string TrailingIcon
        {
            get { return (string)GetValue(TrailingIconProperty); }
            set { SetValue(TrailingIconProperty, value); }
        }
        private static void TrailingIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = bindable as RSInputView;

            if (rsInput.trailingIconImage != null)
            {
                rsInput.Remove(rsInput.trailingIconImage);
                rsInput.trailingIconImage = null;
            }

            if (string.IsNullOrEmpty((string)newValue))
            {
                // Adjust input control margin
                rsInput.Graphics.Invalidate();
                return;
            }

            rsInput.trailingIconImage = new Image()
            {
                BackgroundColor = Colors.Red,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Source = rsInput.TrailingIcon
            };

            rsInput.trailingIconImage.SetBinding(Image.WidthRequestProperty, new Binding("IconWidthRequest", source: rsInput));
            rsInput.trailingIconImage.SetBinding(Image.HeightRequestProperty, new Binding("IconHeightRequest", source: rsInput));

            if (rsInput.graphicsDrawable != null)
            {
                rsInput.SetLeadingIconMargin(rsInput.ContentMargin.Top, rsInput.ContentMargin.Bottom);
                rsInput.SetTrailingIconMargin(rsInput.ContentMargin.Top, rsInput.ContentMargin.Bottom);
            }
            rsInput.Add(rsInput.trailingIconImage, 0, 0);

            rsInput.Graphics.Invalidate();
        }
        private Image trailingIconImage { get; set; }


        public static readonly BindableProperty IconWidthRequestProperty = BindableProperty.Create(nameof(IconWidthRequest), typeof(double), typeof(RSInputView), (double)30, propertyChanged: IconWidthRequestChanged);
        public double IconWidthRequest
        {
            get { return (double)GetValue(IconWidthRequestProperty); }
            set { SetValue(IconWidthRequestProperty, value); }
        }
        private static void IconWidthRequestChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Invalidate();
        }

        public static readonly BindableProperty IconHeightRequestProperty = BindableProperty.Create(nameof(IconHeightRequest), typeof(double), typeof(RSInputView), (double)30, propertyChanged: IconHeightRequestChanged);
        public double IconHeightRequest
        {
            get { return (double)GetValue(IconHeightRequestProperty); }
            set { SetValue(IconHeightRequestProperty, value); }
        }
        private static void IconHeightRequestChanged(BindableObject bindable, object oldValue, object newValue)
        {
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


        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(RSInputView), Colors.Gray, propertyChanged: PlaceholderColorChanged);
        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
        private static void PlaceholderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty HelperProperty = BindableProperty.Create(nameof(Helper), typeof(string), typeof(RSInputView), string.Empty, propertyChanged: MessageChanged);
        public string Helper
        {
            get { return (string)GetValue(HelperProperty); }
            set { SetValue(HelperProperty, value); }
        }

        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(string), typeof(RSInputView), string.Empty, propertyChanged: MessageChanged);
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

            if (size == SizeF.Zero && !string.IsNullOrEmpty((string)newValue))
            {
                graphicsDrawable.SetPlaceholderMargin(graphicsDrawable.messageSpacing + 13);

                Graphics.Invalidate();
                return;
            }

            var multiplier = Math.Floor(size.Width / (Graphics.Width - graphicsDrawable.PlaceholderMargin.Left - graphicsDrawable.PlaceholderMargin.Right) + 1);
            var bottomMarging = size.Width > 0 ? size.Height * multiplier + graphicsDrawable.messageSpacing : 0;

            graphicsDrawable.SetPlaceholderMargin(bottomMarging);

            rsInput.SetContentMargin(new Thickness(rsInput.ContentMargin.Left,
                                                   rsInput.ContentMargin.Top,
                                                   rsInput.ContentMargin.Right,
                                                   graphicsDrawable.PlaceholderMargin.Bottom));

            rsInput.SetLeadingIconMargin(rsInput.ContentMargin.Top, rsInput.ContentMargin.Bottom);
            rsInput.SetTrailingIconMargin(rsInput.ContentMargin.Top , rsInput.ContentMargin.Bottom);

            //Graphics.Invalidate();
        }


        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RSInputView), Colors.Gray, propertyChanged: BorderColorChanged);
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
            if (!(bindable as RSInputView).isDesignSet)
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

        public bool IsActive { get; protected set; }
        public Thickness ContentMargin { get; protected set; }

        private GraphicsDrawable graphicsDrawable;
        public GraphicsView Graphics { get; set; }


        public RSInputView()
        {
            // Main Grid
            this.VerticalOptions = LayoutOptions.Center;
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            Graphics = new GraphicsView();
            //Graphics.StartInteraction += Graphics_StartInteraction;
            this.Add(Graphics, 0, 0);
        }

        private void Graphics_StartInteraction(object sender, TouchEventArgs e)
        {
            Content.Focus();
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
#elif IOS 
                    handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif __MACCATALYST__

#elif WINDOWS
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
#endif
                }
            });

            // Set drawable
            graphicsDrawable = Design == RSInputViewDesign.Outlined ? new OutlineDrawable(this) : new FilledDrawable(this);

            // Margin init
            double top = Design == RSInputViewDesign.Outlined ? graphicsDrawable.PlaceholderMargin.Top: graphicsDrawable.PlaceholderMargin.Top + (graphicsDrawable as FilledDrawable).filledBorderMargin;
            double bottom = graphicsDrawable.PlaceholderMargin.Bottom;
            double left = graphicsDrawable.PlaceholderMargin.Left;
            double right = graphicsDrawable.PlaceholderMargin.Right;

            // Adjust icons margin if any
            SetLeadingIconMargin(top, bottom);
            SetTrailingIconMargin(top, bottom);

            // Adjust input control margin
            SetContentMargin(new Thickness(left + LeadingIconTotalWidth, top, right + TrailingIconTotalWidth, bottom));

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
            IsActive = true;

            if (CheckIfShouldAnimate())
                graphicsDrawable.StartFocusedAnimation();
            else
                Graphics.Invalidate();
        }

        private void Content_Unfocused(object sender, FocusEventArgs e)
        {
            IsActive = false;

            if (CheckIfShouldAnimate())
                graphicsDrawable.StartUnFocusedAnimation();
            else
                Graphics.Invalidate();
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
            else if (e.PropertyName == nameof(View.Margin))
            {
                //graphicsDrawable.SetPlaceholderMargin(Content.Margin);

                // Prevent margin change from user side
                if (Content.Margin != ContentMargin)
                    Content.Margin = ContentMargin;

                //Graphics.Invalidate();
            }
        }

        internal void SetContentMargin(Thickness thickness)
        {
            ContentMargin = thickness;
            Content.Margin = ContentMargin;
        }

        internal void SetTrailingIconMargin(double top = 0, double bottom = 0)
        {
            if (trailingIconImage == null)
                return;

            // Substract top margin for filled drawable
            top -= Design == RSInputViewDesign.Filled ? (graphicsDrawable as FilledDrawable).filledBorderMargin : 0;
            bottom -= Design == RSInputViewDesign.Outlined ? (graphicsDrawable as OutlineDrawable).OutlinedBorderMargin : 0;
            double offset = top - bottom;
            double defaultTopBottomMargin = 5;

            if (offset >= 0)
                trailingIconImage.Margin = new Thickness(0, offset + defaultTopBottomMargin, graphicsDrawable.PlaceholderMargin.Right, defaultTopBottomMargin);
            else
                trailingIconImage.Margin = new Thickness(0, defaultTopBottomMargin, graphicsDrawable.PlaceholderMargin.Right, Math.Abs(offset) + defaultTopBottomMargin);
        }

        internal void SetLeadingIconMargin(double top = 0, double bottom = 0)
        {
            if (leadingIconImage == null)
                return;

            // Substract top margin for filled drawable
            top -= Design == RSInputViewDesign.Filled ? (graphicsDrawable as FilledDrawable).filledBorderMargin : 0;
            bottom -= Design == RSInputViewDesign.Outlined ? (graphicsDrawable as OutlineDrawable).OutlinedBorderMargin : 0;
            double offset = top - bottom;
            double defaultTopBottomMargin = 5;


            if (offset >= 0)
                leadingIconImage.Margin = new Thickness(graphicsDrawable.PlaceholderMargin.Left, offset + defaultTopBottomMargin, 0, defaultTopBottomMargin);
            else
                leadingIconImage.Margin = new Thickness(graphicsDrawable.PlaceholderMargin.Left, defaultTopBottomMargin, graphicsDrawable.PlaceholderMargin.Right, Math.Abs(offset) + defaultTopBottomMargin);
        }

        internal double LeadingIconTotalWidth
        {
            get
            {
                return leadingIconImage == null ? 0 : IconWidthRequest + leadingIconImage.Margin.Left;
            }
        }

        internal double TrailingIconTotalWidth
        {
            get
            {
                return trailingIconImage == null ? 0 : IconWidthRequest + trailingIconImage.Margin.Right;
            }
        }

        internal bool IsFloating()
        {
            if (Content == null)
                return false;

            if (Content.IsFocused)
                return true;

            if (Content is Entry || Content is Editor || Content is SearchBar)
            {
                if (!string.IsNullOrEmpty((Content as Entry).Text))
                    return true;
            }
            else if (Content is Picker)
            {
                if ((Content as Picker).SelectedItem != null || (Content as Picker).SelectedIndex >= 0)
                    return true;
            }
            else if (Content is DatePicker || Content is TimePicker)
            {
                return true;
            }

            return false;
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
            Graphics.StartInteraction -= Graphics_StartInteraction;
        }
    }
}