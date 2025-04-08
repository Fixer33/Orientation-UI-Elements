#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrientationElements
{
    public static class OrientationElementsManager
    {
        public static event Action<Orientation, List<Behaviour>> LoadOrientationDataRequested;
        public static event Action<Orientation> SaveOrientationDataRequested;

#if UNITY_EDITOR
        public static bool ConditionsAreMet
        {
            get
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                bool isInPrefabMode = prefabStage;
                if (isInPrefabMode == false)
                    return false;
                
                bool hasCanvas = prefabStage.prefabContentsRoot.GetComponentInChildren<Canvas>() != null;
                return hasCanvas;
            }
        }
        
        public static void RequestOrientationLoad(Orientation orientation)
        {
            List<Behaviour> failedBehaviours = new();
            LoadOrientationDataRequested?.Invoke(orientation, failedBehaviours);

            if (failedBehaviours.Count > 0)
            {
                EditorUtility.DisplayDialog("Failed load behaviours detected", 
                    $"Failed to load {failedBehaviours.Count} elements. See console for more info", "ok");
                foreach (var failedBehaviour in failedBehaviours)
                {
                    Debug.LogError($"Failed to load {failedBehaviour.name} behaviour on object {failedBehaviour.gameObject.name}");
                }
            }
        }

        public static void RequestOrientationSave(Orientation orientation) =>
            SaveOrientationDataRequested?.Invoke(orientation);
#endif
    }
}