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
using System.Diagnostics;
using System.Windows.Forms;
using SharpDX.Direct2D1;


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

        

        private MethodInvoker refreshDelegate;

        private void Form1_Shown(object sender, EventArgs e)
        {
            refreshDelegate = (MethodInvoker)delegate { RefreshVideo(); };

            new frmInputStatus().Show();
            new frmDashboard().Show();

            Program.Input.Take_off.ButtonPressed += OnTakeOffPressed;
            Program.Input.Land.ButtonPressed += OnLandPressed;
            Program.Input.Hover_Enable.ButtonPressed += OnHoverEnablePressed;
            Program.Input.Hover_Disable.ButtonPressed += OnHoverDisablePressed;
            Program.Input.Flat_Trim.ButtonPressed += OnFlatTrimPressed;
            Program.Input.Take_Photo.ButtonPressed += OnTakePhotoPressed;
            Program.Input.Start_Video.ButtonPressed += OnStartVideoPressed;
            Program.Input.Stop_Video.ButtonPressed += OnStopVideoPressed;
        }

        private void SetupDrone()
        {
            Program.Drone = new Drone.BepopDrone();
            Program.Drone.VideoFrameReady += BepopDrone_VideoFrameReady;
            Program.Drone.Connect();
            sentZero = false;
            sentTiltZero = false;
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
            if (Program.Drone != null)
                Program.Drone.RenderVideo(target);
           
        }

        private void OnTakeOffPressed(Input.IButtonInput input)
        {
            if ((Program.Drone != null) && Program.Drone.IsConnected && !Program.Drone.IsFlying)
                Program.Drone.TakeOff();
        }

        private void OnLandPressed(Input.IButtonInput input)
        {
            if ((Program.Drone != null) && Program.Drone.IsConnected && Program.Drone.IsFlying)
                Program.Drone.Land();
        }


        private bool hoverEnabled = false;

        private void OnHoverEnablePressed(Input.IButtonInput input)
        {
            if (!hoverEnabled)
                hoverEnabled = true;
        }

        private void OnHoverDisablePressed(Input.IButtonInput input)
        {
            if (hoverEnabled)
                hoverEnabled = false;

        }

        private void OnFlatTrimPressed(Input.IButtonInput input)
        {
            if ((Program.Drone != null) && Program.Drone.IsConnected && !Program.Drone.IsFlying)
                Program.Drone.FlatTrim();
        }

        private void OnTakePhotoPressed(Input.IButtonInput input)
        {
            if ((Program.Drone != null) && Program.Drone.IsConnected)
                Program.Drone.TakePhoto();
        }

        private void OnStartVideoPressed(Input.IButtonInput input)
        {
            if ((Program.Drone != null) && Program.Drone.IsConnected && !Program.Drone.IsRecordingVideo)
                Program.Drone.StartRecordingVideo();
        }

        private void OnStopVideoPressed(Input.IButtonInput input)
        {
            if ((Program.Drone != null) && Program.Drone.IsConnected && Program.Drone.IsRecordingVideo)
                Program.Drone.StopRecordingVideo();
        }

        private bool sentZero;
        private bool sentTiltZero;

        private void mainLoop_Tick(object sender, EventArgs e)
        {

            Program.Input.Process();

            if (Program.Drone != null && Program.Drone.IsConnected)
            {

                int roll = hoverEnabled ? 0 : Program.Input.Roll.Value;
                int pitch = hoverEnabled ? 0 : Program.Input.Pitch.Value;
                int yaw = Program.Input.Yaw.Value;
                int climbDescend = hoverEnabled ? 0 : Program.Input.Climb_Descend.Value;
                int tilt = Program.Input.Camera_Tilt.Value;

                bool send = false, sendTilt = false;
                bool isZero = (roll == 0 && pitch == 0 && yaw == 0 && climbDescend == 0) || hoverEnabled;
                bool isTiltZero = tilt == 0;

                if (isZero)
                {
                    if (!sentZero)
                    {
                        send = true;
                        sentZero = true;
                    }
                }
                else
                {
                    send = true;
                    sentZero = false;
                }

                if(isTiltZero)
                {
                    if(!sentTiltZero)
                    {
                        sendTilt = true;
                        sentTiltZero = true;
                    }
                }
                else
                {
                    sendTilt = true;
                    sentTiltZero = false;
                }
                    
                if(send)
                    Program.Drone.Pilot(roll, pitch, yaw, climbDescend);

                if (sendTilt)
                    Program.Drone.Camera(tilt, 0);
  
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Program.Drone == null && e.KeyCode == Keys.F5)
                SetupDrone();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
