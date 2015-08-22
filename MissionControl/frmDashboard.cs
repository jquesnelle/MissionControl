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
    public partial class frmDashboard : Form
    {
        public frmDashboard()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if(Program.Drone != null && Program.Drone.IsConnected)
            {
                battery.Value = Program.Drone.BatteryPercent;
                lblBattery.Text = String.Format("{0}%", Program.Drone.BatteryPercent);

                altitude.Value = Program.Drone.Altitude;
                lblAltitude.Text = String.Format("{0:0.0} m", Program.Drone.Altitude);
            }
        }
    }
}
