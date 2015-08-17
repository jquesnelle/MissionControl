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

namespace MissionControl
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StateChangedCallbackDelegate(eARCONTROLLER_DEVICE_STATE newState, eARCONTROLLER_ERROR error, System.IntPtr customData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CommandReceivedCallbackDelegate(eARCONTROLLER_DICTIONARY_KEY commandKey, System.IntPtr elementDictionary, System.IntPtr customData);

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const string BD_IP_ADDRESS = "192.168.42.1";
        private const int BD_DISCOVERY_PORT = 44444;

        private StateChangedCallbackDelegate OnStateChanged;
        private CommandReceivedCallbackDelegate OnCommandReceived;

        void StateChangedCallback(eARCONTROLLER_DEVICE_STATE newState, eARCONTROLLER_ERROR error, System.IntPtr customData)
        {

        }

        void CommandReceivedCallback(eARCONTROLLER_DICTIONARY_KEY commandKey, System.IntPtr elementDictionary, System.IntPtr customData)
        {

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

            error = ARDroneSDK3.ARCONTROLLER_Device_AddStateChangedCallback(deviceController, OnStateChanged, deviceController.swigCPtr.Handle);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnCommandReceived += CommandReceivedCallback;
            error = ARDroneSDK3.ARCONTROLLER_Device_AddCommandReceivedCallback(deviceController, OnCommandReceived, deviceController.swigCPtr.Handle);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            error = ARDroneSDK3.ARCONTROLLER_Device_Start(deviceController);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;
        }
    }
}
