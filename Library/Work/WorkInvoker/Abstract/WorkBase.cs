using System.Threading.Tasks;
using System.Threading;

using ConsoleLibrary.Models;

namespace WorkInvoker.Abstract
{
    public abstract class WorkBase
    {
        protected DecoratorConsole Console { get; private set; }

        public void BindConsole(DecoratorConsole console)
        {
            Console = console;
            Console.SetDecorationOneElem();
        }
        public abstract Task Start(CancellationToken token);
    }
}
