using HumanInterfaceDevice.Types;
using System;

namespace CorsairLink4.Module.HidDevices.Coolit.BufferManipulation
{
    public class CoolitVisitor: IBufferVisitor
    {
        private byte[] res;

        public CoolitVisitor(byte[] data)
        {
            res = data;
        }

        public void Visit(byte[] buffer)
        {
            for (int i = 0; i < res.Length && i < buffer.Length; i++)
            {
                buffer[i] = res[i];
            }
        }
    }
}
