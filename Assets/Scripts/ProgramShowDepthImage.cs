using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using System;

public class ProgramShowDepthImage : MonoBehaviour
{
    public GameObject depthImagePlane;

    private Device device = null;
    private Capture capture = null;

    protected byte[] depthImageBytes;

    private int depthImageWidth;
    private int depthImageHeight;

    void Start()
    {
        depthImagePlane = GameObject.Find("DepthImagePlane");

        device = Device.Open(0);
        device.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });

        depthImageWidth = device.GetCalibration().DepthCameraCalibration.ResolutionWidth;
        depthImageHeight = device.GetCalibration().DepthCameraCalibration.ResolutionHeight;

        depthImageBytes = new byte[depthImageWidth * depthImageHeight * 4];

        depthImagePlane.transform.localScale = 
            new Vector3((float)depthImageWidth / (float)depthImageHeight, 1.0f, 1.0f);
    }

    void Update()
    {
        capture = device.GetCapture();

        Image depthImage = capture.Depth.Reference();
        Image colorizedDepthImage = DepthPixelColorizer.ColorizeDepthImage(depthImage, StaticImageProperties.GetDepthModeRange(device.CurrentDepthMode));
        depthImageBytes = colorizedDepthImage.Memory.ToArray();

        Texture2D depthImageTex2D = 
            new Texture2D(depthImageWidth, depthImageHeight, TextureFormat.BGRA32, false);
        depthImageTex2D.wrapMode = TextureWrapMode.Clamp;
        depthImageTex2D.filterMode = FilterMode.Point;

        depthImageTex2D.LoadRawTextureData(depthImageBytes);
        depthImageTex2D.Apply();
        depthImagePlane.GetComponent<Renderer>().material.mainTexture = depthImageTex2D;
    }

    
    private void OnDestroy()
    {
        device.StopCameras();
    }

    
}
