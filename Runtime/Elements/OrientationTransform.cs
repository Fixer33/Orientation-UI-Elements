using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace OrientationElements.Elements
{
    public class OrientationTransform : OrientationElementBase<RectTransform, OrientationTransform.RectTransformCache>
    {
        [Serializable]
        public struct RectTransformCache
        {
            public Vector3 Rotation, Scale;
            public Vector2 AnchorMin, AnchorMax;
            public Vector2 AnchoredPosition;
            public Vector2 SizeDelta;
            public Vector2 Pivot;
        }

        [SerializeField] private bool _ignoreScale;
        [SerializeField] private bool _ignoreRotation;
        [SerializeField] private bool _ignoreAnchoredPosition;

        protected override RectTransformCache GetCacheForCurrentElement(RectTransform element)
        {
            RectTransformCache cache = new()
            {
                Pivot = element.pivot,
                AnchorMin = element.anchorMin,
                AnchorMax = element.anchorMax,
                AnchoredPosition = element.anchoredPosition,
                SizeDelta = element.sizeDelta,
                Scale = element.localScale,
                Rotation = element.rotation.eulerAngles
            };
            return cache;
        }

        protected override void WriteCacheToElement(RectTransform element, RectTransformCache cache)
        {
            element.pivot = cache.Pivot;
            element.anchorMin = cache.AnchorMin;
            element.anchorMax = cache.AnchorMax;
            if (_ignoreAnchoredPosition == false)
                element.anchoredPosition = cache.AnchoredPosition;
            element.sizeDelta = cache.SizeDelta;
            if (_ignoreScale == false)
                element.localScale = cache.Scale;
            if (_ignoreRotation == false)
                element.rotation = Quaternion.Euler(cache.Rotation);
        }

        protected override bool IsElementDataEqualTo(RectTransform element, RectTransformCache data)
        {
            bool isEqual = element.pivot == data.Pivot;
            isEqual = isEqual && element.anchorMin == data.AnchorMin;
            isEqual = isEqual && element.anchorMax == data.AnchorMax;
            if (_ignoreAnchoredPosition == false)
                isEqual = isEqual && element.anchoredPosition == data.AnchoredPosition;
            isEqual = isEqual && element.sizeDelta == data.SizeDelta;
            if (_ignoreScale == false)
                isEqual = isEqual && element.localScale == data.Scale;
            if (_ignoreRotation == false)
                isEqual = isEqual && element.rotation == Quaternion.Euler(data.Rotation);
            return isEqual;
        }
    }
}