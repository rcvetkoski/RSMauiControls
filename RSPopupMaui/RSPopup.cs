namespace RSPopupMaui
{
    // All the code in this file is included in all platforms.
    public class RSPopup : ContentPage
    {
        private Grid holder { get; set; }
        private Frame popup { get; set; }
        private bool isModal;
        private Color lightBackgroundColor = Colors.White;
        private Color darkBackgroundColor = Color.FromHex("#212121");
        private RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum;

        public RSPopup(IView view, RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum, bool isModal)
        {
            this.Loaded += RSPopup_Loaded;
            this.BackgroundColor = Color.FromHex("#aa000000");
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

            popup = new Frame()
            {
                CornerRadius = 15,
                VerticalOptions = verticalAlignement,
                HorizontalOptions = horizontalAlignement,
                HasShadow = true,
                Margin = margin
            };

            holder.Children.Add(popup);
            popup.Content = (View)view;
            Content = holder;

            ApplyThemeSpecificStyleToFrame(popup);

            // Listen for theme changes
            Application.Current.RequestedThemeChanged += (s, e) =>
            {
                ApplyThemeSpecificStyleToFrame(popup);
            };
        }

        public void ApplyThemeSpecificStyleToFrame(Frame frame)
        {
            frame.HasShadow = false;
            frame.CornerRadius = 8;

            switch (Application.Current.PlatformAppTheme)
            {
                case AppTheme.Light:
                    frame.BackgroundColor = lightBackgroundColor;
                    break;
                case AppTheme.Dark:
                    frame.BackgroundColor = darkBackgroundColor;
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
            await DismissPopupFromBottom();
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
                DismissPopupPopInEffect();
            else
                DismissPopupFromBottom();

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

        public Task DismissPopupFromBottom()
        {
            var done = new TaskCompletionSource<bool>();

            // Fade out the background
            this.Animate("BackgroundFadeOut",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0.4d, // Semi-transparent
                end: 0d, // Fully transparent
                rate: 16,
                length: 250,
                easing: Easing.Linear);

            // Scale down content and move it back to the bottom
            var contentStartScale = 1.0;
            var contentEndScale = 0.8; // Slightly smaller for the 'pop out' effect

            // Fade out the content
            this.Content.Animate("ContentFadeOut",
                new Animation(
                    v => this.Content.Opacity = v,
                    start: 1,
                    end: 0,
                    easing: Easing.CubicInOut),
                16, 250);

            // Move the content back down to the bottom
            this.Content.Animate("ContentMoveDown",
                new Animation(
                    v => this.Content.TranslationY = v,
                    start: 0, // Original position
                    end: 1000, // Off-screen at the bottom
                    easing: Easing.CubicInOut),
                16, 250);

            // Scale down the content for the 'pop out' effect
            this.Content.Animate("ContentScaleOut",
                new Animation(
                    v => this.Content.Scale = v,
                    start: contentStartScale,
                    end: contentEndScale,
                    easing: Easing.SpringOut),
                16, 250, finished: (v, c) => done.SetResult(true));

            return done.Task;
        }

        public Task DismissPopupPopInEffect()
        {
            var done = new TaskCompletionSource<bool>();

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

            return done.Task;
        }

        public event EventHandler PopupClosedInternal;
        protected void OnPopupClosedInternal(EventArgs e)
        {
            PopupClosedInternal?.Invoke(null, e);
        }
    }
}
