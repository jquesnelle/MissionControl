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
using SharpDX.Direct2D1;

namespace MissionControl.Drone
{
    interface IDrone
    {

        void Connect();

        void RenderVideo(RenderTarget target);

        void Disconnect();

        event EventHandler VideoFrameReady;

        bool IsConnected { get; }

        int BatteryPercent { get; }

        float Altitude { get; }

        float Latitude { get; }

        float Longitude { get; }

        void TakeOff();

        void Land();

        void Pilot(int roll, int pitch, int yaw, int climbDescend);

    }
}
