using System.Threading.Tasks;
using System.Threading;

namespace ConsoleLibrary.Extensions
{
    public static class TaskCompletionSourceExtension
    {
        public static async Task<T> WaitAsyncResultUseCancellationToken<T>(this TaskCompletionSource<T> taskCompletion, CancellationToken token)
        {
            using (token.Register(() =>
            {
                taskCompletion.TrySetCanceled();
            }))
            {
                return await taskCompletion.Task;
            }
        }

        public static Task<T> WaitAsyncResultUseCancellationToken<T>(this TaskCompletionSource<T> taskCompletion, CancellationToken? token)
        {
            if (token == null) return taskCompletion.Task;
            return taskCompletion.WaitAsyncResultUseCancellationToken((CancellationToken)token);
        }
    }
}
