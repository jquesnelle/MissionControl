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

        private IDrone drone;
        private Input.InputManager input;

        private MethodInvoker refreshDelegate;

        private void Form1_Shown(object sender, EventArgs e)
        {
            refreshDelegate = (MethodInvoker)delegate { RefreshVideo(); };

            input = new Input.InputManager();
            input.Take_off.ButtonPressed += OnTakeOffPressed;
            input.Land.ButtonPressed += OnLandPressed;
            input.Hover_Enable.ButtonPressed += OnHoverEnablePressed;
            input.Hover_Disable.ButtonPressed += OnHoverDisablePressed;
        }

        private void SetupDrone()
        {
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

        private void OnTakeOffPressed(Input.IButtonInput input)
        {
            if (drone != null && drone.IsConnected)
            {
                if (drone is BepopDrone)
                {
                    BepopDrone bepop = (BepopDrone)drone;

                    ARDroneSDK3.ARCONTROLLER_FEATURE_ARDrone3_SendPilotingTakeOff(bepop.DeviceController.aRDrone3);
                }
            }
        }

        private void OnLandPressed(Input.IButtonInput input)
        {
            if (drone != null && drone.IsConnected)
            {
                if (drone is BepopDrone)
                {
                    BepopDrone bepop = (BepopDrone)drone;

                    ARDroneSDK3.ARCONTROLLER_FEATURE_ARDrone3_SendPilotingLanding(bepop.DeviceController.aRDrone3);
                }
            }
        }

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

        private bool hoverEnabled = false;

        private void mainLoop_Tick(object sender, EventArgs e)
        {

            input.Process();

            lblRoll.Text = Convert.ToString(input.Roll.Value);
            lblPitch.Text = Convert.ToString(input.Pitch.Value);
            lblYaw.Text = Convert.ToString(input.Yaw.Value);
            lblClimbDescend.Text = Convert.ToString(input.Climb_Descend.Value);
            lblTakeOff.Text = Convert.ToString(input.Take_off.Value);
            lblLand.Text = Convert.ToString(input.Land.Value);
            lblControlsEnable.Text = Convert.ToString(input.Hover_Enable.Value);
            lblControlsDisable.Text = Convert.ToString(input.Hover_Disable.Value);

            if (drone != null && drone.IsConnected)
            {
                if (drone is BepopDrone)
                {
                    BepopDrone bepop = (BepopDrone)drone;


                    sbyte roll = (sbyte)(hoverEnabled ? 0 : input.Roll.Value);
                    sbyte pitch = (sbyte)(hoverEnabled ? 0 : input.Pitch.Value);
                    sbyte yaw = (sbyte)(input.Yaw.Value);
                    sbyte gaz = (sbyte)(hoverEnabled ? 0 : input.Climb_Descend.Value);

                    eARCONTROLLER_ERROR pilotResult =  ARDroneSDK3.ARCONTROLLER_FEATURE_ARDrone3_SetPilotingPCMD(bepop.DeviceController.aRDrone3, 1, 
                        roll, pitch, yaw, gaz, 0);

                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (drone == null && e.KeyCode == Keys.F5)
                SetupDrone();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
