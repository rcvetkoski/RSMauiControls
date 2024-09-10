using RSInputViewMaui;
using RSPickerMaui;
using RSPopupMaui;
using TestApplicationMaui.ViewModels;

namespace TestApplicationMaui.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //for (int i = 0; i < 500; i++)
            //{
            //    stack.Add(new RSInputView()
            //    {
            //        Placeholder = "Placeholder",
            //        Helper = "Helper",
            //        Content = new Entry()
            //    });
            //}
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).Error = "Error message !";
        }

        private void Button_Clicked2(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).Error = "";
        }

        private void Button_Clicked3(object sender, EventArgs e)
        {
            //filledLabel.Text = filled.Height.ToString();
            //outlinedLabel.Text = outlined.Height.ToString();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).SelectedPeople.Add(new Models.Person() { Name = "Troll", Age = 65 });
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).People.Add(new Models.Person() { Name = "Senidah", Age = 38 });
        }

        private void grid_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void Grid_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {

        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).People.RemoveAt(0);
        }

        private void Button_Clicked_4(object sender, EventArgs e)
        {
            RSpopupManager.ShowPopup(new Button()
            {
                Text = "Close",
                Command = new Command(() =>
            {
                RSpopupManager.ClosePopup();
            })
            }, RSPopupAnimationTypeEnum.PopInEffect, true);
            RSpopupManager.PopupClosed += RSpopupManager_PopupClosed;
        }

        private void RSpopupManager_PopupClosed(object sender, EventArgs e)
        {
            RSpopupManager.PopupClosed -= RSpopupManager_PopupClosed;
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
            PoppingIn(sender as ContentPage);
        }

        public void PoppingIn(ContentPage page)
        {
            // Measure the actual content size
            var contentSize = page.Content.Measure(Window.Width, Window.Height, MeasureFlags.IncludeMargins);
            var contentHeight = contentSize.Request.Height;

            // Start by translating the content below / off screen
            page.Content.TranslationY = contentHeight;

            // Animate the translucent background, fading into view
            page.Animate("Background",
                callback: v => page.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0d,
                end: 0.7d,
                rate: 32,
                length: 350,
                easing: Easing.CubicOut,
                finished: (v, k) =>
                    page.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.7f)));

            // Also animate the content sliding up from below the screen
            page.Animate("Content",
                callback: v => page.Content.TranslationY = (int)(contentHeight - v),
                start: 0,
                end: contentHeight,
                length: 500,
                easing: Easing.CubicInOut,
                finished: (v, k) => page.Content.TranslationY = 0);
        }


        public Task PoppingOut(ContentPage page)
        {
            var done = new TaskCompletionSource();

            // Measure the content size so we know how much to translate
            var contentSize = page.Content.Measure(Window.Width, Window.Height, MeasureFlags.IncludeMargins);
            var windowHeight = contentSize.Request.Height;

            // Start fading out the background
            page.Animate("Background",
                callback: v => page.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v)),
                start: 0.7d,
                end: 0d,
                rate: 32,
                length: 350,
                easing: Easing.CubicIn,
                finished: (v, k) => page.Background = new SolidColorBrush(Colors.Black.WithAlpha(0.0f)));

            // Start sliding the content down below the bottom of the screen
            page.Animate("Content",
                callback: v => page.Content.TranslationY = (int)(windowHeight - v),
                start: windowHeight,
                end: 0,
                length: 500,
                easing: Easing.CubicInOut,
                finished: (v, k) =>
                {
                    page.Content.TranslationY = windowHeight;
                    // Important: Set our completion source to done!
                    done.TrySetResult();
                });

            // We return the task so we can wait for the animation to finish
            return done.Task;
        }

        private void RSPicker_CloseButtonPressed(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new TestPage());
        }

        private void OpenPopup(object sender, EventArgs e)
        {
            RSpopupManager.ShowPopup(new Label() { Text = "BlaBla", HeightRequest = 200, HorizontalOptions = LayoutOptions.Center }, RSPopupAnimationTypeEnum.BottomToTop);
        }

        private void OpenPopup2(object sender, EventArgs e)
        {
            RSpopupManager.ShowPopup(new Label() { Text = "BlaBla" });
        }
    }
}