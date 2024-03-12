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
    }


    public class RSBasePicker : Picker
    {

    }


    public class RSEnumPicker : RSBasePicker
    {

    }
}