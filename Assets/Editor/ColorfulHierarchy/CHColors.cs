using UnityEditor;
using UnityEngine;
using static ColorfulHierarchy.Utilities.CHUtilities;

namespace ColorfulHierarchy.Utilities
{
    public static class CHColors
    {
        private static Color defaultDarkColor = new Color(0.2196f, 0.2196f, 0.2196f);
        private static Color defaultLightColor = new Color(0.7843f, 0.7843f, 0.7843f);

        private static Color selectedDarkColor = new Color(0.1725f, 0.3647f, 0.5294f);
        private static Color selectedLightColor = new Color(0.22745f, 0.447f, 0.6902f);

        private static Color selectedUnFocusedDarkColor = new Color(0.3f, 0.3f, 0.3f);
        private static Color selectedUnFocusedLightColor = new Color(0.68f, 0.68f, 0.68f);

        private static Color hoverDarkColor = new Color(0.2706f, 0.2706f, 0.2706f);
        private static Color hoverLightColor = new Color(0.698f, 0.698f, 0.698f);

        public static Color DefaultTextColor = new Color(245f / 255f, 245f / 255f, 245f / 255f);
        public static Color Default => EditorGUIUtility.isProSkin ? defaultDarkColor : defaultLightColor;
        private static Color Selected => EditorGUIUtility.isProSkin ? selectedDarkColor : selectedLightColor;
        private static Color SelectedUnfocused => EditorGUIUtility.isProSkin ? selectedUnFocusedDarkColor : selectedUnFocusedLightColor;
        private static Color Hover => EditorGUIUtility.isProSkin ? hoverDarkColor : hoverLightColor;

        public static Color GetHierarchyColor(CHObjectData _objectData, bool _windowFocused, int _selectedAmount = 0)
        {
            bool isMouseDown = MouseDown;

            if (_objectData.IsSelected)
            {
                if (isMouseDown && !_objectData.IsDropDownHovered && !_objectData.IsHovered && _selectedAmount == 1)
                    return Default;

                return _windowFocused ? Selected : SelectedUnfocused;
            }

            else if (_objectData.IsHovered)
                return isMouseDown && !_objectData.IsDropDownHovered ? Selected : Hover;

            return Default;
        }

        public static Color NameTextColor(CHObjectData _objectData)
        {
            if (_objectData.TextColor == new Color(0f, 0f, 0f, 0f))
                return DefaultTextColor;

            return _objectData.TextColor;
        }

    }
}
