namespace RSPopupMaui
{
    // All the code in this file is included in all platforms.
    public class RSPopup : ContentPage
    {
        private Grid holder { get; set; }
        private Frame popup { get; set; }

        private Color lightBackgroundColor = Colors.White;
        private Color darkBackgroundColor = Color.FromHex("#212121");

        public RSPopup(IView view)
        {
            this.Loaded += RSPopup_Loaded;
            this.BackgroundColor = Color.FromHex("#aa000000");

            holder = new Grid()
            {
                BackgroundColor = Colors.Transparent
            };

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await DismissPopup();
                    await Shell.Current.Navigation.PopAsync(false);
                })
            };
            holder.GestureRecognizers.Add(tapGestureRecognizer);

            popup = new Frame()
            {
                CornerRadius = 15,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HasShadow = true,
                Margin = new Thickness(30)
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
            await AnimatePopup();
        }

        protected override bool OnBackButtonPressed()
        {
            ClosePopup();
            return true;
        }

        private async void ClosePopup()
        {
            await DismissPopup();
            await Shell.Current.Navigation.PopAsync(false);
        }

        public Task AnimatePopup()
        {
            var done = new TaskCompletionSource<bool>();

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

            return done.Task;
        }

        public Task DismissPopup()
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
    }
}
