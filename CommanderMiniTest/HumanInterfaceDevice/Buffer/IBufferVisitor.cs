using System;

namespace HumanInterfaceDevice.Types
{
    public interface IBufferVisitor
    {
        void Visit(byte[] buffer);
    }
}
