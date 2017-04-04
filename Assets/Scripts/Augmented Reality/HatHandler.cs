using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

public class HatHandler : AbstractClothHandler
{
    Vector3 position = new Vector3();
    public override void UpdatePosition(ulong uniqeId, Kinect.Joint joint, Vector3 jointWorldPosition)
    {
        Cloth hat = CurrentCloth;
        GameObject hatObj = GetOrCreateObject("Hat_" + uniqeId, hat, Camera.main.gameObject);
        /*
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        //Debug.Log("hat:" + hatObj.transform.localPosition.x + "," + hatObj.transform.localPosition.y);
        float newX = jointWorldPosition.x - width;
        float newY = jointWorldPosition.y - height;
        */
        Camera cam = Camera.main;
        Vector3 point = cam.ScreenToViewportPoint(jointWorldPosition);
        //hatObj.transform.localPosition = point;
        position = jointWorldPosition;
        //hatObj.transform.localPosition = new Vector3((jointWorldPosition.x * 100) + (hat.xOffset*100), (jointWorldPosition.y * 100) + (hat.yOffset*100), 10);
    }

    public override void DrawObject()
    {
        
        GUI.DrawTexture(new Rect(position.x + CurrentCloth.xOffset, position.y + CurrentCloth.yOffset,
                                100 * CurrentCloth.scaleX, 100 * CurrentCloth.scaleY),
            CurrentCloth.image.texture);
    }

}
