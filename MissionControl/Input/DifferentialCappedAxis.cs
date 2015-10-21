/*
    Copyright (c) 2015 Jeffrey Quesnelle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
