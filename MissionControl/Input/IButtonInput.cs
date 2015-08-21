using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionControl.Input
{
    public interface IButtonInput : IInput
    { 
        event InputManager.InputButtonPressedHandler ButtonPressed;
    }
}
