using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;


public abstract class AbstractClothHandler : UnityEngine.MonoBehaviour
{
    [Serializable]
    public struct Cloth
    {
        public Sprite image;

        public float scaleX;
        public float scaleY;

        public float xOffset;
        public float yOffset;
    }

    public GameObject parentObj;
    public GameObject kinectImageCube;

    public int _selected = 0;

    public Kinect.JointType jointType;

    public Cloth[] clothes;

    protected Cloth CurrentCloth
    {
        get
        {
            return clothes[_selected];
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            _selected = (_selected + 1) % clothes.Length;
        }

        if (_selected < 0 || _selected >= clothes.Length) // If someone has changed the value in the editor, we gotta make sure it's wrapped
        {
            _selected = _selected % clothes.Length;
        }
    }

    protected GameObject CreateGameObjectForSprite(string objectName, Cloth cloth, GameObject parent = null)
    {
        GameObject obj = new GameObject(objectName);

        Image img = obj.AddComponent<Image>(); // Make sure it has an image
        obj.AddComponent<RectTransform>();

        img.sprite = cloth.image;

        if (parent != null)
            obj.transform.SetParent(parent.transform);


        obj.transform.localPosition = new Vector3(); // Reset local position (put it onto the image cube)
        obj.transform.localScale = new Vector3(cloth.scaleX, cloth.scaleY, 10);

        return obj;
    }

    protected GameObject GetOrCreateObject(string name, Cloth cloth)
    {
        GameObject obj = null;
        bool found = false;

        if (parentObj == null)
        {
            throw new ArgumentNullException("parentObj cannot be null");
        }

        foreach (Transform child in parentObj.transform)
        {
            if (child.name == name)
            {
                //Debug.Log("Child exists");
                obj = child.gameObject;
                found = true;
            }
        }

        // If hatSprite is null, we couldn't find it. Time to create it.
        if (!found)
        {
            obj = CreateGameObjectForSprite(name, cloth, parentObj);
            Debug.Log("Created \"" + name + "\"");
        }

        return obj; // Should never be null..
    }

    public abstract void RemoveClothFor(ulong id);

    public abstract void UpdatePosition(ulong uniqueId, Vector3 jointWorldPosition);

    public abstract void PreDrawMaths(ulong id, Kinect.Body body, Kinect.CoordinateMapper mapper);

    public abstract void UpdateExtra(Kinect.Body body, Kinect.CoordinateMapper mapper);

}
