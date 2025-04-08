using System.Collections.Generic;
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

        private void Update()
        {
            var orientation = Screen.width > Screen.height ? Orientation.Landscape : Orientation.Portrait;
            
            _cachedVertical ??= new TT();
            _cachedHorizontal ??= new TT();
            if (Application.isPlaying)
            {
                UpdatePlaying(orientation);
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            OrientationElementsManager.LoadOrientationDataRequested -= LoadOrientation;
            OrientationElementsManager.LoadOrientationDataRequested += LoadOrientation;
            
            OrientationElementsManager.SaveOrientationDataRequested -= SaveOrientation;
            OrientationElementsManager.SaveOrientationDataRequested += SaveOrientation;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            OrientationElementsManager.LoadOrientationDataRequested -= LoadOrientation;
            OrientationElementsManager.SaveOrientationDataRequested -= SaveOrientation;
#endif
        }

#if UNITY_EDITOR
        private void SaveOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Portrait)
                TrySaveOrientation(ref _hasVerticalCache, ref _cachedVertical);
            else
                TrySaveOrientation(ref _hasHorizontalCache, ref _cachedHorizontal);
            
            void TrySaveOrientation(ref bool hasCache, ref TT cache)
            {
                cache = GetCacheForCurrentElement(Element);
                hasCache = true;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        private void LoadOrientation(Orientation orientation, List<Behaviour> behaviours)
        {
            if (orientation == Orientation.Portrait)
                TryLoadOrientation(ref _hasVerticalCache, ref _cachedVertical);
            else
                TryLoadOrientation(ref _hasHorizontalCache, ref _cachedHorizontal);
            
            void TryLoadOrientation(ref bool hasCache, ref TT cache)
            {
                if (hasCache == false)
                {
                    behaviours.Add(this);
                    return;
                }

                WriteCacheToElement(Element, cache);
            }
        }
#endif

        private void UpdatePlaying(Orientation orientation)
        {
            if (orientation == Orientation.Portrait)
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