using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class InputIcons : MonoBehaviour
{
    public static InputIcons instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public RebindableControls rebindableControls;

    [Serializable]
    public struct RebindableControls
    {
        public PerPlayer player1;
        public PerPlayer player2;
        public PerPlayer player3;
        public PerPlayer player4;
    }
    [Serializable]
    public struct PerPlayer
    {
        public string interactPath;
        public string cycleLeftPath;
        public string cycleRightPath;
        public string buildPath;
    }

    public Icons ps4;
    public Icons xbox;
    public Icons keyboard;

    [Serializable]
    public struct Icons
    {
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;
        public Sprite emptySprite;

        public Sprite GetSprite(string controlPath)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (controlPath)
            {
                case "buttonSouth": return buttonSouth;
                case "buttonNorth": return buttonNorth;
                case "buttonEast": return buttonEast;
                case "buttonWest": return buttonWest;
                case "start": return startButton;
                case "select": return selectButton;
                case "leftTrigger": return leftTrigger;
                case "rightTrigger": return rightTrigger;
                case "leftShoulder": return leftShoulder;
                case "rightShoulder": return rightShoulder;
                case "dpad": return dpad;
                case "dpad/up": return dpadUp;
                case "dpad/down": return dpadDown;
                case "dpad/left": return dpadLeft;
                case "dpad/right": return dpadRight;
                case "leftStick": return leftStick;
                case "rightStick": return rightStick;
                case "leftStickPress": return leftStickPress;
                case "rightStickPress": return rightStickPress;
                default: return emptySprite;
            }
        }
    }

    public void UpdateBinding(RebindActionUI ui, string controlDisplayName, string deviceName, string controlPath)
    {
        string actionName = ui.actionLabel.text;
        int playerNumber = ui.GetComponentInParent<Player>().playerNumber;

        switch(playerNumber)
        {
            case 1:
                switch (actionName)
                {
                    case "Interact":
                        rebindableControls.player1.interactPath = controlPath;
                        break;
                    case "Cycle Left":
                        rebindableControls.player1.cycleLeftPath = controlPath;
                        break;
                    case "Cycle Right":
                        rebindableControls.player1.cycleRightPath = controlPath;
                        break;
                    case "Build":
                        rebindableControls.player1.buildPath = controlPath;
                        break;
                }
                break;
            case 2:
                switch (actionName)
                {
                    case "Interact":
                        rebindableControls.player2.interactPath = controlPath;
                        break;
                    case "Cycle Left":
                        rebindableControls.player2.cycleLeftPath = controlPath;
                        break;
                    case "Cycle Right":
                        rebindableControls.player2.cycleRightPath = controlPath;
                        break;
                    case "Build":
                        rebindableControls.player2.buildPath = controlPath;
                        break;
                }
                break;
            case 3:
                switch (actionName)
                {
                    case "Interact":
                        rebindableControls.player3.interactPath = controlPath;
                        break;
                    case "Cycle Left":
                        rebindableControls.player3.cycleLeftPath = controlPath;
                        break;
                    case "Cycle Right":
                        rebindableControls.player3.cycleRightPath = controlPath;
                        break;
                    case "Build":
                        rebindableControls.player3.buildPath = controlPath;
                        break;
                }
                break;
            case 4:
                switch (actionName)
                {
                    case "Interact":
                        rebindableControls.player4.interactPath = controlPath;
                        break;
                    case "Cycle Left":
                        rebindableControls.player4.cycleLeftPath = controlPath;
                        break;
                    case "Cycle Right":
                        rebindableControls.player4.cycleRightPath = controlPath;
                        break;
                    case "Build":
                        rebindableControls.player4.buildPath = controlPath;
                        break;
                }
                break;
        }
    }
}
