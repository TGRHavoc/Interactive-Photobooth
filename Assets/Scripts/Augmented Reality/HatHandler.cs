using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

public class HatHandler : AbstractClothHandler
{
    
    public override void UpdatePosition(ulong uniqeId, Kinect.Joint joint, Vector3 jointWorldPosition)
    {
        Cloth hat = CurrentCloth;
        GameObject hatObj = GetOrCreateObject("Hat_" + uniqeId, hat, kinectImageCube);

        /*
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        //Debug.Log("hat:" + hatObj.transform.localPosition.x + "," + hatObj.transform.localPosition.y);
        float newX = jointWorldPosition.x - width;
        float newY = jointWorldPosition.y - height;
        */

        hatObj.GetComponent<Image>().sprite = hat.image;
        // Scales the hats based on how far they are from the kinect
        hatObj.transform.localScale = new Vector3( (hat.scaleX / jointWorldPosition.z), (hat.scaleY / jointWorldPosition.z), 1);

        hatObj.transform.localPosition = new Vector3( jointWorldPosition.x + (hat.xOffset/jointWorldPosition.z), jointWorldPosition.y + (hat.yOffset/jointWorldPosition.z), 10);
    }
}
