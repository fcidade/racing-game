using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSettings
{
    [SerializeField]
    string horizontalInputName, verticalInputName, driftInputName;

    public PlayerSettings(string horizontalInputName, string verticalInputName, string driftInputName)
    {
        HorizontalInputName = horizontalInputName;
        VerticalInputName = verticalInputName;
        DriftInputName = driftInputName;
    }

    public string HorizontalInputName { get => horizontalInputName; set => horizontalInputName = value; }
    public string VerticalInputName { get => verticalInputName; set => verticalInputName = value; }
    public string DriftInputName { get => driftInputName; set => driftInputName = value; }
}
