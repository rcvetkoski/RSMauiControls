using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSPopupMaui
{
    public class RSpopupManager
    {
        private static RSPopup rSPopup;

        public static void ShowPopup(IView view)
        {
            rSPopup = new RSPopup(view);
            Shell.Current.Navigation.PushModalAsync(rSPopup, false);
        }

        public static async void ClosePopup()
        {
            if (rSPopup == null)
                return;

            await rSPopup.PoppingOut();
            await Shell.Current.Navigation.PopAsync(false);
        }
    }
}
