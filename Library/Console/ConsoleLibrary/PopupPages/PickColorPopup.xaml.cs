using ConsoleLibrary.Views;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace ConsoleLibrary.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickColorPopup : PopupPage
    {
        public static readonly BindableProperty StyleSliderProperty = BindableProperty.Create(nameof(StyleSliderProperty), typeof(Style), typeof(PickColorPopup));
        public static readonly BindableProperty StyleFrameContentProperty = BindableProperty.Create(nameof(StyleFrameContent), typeof(Style), typeof(PickColorPopup));
        public static readonly BindableProperty StyleFrameColorViewProperty = BindableProperty.Create(nameof(StyleFrameColorView), typeof(Style), typeof(PickColorPopup));
        public static readonly BindableProperty StyleRootStackLayoutProperty = BindableProperty.Create(nameof(StyleRootStackLayout), typeof(Style), typeof(PickColorPopup));
        public static readonly BindableProperty StyleLabelProperty = BindableProperty.Create(nameof(StyleLabel), typeof(Style), typeof(PickColorPopup));
        public Style StyleLabel
        {
            get { return (Style)GetValue(StyleLabelProperty); }
            set { SetValue(StyleLabelProperty, value); }
        }
        public Style StyleSlider
        {
            get { return (Style)GetValue(StyleSliderProperty); }
            set { SetValue(StyleSliderProperty, value); }
        }
        public Style StyleFrameContent
        {
            get { return (Style)GetValue(StyleFrameContentProperty); }
            set { SetValue(StyleFrameContentProperty, value); }
        }
        public Style StyleFrameColorView
        {
            get { return (Style)GetValue(StyleFrameColorViewProperty); }
            set { SetValue(StyleFrameColorViewProperty, value); }
        }
        public Style StyleRootStackLayout
        {
            get { return (Style)GetValue(StyleRootStackLayoutProperty); }
            set { SetValue(StyleRootStackLayoutProperty, value); }
        }

        private PickColor _uiElement { get; set; }
        public PickColorPopup(PickColor uiElement)
        {
            _uiElement = uiElement;
            InitializeComponent();
            ViewColor.SetBinding(BackgroundColorProperty, new Binding(nameof(uiElement.Color), source: _uiElement));
            BindingContext = this;
            System.Drawing.Color color = _uiElement.Color;
            TextParamsColorLabelEdit(color);
            RSlider.Value = color.R;
            GSlider.Value = color.G;
            BSlider.Value = color.B;
            ASlider.Value = color.A;
            RSlider.ValueChanged += Slider_ValueChanged;
            GSlider.ValueChanged += Slider_ValueChanged;
            BSlider.ValueChanged += Slider_ValueChanged;
            ASlider.ValueChanged += Slider_ValueChanged;
        }
        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e) => UpdateColor(System.Drawing.Color.FromArgb((int)ASlider.Value, (int)RSlider.Value, (int)GSlider.Value, (int)BSlider.Value));

        private void UpdateColor(System.Drawing.Color color)
        {
            _uiElement.Color = color;
            TextParamsColorLabelEdit(color);
        }
        private void TextParamsColorLabelEdit(System.Drawing.Color color)
        {
            RLabel.Text = $"R {color.R}";
            GLabel.Text = $"G {color.G}";
            BLabel.Text = $"B {color.B}";
            ALabel.Text = $"A {color.A}";
        }
    }
}