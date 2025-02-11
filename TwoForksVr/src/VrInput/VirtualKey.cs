﻿namespace TwoForksVr.VrInput;

public struct VirtualKey
{
    public const string Radio = "RadioButton";
    public const string DialogUp = "DialogUpButton";
    public const string DialogDown = "DialogDownButton";
    public const string LocomotionAction = "LocomotionActionButton";
    public const string Use = "UseButton";
    public const string Jog = "JogButton";
    public const string StoreObject = "StoreObjectButton";
    public const string Cancel = "CancelButton";
    public const string Pause = "PauseButton";
    public const string Confirm = "ConfirmButton";
    public const string NextMenu = "NextMenuButton";
    public const string PreviousMenu = "PreviousMenuButton";
    public const string LookXAxisStick = "LookXAxis_Stick";
    public const string MoveXAxis = "MoveXAxis";
    public const string MoveYAxis = "MoveYAxis";
    public const string MoveForwardKeyboard = "MoveForwardKeyboard";
    public const string MoveBackwardKeyboard = "MoveBackwardKeyboard";
    public const string StrafeLeftKeyboard = "StrafeLeftKeyboard";
    public const string StrafeRightKeyboard = "StrafeRightKeyboard";

    // These are unused in VR, since they can be done with gestures or menus.
    public const string Zoom = "ZoomButton";
    public const string Map = "MapButton";
    public const string MapRegion = "MapRegionButton";
    public const string Flashlight = "FlashlightButton";
    public const string ReadMode = "ReadModeButton";
    public const string Inventory = "InventoryButton";
    public const string Camera = "CameraButton";

    // These I don't know what they're for.
    public const string UIRightStickHorizontal = "UIRightStickHorizontal";
    public const string UIRightStickVertical = "UIRightStickVertical";
    public const string EndBracketKey = "EndBracketKey";
    public const string UILeftStickHorizontal = "UILeftStickHorizontal";
    public const string UILeftStickVertical = "UILeftStickVertical";

    // These are mouse/keyboard only.
    public const string LookXAxisMouse = "LookXAxis_Mouse";
    public const string LookYAxisMouse = "LookYAxis_Mouse";

    // These I just don't need.
    public const string LookYAxisStick = "LookYAxis_Stick";
    public const string ItemsLeftRight = "ItemsLeftRight";
    public const string ItemsUpDown = "ItemsUpDown";
    public const string QuickSave = "QuickSaveButton";
    public const string QuickLoad = "QuickLoadButton";
    public const string DialogSelectionScroll = "DialogSelectionScroll";

    // These are leftover debug stuff, so no need to use them.
    public const string Num0 = "Num0";
    public const string Num1 = "Num1";
    public const string Num2 = "Num2";
    public const string Num3 = "Num3";
    public const string Num4 = "Num4";
    public const string Num5 = "Num5";
    public const string Num6 = "Num6";
    public const string Num7 = "Num7";
    public const string Num8 = "Num8";
    public const string Num9 = "Num9";
    public const string PeriodKey = "PeriodKey";
    public const string PressDemo1 = "PressDemo1";
    public const string PressDemo2 = "PressDemo2";
    public const string PressDemo3 = "PressDemo3";
    public const string PressDemo4 = "PressDemo4";
    public const string TakeDebugScreenshot = "TakeDebugScreenshot";

    // These are special cases, virtual keys created specifically for VR, just for the prompts.
    public const string ToolPicker = "ToolPickerButton";
    public const string ScrollUpDown = "ScrollUpDown";
}