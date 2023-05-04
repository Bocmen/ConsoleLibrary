using System;

using Xamarin.Forms;
using Xamarin.Essentials;

namespace WorkInvoker.Styles
{
    public static class Themes
    {
        private static readonly string CurrentThemeNameProperty = $"{nameof(WorkInvoker)}_{nameof(Styles)}_{nameof(Themes)}_CurrentTheme";
        private const ThemeType DefaultTheme = ThemeType.Dark;
        public static readonly StyleSizesGenerator DefaultSizeGenerator;
        public static readonly StyleGenerator DefaultDarkStyleGenerator;
        public static readonly StyleGenerator DefaultLightStyleGenerator;

        public static ResourceDictionary LightStyle;
        public static ResourceDictionary DarkStyle;

        public static ThemeType CurrentTheme { get; private set; }
        private static ResourceDictionary _currentResources;

        static Themes()
        {
            if (Device.RuntimePlatform == Device.Android)
                DefaultSizeGenerator = new StyleSizesGenerator(10, 10, 15, 8, 10, 0.5);
            else
                DefaultSizeGenerator = new StyleSizesGenerator(10, 17, 20, 15, 17, 0.7);

            DefaultDarkStyleGenerator = new StyleGenerator(Color.White, Color.FromHex("#0A0A0A"), Color.FromHex("#2A3138"), Color.FromHex("#7d7f7d"), Color.FromHex("#524F9D"), Color.FromHex("#C6515C"), DefaultSizeGenerator);
            DefaultLightStyleGenerator = new StyleGenerator(Color.Black, Color.FromHex("#e5ebf1"), Color.FromHex("#ffffff"), Color.FromHex("#99a2ad"), Color.FromHex("#447bba"), Color.FromHex("#ff3347"), DefaultSizeGenerator);

            DarkStyle = CreateStyles(DefaultLightStyleGenerator);
            LightStyle = CreateStyles(DefaultLightStyleGenerator);
            SetThemeIgnoreException(LoadThemeType());
        }

        public static ResourceDictionary CreateStyles(StyleGenerator data) => new ThemeGeneratorDictionary(data);
        public static void SetTheme(ThemeType theme)
        {
            if (theme == CurrentTheme) throw new Exception("Данная тема уже установлена");
            SetThemeIgnoreException(theme);
        }
        private static void SetThemeIgnoreException(ThemeType theme)
        {
            if (_currentResources != null) Application.Current.Resources.MergedDictionaries.Remove(_currentResources);
            SetThemeType(theme);
            switch (theme)
            {
                case ThemeType.Dark:
                    _currentResources = DarkStyle;
                    break;
                case ThemeType.Light:
                    _currentResources = LightStyle;
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Add(_currentResources);
        }

        public static void EditTheme(ResourceDictionary themeResources, ThemeType type)
        {
            if (themeResources == null) throw new ArgumentNullException(nameof(themeResources));
            switch (type)
            {
                case ThemeType.Light:
                    LightStyle = themeResources;
                    break;
                case ThemeType.Dark:
                    DarkStyle = themeResources;
                    break;
            }
            if (CurrentTheme == type) SetThemeIgnoreException(CurrentTheme);
        }

        private static ThemeType LoadThemeType()
        {
            if (Preferences.ContainsKey(CurrentThemeNameProperty))
                return (ThemeType)Preferences.Get(CurrentThemeNameProperty, (int)DefaultTheme);
            return DefaultTheme;
        }
        private static void SetThemeType(ThemeType theme)
        {
            CurrentTheme = theme;
            Preferences.Set(CurrentThemeNameProperty, (int)theme);
        }


        public enum ThemeType
        {
            Light,
            Dark
        }

        public struct StyleSizesGenerator
        {
            public int CornerRadius { get; private set; }
            public double TextSize_Text { get; private set; }
            public double TextSize_Title { get; private set; }
            public float LaTeXSize_Text { get; private set; }
            public float LaTeXSize_Title { get; private set; }
            public double OxyPlotChart_HeightScale { get; private set; }

            public StyleSizesGenerator(int cornerRadius, double textSize_Text, double textSize_Title, float laTeXSize_Text, float laTeXSize_Title, double oxyPlotChart_HeightScale)
            {
                CornerRadius = cornerRadius;
                TextSize_Text = textSize_Text;
                TextSize_Title = textSize_Title;
                LaTeXSize_Text = laTeXSize_Text;
                LaTeXSize_Title = laTeXSize_Title;
                OxyPlotChart_HeightScale = oxyPlotChart_HeightScale;
            }
        }
        public struct StyleGenerator
        {
            public Color TextColor { get; private set; }
            public Color BackgroundColor { get; private set; }
            public Color ForegroundColor { get; private set; }
            public Color UIElemColor { get; private set; }
            public Color PositiveColor { get; private set; }
            public Color NegativeColor { get; private set; }
            public StyleSizesGenerator StyleSizes { get; private set; }

            public StyleGenerator(Color textColor, Color backgroundColor, Color foregroundColor, Color uIElemColor, Color positiveColor, Color negativeColor, StyleSizesGenerator styleSizes)
            {
                TextColor = textColor;
                BackgroundColor = backgroundColor;
                ForegroundColor = foregroundColor;
                UIElemColor = uIElemColor;
                PositiveColor = positiveColor;
                NegativeColor = negativeColor;
                StyleSizes = styleSizes;
            }
        }
    }
}