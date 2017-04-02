using UnityEngine;
using System.Collections;
using Windows.Kinect;
using UnityEngine.UI;

public class ColorSourceView : MonoBehaviour
{
    public GameObject ColorSourceManager;
    public bool ImageObject = false;

    private ColorSourceManager _ColorManager;
    
    void Start ()
    {
        if (ImageObject)
        {
            gameObject.GetComponent<RawImage>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
        }else
        {
            gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
        }

    }
    
    void Update()
    {
        if (ColorSourceManager == null)
        {
            return;
        }
        
        _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
        if (_ColorManager == null)
        {
            return;
        }
        if (ImageObject)
        {
            gameObject.GetComponent<RawImage>().material.mainTexture = _ColorManager.GetColorTexture();
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.mainTexture = _ColorManager.GetColorTexture();
        }
        
    }
}
