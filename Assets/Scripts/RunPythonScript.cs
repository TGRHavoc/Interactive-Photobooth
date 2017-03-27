using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class RunPythonScript : MonoBehaviour {

	public string pythonDirectory = "Python";
    public string pythonFile = "";
    public string pythonArgs = "";

	private string compiledFile;

	// Use this for initialization
	void Start ()
	{
		if (string.IsNullOrEmpty (pythonFile)) 
		{
			UnityEngine.Debug.LogWarning ("I need a python script to run");
			this.enabled = false;
		}

		string directory = Path.Combine (Application.dataPath, pythonDirectory);
		string file = Path.Combine (directory, pythonFile);

		if (!File.Exists (file)) {
			// Copy from resources to file
			string localFile = Path.Combine(pythonDirectory, pythonFile);

			UnityEngine.Debug.Log ("Loading local resource: " + localFile);

			// Copy any python scripts we have in "resources" to the dataPath
			TextAsset pythonContent = Resources.Load(localFile) as TextAsset;

			SavePythonScript (file, pythonContent.text);
		}


		compiledFile = file;

	}

	void SavePythonScript(string _pythonFile, string content)
	{
		if (!string.IsNullOrEmpty (pythonDirectory)) {
			// We need to make sure that the directory exists
			if (!Directory.Exists (Path.Combine (Application.dataPath, pythonDirectory))) 
			{
				Directory.CreateDirectory (Path.Combine (Application.dataPath, pythonDirectory));
			}
		}

		StreamWriter sw = File.CreateText (_pythonFile);
		sw.WriteLine (content);
		sw.Close ();

		UnityEngine.Debug.Log ("Written python file to: " + _pythonFile);
	}

    public void RunPython()
    {
        Thread pythonThread = new Thread(new ThreadStart(StartPython));
        pythonThread.Start();
    }

    private void StartPython()
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python.exe";
		start.Arguments = string.Format("-u \"{0}\" {1}", compiledFile, pythonArgs);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        // Hide the console >:D
        start.WindowStyle = ProcessWindowStyle.Hidden;

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
