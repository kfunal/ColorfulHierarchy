using ColorfulHierarchy.Utilities;
using UnityEditor;
using UnityEngine;
using static ColorfulHierarchy.Utilities.CHEditorGUIHelper;
using static ColorfulHierarchy.Utilities.CHConstants;

namespace ColorfulHierarchy.Popup
{
    public class CHObjectPopup : PopupWindowContent
    {
        private Vector2 windowSize = new Vector2(OBJECT_POPUP_WIDTH, OBJECT_POPUP_HEIGHT);
        private Vector2 scrollPosition;
        private GameObject selectedObject;
        private CHPalette palette;
        private CHSettings settings;
        private Color textColor;
        private ColorArrayWrapper currentGradient;
        private CHIconData iconData;
        private int gradientIndex = 0;
        private int iconIndex = 0;
        private Texture2D gridElementTexture;
        private bool colorFoldout;
        private bool iconFoldout;
        private bool autoIcon;

        private CHObjectData objectData;

        private Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                if (value == textColor) return;

                textColor = value;
                objectData.TextColor = textColor;
                SaveObjectData();
            }
        }

        private bool AutoIcon
        {
            get
            {
                return autoIcon;
            }
            set
            {
                if (value == autoIcon) return;

                autoIcon = value;
                objectData.AutoIcon = autoIcon;
                SaveObjectData();
            }
        }

        private GUIStyle GridElementStyle => new GUIStyle()
        {
            fixedWidth = GRADIENT_WIDTH,
            fixedHeight = GRADIENT_HEIGHT,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(GRADIENT_GRID_ELEMENT_PADDING, GRADIENT_GRID_ELEMENT_PADDING, GRADIENT_GRID_ELEMENT_PADDING, GRADIENT_GRID_ELEMENT_PADDING),
        };

        private GUIStyle GridStyle => new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(GRADIENT_GRID_HORIZONTAL_PADDING, GRADIENT_GRID_HORIZONTAL_PADDING, GRADIENT_GRID_VERTICAL_PADDING, GRADIENT_GRID_VERTICAL_PADDING)
        };

        public CHObjectPopup(GameObject _selectedObject)
        {
            selectedObject = _selectedObject;

            palette = AssetDatabase.LoadAssetAtPath<CHPalette>(PALETTE_PATH);
            settings = AssetDatabase.LoadAssetAtPath<CHSettings>(SETTINGS_PATH);

            gradientIndex = 0;
            iconIndex = 0;

            colorFoldout = true;
            iconFoldout = false;
            InitializeObjectData();
        }

        public override Vector2 GetWindowSize()
        {
            return windowSize;
        }

        public override void OnGUI(Rect rect)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
            {
                EditorGUILayout.LabelField(UNSAVED_CHANGES, new GUIStyle(GUI.skin.label)
                {
                    fixedWidth = OBJECT_POPUP_WIDTH,
                    fixedHeight = UNSAVED_CHANGES_TEXT_HEIGHT
                });
                return;
            }

            ResetData();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            TextColorArea();
            FoldoutControl();
            GradientField();
            IconField();
            EditorGUILayout.EndScrollView();
        }

        private void FoldoutControl()
        {
            EditorGUILayout.Space(TEN);

            CenteredHorizontalLayout(() =>
            {
                HorizontalLayout(COLOR_TEXT_WIDTH * 2, COLOR_TEXT_HEIGHT, () =>
                 {
                     LabelWithoutSpace(COLOR, COLOR_TEXT_WIDTH, COLOR_TEXT_HEIGHT);
                     colorFoldout = EditorGUILayout.Toggle(colorFoldout, GUILayout.Width(COLOR_TEXT_WIDTH), GUILayout.Height(COLOR_TEXT_HEIGHT));

                 });

                HorizontalLayout(ICON_TEXT_WIDTH * 2, ICON_TEXT_HEIGHT, () =>
                  {
                      LabelWithoutSpace(ICON, ICON_TEXT_WIDTH, ICON_TEXT_HEIGHT);
                      iconFoldout = EditorGUILayout.Toggle(iconFoldout, GUILayout.Width(ICON_TEXT_WIDTH), GUILayout.Height(ICON_TEXT_HEIGHT));

                  });
            });
        }

        private void ResetData()
        {
            GUIStyle resetButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = RESET_DATA_BUTTON_WIDTH,
                fixedHeight = RESET_DATA_BUTTON_HEIGHT,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
            };

            resetButtonStyle.normal.textColor = Color.yellow;
            resetButtonStyle.hover.textColor = Color.cyan;

            RightAlignedHorizontalLayout(OBJECT_POPUP_WIDTH, () =>
            {
                if (GUILayout.Button(RESET_DATA, resetButtonStyle))
                {
                    if (EditorUtility.DisplayDialog(ARE_YOU_SURE, ARE_YOU_SURE_CONTENT, YES, NO))
                        settings.DeleteObjectData(objectData);
                }
            });
        }

        private void TextColorArea()
        {
            EditorGUILayout.Space(TEN);

            CenteredHorizontalLayout(() =>
            {
                LabelWithoutSpace(TEXT_COLOR, TEXT_COLOR_TEXT_WIDTH, TEXT_COLOR_TEXT_HEIGHT);
                TextColor = EditorGUILayout.ColorField(TextColor, GUILayout.Width(TEXT_COLOR_FIELD_WIDTH));
            });
        }

        private void GradientField()
        {
            if (!IsInitialized()) return;
            if (palette.Gradients == null || palette.Gradients.Count == 0) return;
            if (!colorFoldout) return;

            EditorGUILayout.Space(TEN);

            gradientIndex = 0;

            for (int row = 0; row < palette.Gradients.Count; row++)
            {
                EditorGUILayout.BeginHorizontal(GridStyle);

                for (int column = 0; column < GRADIENT_GRID_COLUMN; column++)
                {
                    currentGradient = palette.Gradients[gradientIndex];

                    if (currentGradient == null || currentGradient.Colors == null || currentGradient.Colors.Count == 0)
                    {
                        gradientIndex++;
                        if (gradientIndex >= palette.Gradients.Count)
                        {
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        continue;
                    }

                    gridElementTexture = CHUtilities.GenerateGradientTextureFromColors(currentGradient.Colors, GRADIENT_WIDTH, GRADIENT_HEIGHT);

                    if (GUILayout.Button(gridElementTexture, GridElementStyle))
                    {
                        objectData.GradientColors = new ColorArrayWrapper(currentGradient.Colors);
                        SaveObjectData();
                    }

                    gradientIndex++;

                    if (gradientIndex >= palette.Gradients.Count)
                    {
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void IconField()
        {
            if (!IsInitialized()) return;
            if (palette.Icons == null || palette.Icons.Count == 0) return;
            if (!iconFoldout) return;

            EditorGUILayout.Space(TEN);

            iconIndex = 0;

            HorizontalLayout(() =>
            {
                LabelWithoutSpace(AUTO_ICON, AUTO_ICON_TEXT_WIDTH, AUTO_ICON_TEXT_HEIGHT);
                AutoIcon = EditorGUILayout.Toggle(AutoIcon);
            });

            if (autoIcon) return;

            for (int row = 0; row < palette.Icons.Count; row++)
            {
                EditorGUILayout.BeginHorizontal(GridStyle);
                for (int column = 0; column < GRADIENT_GRID_COLUMN; column++)
                {
                    iconData = palette.Icons[iconIndex];

                    if (iconData == null)
                    {
                        iconIndex++;
                        if (iconIndex >= palette.Icons.Count)
                        {
                            EditorGUILayout.EndHorizontal();
                            return;
                        }
                        continue;
                    }

                    if (GUILayout.Button(iconData.Texture, GridElementStyle))
                    {
                        objectData.IconName = iconIndex == 0 ? string.Empty : iconData.Name;
                        SaveObjectData();
                    }

                    iconIndex++;

                    if (iconIndex >= palette.Icons.Count)
                    {
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void InitializeObjectData()
        {
            if (selectedObject == null) return;

            objectData = settings.PickObjectData(selectedObject) ?? new CHObjectData(selectedObject);

            textColor = objectData.TextColor;
            autoIcon = objectData.AutoIcon;
        }

        private void SaveObjectData()
        {
            settings.ChangeData(objectData);
            EditorApplication.RepaintHierarchyWindow();
        }

        private bool IsInitialized()
        {
            if (selectedObject == null) return false;
            if (palette == null) return false;
            if (palette.Gradients == null) return false;
            if (settings == null) return false;

            return true;
        }
    }
}
