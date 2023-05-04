using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConsoleLibrary.Interfaces
{
    public interface IConsole
    {
        Task AddUIElement(View view);
        Task Clear();
        Task InvockeUITheardAction(Action action);
        Task SetStyles(ResourceDictionary resourceStyles);
    }
}
