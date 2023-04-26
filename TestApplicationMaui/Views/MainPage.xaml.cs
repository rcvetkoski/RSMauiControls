using TestApplicationMaui.ViewModels;

namespace TestApplicationMaui.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //for(int i = 0; i < 1; i++) 
            //{
            //    stack.Add(new RSInputView()
            //    {
            //        Placeholder = "Placeholder",
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
    }
}