using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace MissionControl
{
    interface IDrone
    {

        void Connect();

        void RenderVideo(RenderTarget target);

        void Disconnect();

        event EventHandler VideoFrameReady;

        bool IsConnected { get; }

        int BatteryPercent { get; }

    }
}
