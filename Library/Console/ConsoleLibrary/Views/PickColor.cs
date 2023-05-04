using ConsoleLibrary.PopupPages;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace ConsoleLibrary.Views
{
    public class PickColor : Frame
    {
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(PickColor), Color.Black);
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public PickColor() : this(null) { }

        public PickColor(Color? defaultColor = null)
        {
            if (defaultColor != null) Color = (Color)defaultColor;
            SetBinding(BackgroundColorProperty, new Binding(nameof(Color), source: this));
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            GestureRecognizers.Add(tapGestureRecognizer);
        }
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new PickColorPopup(this));
        }
    }
}
