using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ToranomonSDK.Helpers
{
    internal static class CoroutineHelper
    {
        public static CoroutineTask AsCoroutineTask(this IEnumerator coroutine)
        {
            return new CoroutineTask(coroutine);
        }

        public static async Task Yield()
        {
            await YieldCoroutine().AsCoroutineTask();

            static IEnumerator YieldCoroutine()
            {
                yield return null;
            }
        }
    }

    internal readonly struct CoroutineTask
    {
        private readonly IEnumerator _coroutine;

        public CoroutineTask(IEnumerator coroutine)
        {
            _coroutine = coroutine;
        }

        public CoroutineAwaiter GetAwaiter() => new CoroutineAwaiter(_coroutine);
    }

    internal sealed class CoroutineAwaiter : INotifyCompletion
    {
        private readonly IEnumerator _coroutine;

        public bool IsCompleted { get; private set; }

        public void GetResult()
        {
        }

        public CoroutineAwaiter(IEnumerator coroutine)
        {
            _coroutine = coroutine;
        }

        public void OnCompleted(Action continuation)
        {
            CoroutineDispatcher.Dispatch(_coroutine, () =>
            {
                IsCompleted = true;
                continuation();
            });
        }
    }
}
