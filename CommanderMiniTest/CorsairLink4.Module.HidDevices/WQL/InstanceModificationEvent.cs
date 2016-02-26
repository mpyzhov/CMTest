using System;

namespace System.Reactive.Management.Instrumentation
{
    [WmiEventClass("__InstanceModificationEvent")]
    public class InstanceModificationEvent : InstanceOperationEvent
    {
    }
}
