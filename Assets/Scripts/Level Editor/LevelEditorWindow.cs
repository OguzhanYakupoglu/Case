using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class LevelEditorWindow : EditorWindow
{
    [SerializeField] private LevelInfo levelInfoAsset;
    [SerializeField] private int selectedBoxIndex = -1;

    private BoxData CurrentBoxData
    {
        get
        {
            if (levelInfoAsset == null) return null;
            if (selectedBoxIndex >= 0 && selectedBoxIndex < levelInfoAsset.boxDatas.Count)
                return levelInfoAsset.boxDatas[selectedBoxIndex];
            return null;
        }
    }

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
        var controller = FindObjectOfType<LevelEditorController>();
        if (controller != null)
        {
            controller.CurrentLevelInfo = null;
        }
    }
    private void OnGUI()
    {
        var controller = FindObjectOfType<LevelEditorController>();
        if (controller == null)
        {
            EditorGUILayout.LabelField("Open level generator scene");
            levelInfoAsset = null;
            return;
        }

        EditorGUI.BeginChangeCheck();
        levelInfoAsset = (LevelInfo)EditorGUILayout.ObjectField("Level Info", levelInfoAsset, typeof(LevelInfo), false);
        if (EditorGUI.EndChangeCheck())
        {
            controller.CurrentLevelInfo = levelInfoAsset;
            selectedBoxIndex = -1;
            controller.selectedBoxIndex = null;
            Repaint();
            return;
        }

        if (levelInfoAsset == null)
        {
            EditorGUILayout.HelpBox("Please assign a LevelInfo asset.", MessageType.Info);
            return;
        }

        if (CurrentBoxData == null)
        {
            int? hoveredIndex = null;
            for (int i = 0; i < levelInfoAsset.boxDatas.Count; i++)
            {
                if (GUILayout.Button("Select Box " + (i + 1)))
                {
                    selectedBoxIndex = i;
                    controller.selectedBoxIndex = i;
                    Repaint();
                    return;
                }

                Rect rect = GUILayoutUtility.GetLastRect();
                if (rect.Contains(Event.current.mousePosition))
                {
                    hoveredIndex = i;
                }
            }

            controller.selectedBoxIndex = hoveredIndex;
            SceneView.RepaintAll();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Add Box"))
            {
                Undo.RecordObject(levelInfoAsset, "Add Box");
                levelInfoAsset.boxDatas.Add(new BoxData());
                EditorUtility.SetDirty(levelInfoAsset);
            }

            return;
        }

        if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
        {
            selectedBoxIndex = -1;
            controller.selectedBoxIndex = null;
            Repaint();
            return;
        }

        BoxData box = CurrentBoxData;
        int[] slotOrder = { 0, 2, 4, 1, 3, 5 };

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("box", GUILayout.Width(120));
        for (int row = 0; row < 2; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < 3; col++)
            {
                int index = slotOrder[row * 3 + col];
                int? assignedProductIndex = box.slots[index];

                GUIContent content;
                if (assignedProductIndex.HasValue &&
                    assignedProductIndex.Value >= 0 &&
                    assignedProductIndex.Value < controller.productModels.Count)
                {
                    Texture2D icon = AssetPreview.GetAssetPreview(controller.productModels[assignedProductIndex.Value]);
                    content = new GUIContent(assignedProductIndex.Value.ToString(), icon);
                }
                else
                {
                    content = new GUIContent("Select");
                }

                if (GUILayout.Button(content, GUILayout.Width(80), GUILayout.Height(60)))
                {
                    ProductSelectWindow.Show(index, box);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUI.BeginChangeCheck();

        var xPos = EditorGUILayout.FloatField("xPosition", box.position.x);
        var zPos = EditorGUILayout.FloatField("zPosition", box.position.z);
        var layerHeight = EditorGUILayout.IntField("layerHeight", (int)(box.position.y / 2.25f));
        var rotation = EditorGUILayout.FloatField("Rotation", box.rotation);
        var beltCount = EditorGUILayout.IntField("Belt Count", box.beltCount);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(levelInfoAsset, "Modify Box");
            box.position = new Vector3(xPos, layerHeight * 2.25f, zPos);
            box.rotation = rotation;
            box.beltCount = beltCount;
            EditorUtility.SetDirty(levelInfoAsset);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Remove Box"))
        {
            Undo.RecordObject(levelInfoAsset, "Remove Box");
            levelInfoAsset.boxDatas.Remove(box);
            selectedBoxIndex = -1;
            controller.selectedBoxIndex = null;
            EditorUtility.SetDirty(levelInfoAsset);
            Repaint();
        }
    }

    void OnInspectorUpdate()
    {
        SceneView.RepaintAll();
    }
}
#endif