using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

public class HatHandler : AbstractClothHandler
{
    // Hats don't need to calculate anything before beingg drawn.
    public override void PreDrawMaths(ulong id, Kinect.Body body, Kinect.CoordinateMapper mapper)
    { }
    public override void UpdateExtra(Kinect.Body body, Kinect.CoordinateMapper mapper)
    { }

    public override void RemoveClothFor(ulong id)
    {
        GameObject hatObj = GetOrCreateObject("Hat_" + id, CurrentCloth);
        Debug.Log("calling handler.RemoveClothFor(" + id + ")");
        Destroy(hatObj); // Remove it :(
    }

    public override void UpdatePosition(ulong uniqeId, Vector3 jointWorldPosition)
    {
        Cloth hat = CurrentCloth;
        GameObject hatObj = GetOrCreateObject("Hat_" + uniqeId, hat);

        hatObj.GetComponent<Image>().sprite = hat.image;

        // Scales the hats based on how far they are from the kinect
        hatObj.transform.localScale = new Vector3( (hat.scaleX / jointWorldPosition.z), (hat.scaleY / jointWorldPosition.z), 1);

        hatObj.transform.localPosition = new Vector3( jointWorldPosition.x + (hat.xOffset/jointWorldPosition.z), jointWorldPosition.y + (hat.yOffset/jointWorldPosition.z), 10);
    }
}
