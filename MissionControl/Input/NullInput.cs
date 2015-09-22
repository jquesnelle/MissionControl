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
    class NullInput : BaseInput, IButtonInput
    {
        public override int Value
        {
            get { return 0; }
        }

        public event InputManager.InputButtonPressedHandler ButtonPressed;
        public override event InputManager.InputChangedHandler InputChanged;

        internal override void ProcessData(IConnectedDevice device, int incomingValue, int offset)
        {

        }

        internal override void BeforeProcess()
        {
            
        }
    }
}
