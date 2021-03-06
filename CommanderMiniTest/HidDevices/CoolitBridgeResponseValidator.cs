﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices
{
    internal class CoolitBridgeResponseValidator : IResponseValidator
    {
        public bool IsInvalid { get; private set; }

        public void Visit(byte[] buffer)
        {
            // Actually need to add (buffer.Length < 3 || (buffer[2] & 0xF0) != 0) check
            // but HXi/RMi and Lightning node devices return correct values in spite of invalid response buffer
            // so let's ignore this check for now
            IsInvalid = buffer == null;
        }
    }
}
