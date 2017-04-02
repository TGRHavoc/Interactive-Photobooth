using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class TakePhoto : MonoBehaviour
{
    public string basePath = ""; // by default use Application.dataPath
    public string pictureFolder = "Captures";
    public string fileFormat = "capture_{0}.png";

    private void Start()
    {
        if (string.IsNullOrEmpty(basePath))
            basePath = Application.dataPath;
    }

    public void TakePhotoOnClick()
    {
        string baseDir = Path.Combine(basePath, pictureFolder);
        if (!Directory.Exists(baseDir))
            Directory.CreateDirectory(baseDir);

        string formatFile = string.Format(fileFormat, string.Format(fileFormat, DateTime.Now.ToFileTimeUtc() ));

        string filename = Path.Combine(baseDir, formatFile);

        Application.CaptureScreenshot(filename);

        Debug.Log("Saving capture to: " + filename);
    }

}
