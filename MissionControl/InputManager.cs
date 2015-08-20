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

namespace MissionControl
{
    class InputManager
    {

        private DirectInput directInput;
        private List<ConnectedDevice> connectedDevices = new List<ConnectedDevice>();

        private readonly DeviceType[] USABLE_TYPES = { DeviceType.Joystick, DeviceType.Gamepad, DeviceType.Flight, DeviceType.FirstPerson, DeviceType.Supplemental, DeviceType.Keyboard, DeviceType.Mouse };
       
        enum INPUT_TYPE
        {
            PITCH_AXIS = 0,
            ROLL_AXIS = 1,
            YAW_AXIS = 2,
            CLIMB_DESCEND_AXIS = 3,
            TAKEOFF = 4,
            LAND = 5,
            CONTROLS_ENABLE = 6,
        }

        private class ConnectedDevice
        {
            private Device device;
            private Dictionary<string, DeviceObjectId> nameToId = new Dictionary<string, DeviceObjectId>();
            private Dictionary<int, IInput> inputs = new Dictionary<int, IInput>();

            public ConnectedDevice(Device d)
            {
                device = d;
                var objects = device.GetObjects();
                foreach(var obj in objects)
                    nameToId.Add(obj.Name, obj.ObjectId);
            }

            public Device Device
            {
                get { return device; }
            }

            public IInput AddAxisInput(string name)
            {
                DeviceObjectId id;
                if (nameToId.TryGetValue(name, out id))
                {
                    DeviceObjectInstance info = device.GetObjectInfoById(id);
                    ObjectProperties prop = device.GetObjectPropertiesById(id);

                    return new NormalizedAxisInput(device, info, prop, 100);
                }

                return new NullInput();
            }

            public IInput AddButtonAxisInput(string first, string second)
            {
                return new NullInput();
            }

            public IInput AddButtonInput(string name)
            {
                DeviceObjectId id;
                if (nameToId.TryGetValue(name, out id))
                {
                    DeviceObjectInstance info = device.GetObjectInfoById(id);
                    ObjectProperties prop = device.GetObjectPropertiesById(id);

                    return new ButtonInput(device, info, prop);
                }

                return new NullInput();
            }
        }

        public interface IInput
        {
            int Value { get; }

            void ProcessData(IStateUpdate update);
        }

        private class NullInput : IInput
        {
            public int Value
            {
                get { return 0; }
            }

            public void ProcessData(IStateUpdate update)
            {

            }
        }

        private class BaseObjectInput : IInput
        {
            protected int value;
            protected Device device;
            protected DeviceObjectInstance info;
            protected ObjectProperties prop;

            public BaseObjectInput(Device d, DeviceObjectInstance i, ObjectProperties p)
            {
                device = d;
                info = i;
                prop = p;
            }

            public virtual int Value
            {
                get { return value; }
            }
            public virtual void ProcessData(IStateUpdate update)
            {
                value = update.Value;
            }
        }

        private class NormalizedAxisInput : BaseObjectInput
        {
            float nRange;
            float pRange;

            int rawRange;
            int rawOffset;

            public NormalizedAxisInput(Device d, DeviceObjectInstance i, ObjectProperties p, ushort r) : base(d, i, p)
            {
                int sr = r;
                pRange = sr;
                nRange = sr * -1;

                if(info.Aspect == ObjectAspect.Position)
                {
                    rawRange = Math.Abs(p.Range.Maximum - p.Range.Minimum);
                    rawOffset = p.Range.Minimum;
                }
            }

            public override void ProcessData(IStateUpdate update)
            {
                switch(info.Aspect)
                {
                    case ObjectAspect.Position:
                        value = (int)lerp(nRange, pRange, (float)update.Value / rawRange) + rawOffset;
                        break;
                    
                }
            }

            private static float lerp(float v0, float v1, float t)
            {
                return (1 - t) * v0 + t * v1;
            }
        }

        private class ButtonInput : BaseObjectInput
        {

            public ButtonInput(Device d, DeviceObjectInstance i, ObjectProperties p) : base(d, i, p)
            {

            }

            public override void ProcessData(IStateUpdate update)
            {
                value = update.Value == 0 ? 0 : 1;
            }
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

        private Device GetDeviceByName(string name)
        {
            foreach(var deviceType in USABLE_TYPES)
            {
                foreach (var deviceInstance in directInput.GetDevices(deviceType, DeviceEnumerationFlags.AttachedOnly))
                {
                    if (deviceInstance.ProductName.Equals(name) || deviceInstance.InstanceName.Equals(name))
                    {
                        switch(deviceType)
                        {
                            case DeviceType.Mouse:
                                return new Mouse(directInput);
                            case DeviceType.Keyboard:
                                return new Keyboard(directInput);
                            default:
                                return new Joystick(directInput, deviceInstance.InstanceGuid);
                        }
                    }
                        
                }
            }
            return null;
        }

        private Device GetDeviceByGuid(Guid guid)
        {
            foreach (var deviceType in USABLE_TYPES)
            {
                foreach (var deviceInstance in directInput.GetDevices(deviceType, DeviceEnumerationFlags.AttachedOnly))
                {
                    if (deviceInstance.ProductGuid.Equals(guid) || deviceInstance.InstanceGuid.Equals(guid))
                    {
                        switch (deviceType)
                        {
                            case DeviceType.Mouse:
                                return new Mouse(directInput);
                            case DeviceType.Keyboard:
                                return new Keyboard(directInput);
                            default:
                                return new Joystick(directInput, deviceInstance.InstanceGuid);
                        }
                    }

                }
            }
            return null;
        }
        void LoadDefaultMappings()
        {
            ClearMappings();

            var stick = new ConnectedDevice(GetDeviceByName("Saitek Pro Flight X-55 Rhino Stick"));
            stick.AddAxisInput("X Axis");
            stick.AddAxisInput("Y Axis");
            stick.AddAxisInput("Z Rotataion");
            stick.AddButtonInput("Button 5");

            var throttle = new ConnectedDevice(GetDeviceByName("Saitek Pro Flight X-55 Rhino Throttle"));


        }

        void ClearMappings()
        {
            foreach (ConnectedDevice d in connectedDevices)
                d.Device.Unacquire();
            connectedDevices.Clear();
        }
    }
}
