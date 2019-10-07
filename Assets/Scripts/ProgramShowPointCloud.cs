using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using System;

public class ProgramShowPointCloud : MonoBehaviour
{
    public GameObject particleSystemGameObject;
    private ParticleSystem pointCloudParticleSystem;
    private ParticleSystem.Particle[] particles;

    private Device device = null;
    private Capture capture = null;
    private Transformation transformation = null;

    private Color pointColor = Color.white;
    private float size = 0.02f;
    private float scale = 10.0f;
    
    void Start()
    {
        particleSystemGameObject = GameObject.Find("PointCloudParticleSystem");
        

        device = Device.Open(0);
        device.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
        transformation = device.GetCalibration().CreateTransformation();
    }


    void Update()
    {
        capture = device.GetCapture();

        Image colorImage = capture.Color;
        Image depthImage = capture.Depth;

        int colorImageWidth = colorImage.WidthPixels;
        int colorImageHeight = colorImage.HeightPixels;

        List<Vector3> vertices = new List<Vector3>(colorImageWidth * colorImageHeight);
        List<Color32> colors = new List<Color32>(colorImageWidth * colorImageHeight);

        convertRGBDepthToPointXYZRGB(capture, ref vertices, ref colors);

        particles = new ParticleSystem.Particle[colorImageWidth * colorImageHeight];
        for(int i = 0; i < colorImageWidth * colorImageHeight; ++i)
        {
            particles[i].position = vertices[i];
            particles[i].startColor = colors[i];
            particles[i].startSize = size;
            if(vertices[i].z == 0.0f)
            {
                particles[i].startSize = 0;
            }
        }

        pointCloudParticleSystem = particleSystemGameObject.GetComponent<ParticleSystem>();
        pointCloudParticleSystem.Clear();
        pointCloudParticleSystem.SetParticles(particles, particles.Length);
    }
    public struct Point : IEquatable<Point>
    {
        private short x;
        private short y;
        private short z;

        public Point(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public short X { get => x; set => x = value; }
        public short Y { get => y; set => y = value; }
        public short Z { get => z; set => z = value; }

        public bool Equals(Point other)
        {
            return (this.x == other.x) && (this.y == other.y) && (this.z == other.z);
        }
    }
    public void convertRGBDepthToPointXYZRGB(Capture capture, ref List<Vector3> vertices, ref List<Color32> colors)
    {
        Image colorImage = capture.Color.Reference();
        int colorImageWidth =  colorImage.WidthPixels;
        int colorImageHeight = colorImage.HeightPixels;

        Image transformedDepthImage = new Image(ImageFormat.Depth16, colorImageWidth, colorImageHeight, colorImageWidth * sizeof(short));
        Image pointCloudImage = new Image(ImageFormat.Custom, colorImageWidth, colorImageHeight, colorImageWidth * 3 * sizeof(short));

        transformation.DepthImageToColorCamera(capture, transformedDepthImage);
        transformation.DepthImageToPointCloud(transformedDepthImage, pointCloudImage, CalibrationDeviceType.Color);

        for (int h = 0; h < colorImageHeight; ++h)
        {
            for (int w = 0; w < colorImageWidth; ++w)
            {
                short x = pointCloudImage.GetPixel<Point>(h, w).X;
                short y = pointCloudImage.GetPixel<Point>(h, w).Y;
                short z = pointCloudImage.GetPixel<Point>(h, w).Z;

                Vector3 xyz = new Vector3(
                    (float)x / 1000.0f,
                    (float)y / 1000.0f,
                    (float)z / 1000.0f
                    );

                Color32 rgb = new Color32(
                    colorImage.GetPixel<BGRA>(h, w).R,
                    colorImage.GetPixel<BGRA>(h, w).G,
                    colorImage.GetPixel<BGRA>(h, w).B,
                    Byte.MaxValue);

                vertices.Add(xyz);
                colors.Add(rgb);
            }
        }
    }

    private void OnDestroy()
    {
        device.StopCameras();
    }
}
