using System;

namespace System.Reactive.Management.Instrumentation
{
    [WmiEventClass("__InstanceCreationEvent")]
    public class InstanceCreationEvent : InstanceOperationEvent
    {
    }
}
