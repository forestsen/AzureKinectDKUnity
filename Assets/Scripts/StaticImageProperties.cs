using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using System;

public static class StaticImageProperties
{
    public static void ColorConvertHSVtoRGB(float h, float s, float v, ref float out_r, ref float out_g, ref float out_b)
    {
        if (s == 0.0f)
        {
            // gray
            out_r = out_g = out_b = v;
            return;
        }

        h = (h % 1.0f) / (60.0f / 360.0f);
        int i = (int)h;
        float f = h - (float)i;
        float p = v * (1.0f - s);
        float q = v * (1.0f - s * f);
        float t = v * (1.0f - s * (1.0f - f));

        switch (i)
        {
            case 0: out_r = v; out_g = t; out_b = p; break;
            case 1: out_r = q; out_g = v; out_b = p; break;
            case 2: out_r = p; out_g = v; out_b = t; break;
            case 3: out_r = p; out_g = q; out_b = v; break;
            case 4: out_r = t; out_g = p; out_b = v; break;
            case 5: default: out_r = v; out_g = p; out_b = q; break;
        }
    }

    public static Tuple<int, int> GetDepthModeRange(DepthMode depthMode)
    {
        switch (depthMode)
        {
            case DepthMode.NFOV_2x2Binned:
                return Tuple.Create(500, 5800);
            case DepthMode.NFOV_Unbinned:
                return Tuple.Create(500, 4000);
            case DepthMode.WFOV_2x2Binned:
                return Tuple.Create(250, 3000);
            case DepthMode.WFOV_Unbinned:
                return Tuple.Create(250, 2500);

            case DepthMode.PassiveIR:
            default:
                throw new Exception("Invalid depth mode!");
        }
    }

    public static Tuple<int, int> GetDepthDimensions(DepthMode depthMode)
	{
        switch (depthMode)
        {
            case DepthMode.NFOV_2x2Binned:
                return Tuple.Create(320, 288);
            case DepthMode.NFOV_Unbinned:
                return Tuple.Create(640, 576);
            case DepthMode.WFOV_2x2Binned:
                return Tuple.Create(512, 512);
            case DepthMode.WFOV_Unbinned:
                return Tuple.Create(1024, 1024);
            case DepthMode.PassiveIR:
                return Tuple.Create(1024, 1024);
            default:
                throw new Exception("Invalid depth dimensions value!");
        }
	}

    public static Tuple<int, int> GetColorDimensions(ColorResolution resolution)
	{
        switch (resolution)
        {
            case ColorResolution.R720p:
                return Tuple.Create(1280, 720);
            case ColorResolution.R2160p:
                return Tuple.Create(3840, 2160);
            case ColorResolution.R1440p:
                return Tuple.Create(2560, 1440);
            case ColorResolution.R1080p:
                return Tuple.Create(1920, 1080);
            case ColorResolution.R3072p:
                return Tuple.Create(4096, 3072);
            case ColorResolution.R1536p:
                return Tuple.Create(2048, 1536);
            default:
                throw new Exception("Invalid color dimensions value!");
        }
	}

    public static Tuple<int, int> GetIrLevels(DepthMode depthMode)
    {
        switch (depthMode)
        {
            case DepthMode.PassiveIR:
                return Tuple.Create(0, 100);
            case DepthMode.Off:
                throw new Exception("Invalid depth mode!");
            default:
                return Tuple.Create(0, 1000);
        }
    }
}
