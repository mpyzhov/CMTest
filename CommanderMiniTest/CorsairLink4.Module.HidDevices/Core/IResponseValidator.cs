using HumanInterfaceDevice.Types;
using System;

namespace CorsairLink4.Module.HidDevices.Core
{
    internal interface IResponseValidator : IBufferVisitor
    {
        bool IsInvalid { get; }
    }
}
