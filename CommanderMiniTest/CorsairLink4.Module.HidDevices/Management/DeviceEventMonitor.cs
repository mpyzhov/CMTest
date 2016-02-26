using CorsairLink4.Common.Shared.DevicesData;
using CorsairLink4.Module.HidDevices.Models;
using System;
using System.Diagnostics;
using System.Management;
using System.Reactive.Management.Instrumentation;
using System.Reactive.Subjects;

namespace CorsairLink4.Module.HidDevices
{
    public class DeviceEventMonitor : IDisposable
    {
        private const int PollingIntervalSeconds = 3;
        private IDisposable creationWatcher, deletionWatcher;
        private Subject<DeviceChangedEventArgs> changedSubject;

        private DeviceEventMonitor()
        {
            changedSubject = new Subject<DeviceChangedEventArgs>();
            Start();
        }

        public static DeviceEventMonitor Instance
        {
            get { return Nested.Instance; }
        }

        public IObservable<DeviceChangedEventArgs> Changed { get { return changedSubject; } }

        public void Start()
        {
            DeviceNotificationsStart();
        }

        public void Stop()
        {
            DeviceNotificationsStop();
        }

        public void Dispose()
        {
            Stop();
        }

        private void DeviceNotificationsStart()
        {
            creationWatcher = EventProvider.Query<InstanceCreationEvent>()
                .Within(TimeSpan.FromSeconds(PollingIntervalSeconds))
                .Where(x => x.TargetInstance is Device)
                .Select(x => x.TargetInstance)
                .AsObservable()
                .Subscribe(mbo =>
                {
                    var args = new DeviceChangedEventArgs
                    {
                        ChangeType = DeviceChangeType.Arrived,
                        Device = DeviceFactory.CreateDeviceEntity(mbo as ManagementBaseObject)
                    };

                    changedSubject.OnNext(args);
                });

            deletionWatcher = EventProvider.Query<InstanceDeletionEvent>()
                .Within(TimeSpan.FromSeconds(PollingIntervalSeconds))
                .Where(x => x.TargetInstance is Device)
                .Select(x => x.TargetInstance)
                .AsObservable()
                .Subscribe(mbo =>
                {
                    var args = new DeviceChangedEventArgs
                    {
                        ChangeType = DeviceChangeType.Removed,
                        Device = DeviceFactory.CreateDeviceEntity(mbo as ManagementBaseObject)
                    };

                    changedSubject.OnNext(args);
                });
        }

        private void DeviceNotificationsStop()
        {
            try
            {
                if (creationWatcher != null)
                {
                    creationWatcher.Dispose();
                }

                if (deletionWatcher != null)
                {
                    deletionWatcher.Dispose();
                }

                creationWatcher = deletionWatcher = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private class Nested
        {
            internal static readonly DeviceEventMonitor Instance = new DeviceEventMonitor();

            static Nested()
            {
            }
        }
    }
}
