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

namespace MissionControl.Input
{
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
        FLAT_TRIM = 8,
        TAKE_PHOTO = 9,
        START_VIDEO = 10,
        STOP_VIDEO = 11,
        CAMERA_TILT_AXIS = 12,
        CAMERA_PAN_AXIS = 13
    }
}
