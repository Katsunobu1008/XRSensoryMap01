#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System;
using System.Threading;

namespace ToranomonSDK
{
    internal sealed class PressureSensorIOS : IDisposable
    {
        private IntPtr _ptr;
        private bool _isRunning;

        public bool IsDisposed => _ptr == IntPtr.Zero;
        public bool IsRunning => _isRunning;

        public double CurrentAtmosphericPressure
        {
            get
            {
                if (IsDisposed)
                {
                    return 0.0;
                }
                return GetCurrentAtmosphericPressure(_ptr);
            }
        }

        public PressureSensorIOS()
        {
            _ptr = CreatePressureSensor();
        }

        ~PressureSensorIOS() => DisposePrivate();

        public void Start()
        {
            if (IsDisposed)
            {
                return;
            }
            _isRunning = StartAtmosphericPressure(_ptr) != 0;
        }

        public void Stop()
        {
            if (IsDisposed)
            {
                return;
            }
            StopAtmosphericPressure(_ptr);
        }

        public void Dispose()
        {
            DisposePrivate();
            GC.SuppressFinalize(this);
        }

        private void DisposePrivate()
        {
            var ptr = Interlocked.Exchange(ref _ptr, IntPtr.Zero);
            if (ptr != IntPtr.Zero)
            {
                ReleasePressureSensor(ptr);
            }
        }

        [DllImport("__Internal", EntryPoint = "createPressureSensor")]
        private static extern IntPtr CreatePressureSensor();

        [DllImport("__Internal", EntryPoint = "releasePressureSensor")]
        private static extern void ReleasePressureSensor(IntPtr pressureSensor);

        [DllImport("__Internal", EntryPoint = "startAtmosphericPressure")]
        private static extern byte StartAtmosphericPressure(IntPtr pressureSensor);

        [DllImport("__Internal", EntryPoint = "stopAtmosphericPressure")]
        private static extern void StopAtmosphericPressure(IntPtr pressureSensor);

        [DllImport("__Internal", EntryPoint = "getCurrentAtmosphericPressure")]
        private static extern double GetCurrentAtmosphericPressure(IntPtr pressureSensor);
    }
}
#endif
