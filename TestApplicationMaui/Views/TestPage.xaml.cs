namespace TestApplicationMaui.Views;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        chartLine.StartAnimation(1);
        chartSpline.StartAnimation(1);
    }
}