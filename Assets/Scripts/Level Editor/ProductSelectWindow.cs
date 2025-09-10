using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class ProductSelectWindow : EditorWindow
{
    static int targetSlotIndex;
    static BoxData targetBox;

    public static void Show(int slotIndex, BoxData box)
    {
        targetSlotIndex = slotIndex;
        targetBox = box;

        var window = CreateInstance<ProductSelectWindow>();
        window.ShowUtility();
    }

    private void OnGUI()
    {
        var controller = FindObjectOfType<LevelEditorController>();
        var iconsPerRow = 5;

        EditorGUILayout.LabelField("Select a Product", EditorStyles.boldLabel);
        EditorGUILayout.Space();


        var total = controller.productModels.Count;

        for (int i = 0; i < total; i += iconsPerRow)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < iconsPerRow; j++)
            {
                int index = i + j;
                if (index >= total) break;

                Texture2D icon = AssetPreview.GetAssetPreview(controller.productModels[index]);

                if (GUILayout.Button(new GUIContent(icon), GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Undo.RecordObject(controller.CurrentLevelInfo, "Assign Product");
                    targetBox.slots[targetSlotIndex] = index;
                    EditorUtility.SetDirty(controller.CurrentLevelInfo);
                    Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Clear Slot", GUILayout.Height(25)))
        {
            Undo.RecordObject(controller.CurrentLevelInfo, "Clear Slot");
            targetBox.slots[targetSlotIndex] = -1;
            EditorUtility.SetDirty(controller.CurrentLevelInfo);
            Close();
        }
    }

}
#endif