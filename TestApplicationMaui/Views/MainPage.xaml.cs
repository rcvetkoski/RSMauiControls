using RSInputViewMaui;
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
    }


    public class RSBasePicker : Picker
    {

    }


    public class RSEnumPicker : RSBasePicker
    {

    }
}