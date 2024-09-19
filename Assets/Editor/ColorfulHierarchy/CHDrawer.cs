using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using static ColorfulHierarchy.Utilities.CHUtilities;
using static ColorfulHierarchy.Utilities.CHColors;
using ColorfulHierarchy.Utilities;

namespace ColorfulHierarchy.Drawer
{
    [InitializeOnLoad]
    public static class CHDrawer
    {
        private static bool hierarchyHasFocus;
        private static EditorWindow hierarchyEditorWindow;

        private static readonly HashSet<int> additionalSelectedInstanceIDs;

        private static GameObject currentObject;
        private static GUIContent currentObjectContent;
        private static GUIContent iconContent;
        private static CHObjectData currentObjectData;
        private static int selectedAmount;
        private static Color hierarchyColor;
        private static Color iconOriginalColor;
        private static Rect iconBackgroundRect;
        private static Rect textRect;
        private static Rect objectBackgroundRect;
        private static CHSettings settings;
        private static CHPalette palette;
        private static Texture2D backgroundTexture;
        private static Vector2 lastHierarchySize = new Vector2();
        private static Component[] currentObjectComponents;

        static CHDrawer()
        {
            additionalSelectedInstanceIDs = new HashSet<int>();

            settings = AssetDatabase.LoadAssetAtPath<CHSettings>(CHConstants.SETTINGS_PATH);
            palette = AssetDatabase.LoadAssetAtPath<CHPalette>(CHConstants.PALETTE_PATH);

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnHierarchyWindowItemOnGUI(int _instanceID, Rect _selectionRect)
        {
            if (!settings.Enabled) return;
            if (settings.ObjectDataModel == null) return;
            if (lastHierarchySize.x <= 0) return;

            ClearVariables();

            currentObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
            if (currentObject == null) return;

            currentObjectData = GetObjectData(_selectionRect, _instanceID, currentObject);

            if (currentObjectData == null)
                return;

            GetCurrentObjectData(_instanceID, _selectionRect);
            DrawBackground(_selectionRect, currentObject);
            UpdateSelectedObjectsList(currentObjectData, _instanceID);
            ObjectIcon(_selectionRect);
        }

        private static void DrawBackground(Rect _selectionRect, GameObject _obj)
        {
            if (IsPrefab(_obj)) return;

            if (currentObjectData.GradientColors == null || currentObjectData.GradientColors.Colors == null || currentObjectData.GradientColors.Colors.Count == 0)
            {
                backgroundTexture = GenerateGradientTextureFromColors(new List<Color>() { GetHierarchyColor(currentObjectData, hierarchyHasFocus, selectedAmount) }, Mathf.CeilToInt(lastHierarchySize.x), Mathf.CeilToInt(_selectionRect.height));
                objectBackgroundRect = _selectionRect;
            }
            else
            {
                backgroundTexture = GenerateGradientTextureFromColors(currentObjectData.GradientColors.Colors, Mathf.CeilToInt(lastHierarchySize.x), Mathf.CeilToInt(_selectionRect.height));
                objectBackgroundRect = new Rect(new Vector2(0, _selectionRect.y), new Vector2(lastHierarchySize.x, _selectionRect.size.y));
            }

            textRect = new Rect(_selectionRect.position + CHConstants.LABEL_START_OFFSET * Vector2.right, _selectionRect.size);

            ClearBackground(_selectionRect, currentObjectData);

            GUIStyle style = new GUIStyle();
            style.normal.background = backgroundTexture;

            EditorGUI.LabelField(objectBackgroundRect, GUIContent.none, style);
            EditorGUI.LabelField(textRect, _obj.name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = NameTextColor(currentObjectData) }
            });
        }

        private static void OnEditorUpdate()
        {
            if (!hierarchyEditorWindow && IsHierarchyWindowFocused())
                hierarchyEditorWindow = EditorWindow.GetWindow(Type.GetType($"{nameof(UnityEditor)}.SceneHierarchyWindow,{nameof(UnityEditor)}"));

            hierarchyHasFocus = EditorWindow.focusedWindow && EditorWindow.focusedWindow == hierarchyEditorWindow;
            additionalSelectedInstanceIDs.Clear();

            if (hierarchyEditorWindow != null && hierarchyEditorWindow.position.size != lastHierarchySize)
                lastHierarchySize = hierarchyEditorWindow.position.size;
        }

        private static void GetCurrentObjectData(int _instanceID, Rect _selectionRect)
        {
            currentObjectComponents = currentObject.GetComponents<Component>();

            if (currentObjectComponents == null || currentObjectComponents.Length == 0)
                return;

            currentObjectContent = GetTopComponentContent(currentObjectComponents);

            if (currentObjectContent == null || currentObjectContent.image == null)
                return;
        }

        private static void ObjectIcon(Rect _selectionRect)
        {
            if (ShowDefaultIconForPrefab(currentObject))
                return;

            DrawIcon(_selectionRect, currentObjectContent, currentObject);
        }

        private static void DrawIcon(Rect _selectionRect, GUIContent _content, GameObject _obj)
        {
            iconOriginalColor = GUI.color;

            iconContent = null;

            if (currentObjectData.AutoIcon)
                iconContent = _content;
            else if (!string.IsNullOrEmpty(currentObjectData.IconName))
                iconContent = new GUIContent(palette.GetIcon(currentObjectData.IconName));

            if (iconContent == null)
                iconContent = EditorGUIUtility.IconContent(CHConstants.GAME_OBJECT_ICON_NAME);

            if (!_obj.activeInHierarchy)
                GUI.color = new Color(iconOriginalColor.r, iconOriginalColor.g, iconOriginalColor.b, 0.5f);

            EditorGUI.LabelField(_selectionRect, iconContent);

            GUI.color = iconOriginalColor;
        }

        private static void UpdateSelectedObjectsList(CHObjectData _data, int _instanceID)
        {
            if (_data.IsSelected || (_data.IsDropDownHovered && MouseDown))
            {
                if (Selection.instanceIDs.Length > 1)
                    additionalSelectedInstanceIDs.Clear();

                additionalSelectedInstanceIDs.Add(_instanceID);
            }
            else
                additionalSelectedInstanceIDs.Remove(_instanceID);
        }

        private static void ClearBackground(Rect _selectionRect, CHObjectData _objectData)
        {
            hierarchyColor = GetHierarchyColor(_objectData, hierarchyHasFocus, selectedAmount);
            EditorGUI.DrawRect(_selectionRect, hierarchyColor);
        }

        private static void ClearOriginalIcon(Rect _selectionRect, CHObjectData _objectData)
        {
            hierarchyColor = GetHierarchyColor(_objectData, hierarchyHasFocus, selectedAmount);
            iconBackgroundRect = _selectionRect;
            iconBackgroundRect.width = CHConstants.ICON_WIDTH;
            EditorGUI.DrawRect(iconBackgroundRect, hierarchyColor);
        }

        private static void ClearVariables()
        {
            currentObject = null;
            currentObjectComponents = null;
        }
    }
}