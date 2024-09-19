using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ColorfulHierarchy.Popup;

namespace ColorfulHierarchy.Utilities
{
    [InitializeOnLoad]
    public class CHEventController : MonoBehaviour
    {
        private static Event currentEvent;
        private static GameObject currentGameObject;
        private static CHSettings settings;
        private static Rect popupRect;
        private static Vector2 popupOffset;


        static CHEventController()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;

            EditorSceneManager.sceneOpened += OnSceneOpened;

            EditorApplication.quitting -= OnEditorQuitting;
            EditorApplication.quitting += OnEditorQuitting;

            EditorApplication.delayCall -= DelayCall;
            EditorApplication.delayCall += DelayCall;
        }
        private static void DelayCall()
        {
            if (!SessionState.GetBool(CHConstants.SESSION_KEY, false))
            {
                settings = AssetDatabase.LoadAssetAtPath<CHSettings>(CHConstants.SETTINGS_PATH);
                settings.LoadObjectDatas();
                SessionState.SetBool(CHConstants.SESSION_KEY, true);
            }

            EditorApplication.RepaintHierarchyWindow();
            CHUtilities.FocusHierarchy();
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            settings = AssetDatabase.LoadAssetAtPath<CHSettings>(CHConstants.SETTINGS_PATH);
            settings.LoadObjectDatas();
            settings.CheckElementData();
        }

        private static void HandleHierarchyWindowItemOnGUI(int _instanceID, Rect _selectionRect)
        {
            ClearVariables();

            if (!IsControlAltClick()) return;

            currentGameObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
            if (currentGameObject == null) return;
            if (!_selectionRect.Contains(currentEvent.mousePosition)) return;

            popupOffset = new Vector2(_selectionRect.position.x + _selectionRect.size.x + CHConstants.OBJECT_POPUP_OFFSET_X, _selectionRect.position.y - CHConstants.OBJECT_POPUP_OFFSET_Y);
            popupRect = new Rect(popupOffset, _selectionRect.size);
            PopupWindow.Show(popupRect, new CHObjectPopup(currentGameObject));
            currentEvent.Use();
        }

        private static bool IsControlAltClick()
        {
            currentEvent = Event.current;

            if (currentEvent == null) return false;
            if (currentEvent.type != EventType.MouseDown) return false;
            if (currentEvent.button != 0) return false;
            if (!currentEvent.alt) return false;
            if (!currentEvent.control) return false;

            return true;
        }

        private static void OnEditorQuitting()
        {
            settings.CheckElementData();
            settings.SaveObjectData();
        }

        private static void ClearVariables()
        {
            currentEvent = null;
            currentGameObject = null;
        }
    }
}