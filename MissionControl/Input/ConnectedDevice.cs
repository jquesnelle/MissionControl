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
using System.Reflection;

namespace MissionControl.Input
{
    class ConnectedDevice<T, TRaw, TUpdate> : IConnectedDevice
        where T : class, IDeviceState<TRaw, TUpdate>, new()
        where TRaw : struct
        where TUpdate : struct, IStateUpdate
    {
        private CustomDevice<T, TRaw, TUpdate> device;
        private MultiValueDictionary<int, BaseInput> offsetToInput = new MultiValueDictionary<int, BaseInput>();
        private MethodInfo getFromName;

        public ConnectedDevice(CustomDevice<T, TRaw, TUpdate> d)
        {
            device = d;

            //not happy about this...
            var type = device.GetType().BaseType;
            getFromName = type.GetMethod("GetFromName", BindingFlags.NonPublic | BindingFlags.Instance);
                    
        }

        public Device Device
        {
            get { return device; }
        }

        public InputProperties? DeviceInputPropsFromName(string name)
        {
            var info = device.GetObjectInfoByName(name);
            var prop = device.GetObjectPropertiesByName(name);
            object ret = getFromName.Invoke(device, new object[] { name });
            if (info != null && prop != null && ret != null)
            {
                var dof = (DataObjectFormat)ret;
                return new InputProperties { Device = device, Information = info, Properties = prop, RealOffset = dof.Offset };
            }
                
            return null;              
        }

        public void AddInput(InputProperties prop, BaseInput input)
        {
            offsetToInput.Add(prop.RealOffset, input);
        }

        private void DispatchUpdate(int offset, int value)
        {
            IReadOnlyCollection<BaseInput> inputs;
            if (offsetToInput.TryGetValue(offset, out inputs))
            {
                foreach (var input in inputs)
                    input.ProcessData(this, value, offset);
            }   
        }

        public void ProcessInput()
        {
            device.Poll();

            if(device is Joystick)
            {
                var objects = (device as Joystick).GetBufferedData();
                foreach (var obj in objects)
                {
                    //System.Diagnostics.Debug.Print("{0} {1}", Convert.ToString(obj.Offset), Convert.ToString(obj.Value));
                    DispatchUpdate((int)obj.Offset, obj.Value);
                }
                    
            }
            else if(device is Mouse)
            {
                var objects = (device as Mouse).GetBufferedData();
                foreach (var obj in objects)
                    DispatchUpdate((int)obj.Offset, obj.Value);
            }
            else if (device is Keyboard)
            {
                var objects = (device as Keyboard).GetBufferedData();
                foreach (var obj in objects)
                    DispatchUpdate((int)obj.Key, obj.Value);
            }
        }
    }
}
