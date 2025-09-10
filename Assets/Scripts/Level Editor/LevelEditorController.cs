using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorController : SoftSingleton<LevelEditorController>
{
    private LevelInfo currentLevelInfo;
    public LevelInfo CurrentLevelInfo
    {
        get => currentLevelInfo;
        set
        {
            currentLevelInfo = value;
        }
    }
    public int? selectedBoxIndex;
    public List<GameObject> productModels;
    public GameObject boxModel;
    public GameObject beltModel;
    public static readonly int[] slotOrder = { 0, 2, 4, 1, 3, 5 };

    public static readonly Vector3[] slotLocalPositions = new Vector3[]
    {
        new Vector3(-0.9f, 0f,  0.5f),
        new Vector3( 0.0f, 0,  0.5f),
        new Vector3( 0.9f, 0,  0.5f),
        new Vector3(-0.9f, 0, -0.5f),
        new Vector3( 0.0f, 0, -0.5f),
        new Vector3( 0.9f, 0, -0.5f),
    };
}
