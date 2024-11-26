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
        }

        private async void GoToTestPage(object sender, EventArgs e)
        {
            TestPage testPage = new TestPage();
            var vm = testPage.BindingContext as TestViewModel;
            vm.People = (this.BindingContext as MainViewModel).People;
            vm.SelectedPerson = (this.BindingContext as MainViewModel).SelectedPerson;

           
            await Shell.Current.Navigation.PushAsync(testPage);
        }
    }
}