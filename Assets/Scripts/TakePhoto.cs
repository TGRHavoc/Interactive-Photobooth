using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class TakePhoto : MonoBehaviour
{
    [Tooltip("The directory pictureFolder (and the picture) will be stored. When null, defaults to Application.dataPath")]
    public string basePath = ""; // by default use Application.dataPath
    [Tooltip("The folder to store the pictures into")]
    public string pictureFolder = "Captures";
    [Tooltip("The format for the file name. Note: Use \"{0}\" for the current timestamp.")]
    public string fileFormat = "capture_{0}.png";

    [Tooltip("The amount of time to wait until the photo is taken (in seconds). Default: 5")]
    public int timerLife = 10; // 10 seconds

    public GameObject toHide;

    public GameObject countDownUi;
    public UnityEngine.UI.Text countdownText;

    private bool doCountdown;
    private float timeLeft;

    private void Start()
    {
        // If we haven't provided a base path, default to Application.dataPath
        if (string.IsNullOrEmpty(basePath))
            basePath = Application.dataPath;


    }

    private void Update()
    {
    }

    IEnumerator Countdown()
    {
        while (doCountdown)
        {
            if (timeLeft > 0)
            {
                timeLeft -= 1;
                countdownText.text = "" + timeLeft;
                Debug.Log(timeLeft);
            }

            if (timeLeft <= 0)
            {
                Debug.Log("Countdown completed, taking screenshot");
                // Countdown complete
                countDownUi.SetActive(false); // Hide the coundown UI

                TakeScreenshotAndSave(); // Take that beutiful photo
                yield return new WaitForSeconds(1); // Gota wait a second before showing the main UI again, otherwise it'll photobomb the users :(

                toHide.SetActive(true);
                doCountdown = false; // No longer wanting the screenshot
            }

            yield return new WaitForSeconds(1); // Wait one second
        }
        
    }

    public void TakePhotoOnClick()
    {
        //TODO: Countdown and stuff
        toHide.SetActive(false); // Hide the main UI
        countDownUi.SetActive(true);

        timeLeft = timerLife; // Make sure we have X seconds left on timer
        doCountdown = true;

        StopCoroutine(Countdown()); // Make sure we don't run more than one coroutine :(
        StartCoroutine(Countdown());
        //TakeScreenshotAndSave();        
    }

    /// <summary>
    /// Takes a screenshot of the application and saves it in the directory provided.
    /// </summary>
    private void TakeScreenshotAndSave()
    {
        // Make sure the directory we want to use exists
        string baseDir = Path.Combine(basePath, pictureFolder);
        if (!Directory.Exists(baseDir))
            Directory.CreateDirectory(baseDir);

        // The filename, properly formatted. (e.g. capture_131357615436193177.png)
        string formatFile = string.Format(fileFormat, DateTime.Now.ToFileTimeUtc());

        // The full path for the file (e.g. C:\Data\Captures\capture_131357615436193177.png)
        string filename = Path.Combine(baseDir, formatFile);

        Application.CaptureScreenshot(filename);

        Debug.Log("Saving capture to: " + filename);
        //TODO: Send to python script?
    }

}
