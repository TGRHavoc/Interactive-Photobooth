using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Interactive_Photobooth
{
    static class Program
    {
        // Nice boolean to tell the program whether or not to attach console for easy debugging
        public static bool DEBUG = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (DEBUG)
            {
                if (!AllocConsole()) // Think this will only return false if console is already allocated to the program
                {
                    MessageBox.Show("Cannot create console.\nMaybe you already have console attached?",
                        "Debug error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole(); // Attaches a console to our program.

    }
}
