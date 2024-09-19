using UnityEditor;
using UnityEngine;
using static ColorfulHierarchy.Utilities.CHUtilities;
using static ColorfulHierarchy.Utilities.CHEditorGUIHelper;
using static ColorfulHierarchy.Utilities.CHConstants;
using System.Collections.Generic;

namespace ColorfulHierarchy.Utilities
{
    [CustomEditor(typeof(CHPalette))]
    public class CHPaletteEditor : Editor
    {
        private CHPalette palette = null;

        private int colorGridColumn;
        private int colorIndex;
        private bool colorFoldout;
        private bool addColor;

        private int iconGridColumn;
        private int iconIndex;
        private bool iconFoldout;
        private bool iconAdd;

        private Vector2 scrollPosition;

        private SerializedProperty iconsProperty;
        private SerializedProperty iconProp;
        private Texture2D iconTexture;
        private Texture2D colorTexture;
        private Texture2D removeIcon;
        private Texture2D addIcon;
        private List<Texture2D> iconsToAdd;
        private ColorArrayWrapper colorArray;
        private List<Color> colorsToAdd;

        private GUIStyle iconGridElementStyle => new GUIStyle()
        {
            fixedHeight = ICON_CELL_SIZE,
            fixedWidth = ICON_CELL_SIZE,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(ICON_GRID_PADDING, ICON_GRID_PADDING, ICON_GRID_PADDING, ICON_GRID_PADDING),
        };

        private GUIStyle colorGridElementStyle => new GUIStyle()
        {
            fixedHeight = COLOR_CELL_SIZE,
            fixedWidth = COLOR_CELL_SIZE,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(COLOR_CELL_PADDING, COLOR_CELL_PADDING, COLOR_CELL_PADDING, COLOR_CELL_PADDING),
        };

        private GUIStyle removeButtonStyle => new GUIStyle()
        {
            fixedWidth = COLOR_REMOVE_BUTTON_WIDTH,
            fixedHeight = COLOR_REMOVE_BUTTON_HEIGHT,
            alignment = TextAnchor.MiddleCenter,
        };

        private GUIStyle textureAddButtonStyle => new GUIStyle()
        {
            fixedWidth = TEXTURE_ADD_ICON_WIDTH,
            fixedHeight = TEXTURE_ADD_ICON_HEIGHT,
            alignment = TextAnchor.MiddleCenter,
        };

        private GUIStyle textureSaveButtonStyle => new GUIStyle(GUI.skin.button)
        {
            fixedWidth = TEXTURE_SAVE_ICON_WIDTH,
            fixedHeight = TEXTURE_SAVE_ICON_HEIGHT,
            alignment = TextAnchor.MiddleCenter,
        };

        private GUIStyle textureRemoveButtonStyle => new GUIStyle()
        {
            fixedWidth = TEXTURE_REMOVE_ICON_WIDTH,
            fixedHeight = TEXTURE_REMOVE_ICON_HEIGHT,
            alignment = TextAnchor.MiddleCenter,
        };

        private GUIStyle addButtonStyle => new GUIStyle()
        {
            fixedWidth = COLOR_ADD_BUTTON_WIDTH,
            fixedHeight = COLOR_ADD_BUTTON_HEIGHT,
            alignment = TextAnchor.MiddleCenter,
        };

        private GUIStyle FoldoutStyle(Color _textColor)
        {
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.normal.textColor = _textColor;
            foldoutStyle.onNormal.textColor = _textColor;
            foldoutStyle.fontSize = 13;
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutStyle.padding = new RectOffset(20, 0, 0, 0);
            return foldoutStyle;
        }

        private void OnEnable()
        {
            palette = (CHPalette)target;
            removeIcon = (Texture2D)EditorGUIUtility.IconContent(X_ICON_NAME).image;
            addIcon = (Texture2D)EditorGUIUtility.IconContent(PLUS_ICON_NAME).image;
            iconsProperty = serializedObject.FindProperty("icons");
            colorsToAdd = new List<Color>() { Color.black };
            iconsToAdd = new List<Texture2D>() { null };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Separator();
            DrawColors();
            EditorGUILayout.Separator();
            DrawIcons();
            // base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawColors()
        {
            colorFoldout = EditorGUILayout.Foldout(colorFoldout, COLOR, FoldoutStyle(Color.cyan));

            if (!colorFoldout) return;
            if (palette.Gradients == null) return;

            colorGridColumn = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / COLOR_CELL_SIZE) - 1;
            colorIndex = 0;

            EditorGUILayout.Space(FIVE);

            HorizontalLayout(() =>
            {
                LabelWithoutSpace(ADD_COLOR, ADD_COLOR_TEXT_WIDTH, ADD_COLOR_TEXT_HEIGHT);
                addColor = EditorGUILayout.Toggle(addColor);
            });

            if (addColor)
            {
                AddColorArea();
                return;
            }

            for (int row = 0; row < palette.Gradients.Count; row++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int column = 0; column < colorGridColumn; column++)
                {
                    colorArray = palette.Gradients[colorIndex];

                    if (colorArray == null || colorArray.Colors == null || colorArray.Colors.Count == 0)
                    {
                        colorIndex++;

                        if (colorIndex >= palette.Gradients.Count)
                        {
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        continue;
                    }

                    colorTexture = GenerateGradientTextureFromColors(colorArray.Colors, COLOR_CELL_SIZE, COLOR_CELL_SIZE);
                    GUILayout.Label(colorTexture, colorGridElementStyle);

                    HandleColorContextMenu(GUILayoutUtility.GetLastRect(), colorIndex);

                    colorIndex++;

                    if (colorIndex >= palette.Gradients.Count)
                    {
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

        }

        private void AddColorArea()
        {
            if (colorsToAdd == null || colorsToAdd.Count == 0)
            {
                colorsToAdd = new List<Color>() { Color.black };
                return;
            }

            EditorGUILayout.Space(TEN);

            for (int i = 0; i < colorsToAdd.Count; i++)
            {
                HorizontalLayout(() =>
                {
                    LabelWithoutSpace($"{i + 1}.{COLOR}", COLOR_ADD_FIELD_LABEL_WIDTH, COLOR_ADD_FIELD_LABEL_HEIGHT);
                    colorsToAdd[i] = EditorGUILayout.ColorField(colorsToAdd[i]);

                    if (GUILayout.Button(removeIcon, removeButtonStyle))
                    {
                        colorsToAdd.RemoveAt(i);
                    }
                });
            }

            EditorGUILayout.Space(TEN);

            RightAlignedHorizontalLayout(EditorGUIUtility.currentViewWidth - COLOR_ADD_BUTTON_WIDTH / 2f, () =>
            {
                if (GUILayout.Button(SAVE_COLOR, GUILayout.Width(COLOR_SAVE_BUTTON_WIDTH), GUILayout.Height(COLOR_SAVE_BUTTON_HEIGHT)))
                {
                    palette.AddGradient(colorsToAdd);
                    EditorUtility.SetDirty(palette);
                    EditorApplication.ExecuteMenuItem(EDITOR_SAVE_PROJECT_PATH);
                }

                if (colorsToAdd.Count < MAX_COLOR_COUNT && GUILayout.Button(addIcon, addButtonStyle))
                {
                    colorsToAdd.Add(Color.black);
                }


            });

            EditorGUILayout.Space(TEN);
        }

        private void DrawIcons()
        {
            iconFoldout = EditorGUILayout.Foldout(iconFoldout, ICON, FoldoutStyle(Color.magenta));

            if (!iconFoldout) return;

            iconGridColumn = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / ICON_CELL_SIZE) - 1;
            iconIndex = 0;

            EditorGUILayout.Space(TEN);

            HorizontalLayout(() =>
            {
                LabelWithoutSpace(ADD_ICON, ADD_ICON_TEXT_WIDTH, ADD_COLOR_TEXT_HEIGHT);
                iconAdd = EditorGUILayout.Toggle(iconAdd);
            });

            if (iconAdd)
            {
                IconAddArea();
                return;
            }

            EditorGUILayout.Space(FIVE);

            for (int i = 0; i < iconsProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < iconGridColumn; j++)
                {
                    iconProp = iconsProperty.GetArrayElementAtIndex(iconIndex).FindPropertyRelative(TEXTURE);

                    if (iconProp == null)
                    {
                        iconIndex++;
                        if (iconIndex >= iconsProperty.arraySize)
                        {
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        continue;
                    }

                    iconTexture = (Texture2D)iconProp.objectReferenceValue;
                    GUILayout.Label(iconTexture, iconGridElementStyle);
                    HandleIconContextMenu(GUILayoutUtility.GetLastRect(), iconIndex);
                    iconIndex++;
                    if (iconIndex >= iconsProperty.arraySize)
                    {
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void IconAddArea()
        {
            EditorGUILayout.Space(TEN);

            GUILayout.Label("Icons To Add", EditorStyles.boldLabel);

            if (iconsToAdd != null && iconsToAdd.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                for (int i = 0; i < iconsToAdd.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    iconsToAdd[i] = (Texture2D)EditorGUILayout.ObjectField(iconsToAdd[i], typeof(Texture2D), false);

                    if (GUILayout.Button(removeIcon, textureRemoveButtonStyle))
                    {
                        if (iconsToAdd != null && iconsToAdd.Count > 1)
                            iconsToAdd.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();
                }

                EditorGUILayout.EndScrollView();
            }


            EditorGUILayout.Separator();

            RightAlignedHorizontalLayout(EditorGUIUtility.currentViewWidth - TEXTURE_ADD_ICON_WIDTH / 2f, () =>
            {
                if (GUILayout.Button(SAVE_ICONS, textureSaveButtonStyle))
                {
                    palette.AddIcons(iconsToAdd);
                    iconsToAdd = new List<Texture2D>();
                    EditorUtility.SetDirty(palette);
                    EditorApplication.ExecuteMenuItem(EDITOR_SAVE_PROJECT_PATH);
                }

                if (GUILayout.Button(addIcon, textureAddButtonStyle))
                {
                    iconsToAdd.Add(null);
                }
            });

        }

        private void HandleIconContextMenu(Rect iconRect, int index)
        {
            Event evt = Event.current;

            if (evt.type == EventType.ContextClick && iconRect.Contains(evt.mousePosition))
            {
                // Sağ tıklama menüsünü oluştur
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(REMOVE), false, () =>
                {
                    iconsProperty.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(palette);
                    EditorApplication.ExecuteMenuItem(EDITOR_SAVE_PROJECT_PATH);
                });
                menu.ShowAsContext();
                evt.Use(); // Olayı işlediğimizi belirtir
            }
        }

        private void HandleColorContextMenu(Rect iconRect, int index)
        {
            Event evt = Event.current;

            if (evt.type == EventType.ContextClick && iconRect.Contains(evt.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(REMOVE), false, () =>
                {
                    palette.RemoveColor(index);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(palette);
                    EditorApplication.ExecuteMenuItem(EDITOR_SAVE_PROJECT_PATH);
                });
                menu.ShowAsContext();
                evt.Use(); // Olayı işlediğimizi belirtir
            }
        }
    }
}