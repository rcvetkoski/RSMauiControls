namespace RSPopupMaui
{
    public class RSpopupManager
    {
        public static List<RSPopup> PopupStack = new List<RSPopup>();

        public static RSPopup GetCurrentPopup()
        {
            return PopupStack.LastOrDefault();
        }

        public static void ShowPopup(IView view, RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum = RSPopupAnimationTypeEnum.PopInEffect, bool isModal = false)
        {
            RSPopup rSPopup = new RSPopup(view, rSPopupAnimationTypeEnum, isModal);
            PopupStack.Add(rSPopup);
            rSPopup.PopupClosed += RsPopup_PopupClosed;
            Shell.Current.Navigation.PushModalAsync(rSPopup, animated: false);
        }

        private static void RsPopup_PopupClosed(object? sender, EventArgs e)
        {
            (sender as RSPopup).PopupClosed -= RsPopup_PopupClosed;
            PopupStack.Remove(sender as RSPopup);
        }

        public static async void ClosePopup()
        {
            RSPopup currentPopup = GetCurrentPopup();
            if (currentPopup != null)
            {
                PopupStack.Remove(currentPopup);
                currentPopup.ClosePopup();
            }
        }
    }
}
