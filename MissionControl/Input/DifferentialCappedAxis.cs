using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace MissionControl.Input
{
    class DifferentialCappedAxis : BaseObjectInput
    {

        private float currentValue;
        private int axisMax;
        private int axisMin;
        private float sensitivity;

        public DifferentialCappedAxis(InputProperties p, ushort axisMax, float sensitivity) : base(p, false)
        {
            this.axisMax = axisMax;
            this.axisMin = -1 * axisMax;
            this.sensitivity = sensitivity;
            currentValue = 0;

        }

        internal override void ProcessData(IConnectedDevice device, int incomingValue, int offset)
        {
            switch (properties.Information.Aspect)
            {
                case ObjectAspect.Position:
                    float diff = incomingValue * sensitivity;
                    float newValue = currentValue + diff;
                    if (newValue > 0)
                        newValue = Math.Min(axisMax, newValue);
                    else if (newValue < 0)
                        newValue = Math.Max(axisMin, newValue);
                    currentValue = newValue;
                    OnNewValue((int)currentValue);
                    break;
            }
        }
    }
}
