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

                attitudeIndicatorInstrumentControl1.SetAttitudeIndicatorParameters(Program.Drone.Pitch, Program.Drone.Roll);

                speedX.Value = Program.Drone.SpeedX;
                lblSpeedX.Text = String.Format("{0:0.0} m/s", Program.Drone.SpeedX);

                speedY.Value = Program.Drone.SpeedY;
                lblSpeedY.Text = String.Format("{0:0.0} m/s", Program.Drone.SpeedY);

                speedZ.Value = Program.Drone.SpeedZ;
                lblSpeedZ.Text = String.Format("{0:0.0} m/s", Program.Drone.SpeedZ);
            }
        }
    }
}
