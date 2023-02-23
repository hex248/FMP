using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[ExecuteAlways]
public enum Modes {
    Normal = 0,
    Protanopia = 1,
    Protanomaly = 2,
    Deuteranopia = 3,
    Deuteranomaly = 4,
    Tritanopia = 5,
    Tritanomaly = 6,
    Achromatopsia = 7,
    Achromatomaly = 8
}

public class ColorTable
{
    public static float[] Normal = {100.0f, 0.0f, 0.0f, 0.0f, 100.0f, 0.0f, 0.0f, 0.0f, 100.0f};
    public static float[] Protanopia = {56.667f, 43.333f, 0.0f, 55.833f, 44.167f, 0.0f, 0.0f, 24.167f, 75.833f};
    public static float[] Protanomaly = {81.667f, 18.333f, 0.0f, 33.333f, 66.667f, 0.0f, 0.0f, 12.5f, 87.5f};
    public static float[] Deuteranopia = {62.5f, 37.5f, 0.0f, 70.0f, 30.0f, 0.0f, 0.0f, 30.0f, 80.0f};
    public static float[] Deuteranomaly = {80.0f, 20.0f, 0.0f, 0.0f, 25.833f, 74.167f, 0.0f, 14.167f, 85.833f};
    public static float[] Tritanopia = {95.0f, 5.0f, 0.0f, 0.0f, 43.333f, 56.667f, 0.0f, 47.5f, 52.5f};
    public static float[] Tritanomaly = {96.667f, 3.333f, 0.0f, 0.0f, 73.333f, 26.667f, 0.0f, 18.333f, 81.667f};
    public static float[] Achromatopsia = {29.9f, 58.7f, 11.4f, 29.9f, 58.7f, 11.4f, 29.9f, 58.7f, 11.4f};
    public static float[] Achromatomaly = {61.8f, 32.0f, 6.2f, 16.3f, 77.5f, 6.2f, 16.3f, 32.0f, 51.6f};
}

[ExecuteAlways]
public class ColorblindFilters : MonoBehaviour
{
    public Modes mode;
    public VolumeProfile volumeProfile;
    ChannelMixer channelMixer;

    private void Start()
    {
        volumeProfile.TryGet(out channelMixer);
    }

    void Update()
    {
        SetColors();
    }

    void SetColors()
    {
        float[] colorValues;
        switch (mode)
        {
            case Modes.Protanopia:
                colorValues = ColorTable.Protanopia;
                break;
            case Modes.Protanomaly:
                colorValues = ColorTable.Protanomaly;
                break;
            case Modes.Deuteranopia:
                colorValues = ColorTable.Deuteranopia;
                break;
            case Modes.Deuteranomaly:
                colorValues = ColorTable.Deuteranomaly;
                break;
            case Modes.Tritanopia:
                colorValues = ColorTable.Tritanopia;
                break;
            case Modes.Tritanomaly:
                colorValues = ColorTable.Tritanomaly;
                break;
            case Modes.Achromatopsia:
                colorValues = ColorTable.Achromatopsia;
                break;
            case Modes.Achromatomaly:
                colorValues = ColorTable.Achromatomaly;
                break;
            default:
                colorValues = ColorTable.Normal;
                break;
        }
        channelMixer.redOutRedIn.Override(colorValues[0]);
        channelMixer.redOutGreenIn.Override(colorValues[1]);
        channelMixer.redOutBlueIn.Override(colorValues[2]);
        channelMixer.greenOutRedIn.Override(colorValues[3]);
        channelMixer.greenOutGreenIn.Override(colorValues[4]);
        channelMixer.greenOutBlueIn.Override(colorValues[5]);
        channelMixer.blueOutRedIn.Override(colorValues[6]);
        channelMixer.blueOutGreenIn.Override(colorValues[7]);
        channelMixer.blueOutBlueIn.Override(colorValues[8]);
    }

}
