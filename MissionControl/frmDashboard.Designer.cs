namespace MissionControl
{
    partial class frmDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.battery = new MissionControl.UI.AGauge();
            this.attitudeIndicatorInstrumentControl1 = new MissionControl.UI.AvionicsInstrumentsControls.AttitudeIndicatorInstrumentControl();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBattery = new System.Windows.Forms.Label();
            this.lblAltitude = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.altitude = new MissionControl.UI.AGauge();
            this.aGauge2 = new MissionControl.UI.AGauge();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 33;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // battery
            // 
            this.battery.BaseArcColor = System.Drawing.Color.Gray;
            this.battery.BaseArcRadius = 80;
            this.battery.BaseArcStart = 180;
            this.battery.BaseArcSweep = 180;
            this.battery.BaseArcWidth = 2;
            this.battery.Center = new System.Drawing.Point(100, 100);
            this.battery.Location = new System.Drawing.Point(12, 12);
            this.battery.MaxValue = 100F;
            this.battery.MinValue = 0F;
            this.battery.Name = "battery";
            this.battery.NeedleColor1 = MissionControl.UI.AGaugeNeedleColor.Gray;
            this.battery.NeedleColor2 = System.Drawing.Color.DimGray;
            this.battery.NeedleRadius = 80;
            this.battery.NeedleType = MissionControl.UI.NeedleType.Advance;
            this.battery.NeedleWidth = 2;
            this.battery.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.battery.ScaleLinesInterInnerRadius = 73;
            this.battery.ScaleLinesInterOuterRadius = 80;
            this.battery.ScaleLinesInterWidth = 1;
            this.battery.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.battery.ScaleLinesMajorInnerRadius = 70;
            this.battery.ScaleLinesMajorOuterRadius = 80;
            this.battery.ScaleLinesMajorStepValue = 25F;
            this.battery.ScaleLinesMajorWidth = 2;
            this.battery.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.battery.ScaleLinesMinorInnerRadius = 75;
            this.battery.ScaleLinesMinorOuterRadius = 80;
            this.battery.ScaleLinesMinorTicks = 9;
            this.battery.ScaleLinesMinorWidth = 1;
            this.battery.ScaleNumbersColor = System.Drawing.Color.Black;
            this.battery.ScaleNumbersFormat = null;
            this.battery.ScaleNumbersRadius = 95;
            this.battery.ScaleNumbersRotation = 0;
            this.battery.ScaleNumbersStartScaleLine = 0;
            this.battery.ScaleNumbersStepScaleLines = 1;
            this.battery.Size = new System.Drawing.Size(208, 124);
            this.battery.TabIndex = 17;
            this.battery.Value = 0F;
            // 
            // attitudeIndicatorInstrumentControl1
            // 
            this.attitudeIndicatorInstrumentControl1.Location = new System.Drawing.Point(12, 207);
            this.attitudeIndicatorInstrumentControl1.Name = "attitudeIndicatorInstrumentControl1";
            this.attitudeIndicatorInstrumentControl1.Size = new System.Drawing.Size(208, 203);
            this.attitudeIndicatorInstrumentControl1.TabIndex = 20;
            this.attitudeIndicatorInstrumentControl1.Text = "attitudeIndicatorInstrumentControl1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(59, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 21);
            this.label1.TabIndex = 21;
            this.label1.Text = "Battery";
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBattery.Location = new System.Drawing.Point(124, 139);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(18, 18);
            this.lblBattery.TabIndex = 22;
            this.lblBattery.Text = "0";
            // 
            // lblAltitude
            // 
            this.lblAltitude.AutoSize = true;
            this.lblAltitude.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAltitude.Location = new System.Drawing.Point(368, 139);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(18, 18);
            this.lblAltitude.TabIndex = 25;
            this.lblAltitude.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(298, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 21);
            this.label4.TabIndex = 24;
            this.label4.Text = "Altitude";
            // 
            // altitude
            // 
            this.altitude.BaseArcColor = System.Drawing.Color.Gray;
            this.altitude.BaseArcRadius = 80;
            this.altitude.BaseArcStart = 180;
            this.altitude.BaseArcSweep = 180;
            this.altitude.BaseArcWidth = 2;
            this.altitude.Center = new System.Drawing.Point(100, 100);
            this.altitude.Location = new System.Drawing.Point(257, 12);
            this.altitude.MaxValue = 1000F;
            this.altitude.MinValue = 0F;
            this.altitude.Name = "altitude";
            this.altitude.NeedleColor1 = MissionControl.UI.AGaugeNeedleColor.Gray;
            this.altitude.NeedleColor2 = System.Drawing.Color.DimGray;
            this.altitude.NeedleRadius = 80;
            this.altitude.NeedleType = MissionControl.UI.NeedleType.Advance;
            this.altitude.NeedleWidth = 2;
            this.altitude.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.altitude.ScaleLinesInterInnerRadius = 73;
            this.altitude.ScaleLinesInterOuterRadius = 80;
            this.altitude.ScaleLinesInterWidth = 1;
            this.altitude.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.altitude.ScaleLinesMajorInnerRadius = 70;
            this.altitude.ScaleLinesMajorOuterRadius = 80;
            this.altitude.ScaleLinesMajorStepValue = 250F;
            this.altitude.ScaleLinesMajorWidth = 2;
            this.altitude.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.altitude.ScaleLinesMinorInnerRadius = 75;
            this.altitude.ScaleLinesMinorOuterRadius = 80;
            this.altitude.ScaleLinesMinorTicks = 9;
            this.altitude.ScaleLinesMinorWidth = 1;
            this.altitude.ScaleNumbersColor = System.Drawing.Color.Black;
            this.altitude.ScaleNumbersFormat = null;
            this.altitude.ScaleNumbersRadius = 95;
            this.altitude.ScaleNumbersRotation = 0;
            this.altitude.ScaleNumbersStartScaleLine = 0;
            this.altitude.ScaleNumbersStepScaleLines = 1;
            this.altitude.Size = new System.Drawing.Size(208, 121);
            this.altitude.TabIndex = 23;
            this.altitude.Value = 0F;
            // 
            // aGauge2
            // 
            this.aGauge2.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge2.BaseArcRadius = 80;
            this.aGauge2.BaseArcStart = 135;
            this.aGauge2.BaseArcSweep = 270;
            this.aGauge2.BaseArcWidth = 2;
            this.aGauge2.Center = new System.Drawing.Point(100, 100);
            this.aGauge2.Location = new System.Drawing.Point(257, 207);
            this.aGauge2.MaxValue = 50F;
            this.aGauge2.MinValue = 0F;
            this.aGauge2.Name = "aGauge2";
            this.aGauge2.NeedleColor1 = MissionControl.UI.AGaugeNeedleColor.Gray;
            this.aGauge2.NeedleColor2 = System.Drawing.Color.DimGray;
            this.aGauge2.NeedleRadius = 80;
            this.aGauge2.NeedleType = MissionControl.UI.NeedleType.Advance;
            this.aGauge2.NeedleWidth = 2;
            this.aGauge2.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleLinesInterInnerRadius = 73;
            this.aGauge2.ScaleLinesInterOuterRadius = 80;
            this.aGauge2.ScaleLinesInterWidth = 1;
            this.aGauge2.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleLinesMajorInnerRadius = 70;
            this.aGauge2.ScaleLinesMajorOuterRadius = 80;
            this.aGauge2.ScaleLinesMajorStepValue = 10F;
            this.aGauge2.ScaleLinesMajorWidth = 2;
            this.aGauge2.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.aGauge2.ScaleLinesMinorInnerRadius = 75;
            this.aGauge2.ScaleLinesMinorOuterRadius = 80;
            this.aGauge2.ScaleLinesMinorTicks = 9;
            this.aGauge2.ScaleLinesMinorWidth = 1;
            this.aGauge2.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleNumbersFormat = null;
            this.aGauge2.ScaleNumbersRadius = 95;
            this.aGauge2.ScaleNumbersRotation = 0;
            this.aGauge2.ScaleNumbersStartScaleLine = 0;
            this.aGauge2.ScaleNumbersStepScaleLines = 1;
            this.aGauge2.Size = new System.Drawing.Size(208, 187);
            this.aGauge2.TabIndex = 26;
            this.aGauge2.Text = "speed";
            this.aGauge2.Value = 0F;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeed.Location = new System.Drawing.Point(368, 392);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(18, 18);
            this.lblSpeed.TabIndex = 28;
            this.lblSpeed.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(309, 389);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 21);
            this.label3.TabIndex = 27;
            this.label3.Text = "Speed";
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 439);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.aGauge2);
            this.Controls.Add(this.lblAltitude);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.altitude);
            this.Controls.Add(this.lblBattery);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.attitudeIndicatorInstrumentControl1);
            this.Controls.Add(this.battery);
            this.Name = "frmDashboard";
            this.Text = "Dashboard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.AGauge battery;
        private System.Windows.Forms.Timer timer;
        private UI.AvionicsInstrumentsControls.AttitudeIndicatorInstrumentControl attitudeIndicatorInstrumentControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.Label lblAltitude;
        private System.Windows.Forms.Label label4;
        private UI.AGauge altitude;
        private UI.AGauge aGauge2;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label label3;
    }
}