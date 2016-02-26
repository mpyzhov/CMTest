using System;
using System.Linq;

namespace HumanInterfaceDevice.Types
{
    internal class Buffer : IBuffer
    {
        private byte[] buffer;

        public Buffer(int length)
        {
            buffer = new byte[length];
        }

        public Buffer(byte[] source)
        {
            buffer = new byte[source.Length];
            source.CopyTo(buffer, 0);
        }

        public int Length
        {
            get { return buffer.Length; }
        }

        public void Accept(IBufferVisitor visitor)
        {
            visitor.Visit(buffer);
        }

        public byte ElementAt(int index)
        {
            return buffer.ElementAt(index);
        }
    }
}
