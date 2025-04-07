using System;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace OrientationElements.Elements
{
    [RequireComponent(typeof(Image))]
    public class OrientationImage : OrientationElementBase<Image, OrientationImage.ImageCache>
    {
        [Serializable]
        public struct ImageCache
        {
            public Sprite Sprite;
            public Color Color;
            public Material Material;
            public bool RaycastTarget;
            public Vector4 RaycastPadding;
            public bool Maskable;
            public bool FillCenter;
            public float PixelsPerUnitMultiplier;
        }

        protected override ImageCache GetCacheForCurrentElement(Image element)
        {
            return new ImageCache()
            {
                Sprite = element.sprite,
                Color = element.color,
                Material = element.material,
                RaycastTarget = element.raycastTarget,
                RaycastPadding = element.raycastPadding,
                Maskable = element.maskable,
                FillCenter = element.fillCenter,
                PixelsPerUnitMultiplier = element.pixelsPerUnitMultiplier,
            };
        }

        protected override void WriteCacheToElement(Image element, ImageCache cache)
        {
            element.sprite = cache.Sprite;
            element.color = cache.Color;
            element.material = cache.Material;
            element.raycastTarget = cache.RaycastTarget;
            element.raycastPadding = cache.RaycastPadding;
            element.maskable = cache.Maskable;
            element.fillCenter = cache.FillCenter;
            element.pixelsPerUnitMultiplier = cache.PixelsPerUnitMultiplier;
        }

        protected override bool IsElementDataEqualTo(Image element, ImageCache data)
        {
            bool isEqual = element.sprite == data.Sprite;
            isEqual = isEqual && element.color == data.Color;
            isEqual = isEqual && element.material == data.Material;
            isEqual = isEqual && element.raycastTarget == data.RaycastTarget;
            isEqual = isEqual && element.raycastPadding == data.RaycastPadding;
            isEqual = isEqual && element.maskable == data.Maskable;
            isEqual = isEqual && element.fillCenter == data.FillCenter;
            isEqual = isEqual && element.pixelsPerUnitMultiplier == data.PixelsPerUnitMultiplier;
            return isEqual;
        }
    }
}