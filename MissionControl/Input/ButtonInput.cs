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
    class ButtonInput : BaseObjectInput, IButtonInput
    {
        private bool inverted;

        public ButtonInput(InputProperties p, bool inverted) : base(p, false)
        {
            this.inverted = inverted;
        }

        public event InputManager.InputButtonPressedHandler ButtonPressed;
        public event InputManager.InputButtonPressedHandler ButtonReleased;

        internal override void ProcessData(IConnectedDevice device, int incomingValue, int offset)
        {
            bool wasPressed = Value > 0;
            bool isPressed = inverted ? incomingValue <= 0 : incomingValue > 0;

            OnNewValue(isPressed ? 1 : 0);  

            if(ButtonPressed != null)
            {
                if (!wasPressed && isPressed) 
                    ButtonPressed(this);
            }
            if(ButtonReleased != null)
            {
                if (wasPressed && !isPressed)
                    ButtonReleased(this);
            }

        }
    }
}
