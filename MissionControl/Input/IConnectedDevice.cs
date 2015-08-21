using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace MissionControl.Input
{
    interface IConnectedDevice
    {
        Device Device { get; }

        InputProperties? DeviceInputPropsFromName(string name);

        void AddInput(InputProperties prop, BaseInput input);

        void ProcessInput();

    }
}
