using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Interactive_Photobooth.Python
{
    /// <summary>
    /// Class to represent a running Python Script.
    /// This assumes that python is installed on the system and is in it's PATH environment variable
    /// </summary>
    class PythonScript
    {
        string _pythonScript;
        // The python file to run. Will be pre-appended with the Scripts directory. E.g. "Content/Scripts/file.py"
        string _file;
        // The arguments that should be passed to the python script
        string _args;

        /// <summary>
        /// Creates a new object to represent a python script.
        /// To use, create a new PythonScript and run either Start() or StartOnNewThread() to run the script.
        /// </summary>
        /// <param name="file">The python script to run. Must be in Content/Scripts folder.</param>
        /// <param name="args">The arguments to pass to the script.</param>
        public PythonScript(string file, string args = "")
        {
            _pythonScript = file;
            string rawFileString = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Format(@"Content\Scripts\{0}", file));
            _file = string.Format("\"{0}\"", rawFileString);

            _args = args;
        }

        /// <summary>
        /// Runs the python script on a new thread.
        /// Use this for process-heavy scripts that could slow down the program.
        /// </summary>
        public void StartOnNewThread()
        {
            Thread pythonThread = new Thread(new ThreadStart(Start));
            pythonThread.Start();
        }

        /// <summary>
        /// Runs the python script on the main thread.
        /// Be careful with this function, this will halt all processes until the python script has ended.
        /// </summary>
        public void Start()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python.exe";
            start.Arguments = string.Format("-u {0} {1}", _file, _args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            using (Process process = Process.Start(start))
            {
                process.OutputDataReceived += (s, o) =>
                {
                    if (!string.IsNullOrEmpty(o.Data))
                    {
                        string output = string.Format("[{0}] {1} - {2}", DateTime.Now, _pythonScript, o.Data);
                        Console.WriteLine(output);
                    }
                };
                process.ErrorDataReceived += (s, o) =>
                {
                    if(!string.IsNullOrEmpty(o.Data))
                        Console.WriteLine("Python Error (" + _pythonScript + "): " + o.Data);
                };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }
    }
}
