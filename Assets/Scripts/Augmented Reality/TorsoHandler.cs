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

    struct UserData
    {
        public float width;
        public float height;
    }
    private Dictionary<ulong, UserData> usersData = new Dictionary<ulong, UserData>();

    private float leftAngle = 0;
    private float rightAngle = 0;

    public override void PreDrawMaths(ulong id, Body body, CoordinateMapper mapper)
    {
        //TODO: Calculate stuff (e.g. euclidean distance between shoulders)
        Windows.Kinect.Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
        Windows.Kinect.Joint leftArm = body.Joints[JointType.ElbowLeft];

        ColorSpacePoint leftPointShoulder = mapper.MapCameraPointToColorSpace(leftShoulder.Position);
        ColorSpacePoint leftPointArm = mapper.MapCameraPointToColorSpace(leftArm.Position);

        double leftRads = Math.Atan2(leftPointShoulder.Y - leftPointArm.Y, leftPointShoulder.X - leftPointArm.X);
        leftAngle = Convert.ToSingle(leftRads * 180 / Math.PI);

        Windows.Kinect.Joint rightShoulder = body.Joints[JointType.ShoulderRight];
        Windows.Kinect.Joint rightArm = body.Joints[JointType.ElbowRight];

        ColorSpacePoint rightPointShoulder = mapper.MapCameraPointToColorSpace(rightShoulder.Position);
        ColorSpacePoint rightPointArm = mapper.MapCameraPointToColorSpace(rightArm.Position);

        double rightRads = Math.Atan2(rightPointShoulder.Y - rightPointArm.Y, rightPointShoulder.X - rightPointArm.X);
        rightAngle = Convert.ToSingle(rightRads * 180 / Math.PI);

        ColorSpacePoint hipCenter = mapper.MapCameraPointToColorSpace(body.Joints[JointType.SpineBase].Position);
        ColorSpacePoint shoulderCenter = mapper.MapCameraPointToColorSpace(body.Joints[JointType.SpineShoulder].Position);

        /*
         Hbody = |xshouldercenter − xhipcenter|
         Wbody = |xleftshoulder − xrightshoulder|
        */

        double userWidth = Math.Abs(leftPointShoulder.X - rightPointShoulder.X); // Calculates user's width in pixels
        double userHeight = Math.Abs(shoulderCenter.Y - hipCenter.Y); // Calulates height in pixels

        //double userWidth = Math.Sqrt(Math.Pow(leftPointShoulder.X, 2) - Math.Pow(rightPointShoulder.X, 2));
        //double userHeight = Math.Sqrt(Math.Pow(shoulderCenter.Y, 2) - Math.Pow(hipCenter.Y, 2));

        UserData data = new UserData() { height = Convert.ToSingle(userHeight), width = Convert.ToSingle(userWidth) };
        if (usersData.ContainsKey(id))
        {
            usersData[id] = data;
        }else
        {
            usersData.Add(id, data);
        }
    }

    public override void RemoveClothFor(ulong id)
    {
        GameObject prop = GetOrCreateObject("Torso_" + id, CurrentCloth);
        GameObject lSleve = GetOrCreateObject("LeftSleve_" + id, CurrentCloth);
        GameObject rSleve = GetOrCreateObject("RightSleve_" + id, CurrentCloth);

        if (usersData.ContainsKey(id))
        {
            usersData.Remove(id);
        }

        Destroy(prop);
        Destroy(lSleve);
        Destroy(rSleve);
    }

    public override void UpdatePosition(ulong uniqueId, Vector3 jointWorldPosition)
    {
        GameObject torsoProp = GetOrCreateObject("Torso_" + uniqueId, CurrentCloth);
        UserData data = usersData[uniqueId];

        torsoProp.GetComponent<Image>().sprite = CurrentCloth.image;

        //TODO: scale with data.height and data.widt
        // Scales the hats based on how far they are from the kinect
        torsoProp.transform.localScale = new Vector3( CurrentCloth.scaleX,
                                                      CurrentCloth.scaleY, 1);
        
        torsoProp.GetComponent<RectTransform>().sizeDelta = new Vector2(data.width, data.height);

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
        leftSleve.transform.localRotation = Quaternion.Euler(0, 0, leftAngle - 90);
        rightSleve.transform.localRotation = Quaternion.Euler(180, 180,  rightAngle + 90);
    }
}
