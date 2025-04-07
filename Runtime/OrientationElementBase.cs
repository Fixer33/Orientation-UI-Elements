using UnityEditor;
using UnityEngine;

namespace OrientationElements
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public abstract class OrientationElementBase<T, TT> : MonoBehaviour, IOrientationElement 
        where T : Component where TT : new()
    {
        public T Element
        {
            get
            {
                if (_element == false)
                    _element = GetComponent<T>();
                return _element;
            }
        }

        [SerializeField] private TT _cachedVertical, _cachedHorizontal;
        [SerializeField, HideInInspector] private bool _hasVerticalCache, _hasHorizontalCache;
        private T _element;
        private ScreenOrientation? _lastOrientation;

        private void Update()
        {
            var orientation = Screen.width > Screen.height? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
            
            _cachedVertical ??= new TT();
            _cachedHorizontal ??= new TT();
            if (Application.isPlaying)
            {
                UpdatePlaying(orientation);
            }
            else
            {
                UpdateEditor(orientation);
            }
        }

        private void UpdateEditor(ScreenOrientation orientation)
        {
#if UNITY_EDITOR
            if (OrientationElementsManager.ConditionsAreMet == false)
            {
                WriteCacheToElement(Element,
                    orientation == ScreenOrientation.Portrait ? _cachedVertical : _cachedHorizontal);
                return;
            }

            if (_lastOrientation.HasValue == false)
            {
                _lastOrientation = orientation;
                return;
            }

            if (orientation == ScreenOrientation.Portrait)
                HandleEditorFor(ref _hasVerticalCache, ref _cachedVertical, Element);
            else
                HandleEditorFor(ref _hasHorizontalCache, ref _cachedHorizontal, Element);
            
            _lastOrientation = orientation;

            void HandleEditorFor(ref bool hasCache, ref TT cache, T element)
            {
                switch (_lastOrientation.Value == orientation, hasCache)
                {
                    case (false, true):
                        WriteCacheToElement(element, cache);
                        Debug.Log($"[{gameObject.name}] Writing data into element for {_lastOrientation.Value}");
                        break;
                    case (false, false):
                    case (true, false):
                    case (true, true):
                        if (OrientationElementsManager.IsRecording == false)
                            return;
                        
                        if (IsElementDataEqualTo(Element, cache))
                            return;
                        
                        cache = GetCacheForCurrentElement(element);
                        hasCache = true;
                        EditorUtility.SetDirty(this);
                        Debug.Log($"[{gameObject.name}] Writing data to cache for {_lastOrientation.Value}");
                        break;
                }
            }
#endif
        }

        private void UpdatePlaying(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Portrait)
            {
                if (IsElementDataEqualTo(Element, _cachedVertical))
                    return;
                        
                WriteCacheToElement(Element, _cachedVertical);
            }
            else
            {
                if (IsElementDataEqualTo(Element, _cachedHorizontal))
                    return;
                        
                WriteCacheToElement(Element, _cachedHorizontal);
            }
        }

        protected abstract TT GetCacheForCurrentElement(T element);
        protected abstract void WriteCacheToElement(T element, TT cache);
        protected abstract bool IsElementDataEqualTo(T element, TT data);
        
        protected virtual void OnValidateLocal(){}

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnValidateLocal();
            
            if (GetComponent<RectTransform>() != false) 
                return;
            
            Debug.LogError($"[{gameObject.name}] Orientation elements only work on UI elements");
            this.enabled = false;
        }
#endif
    }
}