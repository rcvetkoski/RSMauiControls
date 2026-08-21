namespace RSPopupMaui;

public class RSpopupManager
{
    public static List<RSPopup> PopupStack = new();

    public static RSPopup? GetCurrentPopup()
    {
        return PopupStack.LastOrDefault();
    }

    public static async void ShowPopup(
        IView view,
        RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum = RSPopupAnimationTypeEnum.PopInEffect,
        bool isModal = false)
    {
        var popup = new RSPopup(view, rSPopupAnimationTypeEnum, isModal);
        PopupStack.Add(popup);
        popup.PopupClosed += OnPopupClosed;

        try
        {
            await Shell.Current.Navigation.PushModalAsync(popup, animated: false);
        }
        catch
        {
            popup.PopupClosed -= OnPopupClosed;
            PopupStack.Remove(popup);
            throw;
        }
    }

    public static async Task ClosePopup()
    {
        var popup = GetCurrentPopup();
        if (popup is not null)
            await popup.ClosePopup();
    }

    private static void OnPopupClosed(object? sender, EventArgs e)
    {
        if (sender is not RSPopup popup)
            return;

        popup.PopupClosed -= OnPopupClosed;
        PopupStack.Remove(popup);
    }
}
