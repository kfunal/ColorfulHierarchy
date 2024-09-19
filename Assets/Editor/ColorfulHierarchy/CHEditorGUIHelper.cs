using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace ColorfulHierarchy.Utilities
{
    public static class CHEditorGUIHelper
    {
        /// <summary>
        /// Creates horizontal layout
        /// </summary>
        /// <param name="_content"></param>
        public static void HorizontalLayout(Action _content)
        {
            EditorGUILayout.BeginHorizontal();
            _content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Create horizontal layout by given size
        /// </summary>
        /// <param name="_content"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void HorizontalLayout(float _width, float _height, Action _content)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle()
            {
                fixedHeight = _height,
                fixedWidth = _width
            });
            _content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Create horizontal layout by given Width
        /// </summary>
        /// <param name="_content"></param>
        /// <param name="_width"></param>
        public static void HorizontalLayout(float _width, Action _content)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle()
            {
                fixedWidth = _width
            });
            _content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Aligns elements to right
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_content"></param>
        public static void RightAlignedHorizontalLayout(float _width, Action _content)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle()
            {
                fixedWidth = _width
            });
            GUILayout.FlexibleSpace();
            _content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates horizontal layout and with flexible space to center the element.
        /// </summary>
        /// <param name="_element"></param>
        public static void CenteredHorizontalLayout(Action _layoutContent)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _layoutContent?.Invoke();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates horizontal layout and with flexible space to center the element.
        /// </summary>
        /// <param name="_element"></param>
        public static void CenteredHorizontalLayout(float _width, float _height, Action _layoutContent)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            HorizontalLayout(_width, _height, () => _layoutContent?.Invoke());
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates label without space afterwords with given width and height or default size
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void LabelWithoutSpace(string _text, float _width, float _height)
        {
            GUILayout.Label(_text, new GUIStyle(GUI.skin.label)
            {
                stretchWidth = false,
                stretchHeight = false,
                fixedHeight = _height,
                fixedWidth = _width,
            });
        }

        /// <summary>
        /// Creates centered label without space afterwords with given width and height or default size
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void LabelWithoutSpaceCentered(string _text, float _width, float _height)
        {
            GUILayout.Label(_text, new GUIStyle(GUI.skin.label)
            {
                stretchWidth = false,
                stretchHeight = false,
                fixedHeight = _height,
                fixedWidth = _width,
                alignment = TextAnchor.MiddleCenter
            });
        }

        public static void LabelWithoutSpaceCentered(string _text, float _width, float _height, FontStyle _fontStyle, int _fontSize, Color _textColor)
        {
            GUILayout.Label(_text, new GUIStyle(GUI.skin.label)
            {
                stretchWidth = false,
                stretchHeight = false,
                fixedHeight = _height,
                fixedWidth = _width,
                fontStyle = _fontStyle,
                fontSize = _fontSize,
                normal = new GUIStyleState() { textColor = _textColor }
            });
        }

        /// <summary>
        /// Creates button grid. Since editor renderer top to bottom, in first for loop we go through end to start to make row and column truly.
        /// </summary>
        /// <param name="_row"></param>
        /// <param name="_column"></param>
        /// <param name="_content"></param>
        public static void ButtonGrid(int _row, int _column, Action<int, int> _content)
        {
            for (int row = _row; row >= 0; row--)
            {
                CenteredHorizontalLayout(() =>
                {
                    for (int column = 0; column < _column; column++)
                    {
                        _content?.Invoke(row, column);
                    }
                });
            }
        }

        /// <summary>
        /// Converts given int to enum with given type33333
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static T IntToEnum<T>(int _value)
        {
            return (T)Enum.ToObject(typeof(T), _value);
        }


        /// <summary>
        /// Converts given sprite to texture
        /// </summary>
        /// <param name="_sprite"></param>
        /// <returns></returns>
        public static Texture SpriteToTexture(Sprite _sprite)
        {
            var croppedTexture = new Texture2D((int)_sprite.rect.width, (int)_sprite.rect.height);
            var pixels = _sprite.texture.GetPixels((int)_sprite.textureRect.x,
                                                    (int)_sprite.textureRect.y,
                                                    (int)_sprite.textureRect.width,
                                                    (int)_sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            return croppedTexture;
        }

        /// <summary>
        /// Creates button with given texture
        /// </summary>
        /// <param name="_content"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_buttonIndex"></param>
        /// <param name="_onClick"></param>
        public static void Button(Texture _content, float _width, float _height, int _buttonIndex, Action<int> _onClick)
        {
            GUIContent content = new GUIContent(_content);
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = _height,
                fixedWidth = _width
            };

            if (GUILayout.Button(content, style))
                _onClick?.Invoke(_buttonIndex);
        }

        /// <summary>
        /// Creates button with given string
        /// </summary>
        /// <param name="_content"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_buttonIndex"></param>
        /// <param name="_onClick"></param>
        public static void Button(string _content, float _width, float _height, int _buttonIndex, Action<int> _onClick)
        {
            GUIContent content = new GUIContent(_content);
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = _height,
                fixedWidth = _width
            };

            if (GUILayout.Button(content, style))
                _onClick?.Invoke(_buttonIndex);
        }

        /// <summary>
        /// Creates centered box
        /// </summary>
        /// <param name="_content"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void CenteredBox(Texture _content, float _width, float _height)
        {
            CenteredHorizontalLayout(() =>
            {
                GUIContent content = new GUIContent(_content);
                GUIStyle style = new GUIStyle(GUI.skin.button)
                {
                    fixedHeight = _height,
                    fixedWidth = _width
                };

                GUILayout.Box(content, style);
            });
        }

        /// <summary>
        /// Creates animated foldout
        /// </summary>
        /// <param name="_animBool"></param>
        /// <param name="_spaceBefore"></param>
        /// <param name="_content"></param>
        public static void AnimatedFoldout(AnimBool _animBool, float _spaceBefore, Action _content)
        {
            EditorGUILayout.Space(_spaceBefore);

            if (EditorGUILayout.BeginFadeGroup(_animBool.faded))
                _content?.Invoke();

            EditorGUILayout.EndFadeGroup();
        }
    }
}
