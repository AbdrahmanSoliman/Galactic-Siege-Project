#if UNITY_EDITOR
using ShadowBox.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowBox.IconStudio.Editor
{
    public class IconStudioEditor : EditorWindow
    {
        private EditorWindow window;
        private bool inStudio = false;

        private static IconStudioEditor _instance;
        public static IconStudioEditor Instance { get { return _instance; } }
        private IconStudio studio;

        [MenuItem("Tools/ShadowBox/Icon Studio", priority = 2)]
        private static void OpenWindow()
        {
            var window = (IconStudioEditor)GetWindow(typeof(IconStudioEditor));
            window.titleContent = new GUIContent("ShadowBox: Icon Studio", EditorGUIUtility.ObjectContent(CreateInstance<IconStudioEditor>(), typeof(IconStudioEditor)).image);
            window.Show();
        }
        private void OnGUI()
        {
            if (!_instance || _instance == null) _instance = this;
            if (!_instance.window || _instance.window == null) _instance.window = this;
            _instance.minSize = new Vector2(180f, 150f);
            _instance.maxSize = new Vector2(180f, 150f);

            _instance.inStudio = SceneManager.GetActiveScene().name == "IconStudio" ? true : false;
            DrawWindow();
        }
        private void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
                _instance.window = this;
            }
        }
        private void DrawWindow() 
        { 
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.Space();

            if (!_instance.inStudio)
            {

                if (ShadowedEditorTools.Button(new GUIContent("Open Icon Studio"), ShadowedEditorTools.ButtonSkin_Purple, 120f, true, false))
                {
                    if (Application.isPlaying) return;

                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(Application.dataPath + "/Shadowed/Icon Studio/IconStudio.unity");
                }
            }
            else
            {
                if (_instance.studio == null)
                    _instance.studio = FindObjectOfType<IconStudio>();

                if (ShadowedEditorTools.Button(new GUIContent("Create New Icon"), ShadowedEditorTools.ButtonSkin_Purple, 130f, true, false))
                {
                    _instance.studio.SaveScreenshot(false);
                }

                if (ShadowedEditorTools.Button(new GUIContent("Create Transparent Icon"), ShadowedEditorTools.ButtonSkin_Purple, 130f, true, false))
                {
                    _instance.studio.SaveScreenshot(true);
                }

                if (ShadowedEditorTools.Button(new GUIContent("Create Solid Color Icon"), ShadowedEditorTools.ButtonSkin_Purple, 130f, true, false))
                {
                    _instance.studio.SaveSolidColorIcon();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif