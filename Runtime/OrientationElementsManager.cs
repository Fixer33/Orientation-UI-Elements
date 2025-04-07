using UnityEditor.SceneManagement;
using UnityEngine;

namespace OrientationElements
{
    public static class OrientationElementsManager
    {
        public static bool IsRecording;

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
    }
}