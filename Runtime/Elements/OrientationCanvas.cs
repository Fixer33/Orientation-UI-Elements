using System;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace OrientationElements.Elements
{
    [RequireComponent(typeof(CanvasScaler))]
    public class OrientationCanvas : OrientationElementBase<CanvasScaler, OrientationCanvas.CanvasCache>
    {
        [Serializable]
        public struct CanvasCache
        {
            public CanvasScaler.ScaleMode ScaleMode;
            public CanvasScaler.ScreenMatchMode ScreenMatchMode;
            public float ReferencePixelPerUnit;
            public float MatchWidthOrHeight;
            public float ScaleFactor;
            public float FallbackScreenDPI, DefaultSpriteDPI;
            public Vector2 ReferenceResolution;
            public CanvasScaler.Unit PhysicalUnit;
        }

        protected override CanvasCache GetCacheForCurrentElement(CanvasScaler element)
        {
            return new CanvasCache()
            {
                ScaleMode = element.uiScaleMode,
                ScreenMatchMode = element.screenMatchMode,
                ReferencePixelPerUnit = element.referencePixelsPerUnit,
                MatchWidthOrHeight = element.matchWidthOrHeight,
                ScaleFactor = element.scaleFactor,
                FallbackScreenDPI = element.fallbackScreenDPI,
                DefaultSpriteDPI = element.defaultSpriteDPI,
                ReferenceResolution = element.referenceResolution,
                PhysicalUnit = element.physicalUnit
            };
        }

        protected override void WriteCacheToElement(CanvasScaler element, CanvasCache cache)
        {
            element.uiScaleMode = cache.ScaleMode;
            element.screenMatchMode = cache.ScreenMatchMode;
            element.referencePixelsPerUnit = cache.ReferencePixelPerUnit;
            element.matchWidthOrHeight = cache.MatchWidthOrHeight;
            element.scaleFactor = cache.ScaleFactor;
            element.fallbackScreenDPI = cache.FallbackScreenDPI;
            element.defaultSpriteDPI = cache.DefaultSpriteDPI;
            element.referenceResolution = cache.ReferenceResolution;
            element.physicalUnit = cache.PhysicalUnit;
        }

        protected override bool IsElementDataEqualTo(CanvasScaler element, CanvasCache data)
        {
            bool isEqual = element.uiScaleMode == data.ScaleMode;
            isEqual = isEqual && element.screenMatchMode == data.ScreenMatchMode;
            isEqual = isEqual && element.referencePixelsPerUnit == data.ReferencePixelPerUnit;
            isEqual = isEqual && element.matchWidthOrHeight == data.MatchWidthOrHeight;
            isEqual = isEqual && element.scaleFactor == data.ScaleFactor;
            isEqual = isEqual && element.fallbackScreenDPI == data.FallbackScreenDPI;
            isEqual = isEqual && element.defaultSpriteDPI == data.DefaultSpriteDPI;
            isEqual = isEqual && element.referenceResolution == data.ReferenceResolution;
            isEqual = isEqual && element.physicalUnit == data.PhysicalUnit;
            return isEqual;
        }
    }
}