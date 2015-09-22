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

        private void AddNormalizedAxisMapping(INPUT_TYPE type, IConnectedDevice device, string name, bool inverted, bool reset, int axisMax = AXIS_MAX)
        {
            InputProperties? p = device.DeviceInputPropsFromName(name);
            if(p.HasValue)
            {
                NormalizedAxisInput nai = new NormalizedAxisInput(p.Value, (ushort)axisMax, (ushort)(axisMax / 10), inverted, reset);
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

        private void AddDifferentialMapping(INPUT_TYPE type, IConnectedDevice device, string name, float sensitivity)
        {
            InputProperties? p = device.DeviceInputPropsFromName(name);
            if (p.HasValue)
            {
                DifferentialCappedAxis nai = new DifferentialCappedAxis(p.Value, AXIS_MAX, sensitivity);
                device.AddInput(p.Value, nai);
                mappings.Add(type, nai);
            }
        }

        private void AddAxisMapping(INPUT_TYPE type, IConnectedDevice device, string name, float sensitivity, bool reset, int axisMax = AXIS_MAX)
        {
            InputProperties? p = device.DeviceInputPropsFromName(name);
            if (p.HasValue)
            {
                CappedAxisInput nai = new CappedAxisInput(p.Value, (ushort)axisMax, sensitivity, reset);
                device.AddInput(p.Value, nai);
                mappings.Add(type, nai);
            }
        }

        void LoadDefaultMappings()
        {
            ClearMappings();

            var devices = directInput.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AttachedOnly);

            /*
            var stick = OpenDeviceByNameOrGuid("Saitek Pro Flight X-55 Rhino Stick", null);
            var throttle = OpenDeviceByNameOrGuid("Saitek Pro Flight X-55 Rhino Throttle", null);

            AddNormalizedAxisMapping(INPUT_TYPE.YAW_AXIS, stick, "X", false);
            AddNormalizedAxisMapping(INPUT_TYPE.ROLL_AXIS, stick, "RotationZ", false);
            AddNormalizedAxisMapping(INPUT_TYPE.CLIMB_DESCEND_AXIS, stick, "Y", false);

            AddAxisMapping(INPUT_TYPE.PITCH_AXIS, throttle, "X", true);
            AddButtonMapping(INPUT_TYPE.TAKE_OFF, throttle, "Buttons5", false);
            AddButtonMapping(INPUT_TYPE.LAND, throttle, "Buttons6", false);
            AddButtonMapping(INPUT_TYPE.HOVER_ENABLE, throttle, "Buttons34", false);
            AddButtonMapping(INPUT_TYPE.HOVER_DISABLE, throttle, "Buttons34", true);
            */

            /*
            var mouse = OpenDeviceByNameOrGuid("Mouse", null);
            var keyboard = OpenDeviceByNameOrGuid("Keyboard", null);

            AddAxisMapping(INPUT_TYPE.YAW_AXIS, mouse, "X", 2.0f, true);

            AddButtonMapping(INPUT_TYPE.TAKE_OFF, keyboard, "Key" + (int)Key.Up, false);
            AddButtonMapping(INPUT_TYPE.LAND, keyboard, "Key" + (int)Key.Down, false);
            AddButtonMapping(INPUT_TYPE.FLAT_TRIM, keyboard, "Key" + (int)Key.F12, false);
            */

            var xbox = OpenDeviceByNameOrGuid("Controller (Gamepad for Xbox 360)", null);
            var objects = xbox.Device.GetObjects();

            AddNormalizedAxisMapping(INPUT_TYPE.YAW_AXIS, xbox, "RotationX", false, false);
            AddNormalizedAxisMapping(INPUT_TYPE.CLIMB_DESCEND_AXIS, xbox, "Z", true, false, 10);
            AddNormalizedAxisMapping(INPUT_TYPE.PITCH_AXIS, xbox, "Y", true, false, 10);
            AddNormalizedAxisMapping(INPUT_TYPE.ROLL_AXIS, xbox, "X", false, false, 10);
            AddButtonMapping(INPUT_TYPE.TAKE_OFF, xbox, "Buttons7", false);
            AddButtonMapping(INPUT_TYPE.LAND, xbox, "Buttons7", false);
            AddButtonMapping(INPUT_TYPE.FLAT_TRIM, xbox, "Buttons6", false);
            AddButtonMapping(INPUT_TYPE.TAKE_PHOTO, xbox, "Buttons2", false);
            AddButtonMapping(INPUT_TYPE.START_VIDEO, xbox, "Buttons1", false);
            AddButtonMapping(INPUT_TYPE.STOP_VIDEO, xbox, "Buttons1", false);
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

        public bool HaveInput(INPUT_TYPE type)
        {
            IInput input;
            if (mappings.TryGetValue(type, out input))
                return input != NULL_INPUT;
            return false;
        }

        public static INPUT_CLASS GetClassOfInputType(INPUT_TYPE type)
        {
            switch(type)
            {
                case INPUT_TYPE.PITCH_AXIS:
                case INPUT_TYPE.ROLL_AXIS:
                case INPUT_TYPE.YAW_AXIS:
                case INPUT_TYPE.CLIMB_DESCEND_AXIS:
                    return INPUT_CLASS.AXIS;
                case INPUT_TYPE.HOVER_DISABLE:
                case INPUT_TYPE.HOVER_ENABLE:
                case INPUT_TYPE.LAND:
                case INPUT_TYPE.TAKE_OFF:
                case INPUT_TYPE.FLAT_TRIM:
                case INPUT_TYPE.TAKE_PHOTO:
                case INPUT_TYPE.START_VIDEO:
                case INPUT_TYPE.STOP_VIDEO:
                    return INPUT_CLASS.DIGITAL;
                default:
                    return INPUT_CLASS.NONE;
            }
        }

        public static string GetInputTypeName(INPUT_TYPE type)
        {
            switch(type)
            {
                case INPUT_TYPE.PITCH_AXIS: return "Pitch";
                case INPUT_TYPE.ROLL_AXIS: return "Roll";
                case INPUT_TYPE.YAW_AXIS: return "Yaw";
                case INPUT_TYPE.CLIMB_DESCEND_AXIS: return "Climb/descend";
                case INPUT_TYPE.HOVER_DISABLE: return "Hover enable";
                case INPUT_TYPE.HOVER_ENABLE: return "Hover disable";
                case INPUT_TYPE.LAND: return "Land";
                case INPUT_TYPE.TAKE_OFF: return "Take off";
                case INPUT_TYPE.FLAT_TRIM: return "Flat trim";
                case INPUT_TYPE.TAKE_PHOTO: return "Take photo";
                case INPUT_TYPE.START_VIDEO: return "Start video";
                case INPUT_TYPE.STOP_VIDEO: return "Stop video";
                default: return "";
            }
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

        public IButtonInput Flat_Trim
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.FLAT_TRIM) ?? NULL_INPUT;
            }
        }

        public IButtonInput Take_Photo
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.TAKE_PHOTO) ?? NULL_INPUT;
            }
        }

        public IButtonInput Start_Video
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.START_VIDEO) ?? NULL_INPUT;
            }
        }
        public IButtonInput Stop_Video
        {
            get
            {
                return GetInput<IButtonInput>(INPUT_TYPE.STOP_VIDEO) ?? NULL_INPUT;
            }
        }

        #endregion

    }
}
