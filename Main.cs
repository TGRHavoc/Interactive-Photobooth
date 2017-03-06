using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Interactive_Photobooth.Python;

namespace Interactive_Photobooth
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            // Example of process heavy python script being run on main thread.
            //new PythonScript("test.py", "10").Start();
            // Example of same process being run on another thread
            //new PythonScript("test.py", "10").StartOnNewThread();

        }
    }
}
