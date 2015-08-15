using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const string BD_IP_ADDRESS = "192.168.42.1";
        private const int BD_DISCOVERY_PORT = 44444;

        private void Form1_Shown(object sender, EventArgs e)
        {
            eARDISCOVERY_ERROR errorDiscovery = eARDISCOVERY_ERROR.ARDISCOVERY_OK;
            var device = ARDroneSDK3.ARDISCOVERY_Device_New(ref errorDiscovery);
            if(errorDiscovery == eARDISCOVERY_ERROR.ARDISCOVERY_OK)
            {
                errorDiscovery = ARDroneSDK3.ARDISCOVERY_Device_InitWifi(device, eARDISCOVERY_PRODUCT.ARDISCOVERY_PRODUCT_ARDRONE, "BepopDrone", BD_IP_ADDRESS, BD_DISCOVERY_PORT);
            }
        }
    }
}
