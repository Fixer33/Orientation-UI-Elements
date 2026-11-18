# Orientation UI Elements - Usage Guide

Package `com.fixer33.orientation-ui-elements`, version 1.0.0-alpha, Unity 6000.0+. The package is alpha ("you probably going to encounter some bugs", per its README).

Statements are read from the package code unless marked (assumed).

## 1. Installation

### Via com.fixer33.package-installer

1. Install `com.fixer33.package-installer` (see its own README) and open **Tools > Fixer33 > Package Installer**.
2. Find **Orientation UI Elements** in the "UI Extensions" group, tick it and install.

The installer entry is:

- PackageId: `com.fixer33.orientation-ui-elements`
- PackageUrl: `https://github.com/Fixer33/Orientation-UI-Elements.git`

### Manually

**Window > Package Manager > + > Add package from git URL**, then enter `https://github.com/Fixer33/Orientation-UI-Elements.git`.

`package.json` declares no dependencies. The runtime assembly references `Unity.TextMeshPro` and uses uGUI types (`Image`, `RawImage`, `CanvasScaler`), so the project needs TextMeshPro and uGUI (normally included with Unity 6, assumed).

## 2. Minimal setup

1. Create or open a UI prefab (double-click it to enter prefab mode). The prefab must contain a `Canvas`.
2. On each UI object whose look differs between orientations, add the matching Orientation component (section 3).
3. The scene-view overlay **Orientation Elements tools** appears only in prefab mode with a Canvas in the prefab.
4. Arrange the prefab for one orientation, press **Save** in that orientation's block. Arrange it for the other, press **Save** again.
5. Save the prefab. At runtime the components apply the saved layout that matches the screen shape. No code is required.

## 3. Components and public API

Add Component by class name (no `[AddComponentMenu]` is defined, so there is no special submenu; assumed). All live in namespace `OrientationElements.Elements`.

| Component | Wraps | Notes |
|---|---|---|
| `OrientationCanvas` | `CanvasScaler` | scale mode, reference resolution, match, etc. |
| `OrientationCanvasGroup` | `CanvasGroup` | alpha, interactable, blocks raycasts, ignore parent groups |
| `OrientationImage` | `Image` | sprite, color, material, raycast settings, maskable, fill center, pixels-per-unit multiplier |
| `OrientationRawImage` | `RawImage` | texture, color, material, raycast settings, UV rect, maskable |
| `OrientationTextBasic` | `TMP_Text` | text, size, auto size, margin, alignment, font style. Option `_ignoreText` leaves the text untouched |
| `OrientationTransform` | `RectTransform` | anchors, pivot, anchored position, size delta, scale, rotation. Options `_ignoreScale`, `_ignoreRotation`, `_ignoreAnchoredPosition` |

`OrientationImage`, `OrientationRawImage` and `OrientationCanvas` carry `[RequireComponent]` for their wrapped component. All derive from `OrientationElementBase<T, TT>` (namespace `OrientationElements`), which implements the marker interface `IOrientationElement`.

Public surface of the base class:

```csharp
using OrientationElements;
using OrientationElements.Elements;
using UnityEngine.UI;

var orientationImage = GetComponent<OrientationImage>();
Image image = orientationImage.Element;   // the wrapped component (lazy GetComponent<T>())
```

`Orientation` enum: `Portrait`, `Landscape`.

`OrientationElementsManager` (static class):

```csharp
// Both events exist in builds too, but they are only raised from editor code.
OrientationElementsManager.SaveOrientationDataRequested += o => Debug.Log($"Save {o}");
OrientationElementsManager.LoadOrientationDataRequested += (o, failed) => Debug.Log($"Load {o}");

#if UNITY_EDITOR
bool inPrefabWithCanvas = OrientationElementsManager.ConditionsAreMet;
OrientationElementsManager.RequestOrientationSave(Orientation.Portrait);
OrientationElementsManager.RequestOrientationLoad(Orientation.Landscape);
#endif
```

`ConditionsAreMet`, `RequestOrientationSave` and `RequestOrientationLoad` exist only under `UNITY_EDITOR`.

Runtime behaviour: every `Update`, each element reads `Screen.width > Screen.height`. True means `Landscape`, otherwise `Portrait` (a square screen is portrait). While `Application.isPlaying`, if the wrapped component differs from the matching snapshot, the snapshot is written to it. Nothing is called by user code.

## 4. Editor windows and menu paths

- **Scene view overlay "Orientation Elements tools"** (enable it from the Scene view overlay menu if hidden). It has a **Landscape** block and a **Portrait** block, each with:
  - A width/height field (stored in `EditorPrefs` as `LandscapeSize_x/_y` and `PortraitSize_x/_y`, default 1440 x 900 for both).
  - **Set game view resolution**: selects that size in the Game view. The size must already exist in the Game view size list, otherwise a dialog asks you to add it first.
  - **Load**: sets the Game view size, focuses the Scene view, then writes the saved snapshot for that orientation to every enabled Orientation component.
  - **Save**: copies the current state of each enabled Orientation component into its snapshot for that orientation.
- The overlay is hidden unless a prefab is open in prefab mode and contains a `Canvas`.
- `OrientationElements.Editor.GameViewUtils` is public (reflection helpers for Game view sizes, for example `SizeExists`, `FindSize`, `SetSize`, `AddCustomSize`, `AddAndSelectCustomSize`, `GetCurrentGroupType`).
- It also registers seven debug menu items under a top-level **Test** menu: `Test/AddSize`, `Test/SizeTextQuery`, `Test/Query16:9Test`, `Test/Set16:9`, `Test/SetTestSize`, `Test/SizeDimensionsQuery`, `Test/LogCurrentGroupType`. They look like leftover debug helpers (assumed); you can ignore them.

## 5. Configuration and scripting defines

- There is no settings asset and no required scripting define.
- Per-component options: `_ignoreText` (`OrientationTextBasic`), `_ignoreScale`, `_ignoreRotation`, `_ignoreAnchoredPosition` (`OrientationTransform`), set in the inspector.
- Overlay resolutions are stored per machine in `EditorPrefs` (see section 4).
- `UNITY_EDITOR` guards the editor-only members in runtime files. No `versionDefines` or `defineConstraints` are used.

## 6. Extension points and worked example

To support another component, derive from `OrientationElementBase<T, TT>` where `T : Component` and `TT` is a `[Serializable]` type with a parameterless constructor. Implement the three abstract methods. `OnValidateLocal()` is an optional virtual hook. Example, adapted from `OrientationImage`, for `Toggle`:

```csharp
using System;
using OrientationElements;
using UnityEngine.UI;

public class OrientationToggle : OrientationElementBase<Toggle, OrientationToggle.ToggleCache>
{
    [Serializable]
    public struct ToggleCache
    {
        public bool Interactable;
        public bool IsOn;
    }

    protected override ToggleCache GetCacheForCurrentElement(Toggle element) =>
        new ToggleCache { Interactable = element.interactable, IsOn = element.isOn };

    protected override void WriteCacheToElement(Toggle element, ToggleCache cache)
    {
        element.interactable = cache.Interactable;
        element.isOn = cache.IsOn;
    }

    protected override bool IsElementDataEqualTo(Toggle element, ToggleCache data) =>
        element.interactable == data.Interactable && element.isOn == data.IsOn;
}
```

Add it to a `Toggle` object in a prefab: Save/Load from the overlay will include it automatically, because the base class subscribes to `OrientationElementsManager` events in `OnEnable` (editor only). Note: there is no `[RequireComponent]` here, add it yourself if desired.

## 7. Common pitfalls (current behaviour)

- **Unsaved orientations are still applied at runtime.** `Update` creates default snapshots (`??= new TT()`) and `UpdatePlaying` writes them without checking the `_hasVerticalCache`/`_hasHorizontalCache` flags. If you never pressed Save for one orientation, that orientation gets default values (for `OrientationImage` a null sprite and transparent color; for `OrientationTextBasic` empty/zero values, which can blank the text unless `_ignoreText` is on). Always Save both orientations for every component.
- **Load reports only in the editor.** If an orientation was never saved, Load logs an error and shows a "Failed load behaviours detected" dialog for those components.
- **Load does not check that the resolution exists in the Game view list.** Add the size to the Game view size list first (the "Set game view resolution" button does check, Load does not).
- **Game view group is always Standalone.** The overlay uses `GameViewSizeGroupType.Standalone` regardless of the active build target.
- **Prefab-only toolbar, but Save/Load reach everything.** The overlay shows only in prefab mode with a Canvas, yet the events affect every enabled Orientation component in the editor (including scene objects).
- **Runtime overwrites changes.** Each frame the matching snapshot is enforced. Properties covered by a component (including text, unless `_ignoreText`) cannot be changed from other scripts without being reverted. Use the ignore flags where available (for example localised text).
- **Limited property coverage.** For instance `OrientationImage` does not store `Image.type`, fill settings or preserve aspect, and `OrientationTextBasic` does not store font asset, color or wrapping.
- **RectTransform required.** `OnValidate` logs an error and disables the component if the GameObject has no `RectTransform`.
- **Per-frame cost.** Every element compares all its properties in `Update`; performance with many elements is unknown.
- **Debug menu.** The `Test/...` menu items ship with the package.
- **Terminology.** README says "album"; the code and this guide say `Landscape`.

## 8. Working with other com.fixer33.* packages

- No package depends on this one and it depends on none. It is optional alongside any of them.
- `com.fixer33.ui-core` - https://github.com/Fixer33/UI-Core.git - a UI package; combining is possible since both are uGUI based (assumed, no code link).

## 9. Remaining open questions

These are still open and affect usage:

1. Should unsaved orientations be skipped at runtime instead of applying default snapshots (Open Questions 1)?
2. Is `Screen.width > Screen.height` (square counts as portrait) and per-frame polling the intended detection (2)?
3. Should Load verify the Game view size exists (3)?
4. Should the Game view group follow the active build target instead of Standalone (4)?
5. Is the prefab-only design deliberate given that Save/Load reach all enabled components (5)?
6. Is the property set final or a first pass (6)?
7. Are the `Test/...` menu items intended to ship (10)?
8. Are samples or usage documentation planned (13)?
