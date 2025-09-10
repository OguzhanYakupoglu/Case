#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LevelEditorSceneDrawer
{

    static LevelEditorSceneDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private static void DrawModel(GameObject model, SceneView sceneView, Matrix4x4 matrix)
    {
        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Material[] materials = renderer.sharedMaterials;

        var subMeshCount = mesh.subMeshCount;
        for (int sub = 0; sub < subMeshCount; sub++)
        {
            var mat = (sub < materials.Length) ? materials[sub] : materials[0];
            Graphics.DrawMesh(mesh, matrix, mat, 0, sceneView.camera, sub);
        }
    }
    private static void OnSceneGUI(SceneView sceneView)
    {
        var controller = Object.FindObjectOfType<LevelEditorController>();
        if (controller == null || controller.CurrentLevelInfo == null) return;

        for (int boxIndex = 0; boxIndex < controller.CurrentLevelInfo.boxDatas.Count; boxIndex++)
        {
            var boxData = controller.CurrentLevelInfo.boxDatas[boxIndex];
            var isHovered = controller.selectedBoxIndex.HasValue && controller.selectedBoxIndex.Value == boxIndex;

            Quaternion rot = Quaternion.Euler(0f, boxData.rotation, 0f);
            Vector3 basePos = boxData.position + Vector3.up * (isHovered ? -.5f : 0f);

            GameObject boxModel = controller.boxModel;
            DrawModel(boxModel, sceneView, Matrix4x4.TRS(basePos, rot, Vector3.one));


            var spacing = 0.5f;
            var startX = -(boxData.beltCount - 1) * spacing / 2f;
            for (int beltIndex = 0; beltIndex < boxData.beltCount; beltIndex++)
            {
                GameObject beltModel = controller.beltModel;
                var positionOffset = rot * new Vector3(startX + beltIndex * spacing, 0, 0);
                DrawModel(beltModel, sceneView, Matrix4x4.TRS(basePos + positionOffset, rot, Vector3.one));
            }


            for (int slotIndex = 0; slotIndex < 6; slotIndex++)
            {
                int? productIndex = boxData.slots[LevelEditorController.slotOrder[slotIndex]];
                if (!productIndex.HasValue || productIndex.Value < 0 || productIndex.Value >= controller.productModels.Count)
                    continue;

                GameObject productModel = controller.productModels[productIndex.Value];
                Vector3 localPos = LevelEditorController.slotLocalPositions[slotIndex];
                Vector3 worldPos = basePos + rot * localPos;
                Matrix4x4 matrix = Matrix4x4.TRS(worldPos, rot, Vector3.one);

                DrawModel(productModel, sceneView, matrix);
            }
        }
    }
}
#endif