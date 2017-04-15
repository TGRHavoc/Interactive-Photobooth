using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class TorsoHandler : AbstractClothHandler
{
    public Cloth[] sleves;

    public Vector2 leftOffset = new Vector2(0,0);
    public Vector2 rightOffset = new Vector2(0,0);

    private float leftAngle = 0;
    private float rightAngle = 0;

    public override void PreDrawMaths(ulong id, Body body)
    {
        //TODO: Calculate stuff (e.g. euclidean distance between shoulders)
        Windows.Kinect.Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
        Windows.Kinect.Joint leftArm = body.Joints[JointType.ElbowLeft];

        double leftRads = Math.Atan2(leftShoulder.Position.Y - leftArm.Position.Y, leftShoulder.Position.X - leftArm.Position.X);
        leftAngle = Convert.ToSingle(leftRads * 180 / Math.PI);

        Windows.Kinect.Joint rightShoulder = body.Joints[JointType.ShoulderRight];
        Windows.Kinect.Joint rightArm = body.Joints[JointType.ElbowRight];

        double rightRads = Math.Atan2(rightShoulder.Position.Y - rightArm.Position.Y, rightShoulder.Position.X - rightArm.Position.X);
        rightAngle = Convert.ToSingle(rightRads * 180 / Math.PI);
    }

    public override void RemoveClothFor(ulong id)
    {
        GameObject prop = GetOrCreateObject("Torso_" + id, CurrentCloth);
        GameObject lSleve = GetOrCreateObject("LeftSleve_" + id, CurrentCloth);
        GameObject rSleve = GetOrCreateObject("RightSleve_" + id, CurrentCloth);

        Destroy(prop);
        Destroy(lSleve);
        Destroy(rSleve);
    }

    public override void UpdatePosition(ulong uniqueId, Vector3 jointWorldPosition)
    {
        GameObject torsoProp = GetOrCreateObject("Torso_" + uniqueId, CurrentCloth);

        torsoProp.GetComponent<Image>().sprite = CurrentCloth.image;

        // Scales the hats based on how far they are from the kinect
        torsoProp.transform.localScale = new Vector3((CurrentCloth.scaleX / jointWorldPosition.z), (CurrentCloth.scaleY / jointWorldPosition.z), 1);

        torsoProp.transform.localPosition = new Vector3(jointWorldPosition.x + (CurrentCloth.xOffset / jointWorldPosition.z),
                                                     jointWorldPosition.y + (CurrentCloth.yOffset / jointWorldPosition.z), 10);
    }

    public override void UpdateExtra(Body body, CoordinateMapper mapper)
    {
        //TODO: Sleves
        Cloth sleve = sleves[_selected];

        CameraSpacePoint lS = body.Joints[JointType.ShoulderLeft].Position;
        CameraSpacePoint rS = body.Joints[JointType.ShoulderRight].Position;

        //Sleves are tracked from shoulders
        ColorSpacePoint leftShoulder = mapper.MapCameraPointToColorSpace(lS);
        ColorSpacePoint rightShoulder = mapper.MapCameraPointToColorSpace(rS);
        
        // to get the sleve being used, do
        // sleves[_selected]
        GameObject leftSleve = GetOrCreateObject("LeftSleve_" + body.TrackingId, sleve);
        GameObject rightSleve = GetOrCreateObject("RightSleve_" + body.TrackingId, sleve);


        // Make sure image is updated
        leftSleve.GetComponent<Image>().sprite = sleve.image;
        rightSleve.GetComponent<Image>().sprite = sleve.image;

        // Scale the sleves based on distance and scale given
        leftSleve.transform.localScale = new Vector3(sleve.scaleX / lS.Z, sleve.scaleY / lS.Z, 1);
        rightSleve.transform.localScale = new Vector3( sleve.scaleX / rS.Z, sleve.scaleY / rS.Z, 1);

        // Set their positions, based on distance from kinect
        leftSleve.transform.localPosition = new Vector3(leftShoulder.X + (leftOffset.x/ lS.Z),
                                                        leftShoulder.Y + (leftOffset.y / lS.Z), 10);

        rightSleve.transform.localPosition = new Vector3(rightShoulder.X + (rightOffset.x / rS.Z),
                                                        rightShoulder.Y + (rightOffset.y / rS.Z), 10);
        
        // Set their pivot point to the top of the sleve (got from sprite editor)
        leftSleve.GetComponent<RectTransform>().pivot = new Vector2(0.74f, 1); // Top center of sleve image
        rightSleve.GetComponent<RectTransform>().pivot = new Vector2(0.74f, 1);

        // Set their angles. Don't ask why this works, I don't know :(
        leftSleve.transform.localRotation = Quaternion.Euler(0, 180, leftAngle + 80);
        rightSleve.transform.localRotation = Quaternion.Euler(180, 0,  rightAngle - 80);
    }
}
