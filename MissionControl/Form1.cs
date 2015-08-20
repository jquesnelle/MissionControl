using System;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using SharpDX.DirectInput;

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

            SetupInput();
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

        private static void PrintConnectedDevices(DirectInput directInput)
        {
            for (int i = 17; i <= 28; ++i)
            {
                DeviceType type = (DeviceType)i;
                Debug.Print("--- {0} ---", type.ToString());
                foreach (var deviceInstance in directInput.GetDevices(type, DeviceEnumerationFlags.AttachedOnly))
                    Debug.Print("{0}: {1} (subtype {2})", deviceInstance.InstanceGuid, deviceInstance.InstanceName, Convert.ToString(deviceInstance.Subtype));
            }
        }

        private DirectInput directInput;
        private Joystick stick;
        private Joystick throttle;

        private void SetupInput()
        {
            directInput = new DirectInput();

            Guid stickGuid = Guid.Empty;
            Guid throttleGuid = Guid.Empty;

            PrintConnectedDevices(directInput);
          
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly))
            {
                if (deviceInstance.ProductName.Equals("Saitek Pro Flight X-55 Rhino Stick"))
                    stickGuid = deviceInstance.InstanceGuid;
            }

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.FirstPerson, DeviceEnumerationFlags.AttachedOnly))
            {
                if (deviceInstance.ProductName.Equals("Saitek Pro Flight X-55 Rhino Throttle"))
                    throttleGuid = deviceInstance.InstanceGuid;
            }

            stick = new Joystick(directInput, stickGuid);
            stick.Properties.BufferSize = 128;
            stick.Acquire();

            throttle = new Joystick(directInput, throttleGuid);
            throttle.Properties.BufferSize = 128;
            throttle.Acquire();

            //TODO: set real defaults from directinput
            for(int i = 0; i < lastStickStates.Length; ++i)
                lastStickStates[i] = lastThrottleStates[i] = 0;
            lastStickStates[(int)JoystickOffset.X] = BEPOP_AXIS_OFFSET;
            lastStickStates[(int)JoystickOffset.Y] = BEPOP_AXIS_OFFSET;
            lastStickStates[(int)JoystickOffset.RotationZ] = BEPOP_AXIS_OFFSET;


        }

        private const bool PRINT_STICK_OUTPUTS = true;
        private const bool PRINT_THROTTLE_OUTPUTS = false;
        private const int BEPOP_AXIS_OFFSET = 65535 / 2;
        private const float BEPOP_AXIS_SCALE = 65535.0f / 200.0f;
        private const float SCALED_AXIS_DEADBAND = 3.0f;

        private int[] lastStickStates = new int[300];
        private int[] lastThrottleStates = new int[300];

        private bool sendCommands = false;

        private void mainLoop_Tick(object sender, EventArgs e)
        {

            bool takeOffPressed = false, landPressed = false, resetCommands = false;

            if(stick != null)
            {
                stick.Poll();
                var datas = stick.GetBufferedData();
                foreach (var state in datas)
                {
                    if (PRINT_STICK_OUTPUTS)
                    {
                        if(state.Offset != JoystickOffset.X && state.Offset != JoystickOffset.Y && state.Offset != JoystickOffset.RotationZ)
                            Debug.Print(state.ToString());
                    }
                            
                        
                    lastStickStates[(int)state.Offset] = state.Value;

                }
                       
            }
            if (throttle != null)
            {
                throttle.Poll();
                var datas = throttle.GetBufferedData();
                
                foreach (var state in datas)
                {
                    if (PRINT_THROTTLE_OUTPUTS)
                        Debug.Print(state.ToString());

                    switch (state.Offset)
                    {
                        case JoystickOffset.Buttons5: //sw1
                            takeOffPressed = lastThrottleStates[(int)JoystickOffset.Buttons5] == 0 && state.Value > 0;
                            break;
                        case JoystickOffset.Buttons6: //sw2
                            landPressed = lastThrottleStates[(int)JoystickOffset.Buttons6] == 0 && state.Value > 0;
                            break;
                        case JoystickOffset.Buttons34: //slider
                            if (!sendCommands && state.Value > 0)
                                sendCommands = true;
                            else if (sendCommands && state.Value == 0)
                                resetCommands = true;
                            break;
                    }

                    lastThrottleStates[(int)state.Offset] = state.Value;
                }
                        
            }

            int roll = lastStickStates[(int)JoystickOffset.RotationZ] - BEPOP_AXIS_OFFSET;
            int pitch = lastStickStates[(int)JoystickOffset.Y] - BEPOP_AXIS_OFFSET;
            int yaw = lastStickStates[(int)JoystickOffset.X] - BEPOP_AXIS_OFFSET;
            int gaz = 0;
            if (lastStickStates[(int)JoystickOffset.Buttons10] > 0)
                gaz = 100;
            else if (lastStickStates[(int)JoystickOffset.Buttons12] > 0)
                gaz = -100;

            float froll = (float)roll / BEPOP_AXIS_SCALE;
            float fpitch = (float)pitch / BEPOP_AXIS_SCALE;
            float fyaw = (float)yaw / BEPOP_AXIS_SCALE;

            if (Math.Abs(froll) <= SCALED_AXIS_DEADBAND)
                froll = 0;
            if (Math.Abs(fpitch) <= SCALED_AXIS_DEADBAND)
                fpitch = 0;
            if (Math.Abs(fyaw) <= SCALED_AXIS_DEADBAND)
                fyaw = 0;

            if (resetCommands)
                froll = fpitch = fyaw = 0;

            lblRoll.Text = String.Format("{0:0.0}", froll);
            lblPitch.Text = String.Format("{0:0.0}", fpitch);
            lblYaw.Text = String.Format("{0:0.0}", fyaw);

            if (drone != null && drone.IsConnected)
            {
                if (drone is BepopDrone)
                {
                    BepopDrone bepop = (BepopDrone)drone;

                    if (takeOffPressed)
                        ARDroneSDK3.ARCONTROLLER_FEATURE_ARDrone3_SendPilotingTakeOff(bepop.DeviceController.aRDrone3);
                    else if (landPressed)
                        ARDroneSDK3.ARCONTROLLER_FEATURE_ARDrone3_SendPilotingLanding(bepop.DeviceController.aRDrone3);

                    if(sendCommands)
                    {
                        eARCONTROLLER_ERROR pilotResult =  ARDroneSDK3.ARCONTROLLER_FEATURE_ARDrone3_SetPilotingPCMD(bepop.DeviceController.aRDrone3, 1, (sbyte)froll, (sbyte)fpitch, (sbyte)fyaw, (sbyte)gaz, 0);

                        if (resetCommands)
                            sendCommands = false;

                    }
                        
                }

            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (drone == null && e.KeyCode == Keys.F5)
                SetupDrone();
        }

    }
}
