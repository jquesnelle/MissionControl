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
    class NormalizedAxisInput : BaseObjectInput
    {
        float nRange;
        float pRange;
        bool inverted;

        int rawRange;
        int rawOffset;

        public NormalizedAxisInput(InputProperties p, ushort r, bool inverted) : base(p)
        {
            int sr = r;
            pRange = sr;
            nRange = sr * -1;
            this.inverted = inverted;

            if (properties.Information.Aspect == ObjectAspect.Position)
            {
                rawRange = Math.Abs(p.Properties.Range.Maximum - p.Properties.Range.Minimum);
                rawOffset = p.Properties.Range.Minimum;
            }
        }

        internal override void ProcessData(IConnectedDevice device, int incomingValue, int offset)
        {
            switch (properties.Information.Aspect)
            {
                case ObjectAspect.Position:
                    int pos = (int)lerp(nRange, pRange, (float)incomingValue / rawRange) + rawOffset;
                    if (inverted)
                        pos *= -1;
                    OnNewValue(pos);
                    break;

            }
        }

        private static float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }
    }
}
