#if UNITY_EDITOR
using static ShadowBox.Extensions.ShadowedEditorTools;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShadowBox.Editors
{
    public abstract class ShadowedEditor : Editor
    {
        private static readonly string[] _exclude = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, _exclude);
            serializedObject.ApplyModifiedProperties();
        }

        private static void GenerateTooltip(string text)
        {
            var propRect = GUILayoutUtility.GetLastRect();
            GUI.Label(propRect, new GUIContent("", text));
        }
		
		public static int Toolbar(int value, string[] strings, int width = -1) => (width == -1) ? GUILayout.Toolbar(value, strings) : GUILayout.Toolbar(value, strings, GUILayout.Width(width));

		public static ReorderableList GetListWithFoldout(SerializedObject serializedObject, SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, string title="")
		{
			var list = new ReorderableList(serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);

			list.drawHeaderCallback = (Rect rect) => {
				var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
				var n = (title == "" ? property.displayName : title);
				if (list.count > 0) n += $" [{list.count}]";
				property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, n);
			};
			list.drawElementCallback =
				(Rect rect, int index, bool isActive, bool isFocused) => {
					if (!property.isExpanded)
					{
						GUI.enabled = index == list.count;
						return;
					}

					var element = list.serializedProperty.GetArrayElementAtIndex(index);
					rect.y += 2;
					EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent((element.displayName == "" ? "Empty" : element.displayName)));
				};
			list.elementHeightCallback = (int indexer) => {
				if (!property.isExpanded)
					return 0;
				else
					return list.elementHeight;
			};

			return list;
		}
	}
}
#endif