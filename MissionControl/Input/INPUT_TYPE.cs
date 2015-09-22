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
    }
}
