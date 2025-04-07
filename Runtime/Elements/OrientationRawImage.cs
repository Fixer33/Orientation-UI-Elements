using System;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace OrientationElements.Elements
{
    [RequireComponent(typeof(RawImage))]
    public class OrientationRawImage : OrientationElementBase<RawImage, OrientationRawImage.RawImageCache>
    {
        [Serializable]
        public struct RawImageCache
        {
            public Texture Texture;
            public Color Color;
            public Material Material;
            public bool RaycastTarget;
            public Vector4 RaycastPadding;
            public Rect UVRect;
            public bool Maskable;
        }

        protected override RawImageCache GetCacheForCurrentElement(RawImage element)
        {
            return new RawImageCache()
            {
                Texture = element.texture,
                Color = element.color,
                Material = element.material,
                RaycastTarget = element.raycastTarget,
                RaycastPadding = element.raycastPadding,
                UVRect = element.uvRect,
                Maskable = element.maskable
            };
        }

        protected override void WriteCacheToElement(RawImage element, RawImageCache cache)
        {
            element.texture = cache.Texture;
            element.color = cache.Color;
            element.material = cache.Material;
            element.raycastTarget = cache.RaycastTarget;
            element.raycastPadding = cache.RaycastPadding;
            element.uvRect = cache.UVRect;
            element.maskable = cache.Maskable;
        }

        protected override bool IsElementDataEqualTo(RawImage element, RawImageCache data)
        {
            bool isEqual = element.texture == data.Texture;
            isEqual = isEqual && element.color == data.Color;
            isEqual = isEqual && element.material == data.Material;
            isEqual = isEqual && element.raycastTarget == data.RaycastTarget;
            isEqual = isEqual && element.raycastPadding == data.RaycastPadding;
            isEqual = isEqual && element.uvRect == data.UVRect;
            isEqual = isEqual && element.maskable == data.Maskable;
            return isEqual;
        }
    }
}