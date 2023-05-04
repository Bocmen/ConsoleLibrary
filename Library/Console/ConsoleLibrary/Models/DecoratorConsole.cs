using ConsoleLibrary.ConsoleExtensions;
using ConsoleLibrary.Interfaces;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ConsoleLibrary.Models
{
    public class DecoratorConsole : IConsole
    {
        public static Style StackLayoutStyle = new Style(typeof(StackLayout));

        public IConsole Console { get; private set; }
        public DecorateType CurrentDecorateType { get; private set; }

        private StackLayout _currentCollectionElements;

        protected DecoratorConsole() { }
        public DecoratorConsole(IConsole console)
        {
            if (console is DecoratorConsole) throw new System.Exception("Нельзя создавать декоратор на декоратор");
            Console = console;
        }
        public void SetDecorationOneElem() => SetDecorationType(DecorateType.OneElement);
        public void StartCollectionDecorate() => SetDecorationType(DecorateType.CollectionElements);
        public void EndCollectionDecorate() => OffDecoration();
        public void OffDecoration() => SetDecorationType(DecorateType.None);

        public void SetDecorationType(DecorateType decorateType)
        {
            _currentCollectionElements = null;
            CurrentDecorateType = decorateType;
        }

        public async Task AddUIElement(View view)
        {
            switch (CurrentDecorateType)
            {
                case DecorateType.None: await Console.AddUIElement(view); break;
                case DecorateType.OneElement: await Console.ContentToBlock(view); break;
                case DecorateType.CollectionElements:
                    if (_currentCollectionElements == null)
                    {
                        _currentCollectionElements = new StackLayout() { Style = StackLayoutStyle, HorizontalOptions = LayoutOptions.Fill };
                        await Console.ContentToBlock(_currentCollectionElements);
                    }
                    await Console.InvockeUITheardAction(() => _currentCollectionElements.Children.Add(view));
                    break;
            }
        }
        public Task Clear() => Console.Clear();
        public Task InvockeUITheardAction(Action action) => Console.InvockeUITheardAction(action);
        public Task SetStyles(ResourceDictionary resourceStyles) => Console.SetStyles(resourceStyles);

        public enum DecorateType : byte
        {
            /// <summary>
            /// Не использовать декорирование в блоки, или остановить декорирование CollectionElements
            /// </summary>
            None,
            /// <summary>
            /// Декорировать каждый элемент отдельно
            /// </summary>
            OneElement,
            /// <summary>
            /// Декорировать группу элементов
            /// </summary>
            CollectionElements
        }
    }
}
