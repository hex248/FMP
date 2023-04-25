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
    Achromatomaly = 8,
    Secret = 9
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
    public static float[] Secret = { 0.0f, 100.0f, 0.0f, 0.0f, 0.0f, 100.0f, 100.0f, 0.0f, 0.0f };
}

[RequireComponent(typeof(Camera))]
public class ColorblindFilters : MonoBehaviour
{
    public Modes mode;
    public Volume volume;
    ChannelMixer channelMixer;
    Player player;
    Camera camera;
    bool layersSetup = false;

    float secretnessScale = 5f;

    private void Awake()
    {
        volume.profile = new VolumeProfile(); // create new volume profile
        channelMixer = volume.profile.Add<ChannelMixer>(); // add channel mixer effect
        player = FindObjectOfType<Player>();
        camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!layersSetup)
        {
            Debug.Log(player.playerNumber);
            gameObject.layer = LayerMask.NameToLayer($"Colorblind{player.playerNumber}"); // set layer to independent colorblind layer

            UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData(); // get camera data
            cameraData.volumeLayerMask |= (1 << LayerMask.NameToLayer("PP"));
            cameraData.volumeLayerMask |= (1 << LayerMask.NameToLayer($"Colorblind{player.playerNumber}"));
            //camera.UpdateVolumeStack(cameraData); // update volume masks

            layersSetup = true;
        }
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
            case Modes.Secret:
                colorValues = ColorTable.Secret;
                colorValues[0] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[0] = Mathf.Clamp(colorValues[0], 0.0f, 100.0f);
                colorValues[1] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[1] = Mathf.Clamp(colorValues[1], 0.0f, 100.0f);
                colorValues[2] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[2] = Mathf.Clamp(colorValues[2], 0.0f, 100.0f);
                colorValues[3] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[3] = Mathf.Clamp(colorValues[3], 0.0f, 100.0f);
                colorValues[4] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[4] = Mathf.Clamp(colorValues[4], 0.0f, 100.0f);
                colorValues[5] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[5] = Mathf.Clamp(colorValues[5], 0.0f, 100.0f);
                colorValues[6] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[6] = Mathf.Clamp(colorValues[6], 0.0f, 100.0f);
                colorValues[7] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[7] = Mathf.Clamp(colorValues[7], 0.0f, 100.0f);
                colorValues[8] += UnityEngine.Random.Range(-secretnessScale, secretnessScale);
                colorValues[8] = Mathf.Clamp(colorValues[8], 0.0f, 100.0f);
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

    public void ChangeMode(Modes newMode)
    {
        mode = newMode;
    }

}
