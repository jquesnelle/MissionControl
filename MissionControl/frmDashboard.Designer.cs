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
            this.speedX = new MissionControl.UI.AGauge();
            this.lblSpeedX = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSpeedY = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSpeedZ = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.speedY = new MissionControl.UI.AGauge();
            this.speedZ = new MissionControl.UI.AGauge();
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
            this.attitudeIndicatorInstrumentControl1.Location = new System.Drawing.Point(226, 15);
            this.attitudeIndicatorInstrumentControl1.Name = "attitudeIndicatorInstrumentControl1";
            this.attitudeIndicatorInstrumentControl1.Size = new System.Drawing.Size(208, 203);
            this.attitudeIndicatorInstrumentControl1.TabIndex = 20;
            this.attitudeIndicatorInstrumentControl1.Text = "attitude";
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
            this.lblAltitude.Location = new System.Drawing.Point(568, 142);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(18, 18);
            this.lblAltitude.TabIndex = 25;
            this.lblAltitude.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(498, 139);
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
            this.altitude.Location = new System.Drawing.Point(457, 15);
            this.altitude.MaxValue = 300F;
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
            this.altitude.ScaleLinesMajorStepValue = 100F;
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
            // speedX
            // 
            this.speedX.BaseArcColor = System.Drawing.Color.Gray;
            this.speedX.BaseArcRadius = 80;
            this.speedX.BaseArcStart = 135;
            this.speedX.BaseArcSweep = 270;
            this.speedX.BaseArcWidth = 2;
            this.speedX.Center = new System.Drawing.Point(100, 100);
            this.speedX.Location = new System.Drawing.Point(12, 240);
            this.speedX.MaxValue = 30F;
            this.speedX.MinValue = 0F;
            this.speedX.Name = "speedX";
            this.speedX.NeedleColor1 = MissionControl.UI.AGaugeNeedleColor.Gray;
            this.speedX.NeedleColor2 = System.Drawing.Color.DimGray;
            this.speedX.NeedleRadius = 80;
            this.speedX.NeedleType = MissionControl.UI.NeedleType.Advance;
            this.speedX.NeedleWidth = 2;
            this.speedX.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.speedX.ScaleLinesInterInnerRadius = 73;
            this.speedX.ScaleLinesInterOuterRadius = 80;
            this.speedX.ScaleLinesInterWidth = 1;
            this.speedX.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.speedX.ScaleLinesMajorInnerRadius = 70;
            this.speedX.ScaleLinesMajorOuterRadius = 80;
            this.speedX.ScaleLinesMajorStepValue = 5F;
            this.speedX.ScaleLinesMajorWidth = 2;
            this.speedX.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.speedX.ScaleLinesMinorInnerRadius = 75;
            this.speedX.ScaleLinesMinorOuterRadius = 80;
            this.speedX.ScaleLinesMinorTicks = 9;
            this.speedX.ScaleLinesMinorWidth = 1;
            this.speedX.ScaleNumbersColor = System.Drawing.Color.Black;
            this.speedX.ScaleNumbersFormat = null;
            this.speedX.ScaleNumbersRadius = 95;
            this.speedX.ScaleNumbersRotation = 0;
            this.speedX.ScaleNumbersStartScaleLine = 0;
            this.speedX.ScaleNumbersStepScaleLines = 1;
            this.speedX.Size = new System.Drawing.Size(208, 187);
            this.speedX.TabIndex = 26;
            this.speedX.Text = "speed";
            this.speedX.Value = 0F;
            // 
            // lblSpeedX
            // 
            this.lblSpeedX.AutoSize = true;
            this.lblSpeedX.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeedX.Location = new System.Drawing.Point(123, 425);
            this.lblSpeedX.Name = "lblSpeedX";
            this.lblSpeedX.Size = new System.Drawing.Size(18, 18);
            this.lblSpeedX.TabIndex = 28;
            this.lblSpeedX.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(51, 422);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 21);
            this.label3.TabIndex = 27;
            this.label3.Text = "Speed X";
            // 
            // lblSpeedY
            // 
            this.lblSpeedY.AutoSize = true;
            this.lblSpeedY.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeedY.Location = new System.Drawing.Point(337, 425);
            this.lblSpeedY.Name = "lblSpeedY";
            this.lblSpeedY.Size = new System.Drawing.Size(18, 18);
            this.lblSpeedY.TabIndex = 31;
            this.lblSpeedY.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(265, 422);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 21);
            this.label5.TabIndex = 30;
            this.label5.Text = "Speed Y";
            // 
            // lblSpeedZ
            // 
            this.lblSpeedZ.AutoSize = true;
            this.lblSpeedZ.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeedZ.Location = new System.Drawing.Point(568, 425);
            this.lblSpeedZ.Name = "lblSpeedZ";
            this.lblSpeedZ.Size = new System.Drawing.Size(18, 18);
            this.lblSpeedZ.TabIndex = 34;
            this.lblSpeedZ.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(496, 422);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 21);
            this.label7.TabIndex = 33;
            this.label7.Text = "Speed Z";
            // 
            // speedY
            // 
            this.speedY.BaseArcColor = System.Drawing.Color.Gray;
            this.speedY.BaseArcRadius = 80;
            this.speedY.BaseArcStart = 135;
            this.speedY.BaseArcSweep = 270;
            this.speedY.BaseArcWidth = 2;
            this.speedY.Center = new System.Drawing.Point(100, 100);
            this.speedY.Location = new System.Drawing.Point(226, 232);
            this.speedY.MaxValue = 30F;
            this.speedY.MinValue = 0F;
            this.speedY.Name = "speedY";
            this.speedY.NeedleColor1 = MissionControl.UI.AGaugeNeedleColor.Gray;
            this.speedY.NeedleColor2 = System.Drawing.Color.DimGray;
            this.speedY.NeedleRadius = 80;
            this.speedY.NeedleType = MissionControl.UI.NeedleType.Advance;
            this.speedY.NeedleWidth = 2;
            this.speedY.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.speedY.ScaleLinesInterInnerRadius = 73;
            this.speedY.ScaleLinesInterOuterRadius = 80;
            this.speedY.ScaleLinesInterWidth = 1;
            this.speedY.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.speedY.ScaleLinesMajorInnerRadius = 70;
            this.speedY.ScaleLinesMajorOuterRadius = 80;
            this.speedY.ScaleLinesMajorStepValue = 5F;
            this.speedY.ScaleLinesMajorWidth = 2;
            this.speedY.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.speedY.ScaleLinesMinorInnerRadius = 75;
            this.speedY.ScaleLinesMinorOuterRadius = 80;
            this.speedY.ScaleLinesMinorTicks = 9;
            this.speedY.ScaleLinesMinorWidth = 1;
            this.speedY.ScaleNumbersColor = System.Drawing.Color.Black;
            this.speedY.ScaleNumbersFormat = null;
            this.speedY.ScaleNumbersRadius = 95;
            this.speedY.ScaleNumbersRotation = 0;
            this.speedY.ScaleNumbersStartScaleLine = 0;
            this.speedY.ScaleNumbersStepScaleLines = 1;
            this.speedY.Size = new System.Drawing.Size(208, 187);
            this.speedY.TabIndex = 35;
            this.speedY.Text = "speed";
            this.speedY.Value = 0F;
            // 
            // speedZ
            // 
            this.speedZ.BaseArcColor = System.Drawing.Color.Gray;
            this.speedZ.BaseArcRadius = 80;
            this.speedZ.BaseArcStart = 135;
            this.speedZ.BaseArcSweep = 270;
            this.speedZ.BaseArcWidth = 2;
            this.speedZ.Center = new System.Drawing.Point(100, 100);
            this.speedZ.Location = new System.Drawing.Point(457, 232);
            this.speedZ.MaxValue = 30F;
            this.speedZ.MinValue = 0F;
            this.speedZ.Name = "speedZ";
            this.speedZ.NeedleColor1 = MissionControl.UI.AGaugeNeedleColor.Gray;
            this.speedZ.NeedleColor2 = System.Drawing.Color.DimGray;
            this.speedZ.NeedleRadius = 80;
            this.speedZ.NeedleType = MissionControl.UI.NeedleType.Advance;
            this.speedZ.NeedleWidth = 2;
            this.speedZ.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.speedZ.ScaleLinesInterInnerRadius = 73;
            this.speedZ.ScaleLinesInterOuterRadius = 80;
            this.speedZ.ScaleLinesInterWidth = 1;
            this.speedZ.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.speedZ.ScaleLinesMajorInnerRadius = 70;
            this.speedZ.ScaleLinesMajorOuterRadius = 80;
            this.speedZ.ScaleLinesMajorStepValue = 5F;
            this.speedZ.ScaleLinesMajorWidth = 2;
            this.speedZ.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.speedZ.ScaleLinesMinorInnerRadius = 75;
            this.speedZ.ScaleLinesMinorOuterRadius = 80;
            this.speedZ.ScaleLinesMinorTicks = 9;
            this.speedZ.ScaleLinesMinorWidth = 1;
            this.speedZ.ScaleNumbersColor = System.Drawing.Color.Black;
            this.speedZ.ScaleNumbersFormat = null;
            this.speedZ.ScaleNumbersRadius = 95;
            this.speedZ.ScaleNumbersRotation = 0;
            this.speedZ.ScaleNumbersStartScaleLine = 0;
            this.speedZ.ScaleNumbersStepScaleLines = 1;
            this.speedZ.Size = new System.Drawing.Size(208, 187);
            this.speedZ.TabIndex = 36;
            this.speedZ.Text = "speed";
            this.speedZ.Value = 0F;
            // 
            // frmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 465);
            this.Controls.Add(this.speedZ);
            this.Controls.Add(this.speedY);
            this.Controls.Add(this.lblSpeedZ);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblSpeedY);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblSpeedX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.speedX);
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
        private UI.AGauge speedX;
        private System.Windows.Forms.Label lblSpeedX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSpeedY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSpeedZ;
        private System.Windows.Forms.Label label7;
        private UI.AGauge speedY;
        private UI.AGauge speedZ;
    }
}