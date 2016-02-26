using System;

namespace System.Reactive.Management.Instrumentation
{
    [WmiEventClass("__InstanceOperationEvent")]
    public class InstanceOperationEvent
    {
        public object TargetInstance { get; set; }
    }
}
