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
