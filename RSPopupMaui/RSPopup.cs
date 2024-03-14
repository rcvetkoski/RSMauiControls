namespace RSPopupMaui
{
    // All the code in this file is included in all platforms.
    public class RSPopup : ContentPage
    {
        private Grid holder { get; set; }
        private Frame popup { get; set; }

        public RSPopup(IView view)
        {
            this.Loaded += RSPopup_Loaded;
            this.BackgroundColor = Color.FromHex("#aa000000");

            holder = new Grid()
            {
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(30)
            };

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await PoppingOut();
                    await Shell.Current.Navigation.PopAsync(false);
                })
            };
            holder.GestureRecognizers.Add(tapGestureRecognizer);

            popup = new Frame()
            {
                CornerRadius = 15,
                BackgroundColor = Colors.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            holder.Children.Add(popup);
            popup.Content = (View)view;
            Content = holder;
        }

        private void RSPopup_Loaded(object? sender, EventArgs e)
        {
            this.Loaded -= RSPopup_Loaded;

            // Call open method
            PoppingIn();
        }

        protected override bool OnBackButtonPressed()
        {
            ClosePopup();
            return true;
        }

        private async void ClosePopup()
        {
            await PoppingOut();
            await Shell.Current.Navigation.PopAsync(false);
        }

        public void PoppingIn()
        {
            // Measure the actual content size
            var contentSize = this.Content.Measure(Window.Width, Window.Height, MeasureFlags.IncludeMargins);
            var contentHeight = contentSize.Request.Height;

            // Start by translating the content below / off screen
            this.Content.TranslationY = contentHeight;

            // Animate the translucent background, fading into view
            this.Animate("Background",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0d,
                end: 0.7d,
                rate: 32,
                length: 350,
                easing: Easing.CubicOut,
                finished: (v, k) =>
                    this.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.7f)));

            // Also animate the content sliding up from below the screen
            this.Animate("Content",
                callback: v => this.Content.TranslationY = (int)(contentHeight - v),
                start: 0,
                end: contentHeight,
                length: 500,
                easing: Easing.CubicInOut,
                finished: (v, k) => this.Content.TranslationY = 0);
        }

        public Task PoppingOut()
        {
            var done = new TaskCompletionSource();

            // Measure the content size so we know how much to translate
            var contentSize = this.Content.Measure(Window.Width, Window.Height, MeasureFlags.IncludeMargins);
            var windowHeight = contentSize.Request.Height;

            // Start fading out the background
            this.Animate("Background",
                callback: v => this.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0.7d,
                end: 0d,
                rate: 32,
                length: 350,
                easing: Easing.CubicIn,
                finished: (v, k) => this.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.0f)));

            // Start sliding the content down below the bottom of the screen
            this.Animate("Content",
                callback: v => this.Content.TranslationY = (int)(windowHeight - v),
                start: windowHeight,
                end: 0,
                length: 500,
                easing: Easing.CubicInOut,
                finished: (v, k) =>
                {
                    this.Content.TranslationY = windowHeight;
                    // Important: Set our completion source to done!
                    done.TrySetResult();
                });

            // We return the task so we can wait for the animation to finish
            return done.Task;
        }
    }
}
