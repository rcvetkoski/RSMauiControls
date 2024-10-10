using Microsoft.Maui.Controls.Shapes;

namespace RSPopupMaui
{
    // All the code in this file is included in all platforms.
    public class RSPopup : ContentPage
    {
        private Grid holder { get; set; }
        private Border popup { get; set; }
        private bool isModal;
        private Color lightBackgroundColor = Colors.White;
        private Color darkBackgroundColor = Color.FromRgba("#212121");
        private RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum;
        private PanGestureRecognizer panGesture;

        public RSPopup(IView view, RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum, bool isModal)
        {
            this.Loaded += RSPopup_Loaded;
            //this.BackgroundColor = Color.FromHex("#aa000000");
            this.BackgroundColor = Colors.Transparent;
            this.isModal = isModal;
            this.rSPopupAnimationTypeEnum = rSPopupAnimationTypeEnum;

            holder = new Grid()
            {
                BackgroundColor = Colors.Transparent
            };

            if(!isModal)
            {
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        ClosePopup();
                    })
                };
                holder.GestureRecognizers.Add(tapGestureRecognizer);
            }

            LayoutOptions horizontalAlignement = rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect ? LayoutOptions.Center : LayoutOptions.Fill;
            LayoutOptions verticalAlignement = rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect ? LayoutOptions.Center : LayoutOptions.End;
            Thickness margin = rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect ? new Thickness(30) : new Thickness(0);
            RoundRectangle StrokeShape = rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect ?
                                                                     new RoundRectangle { CornerRadius = new CornerRadius(10, 10, 10, 10) } :
                                                                     new RoundRectangle { CornerRadius = new CornerRadius(25, 25, 0, 0) };
            Thickness padding = rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect ? new Thickness(20) : new Thickness(20, 10, 20, 20);

            popup = new Border()
            {
                StrokeThickness = 0,
                Padding = padding,
                StrokeShape = StrokeShape,
                VerticalOptions = verticalAlignement,
                HorizontalOptions = horizontalAlignement,
                Margin = margin
            };


            if(rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.BottomToTop)
            {
                panGesture = new PanGestureRecognizer();
                panGesture.PanUpdated += PanGesture_PanUpdated;
                popup.GestureRecognizers.Add(panGesture);
            }

            Grid content = null;
            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.BottomToTop)
            {
                content = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                    RowSpacing = 15
                };

                BoxView slider = new BoxView()
                {
                    Color = Colors.Gray,
                    WidthRequest = 40,
                    HeightRequest = 4,
                    CornerRadius = 20
                };

                content.Add(slider, 0, 0);
                content.Add((View)view, 0, 1);
            }


            holder.Children.Add(popup);
            popup.Content = rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect ? (View)view : content;
            Content = holder;

            ApplyThemeSpecificStyleToFrame(popup);

            // Listen for theme changes
            Application.Current.RequestedThemeChanged += (s, e) =>
            {
                ApplyThemeSpecificStyleToFrame(popup);
            };
        }

        private void PanGesture_PanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    // Initialize the current Y translation when the pan starts
                    break;
                case GestureStatus.Running:
                    // Apply translaytion y stop at 0
                    popup.TranslationY = Math.Max(0, popup.TranslationY += e.TotalY);
                    break;
                case GestureStatus.Completed:
                    // Check if the popup was dragged beyond the threshold
                    if (popup.TranslationY > 55)
                    {
                        // Close the popup (you can customize how to close the popup)
                        ClosePopup();
                    }
                    else
                    {
                        // Reset the popup back to its original position
                        popup.TranslateTo(0, 0, 250, Easing.Linear);
                        //popup.TranslationY = 0;
                    }
                    break;
            }
        }

        public void ApplyThemeSpecificStyleToFrame(Border border)
        {
            switch (Application.Current.PlatformAppTheme)
            {
                case AppTheme.Light:
                    border.BackgroundColor = lightBackgroundColor;
                    break;
                case AppTheme.Dark:
                    border.BackgroundColor = darkBackgroundColor;
                    break;
                default:
                    // Optionally handle AppTheme.Unspecified
                    break;
            }
        }

        private async void RSPopup_Loaded(object? sender, EventArgs e)
        {
            this.Loaded -= RSPopup_Loaded;

            // Call open method
            await OpenAnimatePopup();
        }

        protected override bool OnBackButtonPressed()
        {
            if(!isModal)
                ClosePopup();

            return true;
        }

        private async void ClosePopup()
        {
            await CloseAnimatePopup();
            await Shell.Current.Navigation.PopAsync(false);
            OnPopupClosedInternal(EventArgs.Empty);
        }

        public Task OpenAnimatePopup()
        {
            var done = new TaskCompletionSource<bool>();

            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect)
                AnimatePopupPopInEffect(done);
            else
                AnimatePopupFromBottom(done);

            return done.Task;
        }

        public Task CloseAnimatePopup()
        {
            var done = new TaskCompletionSource<bool>();

            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect)
                DismissPopupPopInEffect(done);
            else
                DismissPopupFromBottom(done);

            if(panGesture != null)
                panGesture.PanUpdated -= PanGesture_PanUpdated;

            return done.Task;
        }

        public void AnimatePopupPopInEffect(TaskCompletionSource<bool> done)
        {
            // Fade in the background to dim
            this.Animate("BackgroundFadeIn",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0d,
                end: 0.4d, // Semi-transparent
                rate: 16,
                length: 250,
                easing: Easing.Linear);

            // Scale up content from a smaller size for the 'pop' effect
            var contentStartScale = 0.8;
            var contentEndScale = 1.0;
            this.Content.Opacity = 0; // Start fully transparent
            this.Content.Scale = contentStartScale; // Start scaled down
            this.Content.Animate("ContentFadeIn",
                new Animation(
                    v => this.Content.Opacity = v,
                    start: 0,
                    end: 1,
                    easing: Easing.CubicInOut),
                16, 250);
            this.Content.Animate("ContentScaleIn",
                new Animation(
                    v => this.Content.Scale = v,
                    start: contentStartScale,
                    end: contentEndScale,
                    easing: Easing.SpringOut),
                16, 250, finished: (v, c) => done.SetResult(true));
        }

        public void AnimatePopupFromBottom(TaskCompletionSource<bool> done)
        {
            // Fade in the background to dim
            this.Animate("BackgroundFadeIn",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0d,
                end: 0.4d, // Semi-transparent
                rate: 16,
                length: 250,
                easing: Easing.Linear);

            double startY = popup.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins).Request.Height;

            // Start fully transparent and off-screen (from the bottom)
            this.Content.TranslationY = startY; // Off-screen, adjust this based on the screen height

            // Move the content from the bottom upwards
            this.Content.Animate("ContentMoveUp",
                new Animation(
                    v => this.Content.TranslationY = v,
                    start: startY, // Starts from the bottom
                    end: 0, // Ends in its original position
                    easing: Easing.CubicInOut),
                16, 250, finished: (v, c) => done.SetResult(true));
        }

        public void DismissPopupFromBottom(TaskCompletionSource<bool> done)
        {
            // Fade out the background
            this.Animate("BackgroundFadeOut",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0.4d,
                end: 0d, // Fully transparent
                rate: 16,
                length: 250, // Slightly longer duration
                easing: Easing.Linear);


            double startY = popup.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins).Request.Height;

            // Start fully transparent and off-screen (from the bottom)
            this.Content.TranslationY = startY; // Off-screen, adjust this based on the screen height

            // Move the content from the bottom upwards
            this.Content.Animate("ContentMoveUp",
                new Animation(
                    v => this.Content.TranslationY = v,
                    start: 0, // Starts from the bottom
                    end: startY, // Ends in its original position
                    easing: Easing.CubicInOut),
                16, 250, finished: (v, c) => done.SetResult(true));
        }

        public void DismissPopupPopInEffect(TaskCompletionSource<bool> done)
        {
            // Fade out the background
            this.Animate("BackgroundFadeOut",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0.4d,
                end: 0d, // Fully transparent
                rate: 16,
                length: 250, // Slightly longer duration
                easing: Easing.Linear);

            // Scale down content slightly and fade out with adjusted timing and easing
            var contentEndScale = 0.8;
            this.Content.Animate("ContentFadeOut",
                new Animation(
                    v => this.Content.Opacity = v,
                    start: 1,
                    end: 0,
                    easing: Easing.CubicInOut),
                16, 300); // Matched duration with the background fade out for synchrony
            this.Content.Animate("ContentScaleOut",
                new Animation(
                    v => this.Content.Scale = v,
                    start: 1.0,
                    end: contentEndScale,
                    easing: Easing.CubicInOut), // Adjusted easing for a smoother transition
                16, 300, finished: (v, c) => done.SetResult(true)); // Adjusted duration to match fade-out
        }

        public event EventHandler PopupClosedInternal;
        protected void OnPopupClosedInternal(EventArgs e)
        {
            PopupClosedInternal?.Invoke(null, e);
        }
    }
}
