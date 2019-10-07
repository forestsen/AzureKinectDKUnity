using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using System;

public static class DepthPixelColorizer
{
    public static BGRA ColorizeBlueToRed(short depthPixel, Tuple<int, int> min_max)
    {
        int min = min_max.Item1;
        int max = min_max.Item2;

        byte PixelMax = Byte.MaxValue;

        BGRA result = new BGRA(0, 0, 0, PixelMax);

        if (depthPixel == 0)
        {
            return result;
        }

        int clampedValue = depthPixel;
        clampedValue = Math.Min(clampedValue, max);
        clampedValue = Math.Max(clampedValue, min);
        float hue = (float)(clampedValue - min) / (float)(max - min);

        float range = 2.0f / 3.0f;
        hue *= range;
        hue = range - hue;

        float fRed = 0.0f;
        float fGreen = 0.0f;
        float fBlue = 0.0f;
        StaticImageProperties.ColorConvertHSVtoRGB(hue, 1.0f, 1.0f, ref fRed, ref fGreen, ref fBlue);

        result.R = Convert.ToByte(fRed * (float)PixelMax);
        result.G = Convert.ToByte(fGreen * (float)PixelMax);
        result.B = Convert.ToByte(fBlue * (float)PixelMax);

        return result;
    }

    public static Image ColorizeDepthImage(Image depthImage, Tuple<int, int> expectedValueRange)
    {
        ImageFormat imageFormat = depthImage.Format;
        if(imageFormat != ImageFormat.Depth16 && imageFormat != ImageFormat.IR16)
        {
            throw new Exception("Attempted to colorize a non-depth image!");
        }

        int width = depthImage.WidthPixels;
        int height = depthImage.HeightPixels;

        Image colorizedDepthImage = new Image(ImageFormat.ColorBGRA32, width, height);
        for (int h = 0; h < height; ++h)
        {
            for (int w = 0; w < width; ++w)
            {
                BGRA colorDepthPixel = new BGRA(0, 0, 0, 0);
                colorDepthPixel = ColorizeBlueToRed(depthImage.GetPixel<short>(h, w), expectedValueRange);
                colorizedDepthImage.SetPixel<BGRA>(h, w, colorDepthPixel);
            }
        }
        return colorizedDepthImage;
    }
}
