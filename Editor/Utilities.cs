using UnityEditor;
using UnityEngine;

namespace OrientationElements.Editor
{
    public static class Utilities
    {
        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
#if PACKAGES_DEV
            path = "Assets/" + path;
#endif
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}