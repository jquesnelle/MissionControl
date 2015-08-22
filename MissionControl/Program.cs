using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionControl
{
    static class Program
    {

        public static Drone.IDrone Drone { get; set; }
        public static Input.InputManager Input { get; private set; }

        public static Form1 MainForm { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Input = new MissionControl.Input.InputManager();

            Application.Run(MainForm = new Form1());
        }
    }
}
