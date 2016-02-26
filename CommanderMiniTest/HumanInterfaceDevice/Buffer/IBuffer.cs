using System;

namespace HumanInterfaceDevice.Types
{
    public interface IBuffer
    {
        int Length { get; }

        byte ElementAt(int index);

        void Accept(IBufferVisitor visitor);
    }
}
