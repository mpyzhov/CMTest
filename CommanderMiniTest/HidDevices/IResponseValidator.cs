using HumanInterfaceDevice.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    internal interface IResponseValidator : IBufferVisitor
    {
        bool IsInvalid { get; }
    }
}
