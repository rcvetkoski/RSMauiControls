using System.ComponentModel;
using System.Net.Http.Headers;
using System.Windows.Input;

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

            if (rsInput.LeadingIconImage != null)
            {
                rsInput.Remove(rsInput.LeadingIconImage);
                rsInput.LeadingIconImage = null;
            }

            if (string.IsNullOrEmpty((string)newValue))
            {
                // Adjust input control margin
                if (rsInput.graphicsDrawable != null)
                {
                    rsInput.graphicsDrawable.SetIconMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
                    rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
                }
                rsInput.Graphics.Invalidate();
                return;
            }

            rsInput.LeadingIconImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                Source = rsInput.LeadingIcon
            };

            rsInput.LeadingIconImage.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = rsInput.IconCommand,
                CommandParameter = rsInput.LeadingIconImage
            });

            rsInput.LeadingIconImage.SetBinding(Image.WidthRequestProperty, new Binding("IconWidthRequest", source: rsInput));
            rsInput.LeadingIconImage.SetBinding(Image.HeightRequestProperty, new Binding("IconHeightRequest", source: rsInput));
            if (rsInput.graphicsDrawable != null)
            {
                rsInput.graphicsDrawable.SetIconMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
                rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
            }

            rsInput.Add(rsInput.LeadingIconImage, 0, 0);

            rsInput.Graphics.Invalidate();
        }
        internal Image LeadingIconImage { get; set; }
        public static readonly BindableProperty LeadingIconCommandProperty = BindableProperty.Create(nameof(LeadingIconCommand), typeof(ICommand), typeof(RSInputView), null);
        public ICommand LeadingIconCommand
        {
            get { return (ICommand)GetValue(LeadingIconCommandProperty); }
            set { SetValue(LeadingIconCommandProperty, value); }
        }
        public static readonly BindableProperty LeadingIconCommandParameterProperty = BindableProperty.Create(nameof(LeadingIconCommandParameter), typeof(object), typeof(RSInputView), null);
        public object LeadingIconCommandParameter
        {
            get { return (object)GetValue(LeadingIconCommandParameterProperty); }
            set { SetValue(LeadingIconCommandParameterProperty, value); }
        }


        public static readonly BindableProperty TrailingIconProperty = BindableProperty.Create(nameof(TrailingIcon), typeof(string), typeof(RSInputView), null, propertyChanged: TrailingIconChanged);
        public string TrailingIcon
        {
            get { return (string)GetValue(TrailingIconProperty); }
            set { SetValue(TrailingIconProperty, value); }
        }
        private static void TrailingIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = bindable as RSInputView;

            if (rsInput.TrailingIconImage != null)
            {
                rsInput.Remove(rsInput.TrailingIconImage);
                rsInput.TrailingIconImage = null;
            }

            if (string.IsNullOrEmpty((string)newValue))
            {
                // Adjust input control margin
                if (rsInput.graphicsDrawable != null)
                {
                    rsInput.graphicsDrawable.SetIconMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
                    rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
                }
                rsInput.Graphics.Invalidate();
                return;
            }

            rsInput.TrailingIconImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Source = rsInput.TrailingIcon,
            };

            rsInput.TrailingIconImage.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = rsInput.IconCommand,
                CommandParameter = rsInput.TrailingIconImage
            });

            rsInput.TrailingIconImage.SetBinding(Image.WidthRequestProperty, new Binding("IconWidthRequest", source: rsInput));
            rsInput.TrailingIconImage.SetBinding(Image.HeightRequestProperty, new Binding("IconHeightRequest", source: rsInput));

            if (rsInput.graphicsDrawable != null)
            {
                rsInput.graphicsDrawable.SetIconMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
                rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
            }
            rsInput.Add(rsInput.TrailingIconImage, 0, 0);

            rsInput.Graphics.Invalidate();
        }
        internal Image TrailingIconImage { get; set; }

        public static readonly BindableProperty TrailingIconCommandProperty = BindableProperty.Create(nameof(TrailingIconCommand), typeof(ICommand), typeof(RSInputView), null);
        public ICommand TrailingIconCommand
        {
            get { return (ICommand)GetValue(TrailingIconCommandProperty); }
            set { SetValue(TrailingIconCommandProperty, value); }
        }
        public static readonly BindableProperty TrailingIconCommandParameterProperty = BindableProperty.Create(nameof(TrailingIconCommandParameter), typeof(object), typeof(RSInputView), null);
        public object TrailingIconCommandParameter
        {
            get { return (object)GetValue(TrailingIconCommandParameterProperty); }
            set { SetValue(TrailingIconCommandParameterProperty, value); }
        }


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


        public static readonly BindableProperty HasDropDownIconProperty = BindableProperty.Create(nameof(HasDropDownIcon), typeof(bool), typeof(RSInputView), false, propertyChanged: HasDropDownIconChanged);
        public bool HasDropDownIcon
        {
            get { return (bool)GetValue(HasDropDownIconProperty); }
            set { SetValue(HasDropDownIconProperty, value); }
        }
        private static void HasDropDownIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSInputView).Graphics.Invalidate();
        }

        public static readonly BindableProperty HasClearIconProperty = BindableProperty.Create(nameof(HasClearIcon), typeof(bool), typeof(RSInputView), false, propertyChanged: HasClearIconChanged);
        public bool HasClearIcon
        {
            get { return (bool)GetValue(HasClearIconProperty); }
            set { SetValue(HasClearIconProperty, value); }
        }
        private static void HasClearIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);
           rsInput.Graphics.Invalidate();
        }
        private void ClearText()
        {
            if (Content is Picker)
                (Content as Picker).SelectedItem = null;
            else if (Content is InputView)
                (Content as InputView).Text = string.Empty;
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


        public static readonly BindableProperty HelperMessageProperty = BindableProperty.Create(nameof(HelperMessage), typeof(string), typeof(RSInputView), string.Empty, propertyChanged: HelperMessageChanged);
        public string HelperMessage
        {
            get { return (string)GetValue(HelperMessageProperty); }
            set { SetValue(HelperMessageProperty, value); }
        }
        private static void HelperMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);
            rsInput.SetBottomMessageMargin(rsInput);
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(RSInputView), string.Empty, propertyChanged: ErrorMessageChanged);
        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        private static void ErrorMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);
            if (!rsInput.ErrorMessageEnabled)
                return;

            if (string.IsNullOrEmpty(newValue?.ToString()))
                rsInput.GetCharacterCounterString();

            rsInput.SetBottomMessageMargin(rsInput);
            (bindable as RSInputView).Graphics.Invalidate();
        }


        public static readonly BindableProperty ErrorMessageEnabledProperty = BindableProperty.Create(nameof(ErrorMessageEnabled), typeof(bool), typeof(RSInputView), false, propertyChanged: ErrorMessageEnabledChanged);
        public bool ErrorMessageEnabled
        {
            get { return (bool)GetValue(ErrorMessageEnabledProperty); }
            set { SetValue(ErrorMessageEnabledProperty, value); }
        }
        private static void ErrorMessageEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);
            rsInput.SetBottomMessageMargin(rsInput);
            (bindable as RSInputView).Graphics.Invalidate();
        }


        /// <summary>
        /// Adds bottom margin so that the bottom message can be seen
        /// </summary>
        /// <param name="rsInput"></param>
        internal void SetBottomMessageMargin(RSInputView rsInput)
        {
            var Graphics = rsInput.Graphics;
            var graphicsDrawable = rsInput.graphicsDrawable;
            int multiplier = 1;
            double bottomMarging = 0;
            SizeF size = SizeF.Zero;
            float CharacterCountSize = 0;
            bool isCharacterCount = false;

            if (graphicsDrawable == null)
                return;

            string message = string.Empty;

            if (rsInput.ErrorMessageEnabled && !string.IsNullOrEmpty(rsInput.ErrorMessage))
                message = rsInput.ErrorMessage;
            else if (!string.IsNullOrEmpty(rsInput.HelperMessage))
                message = rsInput.HelperMessage;
            else
            {
                isCharacterCount = true;
                message = rsInput.characterCounterString;
            }

            if (!isCharacterCount)
                CharacterCountSize = graphicsDrawable.GetCanvasStringSize(graphicsDrawable.Canvas, rsInput.characterCounterString, rsInput.graphicsDrawable.TextFont, rsInput.graphicsDrawable.FontSize).Width + graphicsDrawable.PlaceholderMargin.Right;

            size = graphicsDrawable.GetCanvasStringSize(graphicsDrawable.Canvas, message, rsInput.graphicsDrawable.TextFont, rsInput.graphicsDrawable.FontSize);
            multiplier = (int)Math.Floor(size.Width / (Graphics.Width - graphicsDrawable.PlaceholderMargin.Left - graphicsDrawable.PlaceholderMargin.Right - CharacterCountSize) + 1);
            bottomMarging = size.Width > 0 ? size.Height * multiplier + graphicsDrawable.messageSpacing : 0;

            graphicsDrawable.SetIconMargin(bottomMarging);
            graphicsDrawable.SetContentMargin(bottomMarging);
            graphicsDrawable.SetBorderMargin((float)bottomMarging);
            graphicsDrawable.SetPlaceholderMargin((float)bottomMarging);
        }


        public static readonly BindableProperty CharacterCounterProperty = BindableProperty.Create(nameof(CharacterCounter), typeof(int), typeof(RSInputView), (int)-1, propertyChanged: CharacterCounterChanged);
        public int CharacterCounter
        {
            get { return (int)GetValue(CharacterCounterProperty); }
            set { SetValue(CharacterCounterProperty, value); }
        }
        internal string characterCounterString;
        internal string GetCharacterCounterString()
        {
            if (CharacterCounter < 0)
                return string.Empty;

            string result = string.Empty;
            int count = 0;

            if (Content is InputView)
                count = (Content as InputView).Text != null ? (Content as InputView).Text.Length : 0;
            else if (Content is Picker)
                count = (Content as Picker).SelectedItem != null ? (Content as Picker).SelectedItem.ToString().Length : 0;


            if (count > CharacterCounter)
            {
                ErrorMessageEnabled = true;
                if (string.IsNullOrEmpty(ErrorMessage))
                    ErrorMessage = $"Error : Max 20 characters";
            }
            else if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessageEnabled = false;
            }

            result = $"{count} / {CharacterCounter}";

            return result;
        }
        private static void CharacterCounterChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);

            if ((int)newValue >= 0)
                rsInput.characterCounterString = $"0 / {newValue}";
            else
            {
                rsInput.characterCounterString = string.Empty;
                rsInput.ErrorMessageEnabled = false;
            }

            rsInput.SetBottomMessageMargin(rsInput);
            (bindable as RSInputView).Graphics.Invalidate();
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


        public static readonly BindableProperty PrefixProperty = BindableProperty.Create(nameof(Prefix), typeof(object), typeof(RSInputView), null, propertyChanged: PrefixChanged);
        public object Prefix
        {
            get { return (object)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }
        private static void PrefixChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);

            if (rsInput.graphicsDrawable == null)
                return;

            rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
            rsInput.Graphics.Invalidate();
        }

        public static readonly BindableProperty SuffixProperty = BindableProperty.Create(nameof(Suffix), typeof(object), typeof(RSInputView), null, propertyChanged: SuffixChanged);
        public object Suffix
        {
            get { return (object)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }
        private static void SuffixChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);

            if (rsInput.graphicsDrawable == null)
                return;

            rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
            rsInput.Graphics.Invalidate();
        }

        public static readonly BindableProperty PrefixSpacingProperty = BindableProperty.Create(nameof(PrefixSpacing), typeof(float), typeof(RSInputView), 1f, propertyChanged: PrefixSpacingChanged);
        public float PrefixSpacing
        {
            get { return (float)GetValue(PrefixSpacingProperty); }
            set { SetValue(PrefixSpacingProperty, value); }
        }
        private static void PrefixSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);

            if (rsInput.graphicsDrawable == null)
                return;

            rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
            rsInput.Graphics.Invalidate();
        }

        public static readonly BindableProperty SuffixSpacingSpacingProperty = BindableProperty.Create(nameof(SuffixSpacing), typeof(float), typeof(RSInputView), 1f, propertyChanged: SuffixSpacingChanged);
        public float SuffixSpacing
        {
            get { return (float)GetValue(SuffixSpacingSpacingProperty); }
            set { SetValue(SuffixSpacingSpacingProperty, value); }
        }
        private static void SuffixSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rsInput = (bindable as RSInputView);

            if (rsInput.graphicsDrawable == null)
                return;

            rsInput.graphicsDrawable.SetContentMargin(rsInput.graphicsDrawable.BorderMargin.Bottom);
            rsInput.Graphics.Invalidate();
        }

        internal double LeadingIconTotalWidth
        {
            get
            {
                return LeadingIconImage == null ? 0 : IconWidthRequest + LeadingIconImage.Margin.Left;
            }
        }

        internal double TrailingIconTotalWidth
        {
            get
            {
                return TrailingIconImage == null ? 0 : IconWidthRequest + TrailingIconImage.Margin.Right;
            }
        }

        internal bool IsClearIconVisible
        {
            get
            {
                bool res = false;

                //if (!HasClearIcon)
                //    res = false;

                if (Content is Picker && HasClearIcon)
                {
                    if ((Content as Picker).SelectedItem != null)
                        res = true;
                }
                else if (Content is InputView && HasClearIcon)
                {
                    if (!string.IsNullOrEmpty((Content as InputView).Text))
                        res = true;
                }

                // Hide or show TrailingIcon
                if (TrailingIconImage != null)
                    TrailingIconImage.IsVisible = res ? false : true;

                return res;
            }
        }

        private ICommand IconCommand { get; set; }

        public bool IsActive { get; protected set; }

        public Thickness ContentMargin { get; internal set; }

        private GraphicsDrawable graphicsDrawable;

        public GraphicsView Graphics { get; set; }


        public RSInputView()
        {
            // Main Grid
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            Graphics = new GraphicsView();
            Graphics.EndInteraction += Graphics_EndInteraction;
            //Graphics.GestureRecognizers.Add(new TapGestureRecognizer()
            //{
            //    Command = new Command( () =>
            //    {
            //        if(!Content.IsFocused) 
            //            Content.Focus();
            //    })
            //});

            this.Add(Graphics, 0, 0);

            IconCommand = new Command<Image>(IconCommandInvoke);
        }

        private void Graphics_EndInteraction(object sender, TouchEventArgs e)
        {
            var touchLocation = e.Touches.Last();

            if (touchLocation.X >= this.Width - TrailingIconTotalWidth && IsClearIconVisible)
                ClearText();
            else
            {
                if (!Content.IsFocused)
                    Content.Focus();
            }
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
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif __IOS__ || __MACCATALYST__
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.Style = null;
#endif
                }
            });

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("BorderlessEditor", (handler, view) =>
            {
                if (view == Content)
                {
#if __ANDROID__
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
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
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
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
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
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
            Graphics.Drawable = graphicsDrawable;
            //Graphics.SetBinding(View.HeightRequestProperty, new Binding("Height", converter: new HeightConverter(), source: Content));
            isDesignSet = true;

            this.Add(Content, 0, 0);
            Content.VerticalOptions = LayoutOptions.Center;
            Content.Focused += Content_Focused;
            Content.Unfocused += Content_Unfocused;
            Content.PropertyChanged += Content_PropertyChanged;

            if (Content is InputView)
                (Content as InputView).PlaceholderColor = Colors.Transparent;

            // Must enable this, otherwise there is graphical bug when margin is applied to Editor
            if (Content is Editor)
                (Content as Editor).AutoSize = EditorAutoSizeOption.TextChanges;

            if(Content is Picker && TrailingIconImage == null)
            {
                HasDropDownIcon = true;
                TrailingIconImage = new Image() { WidthRequest = IconWidthRequest, HeightRequest = IconHeightRequest };
            }
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
                if (graphicsDrawable.FontSize != (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize)
                {
                    graphicsDrawable.FontSize = (float)(sender as Microsoft.Maui.Controls.Internals.IFontElement).FontSize;
                    Graphics.Invalidate();
                }
            }
            else if (e.PropertyName == nameof(Entry.Text) || e.PropertyName == nameof(Picker.SelectedItem) || e.PropertyName == nameof(Picker.SelectedIndex))
            {
                if(CharacterCounter >= 0) 
                    characterCounterString = GetCharacterCounterString();

                Graphics.Invalidate();
            }
            else if (e.PropertyName == nameof(View.Margin))
            {
                // Prevent margin change from user side
                if (Content.Margin != ContentMargin)
                    Content.Margin = ContentMargin;

                //Graphics.Invalidate();
            }
        }

        private async void IconCommandInvoke(Image image)
        {
            if (image == LeadingIconImage)
            {
                if (LeadingIconCommand == null)
                {
                    Content.Focus();
                    return;
                }
            }
            else if (image == TrailingIconImage)
            {
                if (TrailingIconCommand == null)
                {
                    Content.Focus();
                    return;
                }
            }

            image.Opacity = 0.5;
            await image.ScaleTo(0.8, 100);
            await image.ScaleTo(1, 100);
            image.Opacity = 1;

            TrailingIconCommand?.Execute(TrailingIconCommandParameter);
            LeadingIconCommand?.Execute(LeadingIconCommandParameter);
        }

        internal bool IsFloating()
        {
            if (Content == null)
                return false;

            if (Content.IsFocused)
                return true;

            if (Content is InputView)
            {
                if (!string.IsNullOrEmpty((Content as InputView).Text))
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
            if (Content is InputView)
            {
                if (!string.IsNullOrEmpty((Content as InputView).Text))
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
            Graphics.EndInteraction -= Graphics_EndInteraction;
            Content.Focused -= Content_Focused;
            Content.Unfocused -= Content_Unfocused;
            Content.PropertyChanged -= Content_PropertyChanged;
        }
    }
}