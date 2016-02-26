using System;

namespace System.Reactive.Management.Instrumentation
{
    [WmiEventClass("__InstanceDeletionEvent")]
    public class InstanceDeletionEvent : InstanceOperationEvent
    {
    }
}
