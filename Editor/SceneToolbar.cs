using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrientationElements.Editor
{
    [Overlay(typeof(SceneView), "Orientation Elements tools", true)]
    internal class SceneToolbar : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui += OnSceneViewGUI;
            
            var container = new VisualElement();
            container.styleSheets.Add(Utilities.LoadAssetAtPath<StyleSheet>("Packages/com.fixer33.orientation-ui-elements/Editor/Styles/SceneToolbar.uss"));

            CreateScreenSizeElements(container, "Landscape", "LandscapeSize", Orientation.Landscape);
            CreateScreenSizeElements(container, "Portrait", "PortraitSize", Orientation.Portrait);

            return container;
        }

        public override void OnWillBeDestroyed()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
            base.OnWillBeDestroyed();
        }

        private void CreateScreenSizeElements(VisualElement rootElement, string headerText, string saveKey, Orientation orientation)
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
                text = "Set game view resolution"
            };

            setBtn.clicked += async () =>
            {
                if (GameViewUtils.SizeExists(GameViewSizeGroupType.Standalone, size.x, size.y) == false)
                {
                    EditorUtility.DisplayDialog("No size detected",
                        "Please, first add the resolution to game view resolution templates", "ok");
                    return;
                }

                await SetGameViewSize(size);
            };

            container.Add(setBtn);

            VisualElement manipulationButtonsContainer = new VisualElement()
            {
                name = "ManipulationButtons"
            };
            container.Add(manipulationButtonsContainer);

            Button loadBtn = new Button()
            {
                name = "LoadOrientationBtn",
                text = "Load"
            };
            loadBtn.clicked += async () =>
            {
                await SetGameViewSize(size);
                OrientationElementsManager.RequestOrientationLoad(orientation);
            };
            manipulationButtonsContainer.Add(loadBtn);
            
            Button saveBtn = new Button()
            {
                name = "SaveOrientationBtn",
                text = "Save"
            };
            saveBtn.clicked += () => OrientationElementsManager.RequestOrientationSave(orientation);
            manipulationButtonsContainer.Add(saveBtn);
        }

        private static async Task SetGameViewSize(Vector2Int size)
        {
            GameViewUtils.SetSize(GameViewUtils.FindSize(GameViewSizeGroupType.Standalone, size.x, size.y));
            await Task.Delay(100);
            SwitchToSceneView();
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
                    return;
                case (true, DisplayStyle.None):
                    rootVisualElement.style.display = DisplayStyle.Flex;
                    break;
                case (true, DisplayStyle.Flex):
                    return;
            }
        }
    }
}