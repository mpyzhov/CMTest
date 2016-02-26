using CorsairLink4.Common.Shared.Communication;
using CorsairLink4.Common.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairLink4.Module.HidDevices.Coolit
{
    public interface ICoolitConnectedDevice : IControlFansComponent, IControlLedComponent
    {
        Task UpdateProperties();

        Task UpdateInfo(IGroupsService groupsService);

        Task Accept(ISensorVisitor visitor);
    }
}
