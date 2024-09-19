using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Type = System.Type;

namespace ColorfulHierarchy.Utilities
{
    [InitializeOnLoad]
    public static class CHUtilities
    {
        private static bool mouseDown;
        private static CHSettings settings;
        private static CHObjectData data;
        public static string CurrentSceneName => SceneManager.GetActiveScene().path;

        public static bool MouseDown
        {
            get
            {
                UpdateMouseState();
                return mouseDown;
            }
        }

        static CHUtilities()
        {
            settings = AssetDatabase.LoadAssetAtPath<CHSettings>(CHConstants.SETTINGS_PATH);
        }

        public static bool IsHierarchyWindowFocused() => EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType().Name == "SceneHierarchyWindow";
        public static bool ShowDefaultIconForPrefab(GameObject _obj) => IsPrefab(_obj) && !settings.OverridePrefabIcons;

        public static bool IsPrefab(GameObject _obj) => PrefabUtility.GetCorrespondingObjectFromOriginalSource(_obj) != null;

        public static GUIContent GetTopComponentContent(Component[] components)
        {
            Component component = components.Length > 1 ? components[1] : components[0];

            if (component != null && component.GetType() == typeof(CanvasRenderer))
                component = components[components.Length - 1];

            Type componentType = component ? component.GetType() : typeof(Component);

            return GetContent(componentType, component);
        }

        public static GUIContent GetContent(Type type, Component component = null)
        {
            GUIContent content = EditorGUIUtility.ObjectContent(component, type);
            content.text = null;

            return content;
        }

        public static CHObjectData GetObjectData(Rect _selectionRect, int _instanceID, GameObject _object)
        {
            data = settings.PickObjectData(_object);

            if (data == null)
                return null;

            Rect rowRect = _selectionRect;
            rowRect.x = 0;
            rowRect.width = short.MaxValue;

            Rect expandChildrenIconRect = _selectionRect;
            expandChildrenIconRect.x -= CHConstants.EXPAND_ICON_X_OFFSET;
            expandChildrenIconRect.width = CHConstants.EXPAND_ICON_WIDTH;

            data.IsSelected = Selection.instanceIDs.Contains(_instanceID);
            data.IsHovered = rowRect.Contains(Event.current.mousePosition);
            data.IsDropDownHovered = expandChildrenIconRect.Contains(Event.current.mousePosition);

            return data;
        }

        public static Gradient CustomGradient(Color _startColor, Color _endColor)
        {
            GradientColorKey[] keys = new GradientColorKey[] { new GradientColorKey(_startColor, 1f), new GradientColorKey(_endColor, 1f) };
            GradientAlphaKey[] alphas = new GradientAlphaKey[] { new GradientAlphaKey(_startColor.a, 1f), new GradientAlphaKey(_endColor.a, 1f) };

            return new Gradient() { colorKeys = keys, alphaKeys = alphas };
        }

        private static void UpdateMouseState()
        {
            if (Event.current == null)
                return;

            if (Event.current.type == EventType.MouseDrag)
                mouseDown = true;
            if (Event.current.type == EventType.DragExited)
                mouseDown = false;
            if (Event.current.type == EventType.MouseDown)
                mouseDown = true;
            if (Event.current.type == EventType.MouseUp)
                mouseDown = false;
        }

        public static Texture2D CreateColorTexture(Color color, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        public static Texture2D GenerateGradientTextureFromColors(List<Color> _colors, int _width, int _height)
        {
            Texture2D texture = new Texture2D(_width, _height);

            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[_colors.Count];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[_colors.Count];

            for (int i = 0; i < _colors.Count; i++)
            {
                float time = (float)i / (_colors.Count - 1);
                colorKeys[i] = new GradientColorKey(_colors[i], time);
                alphaKeys[i] = new GradientAlphaKey(_colors[i].a, time);
            }

            gradient.SetKeys(colorKeys, alphaKeys);

            for (int x = 0; x < _width; x++)
            {
                Color color = gradient.Evaluate((float)x / (_width - 1));

                for (int y = 0; y < _height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        public static int GetLocalIdentifier(GameObject _object)
        {
            if (_object == null)
            {
                return -1;
            }

            PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

            if (inspectorModeInfo == null)
            {
                return -1;
            }

            SerializedObject serializedObject = new SerializedObject(_object);

            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

            SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

            if (localIdProp == null)
            {
                return -1;
            }

            return localIdProp.intValue;
        }

        public static void FocusHierarchy()
        {
            foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                if (window.GetType().Name == "SceneHierarchyWindow")
                {
                    window.Focus();
                    return;

                }
            }
        }
    }
}