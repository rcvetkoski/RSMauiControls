namespace TestApplicationMaui.Views;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }

    //private double ChildWidth = 415;
    //private double _gridWidth = 830;      
    //private double currentTranslation = 0;
    //int currentIndex = 0;

    //private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    //{
    //    Grid MyGrid = sender as Grid;
    //    ChildWidth = grid.Width;
    //    _gridWidth = ChildWidth * 2;
    //    MyGrid.WidthRequest = MyGrid.WidthRequest != (ChildWidth * 3) ? (ChildWidth * 3) : MyGrid.WidthRequest;

    //    switch (e.StatusType)
    //    {
    //        case GestureStatus.Started:
    //            // Save the starting position when the gesture begins
    //            currentTranslation = MyGrid.TranslationX;
    //            break;

    //        case GestureStatus.Running:
    //            // Directly calculate new translation with bounds
    //            Console.WriteLine($"Running  {e.TotalX}  /  {MyGrid.TranslationX}");

    //            currentTranslation += e.TotalX;
    //            if (currentTranslation <= 0 && currentTranslation >= -_gridWidth)
    //            {
    //                MyGrid.TranslationX = currentTranslation;
    //            }

    //            break;

    //        case GestureStatus.Completed:
    //            // Optional: Snap to nearest column for a clean stop

    //            Console.WriteLine($"Completed  {e.TotalX}  /  {MyGrid.TranslationX}");
    //            currentTranslation += e.TotalX;


    //            double minThreshold = -(currentIndex * ChildWidth) + (ChildWidth * 0.25);
    //            double maxThreshold = -(currentIndex * ChildWidth) - (ChildWidth * 0.25);

    //            if (currentIndex > 0 && currentTranslation >= minThreshold)
    //            {
    //                currentIndex--;
    //            }
    //            else if (currentIndex < 2 && currentTranslation <= maxThreshold)
    //            {
    //                currentIndex++;
    //            }

    //            await MyGrid.TranslateTo(-(currentIndex * ChildWidth), 0, 200, Easing.Linear);


    //            //if (currentTranslation > 0)
    //            //    MyGrid.TranslationX = 0;
    //            //else if (currentTranslation < -_gridWidth)
    //            //    MyGrid.TranslationX = -_gridWidth;

    //            break;
    //    }
    //}
}