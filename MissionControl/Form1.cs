using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpDX.MediaFoundation;

/*
If using Windows 7:
https://support.microsoft.com/en-us/kb/2670838
*/

namespace MissionControl
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StateChangedCallbackDelegate(eARCONTROLLER_DEVICE_STATE newState, eARCONTROLLER_ERROR error, System.IntPtr customData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CommandReceivedCallbackDelegate(eARCONTROLLER_DICTIONARY_KEY commandKey, System.IntPtr elementDictionary, System.IntPtr customData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DidReceiveFrameCallbackDelegate(System.IntPtr frame, System.IntPtr customData);

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const string BD_IP_ADDRESS = "192.168.42.1";
        private const int BD_DISCOVERY_PORT = 44444;

        public StateChangedCallbackDelegate OnStateChanged;
        private CommandReceivedCallbackDelegate OnCommandReceived;
        private DidReceiveFrameCallbackDelegate OnFrameReceived;

        void StateChangedCallback(eARCONTROLLER_DEVICE_STATE newState, eARCONTROLLER_ERROR error, System.IntPtr customData)
        {
            switch(newState)
            {
                case eARCONTROLLER_DEVICE_STATE.ARCONTROLLER_DEVICE_STATE_RUNNING:
                    StartVideo();
                    break;

            }
        }

        void CommandReceivedCallback(eARCONTROLLER_DICTIONARY_KEY commandKey, System.IntPtr elementDictionary, System.IntPtr customData)
        {

        }

        void DidReceiveFrameCallbackDelegate(System.IntPtr frame, System.IntPtr customData)
        {

        }

        static Guid CLSID_CMSH264DecoderMFT = Guid.Parse("62CE7E72-4C71-4D20-B15D-452831A87D9D");
        static Guid CODECAPI_AVDecVideoAcceleration_H264 = Guid.Parse("f7db8a2f-4f48-4ee8-ae31-8b6ebe558ae2");
        static Guid CODECAPI_AVLowLatencyMode = Guid.Parse("9c27891a-ed7a-40e1-88e8-b22727a024ee");

        private static ulong PackLong(uint a, uint b)
        {
            ulong ret = a;
            a <<= 32;
            ret |= b;
            return ret;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            eARDISCOVERY_ERROR errorDiscovery = eARDISCOVERY_ERROR.ARDISCOVERY_OK;
            eARCONTROLLER_ERROR error = eARCONTROLLER_ERROR.ARCONTROLLER_OK;

            var device = ARDroneSDK3.ARDISCOVERY_Device_New(ref errorDiscovery);
            if (errorDiscovery != eARDISCOVERY_ERROR.ARDISCOVERY_OK)
                return;
            errorDiscovery = ARDroneSDK3.ARDISCOVERY_Device_InitWifi(device, eARDISCOVERY_PRODUCT.ARDISCOVERY_PRODUCT_ARDRONE, "BepopDrone", BD_IP_ADDRESS, BD_DISCOVERY_PORT);
            if (errorDiscovery != eARDISCOVERY_ERROR.ARDISCOVERY_OK)
                return;

            var deviceController = ARDroneSDK3.ARCONTROLLER_Device_New(device, ref error);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnStateChanged += StateChangedCallback;

            error = ARDroneSDK3.ARCONTROLLER_Device_AddStateChangedCallback(deviceController, Marshal.GetFunctionPointerForDelegate(OnStateChanged), deviceController.swigCPtr.Handle);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnCommandReceived += CommandReceivedCallback;
            error = ARDroneSDK3.ARCONTROLLER_Device_AddCommandReceivedCallback(deviceController, Marshal.GetFunctionPointerForDelegate(OnCommandReceived), deviceController.swigCPtr.Handle);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnFrameReceived += DidReceiveFrameCallbackDelegate;
            error = ARDroneSDK3.ARCONTROLLER_Device_SetVideoReceiveCallback(deviceController, Marshal.GetFunctionPointerForDelegate(OnFrameReceived), System.IntPtr.Zero, IntPtr.Zero);

            error = ARDroneSDK3.ARCONTROLLER_Device_Start(deviceController);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

        
        }

        private void StartVideo()
        {
            var decoder = new SharpDX.MediaFoundation.Transform(CLSID_CMSH264DecoderMFT);

            MediaType inputType = new MediaType();
            inputType.Set(MediaTypeAttributeKeys.MajorType.Guid, MediaTypeGuids.Video);
            inputType.Set(MediaTypeAttributeKeys.Subtype.Guid, VideoFormatGuids.H264);
            inputType.Set(MediaTypeAttributeKeys.FrameSize.Guid, PackLong(640, 380));
            inputType.Set(MediaTypeAttributeKeys.FrameRate.Guid, PackLong(30, 1));
            inputType.Set(MediaTypeAttributeKeys.PixelAspectRatio.Guid, PackLong(1, 1));
            inputType.Set(MediaTypeAttributeKeys.InterlaceMode.Guid, VideoInterlaceMode.MixedInterlaceOrProgressive);
            decoder.SetInputType(0, inputType, 0);

            MediaType outputType = new MediaType();
            inputType.Set(MediaTypeAttributeKeys.MajorType.Guid, MediaTypeGuids.Video);
            inputType.Set(MediaTypeAttributeKeys.Subtype.Guid, VideoFormatGuids.Iyuv);
            inputType.Set(MediaTypeAttributeKeys.FrameSize.Guid, PackLong(640, 380));
            inputType.Set(MediaTypeAttributeKeys.FrameRate.Guid, PackLong(30, 1));
            inputType.Set(MediaTypeAttributeKeys.PixelAspectRatio.Guid, PackLong(1, 1));
            inputType.Set(MediaTypeAttributeKeys.InterlaceMode.Guid, VideoInterlaceMode.MixedInterlaceOrProgressive);
            decoder.SetOutputType(0, outputType, 0);

            decoder.Attributes.Set<uint>(CODECAPI_AVDecVideoAcceleration_H264, 1);
            decoder.Attributes.Set<uint>(CODECAPI_AVLowLatencyMode, 1);

            int inputStatus;
            decoder.GetInputStatus(0, out inputStatus);

            if (inputStatus == (int)MftInputStatusFlags.MftInputStatusAcceptData)
            {

            }
        }


    }
}
