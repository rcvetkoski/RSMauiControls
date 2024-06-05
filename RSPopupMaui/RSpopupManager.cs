namespace RSPopupMaui
{
    public class RSpopupManager
    {
        private static RSPopup rSPopup;

        public static void ShowPopup(IView view, bool isModal = false)
        {
            rSPopup = new RSPopup(view, isModal);
            rSPopup.PopupClosedInternal += RSPopup_PopupClosedInternal;
            Shell.Current.Navigation.PushModalAsync(rSPopup, false);
        }

        private static void RSPopup_PopupClosedInternal(object? sender, EventArgs e)
        {
            rSPopup.PopupClosedInternal -= RSPopup_PopupClosedInternal;
            OnPopupClosed(e);
        }

        public static async Task ClosePopup()
        {
            if (rSPopup == null)
                return;

            await rSPopup.DismissPopup();
            await Shell.Current.Navigation.PopAsync(false);
        }

        public static event EventHandler PopupClosed;
        protected static void OnPopupClosed(EventArgs e)
        {
            PopupClosed?.Invoke(null, e);
        }
    }
}
