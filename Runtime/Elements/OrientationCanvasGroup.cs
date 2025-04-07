using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace OrientationElements.Elements
{
    public class OrientationCanvasGroup : OrientationElementBase<CanvasGroup, OrientationCanvasGroup.CanvasGroupCache>
    {
        [Serializable]
        public struct CanvasGroupCache
        {
            public float Alpha;
            public bool IgnoreParentGroups;
            public bool BlockRaycasts;
            public bool Interactable;
        }

        protected override CanvasGroupCache GetCacheForCurrentElement(CanvasGroup element) => new CanvasGroupCache()
        {
            Alpha = element.alpha,
            BlockRaycasts = element.blocksRaycasts,
            IgnoreParentGroups = element.ignoreParentGroups,
            Interactable = element.interactable,
        };

        protected override void WriteCacheToElement(CanvasGroup element, CanvasGroupCache cache)
        {
            element.alpha = cache.Alpha;
            element.blocksRaycasts = cache.BlockRaycasts;
            element.ignoreParentGroups = cache.IgnoreParentGroups;
            element.interactable = cache.Interactable;
        }

        protected override bool IsElementDataEqualTo(CanvasGroup element, CanvasGroupCache data)
        {
            bool isEqual = true;
            isEqual = isEqual && element.alpha == data.Alpha;
            isEqual = isEqual && element.blocksRaycasts == data.BlockRaycasts;
            isEqual = isEqual && element.ignoreParentGroups == data.IgnoreParentGroups;
            isEqual = isEqual && element.interactable == data.Interactable;
            return isEqual;
        }
    }
}