using ConsoleLibrary.Interfaces;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConsoleLibrary.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Console : ContentView, IConsole
    {
        public Console() => InitializeComponent();
        public Task AddUIElement(View view) => InvockeUITheardAction(() => ConsoleContent.Children.Add(view));
        public Task Clear() => Device.InvokeOnMainThreadAsync(() => ConsoleContent.Children.Clear());
        public Task SetStyles(ResourceDictionary resourceStyles) => InvockeUITheardAction(() => Resources = resourceStyles);
        public Task InvockeUITheardAction(Action action) => Device.InvokeOnMainThreadAsync(action);
    }
}