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

    public partial class frmInputStatus : Form
    {
        public frmInputStatus()
        {
            InitializeComponent();
        }

        private class StatusElements
        {
            private Input.INPUT_TYPE type;

            private Panel panel;
            private Label name;
            private Label value;
            private CheckBox box;

            public Control Control
            {
                get
                {
                    return (Control)value ?? box;
                }
            }

            public StatusElements(frmInputStatus form, Input.INPUT_TYPE type)
            {
                this.type = type;

                panel = new Panel();
                panel.Parent = form.flow;

                name = new Label();
                name.Parent = panel;
                name.Text = Input.InputManager.GetInputTypeName(type);
                name.Font = new Font(name.Font.Name, 12);
                name.Width = 150;

                value = new Label();
                value.Parent = panel;
                value.Left = name.Width;
                value.Font = name.Font;

                panel.AutoSize = true;
                
            }

            public void Update()
            {
                Input.IInput input = Program.Input.GetInput<Input.IInput>(type);
                if (input != null)
                {
                    value.Text = Convert.ToString(input.Value);
                }
                else
                    value.Text = "";
            }
        }

        private readonly List<StatusElements> statusElements = new List<StatusElements>();

        private void frmInputStatus_Shown(object sender, EventArgs e)
        {
            foreach (var type in EnumExtensions.GetAllItems<Input.INPUT_TYPE>())
            {
                if(Program.Input.HaveInput(type))
                    statusElements.Add(new StatusElements(this, type));
            }

            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (var element in statusElements)
                element.Update();
        }

        private void flow_Resize(object sender, EventArgs e)
        {
            Height = flow.Height + flow.Top + 5;
            Width = flow.Width + flow.Left;
        }
    }
}
