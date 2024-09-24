using System;
using System.Threading.Tasks;
using ToranomonSDK.Helpers;
using UnityEngine;

namespace ToranomonSDK
{
    internal sealed class LocationProvider : ILocationProvider
    {
        private readonly LocationService _locationService;
        private bool _isRunning;
        public bool IsRunning => _isRunning;

        public LocationProvider()
        {
            _locationService = Input.location;
        }
#if UNITY_ANDROID
        private async Task<bool> AndroidRequestPermission()
        {
            var tcs = new TaskCompletionSource<bool>();
            var callbacks = new UnityEngine.Android.PermissionCallbacks();
            callbacks.PermissionGranted += _ => tcs.TrySetResult(true);
            callbacks.PermissionDenied += _ => tcs.TrySetResult(false);
            callbacks.PermissionDeniedAndDontAskAgain += _ => tcs.TrySetResult(false);
            UnityEngine.Android.Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION", callbacks);
            return await tcs.Task;
        }
#endif

        public async Task<bool> Start()
        {
            if (_isRunning)
            {
                return true;
            }
            if (_locationService.isEnabledByUser == false)
            {
                Debug.LogWarning("Location service permission is not granted by user.");
#if UNITY_ANDROID

                if (await AndroidRequestPermission() == false)
                {
                    return false;
                }
#else
               return false;
#endif
            }
            _locationService.Start();

            int maxWait = 20;
            while (_locationService.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                await Task.Delay(1000);
                maxWait--;
            }

            if (maxWait < 1)
            {
                Debug.LogError("Timed out in location service initialization");
                return false;
            }

            if (_locationService.status == LocationServiceStatus.Running)
            {
                _isRunning = true;
                ToraDebug.Log("location provider is initialized");
                return true;
            }
            else
            {
                Debug.LogError("Failed to initialize location service");
                return false;
            }
        }

        public LocationData GetLocation()
        {
            ToraDebug.Log("get location");
            if (_isRunning == false)
            {
                throw new InvalidOperationException("not initialized");
            }
            var data = _locationService.lastData;
            var datetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(data.timestamp);
            return new LocationData
            {
                Latitude = (double)data.latitude,
                Longitude = (double)data.longitude,
                Accuracy = (double)data.horizontalAccuracy,
                Timestamp = datetime,
            };
        }

        public void Stop()
        {
            _locationService.Stop();
        }

        public static ILocationProvider New()
        {
#if UNITY_EDITOR
            return new DummyLocationProvider();
#else
            return new LocationProvider();
#endif
        }
    }

    internal sealed class DummyLocationProvider : ILocationProvider
    {
        private (double Lat, double Lon)? _dummyLocation;
        private bool _isInitialized;

        public bool IsRunning => _isInitialized;

        public DummyLocationProvider((double Lat, double Lon)? dummyLocation = null)
        {
            _dummyLocation = dummyLocation;
        }

        public LocationData GetLocation()
        {
            ToraDebug.Log("get location");
            if (_isInitialized == false)
            {
                throw new InvalidOperationException("not initialized");
            }
            return new LocationData
            {
                Latitude = _dummyLocation?.Lat ?? 35.68157850455052,
                Longitude = _dummyLocation?.Lon ?? 139.76513383358508,
                Accuracy = 15.0,
                Timestamp = DateTime.UtcNow,
            };
        }

        public Task<bool> Start()
        {
            _isInitialized = true;
            ToraDebug.Log("init dummy location provider");
            return Task.FromResult(true);
        }

        public void Stop()
        {
            ToraDebug.Log("stop dummy location provider");
        }
    }

    internal interface ILocationProvider
    {
        bool IsRunning { get; }
        Task<bool> Start();
        LocationData GetLocation();
        void Stop();
    }

    internal struct LocationData
    {
        public double Latitude;
        public double Longitude;
        public double Accuracy;
        public DateTime Timestamp;
    }
}
