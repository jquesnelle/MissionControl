using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace MissionControl.Input
{
    class CappedAxisInput : BaseObjectInput
    {
        private int axisMax;
        private int axisMin;
        private float sensitivity;

        public CappedAxisInput(InputProperties p, ushort axisMax, float sensitivity, bool reset) : base(p, reset)
        {
            this.axisMax = axisMax;
            this.axisMin = -1 * axisMax;
            this.sensitivity = sensitivity;
        }

        internal override void ProcessData(IConnectedDevice device, int incomingValue, int offset)
        {
            switch (properties.Information.Aspect)
            {
                case ObjectAspect.Position:
                    float newValue = incomingValue * sensitivity;
                    if (newValue > 0)
                        newValue = Math.Min(axisMax, newValue);
                    else if (newValue < 0)
                        newValue = Math.Max(axisMin, newValue);
                    OnNewValue((int)newValue);
                    break;
            }
        }

    }
}
