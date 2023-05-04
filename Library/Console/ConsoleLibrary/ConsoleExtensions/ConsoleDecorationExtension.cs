using ConsoleLibrary.Interfaces;
using System;
using System.Threading.Tasks;
using ConsoleLibrary.Tools;

using Xamarin.Forms;
using ConsoleLibrary.Models;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static class ConsoleDecorationExtension
    {
        public readonly static string BlockStyle = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string SeparatorLine = Styles.CreateNameStyleConsoleLibrary();

        public static readonly string ButtonFullViewStyle = Styles.CreateNameStyleConsoleLibrary();
        public static readonly string DefaultViewStyle = Styles.CreateNameStyleConsoleLibrary();
        public static readonly string FullViewPageStyle = Styles.CreateNameStyleConsoleLibrary();

        public static View CreateFrameContentInBlock(View content)
        {
            var frame = new Frame
            {
                HorizontalOptions = content.HorizontalOptions,
                Content = content,
                BindingContext = content
            };
            frame.SetDynamicResource(VisualElement.StyleProperty, BlockStyle);
            frame.SetBinding(View.HorizontalOptionsProperty, nameof(View.HorizontalOptions));
            return frame;
        }
        public static View CreateSeparatorLine()
        {
            var frame = new Frame()
            {
                HorizontalOptions = LayoutOptions.Fill
            };
            frame.SetDynamicResource(VisualElement.StyleProperty, SeparatorLine);
            return frame;
        }


        public static Task ContentToBlock(this IConsole console, View view) => console.AddUIElement(CreateFrameContentInBlock(view));
        public static Task DrawSeparatorLine(this IConsole console) => console.AddUIElement(CreateSeparatorLine());
        public static DecoratorConsole CreateDecorator(this IConsole console) => new DecoratorConsole(console);

        public static View CreateViewOnFullScreenMode(Func<ModeView, View> getView)
        {
            Button btn = new Button();
            btn.SetDynamicResource(VisualElement.StyleProperty, ButtonFullViewStyle);
            var view = getView.Invoke(ModeView.Default);
            view.SetDynamicResource(VisualElement.StyleProperty, DefaultViewStyle);
            var stackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    view,
                    btn
                }
            };
            btn.Clicked += async (d, dd) =>
            {
                stackLayout.Children.RemoveAt(0);
                view = getView.Invoke(ModeView.Full);
                view.SetDynamicResource(VisualElement.StyleProperty, FullViewPageStyle);
                ContentPage page = new ContentPage
                {
                    Content = view,
                    Title = "График"
                };
                await Application.Current.MainPage.Navigation.PushAsync(page);
#pragma warning disable IDE0039 // Использовать локальную функцию
                EventHandler<Page> action = null;
#pragma warning restore IDE0039 // Использовать локальную функцию
                action = (object sender, Page e) =>
                {
                    if (e == page)
                    {
                        page.Content = null;
                        view = getView.Invoke(ModeView.Default);
                        view.SetDynamicResource(VisualElement.StyleProperty, DefaultViewStyle);
                        stackLayout.Children.Insert(0, view);
                        Application.Current.PageDisappearing -= action;
                    }
                };
                Application.Current.PageDisappearing += action;
            };
            return stackLayout;
        }
        public static Task AddViewOnFullScreenMode(this IConsole console, Func<ModeView, View> getView) => console.AddUIElement(CreateViewOnFullScreenMode(getView));

        public enum ModeView : byte
        {
            Default,
            Full
        }
    }
}
