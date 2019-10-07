using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

public class ProgramShowColorImage : MonoBehaviour
{
    public GameObject colorImagePlane;

    private Device device = null;
    private Capture capture = null;

    protected byte[] colorImageBytes;

    private int colorImageWidth;
    private int colorImageHeight;

    void Start()
    {
        colorImagePlane = GameObject.Find("ColorImagePlane");

        device = Device.Open(0);

        device.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });

        colorImageWidth = device.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        colorImageHeight = device.GetCalibration().ColorCameraCalibration.ResolutionHeight;

        colorImageBytes = new byte[colorImageWidth * colorImageHeight * 4];

        colorImagePlane.transform.localScale = 
            new Vector3((float)colorImageWidth / (float)colorImageHeight, 1.0f, 1.0f);
    }

    void Update()
    {
        capture = device.GetCapture();

        Image colorImage = capture.Color.Reference();
        colorImageBytes = colorImage.Memory.ToArray();

        Texture2D colorImageTex2D =
            new Texture2D(colorImageWidth, colorImageHeight, TextureFormat.BGRA32, false);
        colorImageTex2D.wrapMode = TextureWrapMode.Clamp;
        colorImageTex2D.filterMode = FilterMode.Point;
        colorImageTex2D.LoadRawTextureData(colorImageBytes);
        colorImageTex2D.Apply();

        colorImagePlane.GetComponent<Renderer>().material.mainTexture = colorImageTex2D;

    }

    private void OnDestroy()
    {
        device.StopCameras();
    }
}
