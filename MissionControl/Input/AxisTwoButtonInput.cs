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
    class AxisTwoButtonInput : BaseInput
    {
        ButtonInput positive;
        ButtonInput negative;
        int value;
        int toSet;

        public override event InputManager.InputChangedHandler InputChanged;

        public AxisTwoButtonInput(InputProperties positiveProp, InputProperties negativeProp, ushort axisValue)
        {
            toSet = axisValue;
            positive = new ButtonInput(positiveProp, false);
            negative = new ButtonInput(negativeProp, false);
        }

        public override int Value
        {
            get
            {
                return value;
            }
        }

        internal override void ProcessData(IConnectedDevice device, int incomingValue, int offset)
        {
            int oldValue = value;

            if (device.Device == positive.Properties.Device && offset == positive.Properties.RealOffset)
                positive.ProcessData(device, incomingValue, offset);
            else if (device.Device == negative.Properties.Device && offset == negative.Properties.RealOffset)
                negative.ProcessData(device, incomingValue, offset);

            if (positive.Value > 0)
                value = toSet;
            else if (negative.Value > 0)
                value = -toSet;
            else
                value = 0;

            if ((oldValue != value) && (InputChanged != null))
                InputChanged(this, oldValue, incomingValue);

        }

        internal override void BeforeProcess()
        {
            positive.BeforeProcess();
            negative.BeforeProcess();
        }
    }
}
