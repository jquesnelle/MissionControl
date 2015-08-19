using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpDX.MediaFoundation;
using SharpDX.Direct2D1;
using SharpDX;
/*
If using Windows 7:
https://support.microsoft.com/en-us/kb/2670838
*/

namespace MissionControl
{


    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private IDrone drone;

        private MethodInvoker refreshDelegate;

        private void Form1_Shown(object sender, EventArgs e)
        {
            refreshDelegate = (MethodInvoker)delegate { RefreshVideo(); };

            drone = new BepopDrone();
            drone.VideoFrameReady += BepopDrone_VideoFrameReady;
            drone.Connect();
        }

        private void RefreshVideo()
        {
            window.Refresh();
        }

        private void BepopDrone_VideoFrameReady(object sender, EventArgs e)
        {
            if (window.InvokeRequired)
                window.Invoke(refreshDelegate);
            else
                refreshDelegate();
        }

        private void window_Render_1(object sender, EventArgs e, RenderTarget target)
        {
            target.Clear(SharpDX.Color.Black);
            if (drone != null)
                drone.RenderVideo(target);
           
        }
    }
}
