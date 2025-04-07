using System;
using TMPro;
using UnityEngine;
using FontStyles = TMPro.FontStyles;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace OrientationElements.Elements
{
    public class OrientationTextBasic : OrientationElementBase<TMP_Text, OrientationTextBasic.TextBasicCache>
    {
        [Serializable]
        public record TextBasicCache
        {
            public string Text;
            public float FontSize;
            public bool EnableAutoSize;
            public float FontMinSize;
            public float FontMaxSize;
            public Vector4 Margin;
            public TextAlignmentOptions AlignmentOptions;
            public FontStyles FontStyles;
        }

        [SerializeField] private bool _ignoreText;

        protected override TextBasicCache GetCacheForCurrentElement(TMP_Text element) => new TextBasicCache()
        {
            Text = element.text,
            FontSize = element.fontSize,
            EnableAutoSize = element.enableAutoSizing,
            FontMinSize = element.fontSizeMin,
            FontMaxSize = element.fontSizeMax,
            Margin = element.margin,
            AlignmentOptions = element.alignment,
            FontStyles = element.fontStyle,
        };

        protected override void WriteCacheToElement(TMP_Text element, TextBasicCache cache)
        {
            if (_ignoreText == false)
                element.text = cache.Text;
            element.fontSize = cache.FontSize;
            element.enableAutoSizing = cache.EnableAutoSize;
            element.fontSizeMin = cache.FontMinSize;
            element.fontSizeMax = cache.FontMaxSize;
            element.margin = cache.Margin;
            element.alignment = cache.AlignmentOptions;
            element.fontStyle = cache.FontStyles;
        }

        protected override bool IsElementDataEqualTo(TMP_Text element, TextBasicCache data)
        {
            bool isEqual = true;
            if (_ignoreText == false)
                isEqual = element.text == data.Text;
            isEqual = isEqual && data.FontSize == element.fontSize;
            isEqual = isEqual && data.EnableAutoSize == element.enableAutoSizing;
            isEqual = isEqual && data.FontMinSize == element.fontSizeMin;
            isEqual = isEqual && data.FontMaxSize == element.fontSizeMax;
            isEqual = isEqual && data.Margin == element.margin;
            isEqual = isEqual && data.AlignmentOptions == element.alignment;
            isEqual = isEqual && data.FontStyles == element.fontStyle;
            return isEqual;
        }
    }
}