using System;
using System.Threading.Tasks;
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
        public static bool IsRecording
        {
            get => _isRecording;
            set
            {
                _isRecording = value;
                OrientationElementsManager.IsRecording = value;
                Updated?.Invoke();
            }
        }
        private static bool _isRecording;
        private static event Action Updated;
        
        private const string RECORDING_BTN_RECORDING_CLASS_NAME = "isRecording";

        private Button _recordingBtn;
        
        public override VisualElement CreatePanelContent()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui += OnSceneViewGUI;
            
            Updated += OnUpdated;
            
            var container = new VisualElement();
            container.styleSheets.Add(Utilities.LoadAssetAtPath<StyleSheet>("Packages/com.fixer33.orientation-ui-elements/Editor/Styles/SceneToolbar.uss"));

            CreateScreenSizeElements(container, "Album", "AlbumSize");
            CreateScreenSizeElements(container, "Portrait", "PortraitSize");

            _recordingBtn = new Button()
            {
                name = "RecordingBtn",
                text = ""
            };
            _recordingBtn.clicked += () =>
            {
                IsRecording = IsRecording == false;
            };
            container.Add(_recordingBtn);

            IsRecording = false;
            return container;
        }

        public override void OnWillBeDestroyed()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
            Updated -= OnUpdated;
            base.OnWillBeDestroyed();
        }

        private void CreateScreenSizeElements(VisualElement rootElement, string headerText, string saveKey)
        {
            VisualElement container = new VisualElement()
            {
                name = "ScreenSizeElement"
            };
            rootElement.Add(container);

            container.Add(new Label()
            {
                name = "Header",
                text = headerText
            });

            Vector2IntField field = new Vector2IntField()
            {
                name = "InputField"
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
                name = "SetOrientationBtn",
                text = "Set"
            };

            setBtn.clicked += async () =>
            {
                if (GameViewUtils.SizeExists(GameViewSizeGroupType.Standalone, size.x, size.y) == false)
                {
                    EditorUtility.DisplayDialog("No size detected",
                        "Please, first add the resolution to game view resolution templates", "ok");
                    return;
                }

                IsRecording = false;
                GameViewUtils.SetSize(GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, size.x, size.y));
                await Task.Delay(300);
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
                Debug.LogWarning("No Scene View available.");
            }
        }

        private void OnSceneViewGUI(SceneView sceneView)
        {
            var display = rootVisualElement.style.display.value;

            switch (OrientationElementsManager.ConditionsAreMet, display)
            {
                case (false, DisplayStyle.None):
                    return;
                case (false, DisplayStyle.Flex):
                    rootVisualElement.style.display = DisplayStyle.None;
                    IsRecording = false;
                    return;
                case (true, DisplayStyle.None):
                    rootVisualElement.style.display = DisplayStyle.Flex;
                    break;
                case (true, DisplayStyle.Flex):
                    return;
            }
        }

        private void OnUpdated()
        {
            if (IsRecording)
                _recordingBtn.AddToClassList(RECORDING_BTN_RECORDING_CLASS_NAME);
            else 
                _recordingBtn.RemoveFromClassList(RECORDING_BTN_RECORDING_CLASS_NAME);
        }
    }
}