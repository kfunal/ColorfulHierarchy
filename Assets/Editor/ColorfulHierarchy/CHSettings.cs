using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ColorfulHierarchy.Utilities
{
    [CreateAssetMenu(fileName = "Colorful Hierarchy Settings", menuName = "Colorful Hierarchy/Settings")]
    public class CHSettings : ScriptableObject
    {
        [SerializeField] private bool enabled;
        [SerializeField] private bool overridePrefabIcons;

        [SerializeField] private CHObjectDataModel objectDataModel;

        private List<CHObjectData> tempDataList;
        private GameObject[] sceneObjects;

        public bool Enabled => enabled;
        public bool OverridePrefabIcons => overridePrefabIcons;
        public CHObjectDataModel ObjectDataModel => objectDataModel;

        public void LoadObjectDatas()
        {
            string json = EditorPrefs.GetString(CHConstants.OBJECT_DATA_LIST_PREFS, string.Empty);

            if (string.IsNullOrEmpty(json))
            {
                objectDataModel = new CHObjectDataModel();
                EditorUtility.SetDirty(this);
                EditorApplication.ExecuteMenuItem("File/Save Project");
                return;
            }

            objectDataModel = JsonUtility.FromJson<CHObjectDataModel>(json);

            EditorUtility.SetDirty(this);
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        public void SaveObjectData()
        {
            string json = JsonUtility.ToJson(objectDataModel);
            EditorPrefs.SetString(CHConstants.OBJECT_DATA_LIST_PREFS, json);
            EditorUtility.SetDirty(this);
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        public CHObjectData PickObjectData(GameObject _gameObject)
        {
            if (objectDataModel == null)
                return null;

            for (int i = 0; i < objectDataModel.ObjectDataList.Count; i++)
            {
                if (objectDataModel.ObjectDataList[i].ObjectIdentifier != 0 && objectDataModel.ObjectDataList[i].ObjectIdentifier == CHUtilities.GetLocalIdentifier(_gameObject))
                {
                    return objectDataModel.ObjectDataList[i];
                }
            }

            return null;
        }

        public void ChangeData(CHObjectData _objectData)
        {
            if (objectDataModel == null) return;
            if (objectDataModel.ObjectDataList == null) return;

            for (int i = 0; i < objectDataModel.ObjectDataList.Count; i++)
            {
                if (objectDataModel.ObjectDataList[i] == _objectData)
                {
                    objectDataModel.ObjectDataList[i] = _objectData;
                    SaveObjectData();
                    return;
                }
            }

            objectDataModel.ObjectDataList.Add(_objectData);
            SaveObjectData();
        }

        public void DeleteObjectData(CHObjectData _data)
        {
            if (objectDataModel == null) return;
            if (objectDataModel.ObjectDataList == null) return;

            for (int i = 0; i < objectDataModel.ObjectDataList.Count; i++)
            {
                if (objectDataModel.ObjectDataList[i] == _data)
                {
                    objectDataModel.ObjectDataList.RemoveAt(i);
                    SaveObjectData();
                    return;
                }
            }
        }

        public void CheckElementData()
        {
            if (objectDataModel == null) return;
            if (objectDataModel.ObjectDataList == null) return;

            tempDataList = objectDataModel.ObjectDataList.Where(x => x.SceneName == AssetDatabase.AssetPathToGUID(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path)).ToList();
            sceneObjects = FindObjectsOfType<GameObject>();

            if (tempDataList == null || tempDataList.Count == 0) return;
            if (sceneObjects == null || sceneObjects.Length == 0) return;

            foreach (CHObjectData data in tempDataList)
            {
                if (sceneObjects.Where(obj => CHUtilities.GetLocalIdentifier(obj) == data.ObjectIdentifier).Count() == 0)
                    objectDataModel.ObjectDataList.Remove(data);
            }

            SaveObjectData();
        }

        [ContextMenu("Reset Data")]
        public void ResetData()
        {
            objectDataModel = new CHObjectDataModel();
            SaveObjectData();
        }
    }
}
