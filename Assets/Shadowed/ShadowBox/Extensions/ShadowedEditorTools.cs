#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;

namespace ShadowBox.Extensions
{
    public class ShadowedEditorTools
    {

        public static string ShadowedPurpleHex = "#6523A3";

        public static GUIStyle BoldFoldout
        {
            get
            {
                var boldFoldout = new GUIStyle(EditorStyles.foldout);
                boldFoldout.fontStyle = FontStyle.Bold;
                return boldFoldout;
            }
        }

        public static GUIStyle Skin_Blue => Style(new Color(0, 0.5f, 1f, 0.3f));
        public static GUIStyle Skin_Red => Style(new Color(1, 0.3f, 0f, 0.3f));
        public static GUIStyle Skin_Green => Style(new Color(0f, 1f, 0.5f, 0.3f));
        public static GUIStyle Skin_Gray => Style(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        public static GUIStyle Skin_Orange => Style(new Color(1f, 0.5f, 0.0f, 0.3f));
        public static GUIStyle Skin_Purple => Style(new Color(0.57f, 0f, 1f, 0.65f));
        public static GUIStyle Bordered => Style(new Color(0, 0.5f, 1f, 0.0f));
        public static GUIStyle Flat => Style(new Color(0.35f, 0.35f, 0.35f, 0.1f));
        public static GUIStyle Skin_Pro => Style(new Color(0.1f, 0.1f, 0.1f, 0.2f));
        public static GUIStyle ButtonSkin_Blue => ButtonStyle(new Color(0, 0.5f, 1f, 0.3f));
        public static GUIStyle ButtonSkin_Red => ButtonStyle(new Color(1, 0f, 0f, 0.5f));
        public static GUIStyle ButtonSkin_Green => ButtonStyle(new Color(0f, 1f, 0.5f, 0.3f));
        public static GUIStyle ButtonSkin_Gray => ButtonStyle(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        public static GUIStyle ButtonSkin_Orange => ButtonStyle(new Color(1f, 0.3f, 0.0f, 0.5f));
        public static GUIStyle ButtonSkin_Purple => ButtonStyle(new Color(0.57f, 0f, 1f, 0.35f));
        public static GUIStyle ButtonSkin_LightPurple => ButtonStyle(new Color(0.37f, 0f, 8f, 0.35f));
        public static GUIStyle ButtonBordered => ButtonStyle(new Color(0, 0.5f, 1f, 0.0f));
        public static GUIStyle ButtonFlat => ButtonStyle(new Color(0.35f, 0.35f, 0.35f, 0.1f));
        public static GUIStyle ButtonSkin_Pro => ButtonStyle(new Color(0.1f, 0.1f, 0.1f, 1.2f));
        public static GUIStyle ToolbarSkin_Blue => ToolbarStyle(new Color(0, 0.5f, 1f, 0.3f));
        public static GUIStyle ToolbarSkin_Red => ToolbarStyle(new Color(1, 0.3f, 0f, 0.5f));
        public static GUIStyle ToolbarSkin_Green => ToolbarStyle(new Color(0f, 1f, 0.5f, 0.3f));
        public static GUIStyle ToolbarSkin_Gray => ToolbarStyle(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        public static GUIStyle ToolbarSkin_Orange => ToolbarStyle(new Color(1f, 0.5f, 0f, 0.3f));
        public static GUIStyle ToolbarSkin_Purple => ToolbarStyle(new Color(0.57f, 0f, 1f, 0.35f));
        public static GUIStyle ToolbarBordered => ToolbarStyle(new Color(0, 0.5f, 1f, 0.0f));
        public static GUIStyle ToolbarFlat => ToolbarStyle(new Color(0.35f, 0.35f, 0.35f, 0.1f));
        public static GUIStyle ToolbarSkin_Pro => ToolbarStyle(new Color(0.1f, 0.1f, 0.1f, 1.2f));

        public static GUIStyle Style(string color, GUIStyle stylePref = null)
        {
            Color x;
            if (ColorUtility.TryParseHtmlString(color, out x))
            {
                return Style(x, stylePref);
            }
            else
                return Skin_Pro;
        }

        public static GUIStyle Style(Color color, GUIStyle stylePref = null)
        {
            if (stylePref == null) stylePref = GUI.skin.box;
            GUIStyle currentStyle = new GUIStyle(stylePref)
            {
                border = new RectOffset(-1, -1, -1, -1)
            };

            Color[] pix = new Color[1];
            pix[0] = color;
            Texture2D bg = new Texture2D(1, 1);
            bg.SetPixels(pix);
            bg.Apply();

            currentStyle.normal.background = bg;
            currentStyle.normal.textColor = Color.white;
            currentStyle.alignment = TextAnchor.MiddleCenter;
            return currentStyle;
        }

        public static GUIStyle ButtonStyle(Color color)
        {
            return Style(color, GUI.skin.customStyles[0]);
        }

        public static GUIStyle ButtonStyle(string color)
        {
            Color x;
            if (ColorUtility.TryParseHtmlString(color, out x))
            {
                return Style(x, GUI.skin.customStyles[0]);
            }
            else
                return ButtonSkin_Pro;
        }

        public static GUIStyle ToolbarStyle(Color color)
        { 
            GUIStyle currentStyle = new GUIStyle(GUI.skin.customStyles[0])
            {
                border = new RectOffset(-1, -1, -1, -1)
            };

            Color[] pix = new Color[1];
            Color[] selectedPix = new Color[1];
            pix[0] = color;
            selectedPix[0] = color.WithAlphaSetTo(0.2F); 
            Texture2D bg = new Texture2D(1, 1);
            Texture2D selBG = new Texture2D(1, 1);

            bg.SetPixels(pix);
            bg.Apply();
            selBG.SetPixels(selectedPix);
            selBG.Apply();

            currentStyle.normal.background = bg;
            currentStyle.onNormal.background = selBG;
            currentStyle.hover.background = selBG;
            currentStyle.onHover.background = selBG;
            currentStyle.active.background = bg;
            currentStyle.onActive.background = bg;
            currentStyle.focused.background = selBG;
            currentStyle.onFocused.background = selBG;
            currentStyle.normal.scaledBackgrounds = new Texture2D[] { bg };
            currentStyle.onNormal.scaledBackgrounds = new Texture2D[] { selBG };
            currentStyle.hover.scaledBackgrounds = new Texture2D[] { selBG };
            currentStyle.onHover.scaledBackgrounds = new Texture2D[] { selBG };
            currentStyle.active.scaledBackgrounds = new Texture2D[] { bg };
            currentStyle.onActive.scaledBackgrounds = new Texture2D[] { bg };
            currentStyle.focused.scaledBackgrounds = new Texture2D[] { selBG };
            currentStyle.onFocused.scaledBackgrounds = new Texture2D[] { selBG };

            currentStyle.normal.textColor = Color.white;
            currentStyle.hover.textColor = Color.white;
            currentStyle.active.textColor = Color.white;
            currentStyle.focused.textColor = Color.white;
            currentStyle.alignment = TextAnchor.MiddleCenter;
            return currentStyle;
        }

        public static GUIStyle ToolbarStyle(string color)
        {
            Color x;
            if (ColorUtility.TryParseHtmlString(color, out x))
            {
                return ToolbarStyle(x);
            }
            else
                return ToolbarSkin_Pro;
        }

        public static string GetPropertyTypeName(SerializedProperty property)
        {
            var type = property.type;
            var match = Regex.Match(type, @"PPtr<\$(.*?)>");
            if (match.Success)
                type = match.Groups[1].Value;
            return type;
        }

        public static System.Type[] GetTypesByName(string className)
        {
            List<System.Type> returnVal = new List<System.Type>();

            foreach (Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type[] assemblyTypes = a.GetTypes();
                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    if (assemblyTypes[j].Name == className)
                    {
                        returnVal.Add(assemblyTypes[j]);
                    }
                }
            }

            return returnVal.ToArray();
        }

        public static System.Type GetTypeByName(string className)
        {
            return System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(t => t.Name == className);
        }

        /// <summary> Check if an Object is an Asset Folder </summary>
        public static bool IsFolder(Object obj)
        {
            string path = "";

            if (obj == null)
            {
                return false;
            }

            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public static void AddParametersOnAnimator(UnityEditor.Animations.AnimatorController fromController, UnityEditor.Animations.AnimatorController toController)
        {
            AnimatorControllerParameter[] fromParameters = fromController.parameters;
            AnimatorControllerParameter[] toParameters = toController.parameters;

            foreach (var parameter in fromParameters)
            {
                if (!AnimatorHasParameter(toParameters, parameter.name))
                {
                    toController.AddParameter(parameter);
                }
            }
        }

        public static bool AnimatorHasParameter(AnimatorControllerParameter[] parameters, string name)
        {
            foreach (AnimatorControllerParameter item in parameters)
                if (item.name == name) return true;

            return false;
        }

        public static void DrawHeader(string title)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        public static void DrawBoldIndentedHeader(string title, string tip = "")
        {
            EditorGUI.indentLevel++;
            var x = new GUIContent(title, tip);
            EditorGUILayout.LabelField(x, EditorStyles.boldLabel);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        public static void DrawBoldHeader(string title, string tip = "")
        {
            var x = new GUIContent(title, tip);
            EditorGUILayout.LabelField(x, EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        public static void DrawBoldCenteredHeader(string title, string tip = "")
        {
            var x = new GUIContent(title, tip);
            GUILayout.Label(x, EditorStyles.boldLabel);
            var r = GUILayoutUtility.GetLastRect();
            //var r = GUILayoutUtility.GetRect(x, EditorStyles.boldLabel);
            r.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, r.center.y);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Colors and centers a label in the Editor Window.
        /// </summary>
        /// <param name="label">The text for the button</param>
        /// <param name="style">The desired GUIStyle</param>
        /// <returns>Rect - styled and positioned</returns>
        public static Rect DrawExpandedLabel(string label, GUIStyle style, bool whiteText = false)
        {
            var tmpColor = GUI.contentColor;
            if (!whiteText)
                GUI.contentColor = Color.black;
            GUIContent labelText = new GUIContent(label);
            var rt = GUILayoutUtility.GetRect(labelText, style, GUILayout.ExpandWidth(false));
            rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);

            if (!whiteText)
                GUI.contentColor = tmpColor;
            return rt;
        }

        public static void DrawExpandedLabel(string label, GUIStyle style, bool whiteText = false, float width = 300f)
        {
            var tmpColor = GUI.contentColor;
            if (!whiteText)
                GUI.contentColor = Color.black;
            GUIContent labelText = new GUIContent(label);
            var rt = GUILayoutUtility.GetRect(labelText, style, GUILayout.Width(width), GUILayout.ExpandWidth(false));

            if (!whiteText)
                GUI.contentColor = tmpColor;
            EditorGUI.LabelField(rt, labelText, style);
        }

        public static Rect DrawExpandedLabel(string label, GUIStyle style, float width = 300f, bool centered = false, bool whiteText = false)
        {
            var tmpColor = GUI.contentColor;
            if (!whiteText)
                GUI.contentColor = Color.black;
            GUIContent labelText = new GUIContent(label);
            var rt = GUILayoutUtility.GetRect(labelText, style, GUILayout.Width(width), GUILayout.ExpandWidth(false));
    
            if(centered)
                rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);

            if (!whiteText)
                GUI.contentColor = tmpColor;

            return rt; //EditorGUI.LabelField(rt, labelText, style);
        }

        /// <summary>
        /// Colors and centers a button in the Editor Window.
        /// </summary>
        /// <param name="label">The text for the button</param>
        /// <param name="style">The desired GUIStyle</param>
        /// <returns>Rect - styled and positioned</returns>
        public static Rect DrawButton(string label, GUIStyle style, bool centered = true, bool whiteText = false)
        {
            Color tmpColor = GUI.contentColor;
            if (!whiteText)
                GUI.contentColor = Color.black;
            
            GUIContent buttonText = new GUIContent(label);
            var rt = GUILayoutUtility.GetRect(buttonText, style, GUILayout.ExpandWidth(false));
            if (centered)
                rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);

            if (!whiteText)
                GUI.contentColor = tmpColor;

            return rt;
        }

        public static Rect DrawButton(GUIContent content, GUIStyle style, bool centered = true, bool drawSpace = false, bool whiteText = false)
        {
            return DrawButton(content, style, 100f, centered, drawSpace, whiteText);
        }
        public static Rect DrawButton(GUIContent content, GUIStyle style, float width = 100f, bool centered = true, bool drawSpace = false, bool whiteText = false, bool inPlace = false)
        {
            var tmpColor = GUI.contentColor;

            if (!whiteText)
                GUI.contentColor = Color.black;

            GUIContent buttonText = content;
            var rt = GUILayoutUtility.GetRect(buttonText, style, GUILayout.ExpandWidth(false), GUILayout.Width(width), GUILayout.MaxWidth(width), GUILayout.Height(20));
    
            //rt.size = new Vector2(width,20);

            if (centered)
                rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
            else if (inPlace)
            {

                rt.center = new Vector2(rt.xMin + rt.width / 2, rt.center.y);
                rt.xMax = rt.center.x + rt.width / 2;
            }


            if (drawSpace)
                EditorGUILayout.Space();

            if (!whiteText)
                GUI.contentColor = tmpColor;

            return rt;
        }

        public static bool Button(Rect button)
        {
            bool state = GUI.Button(button, "");

            var e = Event.current;
            if (e.type == EventType.MouseDown && button.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool Button(string label, GUIStyle style, bool centered = true)
        {
            Rect btn = DrawButton(label, GUI.skin.button, centered);//, label, style);
            bool state = GUI.Button(btn, label, style);

            var e = Event.current;
            if (e.type == EventType.MouseDown && btn.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool Button(string label, GUIStyle style, float width, bool centered = true, bool drawSpace = false)
        {
            GUIContent content = new GUIContent(label);
            Rect btn = DrawButton(content, GUI.skin.button, width, centered, drawSpace);//, label, style);
            bool state = GUI.Button(btn, content, style);

            var e = Event.current;
            if (e.type == EventType.MouseDown && btn.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool Button(GUIContent content, GUIStyle style, float width = 100f, bool centered = true, bool drawSpace = false)
        {
            Rect btn = DrawButton(content, GUI.skin.button, width, centered, drawSpace);//, label, style);
            bool state = GUI.Button(btn, content, style);

            var e = Event.current;
            if (e.type == EventType.MouseDown && btn.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool Button(string label, GUIStyle style, float width, bool centered = true, bool drawSpace = false, bool inPlace = false)
        {
            if (centered) inPlace = false;
            else inPlace = true;
            GUIContent content = new GUIContent(label);
            Rect btn = DrawButton(content, GUI.skin.button, width, centered, drawSpace, inPlace);//, label, style);
            bool state = GUI.Button(btn, content, style);

            var e = Event.current;
            if (e.type == EventType.MouseDown && btn.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool Button(string label, GUIStyle style, float width, bool centered = true, bool drawSpace = false, bool whiteText = false, bool inPlace = false)
        {
            if (centered) inPlace = false;
            else inPlace = true;
            GUIContent content = new GUIContent(label);
            Rect btn = DrawButton(content, GUI.skin.button, width, centered, drawSpace, whiteText, inPlace);//, label, style);
            bool state = GUI.Button(btn, content, style);

            var e = Event.current;
            if (e.type == EventType.MouseDown && btn.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool Button(GUIContent content, GUIStyle style, bool centered = true, bool drawSpace = false)
        {
            Rect btn = DrawButton(content, GUI.skin.button, centered, drawSpace);//, label, style);
            bool state = GUI.Button(btn, content, style);

            var e = Event.current;
            if (e.type == EventType.MouseDown && btn.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static string DrawText(string label, string text, GUIStyle style, float width, float height = 20f)
        {
            string ret;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            ret = GUILayout.TextField(
                  text, style,
                  GUILayout.Width(width),
                  GUILayout.Height(height));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return ret;
        }

        public static void DrawSplitter()
        {
            EditorGUILayout.Space();
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 20f;
            rect.width += 4f;
            rect.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rect.center.y);

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));

        }

        public static void DrawSplitter(Color color)
        {
            EditorGUILayout.Space();
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 20f;
            rect.width += 4f;
            rect.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rect.center.y);

            if (Event.current.type != EventType.Repaint)
                return;

            color.a = 1.333f;
            EditorGUI.DrawRect(rect, color);
        }

        public static bool DrawHeaderFoldout(string title, bool state)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);
            backgroundRect.x += EditorGUI.indentLevel;
            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            //backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            //float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            float backgroundTint = 0.1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.1f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);


            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            var e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }
        public static void DrawCross(Transform m_transform)
        {
            var gizmoSize = 0.25f;
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.forward * gizmoSize / m_transform.localScale.z));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.forward * -gizmoSize / m_transform.localScale.z));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.up * gizmoSize / m_transform.localScale.y));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.up * -gizmoSize / m_transform.localScale.y));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.right * gizmoSize / m_transform.localScale.x));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.right * -gizmoSize / m_transform.localScale.x));
        }

        public static void GenerateTooltip(string text)
        {
            var propRect = GUILayoutUtility.GetLastRect();
            GUI.Label(propRect, new GUIContent("", text));
        }

        public static void DrawLineHelpBox()
        {
            EditorGUILayout.BeginVertical(EditorStyles.textField);
            EditorGUILayout.EndVertical();
        }

        internal static void DrawDescription(string v)
        {
            EditorGUILayout.BeginVertical(Skin_Orange);
            EditorGUILayout.HelpBox(v, MessageType.None);
            EditorGUILayout.EndVertical();
        }

        public static GUILayoutOption[] Widths(float width)
        {
            return new GUILayoutOption[] { GUILayout.Width(width), GUILayout.MaxWidth(width), GUILayout.ExpandWidth(false) };
        }

        public static GUIContent GetLabel(string text, GUIStyle style, bool centered = false)
        {
            GUIContent labelText = new GUIContent(text);
            var rt = GUILayoutUtility.GetRect(labelText, style, GUILayout.ExpandWidth(false));
            if (centered)
                rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
            EditorGUI.LabelField(rt, labelText, style);
            return labelText;
        }

        public static void DrawFullWidthLabel(string text, GUIStyle style, bool whiteText = false)
        {
            Color tmpColor = GUI.backgroundColor;
            if (!whiteText)
                GUI.backgroundColor = Color.black;

            GUIContent labelText = new GUIContent(text);

            var rt = GUILayoutUtility.GetRect(labelText, style, GUILayout.ExpandWidth(false));
            float width = EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.2f);
            rt.width = width;
            rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
            EditorGUI.LabelField(rt, labelText, style);

            if (!whiteText)
                GUI.backgroundColor = tmpColor;

            //return labelText;
        }

        internal static void BoolButton(SerializedProperty prop, GUIContent content)
        {
            prop.boolValue = GUILayout.Toggle(prop.boolValue, content, EditorStyles.miniButton);
        }

        internal static void Arrays(SerializedProperty prop, GUIContent content = null)
        {
            EditorGUI.indentLevel++;
            if (content != null)
                EditorGUILayout.PropertyField(prop, content, true);
            else
                EditorGUILayout.PropertyField(prop, true);
            EditorGUI.indentLevel--;
        }

        public static bool Foldout(bool val, string name, bool bold = false)
        {
            EditorGUI.indentLevel++;
            if (bold)
                val = EditorGUILayout.Foldout(val, name, BoldFoldout);
            else
                val = EditorGUILayout.Foldout(val, name);
            EditorGUI.indentLevel--;

            return val;
        }

        internal static bool Foldout(SerializedProperty prop, string name, bool bold = false)
        {
            EditorGUI.indentLevel++;
            if (bold)
                prop.boolValue = EditorGUILayout.Foldout(prop.boolValue, name, BoldFoldout);
            else
                prop.boolValue = EditorGUILayout.Foldout(prop.boolValue, name);
            EditorGUI.indentLevel--;

            return prop.boolValue;
        }

        // Credit: https://forum.unity.com/threads/im-going-to-teach-you-guys-how-to-create-link-labels-for-the-unity-editor.513606/
        public static bool LinkLabel(string labelText)
        {
            return LinkLabel(labelText, Color.black, new Vector2(), 0);
        }

        public static bool LinkLabel(string labelText, Color labelColor)
        {
            return LinkLabel(labelText, labelColor, new Vector2(), 0);
        }

        public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset)
        {
            return LinkLabel(labelText, labelColor, contentOffset, 0);
        }

        public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize)
        {
            //Let's use Unity's label style for this
            GUIStyle stl = EditorStyles.label;
            //Next let's record the settings for Unity's label style because we will have to make sure these settings get returned back to
            //normal after we are done changing them and drawing our LinkLabel.
            Color col = stl.normal.textColor;
            Vector2 os = stl.contentOffset;
            int size = stl.fontSize;
            //Now we can modify the label's settings via the editor style : EditorStyles.label (stl).
            stl.normal.textColor = labelColor;
            stl.contentOffset = contentOffset;
            stl.fontSize = fontSize;
            //We are now ready to draw our Linklabel. I will actually use a GUILayout.Button to do this and our "stl" style will
            //make the button appear as a label.

            //Note : You may include a web address parameter in this method and open a URL at this point if the button is clicked,
            //however, I am going to just return bool based on weather or not the link was clicked. This gives me more control over
            //what actually happens when a link label is used. I also will instead include a "URL version" of this method below.

            //Since the button already returns bool, I will just return that result straight across like this.

            try
            {
                return GUILayout.Button(labelText, stl);
            }
            finally
            {
                //Remember to set the editor style (stl) back to normal here. A try / finally clause will work perfectly for this!!!

                stl.normal.textColor = col;
                stl.contentOffset = os;
                stl.fontSize = size;
            }
        }

        //This is a modified version of link label that opens a URL automatically. Note : this can also return bool if you want.
        public static void LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize, string webAddress)
        {
            if (LinkLabel(labelText, labelColor, contentOffset, fontSize))
            {
                try
                {
                    Application.OpenURL(@webAddress);
                    //if returning bool, return true here.
                }
                catch
                {
                    //In most cases, the catch clause would not happen but in the interest of being thorough I will log an
                    //error and have Unity "beep" if an exception gets thrown for any reason.
                    Debug.LogError("Could not open URL. Please check your network connection and ensure the web address is correct.");
                    EditorApplication.Beep();
                }
            }
            //if returning bool, return false here.
        }

        // Credit: https://answers.unity.com/questions/1139985/gizmosdrawline-thickens.html
        public static void DrawLine(Vector3 p1, Vector3 p2, float width, Color lineColor)
        {
            if (lineColor == null) lineColor = Color.white;

            Handles.color = lineColor;
            Gizmos.color = lineColor;
            int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
            if (count == 1)
            {
                Gizmos.DrawLine(p1, p2);
            }
            else
            {
                Camera c = Camera.current;
                if (c == null)
                {
                    Debug.LogError("Camera.current is null");
                    return;
                }
                var scp1 = c.WorldToScreenPoint(p1);
                var scp2 = c.WorldToScreenPoint(p2);

                Vector3 v1 = (scp2 - scp1).normalized; // line direction
                Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector

                for (int i = 0; i < count; i++)
                {
                    Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                    Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                    Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                    Gizmos.DrawLine(origin, destiny);
                }
            }
        }

        public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
        {
            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (obj == null) { return null; }

            T actualObject = null;
            if (obj.GetType().IsArray)
            {
                var index = System.Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                actualObject = ((T[])obj)[index];
            }
            else
            {
                actualObject = obj as T;
            }
            return actualObject;
        }
    }
}
#endif