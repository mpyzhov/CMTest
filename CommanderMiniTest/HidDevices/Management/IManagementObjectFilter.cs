﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidDevices.Management
{
    public interface IManagementObjectFilter
    {
        string QueryString { get; }
    }
}
