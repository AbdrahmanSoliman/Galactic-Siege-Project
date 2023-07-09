#if UNITY_EDITOR
using ShadowBox.Editors;
using UnityEditor;
using UnityEngine;

namespace ShadowBox.IconStudio.Editor
{
    [CustomEditor(typeof(IconStudio))]
    public class IconStudioInspector : ShadowedEditor
    {
        string[] sizes = { "256", "512", "1024", "2048", "3072", "4096" };
        private int idx = 3;
        public override void OnInspectorGUI()
        {
            IconStudio t = target as IconStudio;

            idx = EditorGUILayout.Popup("Icon Size:", idx, sizes);
            t.SetIconSize(System.Int32.Parse(sizes[idx]));

            base.OnInspectorGUI();

            if (GUILayout.Button("Create New Icon"))
            {
                t.SaveScreenshot(false);
            }

            if (GUILayout.Button("Create Transparent Icon"))
            {
                t.SaveScreenshot(true);
            }

            if (GUILayout.Button("Create Solid Color Icon"))
            {
                t.SaveSolidColorIcon();
            }
        }
    }
}
#endif