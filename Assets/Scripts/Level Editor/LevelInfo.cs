using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Create New Level", order = 1)]
public class LevelInfo : ScriptableObject
{
    public List<BoxData> boxDatas = new();
}
[System.Serializable]
public class BoxData
{
    [HideInInspector] public int[] slots = new int[6] { -1, -1, -1, -1, -1, -1 };
    public Vector3 position = Vector2.zero;
    public float rotation = 0f;
    public int beltCount = 0;
}