using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrientationElements.Editor
{
    [Overlay(typeof(SceneView), "Orientation Elements tools", true)]
    internal class SceneToolbar : Overlay
    {
        public static bool IsRecording { get; private set; }
        
        private const string RECORDING_BTN_RECORDING_CLASS_NAME = "isRecording";
        
        public override VisualElement CreatePanelContent()
        {
            SceneView.windowFocusChanged += SceneViewOnWindowFocusChanged;
            
            Debug.Log("AAA");
            var container = new VisualElement();
            container.styleSheets.Add(Utilities.LoadAssetAtPath<StyleSheet>("Packages/com.fixer33.orientation-ui-elements/Editor/Styles/SceneToolbar.uss"));

            CreateScreenSizeElements(container, "Album", "AlbumSize");
            CreateScreenSizeElements(container, "Portrait", "PortraitSize");

            Button recordingBtn = new Button()
            {
                name = "RecordingBtn",
                text = ""
            };
            recordingBtn.clicked += () =>
            {
                IsRecording = IsRecording == false;
                if (IsRecording)
                    recordingBtn.AddToClassList(RECORDING_BTN_RECORDING_CLASS_NAME);
                else 
                    recordingBtn.RemoveFromClassList(RECORDING_BTN_RECORDING_CLASS_NAME);
            };
            container.Add(recordingBtn);

            return container;
        }

        public override void OnWillBeDestroyed()
        {
            SceneView.windowFocusChanged -= SceneViewOnWindowFocusChanged;
            base.OnWillBeDestroyed();
        }

        private void CreateScreenSizeElements(VisualElement rootElement, string headerText, string saveKey)
        {
            VisualElement container = new VisualElement()
            {
                name = "Screen size element"
            };
            rootElement.Add(container);

            container.Add(new Label()
            {
                name = "Header",
                text = headerText
            });

            Vector2IntField field = new Vector2IntField()
            {
                name = "Input field"
            };

            Vector2Int size = new Vector2Int(EditorPrefs.GetInt(saveKey + "_x", 1440),
                EditorPrefs.GetInt(saveKey + "_y", 900));
            field.value = size;
            field.RegisterValueChangedCallback(data =>
            {
                size = data.newValue;
                EditorPrefs.SetInt(saveKey + "_x", size.x);
                EditorPrefs.SetInt(saveKey + "_y", size.y);
            });

            container.Add(field);

            Button setBtn = new Button()
            {
                name = "Set orientation btn",
                text = "Set"
            };

            setBtn.clicked += () =>
            {
                if (GameViewUtils.SizeExists(GameViewSizeGroupType.Standalone, size.x, size.y) == false)
                {
                    EditorUtility.DisplayDialog("No size detected",
                        "Please, first add the resolution to game view resolution templates", "ok");
                    return;
                }

                GameViewUtils.SetSize(GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, size.x, size.y));
                SwitchToSceneView();
            };

            container.Add(setBtn);
        }

        private static void SwitchToSceneView()
        {
            // Get the last active SceneView or create one if none exists
            SceneView sceneView = SceneView.lastActiveSceneView ?? SceneView.CreateInstance<SceneView>();

            if (sceneView != null)
            {
                sceneView.Focus(); // Brings Scene View into focus
            }
            else
            {
                UnityEngine.Debug.LogWarning("No Scene View available.");
            }
        }

        private void SceneViewOnWindowFocusChanged()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            bool isInPrefabMode = prefabStage;
            var display = rootVisualElement.style.display.value;

            switch (isInPrefabMode, display)
            {
                case (false, DisplayStyle.None):
                    return;
                case (false, DisplayStyle.Flex):
                    rootVisualElement.style.display = DisplayStyle.None;
                    return;
            }
            
            bool hasCanvas = prefabStage.prefabContentsRoot.GetComponentInChildren<Canvas>() != null;
            Debug.Log(hasCanvas);
            switch (isInPrefabMode, display, hasCanvas)
            {
                case (true, DisplayStyle.None, false):
                    return;
                case (true, DisplayStyle.Flex, false):
                    rootVisualElement.style.display = DisplayStyle.None;
                    return;
                case (true, DisplayStyle.None, true):
                    rootVisualElement.style.display = DisplayStyle.Flex;
                    return;
                case (true, DisplayStyle.Flex, true):
                    return;
            }
        }
    }
}