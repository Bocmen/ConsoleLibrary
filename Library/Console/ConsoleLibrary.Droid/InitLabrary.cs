using Android.App;
using Android.OS;

namespace ConsoleLibrary.Platform.Droid
{
    public static class InitLabrary
    {
        public static void Init(Activity activity, Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(activity, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            Rg.Plugins.Popup.Popup.Init(activity);
        }

        public static bool SendBackPressed(System.Action action) => Rg.Plugins.Popup.Popup.SendBackPressed(action);
    }
}