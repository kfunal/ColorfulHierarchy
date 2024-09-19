using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ColorfulHierarchy.Utilities
{
    [System.Serializable]
    public class CHObjectData
    {
        public int ObjectIdentifier;
        public bool IsSelected;
        public bool IsHovered;
        public bool IsDropDownHovered;
        public bool AutoIcon;
        public ColorArrayWrapper GradientColors;
        public Color TextColor;
        public string IconName;
        public string SceneName;

        public CHObjectData(GameObject _object)
        {
            IsSelected = false;
            IsHovered = false;
            IsDropDownHovered = false;
            AutoIcon = false;
            GradientColors = null;
            TextColor = new Color(0f, 0f, 0f, 0f);
            IconName = string.Empty;
            SceneName = AssetDatabase.AssetPathToGUID(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
            ObjectIdentifier = CHUtilities.GetLocalIdentifier(_object);
        }
    }

    [System.Serializable]
    public class CHObjectDataModel
    {
        public List<CHObjectData> ObjectDataList;

        public CHObjectDataModel()
        {
            ObjectDataList = new List<CHObjectData>();
        }
    }
}