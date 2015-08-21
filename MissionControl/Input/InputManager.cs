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
using System.Diagnostics;

namespace MissionControl.Input
{

    public class InputManager
    {

        private DirectInput directInput;
        private List<IConnectedDevice> connectedDevices = new List<IConnectedDevice>();
        private Dictionary<INPUT_TYPE, IInput> mappings = new Dictionary<INPUT_TYPE, IInput>();

        public const int AXIS_MAX = 100;

        private readonly DeviceType[] USABLE_TYPES = { DeviceType.Joystick, DeviceType.Gamepad, DeviceType.Flight, DeviceType.FirstPerson, DeviceType.Supplemental, DeviceType.Keyboard, DeviceType.Mouse };
        private readonly NullInput NULL_INPUT = new NullInput();

        public delegate void InputChangedHandler(IInput input, int oldValue, int newValue);
        public delegate void InputButtonPressedHandler(IButtonInput input);

        public enum INPUT_TYPE
        {
            PITCH_AXIS = 0,
            ROLL_AXIS = 1,
            YAW_AXIS = 2,
            CLIMB_DESCEND_AXIS = 3,
            TAKE_OFF = 4,
            LAND = 5,
            HOVER_ENABLE = 6,
            HOVER_DISABLE = 7,
        }

        void PrintConnectedDevices()
        {
            for (int i = 17; i <= 28; ++i)
            {
                DeviceType type = (DeviceType)i;
                Debug.Print("--- {0} ---", type.ToString());
                foreach (var deviceInstance in directInput.GetDevices(type, DeviceEnumerationFlags.AttachedOnly))
                    Debug.Print("{0}: {1} (subtype {2})", deviceInstance.InstanceGuid, deviceInstance.InstanceName, Convert.ToString(deviceInstance.Subtype));
            }
        }

        public InputManager()
        {
            directInput = new DirectInput();

            LoadDefaultMappings();
        }

        private IConnectedDevice OpenDeviceByNameOrGuid(string name, Guid? guid)
        {
            IConnectedDevice ret = null;
            foreach (var deviceType in USABLE_TYPES)
            {
                foreach (var deviceInstance in directInput.GetDevices(deviceType, DeviceEnumerationFlags.AttachedOnly))
                {
                    bool isDevice = guid != null && (deviceInstance.ProductGuid.Equals(guid) || deviceInstance.InstanceGuid.Equals(guid));
                    if (!isDevice && name != null)
                        isDevice = deviceInstance.ProductName.Equals(name) || deviceInstance.InstanceName.Equals(guid);
                    if(isDevice)
                    {
                        switch (deviceType)
                        {
                            case DeviceType.Mouse:
                                var m = new Mouse(directInput);
                                m.Properties.BufferSize = 128;
                                m.Acquire();
                                ret = new ConnectedDevice<MouseState, RawMouseState, MouseUpdate>(m);
                                break;
                            case DeviceType.Keyboard:
                                var k = new Keyboard(directInput);
                                k.Properties.BufferSize = 128;
                                k.Acquire();
                                ret = new ConnectedDevice<KeyboardState, RawKeyboardState, KeyboardUpdate>(k);
                                break;
                            default:
                                var j = new Joystick(directInput, deviceInstance.InstanceGuid);
                                j.Properties.BufferSize = 128;
                                j.Acquire();
                                ret = new ConnectedDevice<JoystickState, RawJoystickState, JoystickUpdate>(j);
                                break;
                        }
                    }

                }
            }
           
            if(ret != null)
            {
                connectedDevices.Add(ret);
                return ret;
            }

            return null;
        }

        private void AddAxisMapping(INPUT_TYPE type, IConnectedDevice device, string name, bool inverted)
        {
            InputProperties? p = device.DeviceInputPropsFromName(name);
            if(p.HasValue)
            {
                NormalizedAxisInput nai = new NormalizedAxisInput(p.Value, AXIS_MAX, inverted);
                device.AddInput(p.Value, nai);
                mappings.Add(type, nai);
            }
        }

        private void AddButtonMapping(INPUT_TYPE type, IConnectedDevice device, string name, bool inverted)
        {
            InputProperties? p = device.DeviceInputPropsFromName(name);
            if (p.HasValue)
            {
                ButtonInput bi = new ButtonInput(p.Value, inverted);
                device.AddInput(p.Value, bi);
                mappings.Add(type, bi);
            }
        }

        private void AddAxisTwoButtonMapping(INPUT_TYPE type, IConnectedDevice positiveDevice, string positiveName, 
            IConnectedDevice negativeDevice, string negativeName)
        {
            InputProperties? pp = positiveDevice.DeviceInputPropsFromName(positiveName);
            InputProperties? np = negativeDevice.DeviceInputPropsFromName(negativeName);
            if(pp.HasValue && np.HasValue)
            {
                AxisTwoButtonInput atbi = new AxisTwoButtonInput(pp.Value, np.Value, AXIS_MAX);
                positiveDevice.AddInput(pp.Value, atbi);
                negativeDevice.AddInput(np.Value, atbi);
                mappings.Add(type, atbi);
            }
        }

        void LoadDefaultMappings()
        {
            ClearMappings();

            var stick = OpenDeviceByNameOrGuid("Saitek Pro Flight X-55 Rhino Stick", null);
            var throttle = OpenDeviceByNameOrGuid("Saitek Pro Flight X-55 Rhino Throttle", null);

            AddAxisMapping(INPUT_TYPE.YAW_AXIS, stick, "X", false);
            AddAxisMapping(INPUT_TYPE.ROLL_AXIS, stick, "RotationZ", false);
            AddAxisMapping(INPUT_TYPE.CLIMB_DESCEND_AXIS, stick, "Y", false);

            AddAxisMapping(INPUT_TYPE.PITCH_AXIS, throttle, "X", true);
            AddButtonMapping(INPUT_TYPE.TAKE_OFF, throttle, "Buttons5", false);
            AddButtonMapping(INPUT_TYPE.LAND, throttle, "Buttons6", false);
            AddButtonMapping(INPUT_TYPE.HOVER_ENABLE, throttle, "Buttons34", false);
            AddButtonMapping(INPUT_TYPE.HOVER_DISABLE, throttle, "Buttons34", true);
        }

        private void ClearMappings()
        {
            foreach (IConnectedDevice d in connectedDevices)
                d.Device.Unacquire();
            connectedDevices.Clear();
        }


        public T GetInput<T>(INPUT_TYPE type) where T : IInput
        {
            IInput input;
            if(mappings.TryGetValue(type, out input))
            {
                if (input is T)
                    return (T)input;
            }
            return default(T);
        }

        public void Process()
        {
            foreach (var device in connectedDevices)
                device.ProcessInput();
        }


        #region PUBLIC INPUT GETTERS

        public IInput Roll
        {
            get
            {
                return GetInput<IInput>(INPUT_TYPE.ROLL_AXIS) ?? NULL_INPUT;
            }
        }

        public IInput Pitch
        {
            get
            {
                return GetInput<IInput>(INPUT_TYPE.PITCH_AXIS) ?? NULL_INPUT;
            }
        }

        public IInput Yaw
        {
            get
            {
                return GetInput<IInput>(INPUT_TYPE.YAW_AXIS) ?? NULL_INPUT;
            }
        }

        public IInput Climb_Descend
        {
            get
            {
                return GetInput<IInput>(INPUT_TYPE.CLIMB_DESCEND_AXIS) ?? NULL_INPUT;
            }
        }

        public IButtonInput Take_off
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.TAKE_OFF) ?? NULL_INPUT;
            }
        }

        public IButtonInput Land
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.LAND) ?? NULL_INPUT;
            }
        }

        public IButtonInput Hover_Enable
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.HOVER_ENABLE) ?? NULL_INPUT;
            }
        }

        public IButtonInput Hover_Disable
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.HOVER_DISABLE) ?? NULL_INPUT;
            }
        }

        #endregion

    }
}
