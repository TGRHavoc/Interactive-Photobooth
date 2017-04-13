using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class RunPythonScript : MonoBehaviour {

    [Tooltip("The directory (under \"Resources\") where the python scripts are stored. Default: Python")]
	public string pythonDirectory = "Python";

    // Note: This needs to have a txt file and the .txt file extension (Unity only likes text files :()
    //       You don't need to add the final extension (.txt) though.. Unity does that automatically or some shit.
    //     E.g. Just put "test.py" for the "test.py.txt" file
    [Tooltip("The name of the python file under (\"Resources /<pythonDirectory>\").\n" +
        "Note: Check source code for usage etc.")]
    public string pythonFile = "";

    // The arguments to be passed to the python script. Must be in a string format each argument seperated by a space.
    // E.g. for the test file, this can be any number (i.e. "100").
    [Tooltip("The arguments to be passed to the python script. Each argument seperated by a space.")]
    public string pythonArgs = "";

    // When we copy the txt file into a py file, this will be it's location.
	private string compiledFile;

	/// <summary>
    /// Initialization method. Should make sure that the python script exists so we can pass it to the python process.
    /// If the script doesn't exist in filesystem (Application.dataPath) it should copy it over from the local txt file.
    /// </summary>
	void Start ()
	{
		if (string.IsNullOrEmpty (pythonFile)) 
		{
            // You forgot to supply a python file :(
			UnityEngine.Debug.LogWarning ("I need a python script to run");
            this.enabled = false; // Might as well stop this from running
		}
        // Where we store the actual python script. (Not text file)
		string directory = Path.Combine (Application.dataPath, pythonDirectory);
        
        // Where the final file will be located
		string file = Path.Combine (directory, pythonFile);

        // If the file doesn't exist on the file system, copy it from resources.
		if (!File.Exists (file)) {
			
			string localFile = Path.Combine(pythonDirectory, pythonFile);

			UnityEngine.Debug.Log ("Loading local resource: " + localFile);

			// Copy any python scripts we have in "resources" to the dataPath
			TextAsset pythonContent = Resources.Load(localFile) as TextAsset; // For some reason this wont work with file exensions other than ".txt" :(
            
			SavePythonScript (file, pythonContent.text);
		}
        // Make sure we know what file we'll be working with :)
		compiledFile = file;
	}

    /// <summary>
    /// Saves a given string into a file at the given location.
    /// </summary>
    /// <param name="_pythonFile">The name of the python file to write. Will be located in "Application.dataPath/pythonDirectory"</param>
    /// <param name="content">The contents to write. Can be obtained from "(Resources.Load(file) as TextAsset).text"</param>
	void SavePythonScript(string _pythonFile, string content)
	{
		if (!string.IsNullOrEmpty (pythonDirectory)) {
			// We need to make sure that the directory exists
			if (!Directory.Exists (Path.Combine (Application.dataPath, pythonDirectory))) 
			{
				Directory.CreateDirectory (Path.Combine (Application.dataPath, pythonDirectory));
			}
		}
        //Write the contents to the file.
		StreamWriter sw = File.CreateText (_pythonFile);
		sw.WriteLine (content);
		sw.Close ();

		UnityEngine.Debug.Log ("Written python file to: " + _pythonFile);
	}

    /// <summary>
    /// Starts the python script on a new thread.
    /// </summary>
    public void RunPython()
    {
        Thread pythonThread = new Thread(new ThreadStart(StartPython));
        pythonThread.Start();
    }

    private void StartPython()
    {
        UnityEngine.Debug.Log(string.Format("Starting the python script \"{0}\"\nArguments: {1}", pythonFile, pythonArgs));

        // Start a new python process with the file and args defined above.

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python.exe"; // Assumes that they have python installed. May need to change?
		start.Arguments = string.Format("-u \"{0}\" {1}", compiledFile, pythonArgs);
        start.UseShellExecute = false;
        // Allows us to display the output of the script where we want. In this case, Debug.Log()
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;

        // Hide the console >:D
        start.WindowStyle = ProcessWindowStyle.Hidden;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            process.OutputDataReceived += (s, o) =>
            {
                if (!string.IsNullOrEmpty(o.Data))
                {
                    string output = string.Format("[{0}] {1} - {2}", DateTime.Now, pythonFile, o.Data);
                    UnityEngine.Debug.Log(output);
                }
            };
            process.ErrorDataReceived += (s, o) =>
            {
                if (!string.IsNullOrEmpty(o.Data))
                    UnityEngine.Debug.Log("Python Error (" + pythonFile + "): " + o.Data);
            };

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
    }

}
