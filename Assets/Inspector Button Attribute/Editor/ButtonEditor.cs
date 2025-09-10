using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0)
            .Where(m => m.GetParameters().Length == 0);

        foreach (var method in methods)
        {
            var attribute = (ButtonAttribute)method.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
            string label = attribute.Label ?? method.Name;

            if (GUILayout.Button(label))
            {
                method.Invoke(target, null);
            }
        }
    }
}
