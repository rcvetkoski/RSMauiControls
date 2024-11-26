using TestApplicationMaui.Models;
using TestApplicationMaui.ViewModels;

namespace TestApplicationMaui.Views;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }

    private void carrousel_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
		var ratio = 105 / 411.42;
		slider.TranslationX += e.HorizontalDelta * ratio;
    }
}