# Orientation UI Elements - Package Description

Package: `com.fixer33.orientation-ui-elements`, version 1.0.0-alpha (`package.json`), Unity 6000.0+, author Fixer33, MIT license.
Repository: https://github.com/Fixer33/Orientation-UI-Elements.git

Everything below was read from the package files unless marked (assumed).

## 1. Purpose and users

Lets a single uGUI (Canvas based) prefab carry two layouts, one for portrait and one for landscape. Each supported UI component gets a companion "Orientation" component that stores two snapshots of that component's properties (portrait and landscape). In the editor, a scene-view overlay captures and restores the snapshots. At runtime the companion applies the snapshot that matches the current screen shape.

Users are Unity developers building uGUI screens that must work in both orientations (assumed: mobile or cross-platform UI). README status: "Package is in development - you probably going to encounter some bugs"; CHANGELOG has a single `1.0.0-alpha` entry (2025-02-24).

## 2. Architecture and main components

Two assemblies:

| Assembly | Folder | Namespace | Notes |
|---|---|---|---|
| `com.fixer33.orientation-ui-elements` | `Runtime` | `OrientationElements`, `OrientationElements.Elements` | References `Unity.TextMeshPro`. `overrideReferences: true`, no platform restrictions. |
| `com.fixer33.orientation-ui-elements.Editor` | `Editor` | `OrientationElements.Editor` | Editor platform only. References the runtime assembly and `UnityEditor.UI`. |

Runtime pieces:
- `Orientation` enum: `Portrait`, `Landscape`.
- `IOrientationElement`: empty marker interface.
- `OrientationElementBase<T, TT>`: abstract `MonoBehaviour` (constraints `T : Component`, `TT : new()`). `T` is the wrapped component type, `TT` the serialisable snapshot type. It holds two serialised snapshots (`_cachedVertical`, `_cachedHorizontal`) and two hidden flags (`_hasVerticalCache`, `_hasHorizontalCache`). It is `[ExecuteInEditMode]` in the editor.
  - Every `Update`, it picks the orientation with `Screen.width > Screen.height` (landscape; otherwise portrait, so a square screen counts as portrait). Only while `Application.isPlaying` does it compare the element with the matching snapshot and, if different, write the snapshot to the element.
  - In the editor only (`#if UNITY_EDITOR`): `OnEnable`/`OnDisable` subscribe to the static events of `OrientationElementsManager`; `SaveOrientation` copies the element state into the snapshot and sets the flag; `LoadOrientation` writes the snapshot back, or adds itself to a "failed" list when the flag is not set. `OnValidate` calls `OnValidateLocal()`, then logs an error and disables the component if the GameObject has no `RectTransform`.
  - Abstract members each concrete class implements: `GetCacheForCurrentElement`, `WriteCacheToElement`, `IsElementDataEqualTo`. Virtual: `OnValidateLocal`.
  - Public property `Element` returns the wrapped component (lazy `GetComponent<T>()`).
- `OrientationElementsManager`: static class, the bridge between editor UI and components. Public events `LoadOrientationDataRequested` (`Action<Orientation, List<Behaviour>>`) and `SaveOrientationDataRequested` (`Action<Orientation>`). Editor-only members: `ConditionsAreMet` (true only in prefab-editing mode when the prefab contents contain a `Canvas`), `RequestOrientationLoad(Orientation)` (shows a dialog and logs errors for components that had nothing saved), `RequestOrientationSave(Orientation)`.
- Concrete elements in `Runtime/Elements`:

| Component | Wraps | Snapshot type | Extra options |
|---|---|---|---|
| `OrientationCanvas` | `CanvasScaler` (`[RequireComponent]`) | struct `CanvasCache` (scale mode, match mode, pixels per unit, match width/height, scale factor, DPI values, reference resolution, physical unit) | none |
| `OrientationCanvasGroup` | `CanvasGroup` | struct `CanvasGroupCache` (alpha, ignore parent groups, blocks raycasts, interactable) | none |
| `OrientationImage` | `Image` (`[RequireComponent]`) | struct `ImageCache` (sprite, color, material, raycast target/padding, maskable, fill center, pixels-per-unit multiplier) | none |
| `OrientationRawImage` | `RawImage` (`[RequireComponent]`) | struct `RawImageCache` (texture, color, material, raycast target/padding, UV rect, maskable) | none |
| `OrientationTextBasic` | `TMP_Text` | record `TextBasicCache` (text, font size, auto size and min/max, margin, alignment, font style) | `_ignoreText` (private serialised field: do not drive the text, useful for localised text, assumed) |
| `OrientationTransform` | `RectTransform` | struct `RectTransformCache` (rotation, scale, anchors, anchored position, size delta, pivot) | `_ignoreScale`, `_ignoreRotation`, `_ignoreAnchoredPosition` (private serialised fields) |

Editor pieces:
- `SceneToolbar` (internal `Overlay` for `SceneView`, display name "Orientation Elements tools"). Visible only while `OrientationElementsManager.ConditionsAreMet` is true (toggled from `SceneView.duringSceneGui`). It shows two blocks (Landscape, Portrait), each with a `Vector2IntField` for the game-view resolution (stored in `EditorPrefs` under `LandscapeSize_x/_y` and `PortraitSize_x/_y`, defaults 1440 x 900 for both), a "Set game view resolution" button, and "Load" and "Save" buttons. Load sets the game view size, focuses the Scene view, then calls `RequestOrientationLoad`. Save calls `RequestOrientationSave`. Styles come from `Editor/Styles/SceneToolbar.uss`.
- `GameViewUtils`: public static reflection helper over internal `UnityEditor.GameViewSizes` / `GameView` (find, add, select game-view sizes by text or by width/height).
- `Utilities.LoadAssetAtPath<T>`: wraps `AssetDatabase.LoadAssetAtPath`; contains a conditional-compilation path for development use.

Intended workflow (assumed from the code): open a UI prefab that contains a Canvas, add the Orientation components next to the UI components, set the portrait layout, press Save in the Portrait block, set the landscape layout, press Save in the Landscape block; use Load to switch the editor view between them. At runtime the layout follows the screen shape automatically.

## 3. Public API surface

- Types: `Orientation`, `IOrientationElement`, `OrientationElementBase<T, TT>`, `OrientationElementsManager`, `OrientationCanvas`, `OrientationCanvasGroup`, `OrientationImage`, `OrientationRawImage`, `OrientationTextBasic`, `OrientationTransform` (plus their nested snapshot types). Editor: `GameViewUtils`, `Utilities` (public); `SceneToolbar` is internal.
- Entry points: add the Orientation components in the inspector; there is no runtime call to make. Optional: subscribe to `OrientationElementsManager.LoadOrientationDataRequested` / `SaveOrientationDataRequested`.
- Extension point: derive from `OrientationElementBase<T, TT>` for another component type (implement the three abstract methods; `TT` must be `[Serializable]` with a parameterless constructor).
- No custom inspectors and no `[AddComponentMenu]` attributes: the components are found under Add Component by name (assumed).

## 4. Dependencies

- Declared in `package.json`: none (no `dependencies` block).
- Implicit: `Unity.TextMeshPro` (asmdef reference; TextMeshPro ships with uGUI `com.unity.ugui` in Unity 6, assumed), uGUI (`UnityEngine.UI` is used but the runtime asmdef does not list it explicitly; it resolves through auto-referenced engine assemblies, assumed), `UnityEditor.UI` (Editor asmdef).
- Define symbols: `UNITY_EDITOR` (built-in) guards editor code in runtime files. No `versionDefines`, no `defineConstraints`.
- Other com.fixer33.* packages: none.

## 5. Editor tools and menu items

- Scene-view overlay "Orientation Elements tools" (see section 2).
- Menu items in `GameViewUtils`, all under a top-level `Test` menu: `Test/AddSize`, `Test/SizeTextQuery`, `Test/Query16:9Test`, `Test/Set16:9`, `Test/SetTestSize`, `Test/SizeDimensionsQuery`, `Test/LogCurrentGroupType`. These look like leftover debug helpers (assumed) and ship to every consumer.

## 6. Samples and tests

None. No `Samples~`, no test asmdefs, and no `Documentation` folder before this one.

## 7. How other projects consume it

Install from the Git URL above through Package Manager (README instructions). Add the Orientation components to UI prefabs. Nothing else needs wiring.

## 8. Current state

- Working but alpha (README, CHANGELOG and `package.json` all say `1.0.0-alpha`).
- Possible issues found in code (see Open Questions): runtime writes snapshots even when none were saved; the Load button does not check that a game-view size exists; debug menu items; unused USS rule `#RecordingBtn`; the `_has*Cache` flags are not consulted at runtime; `GameViewUtils` relies on Unity internals via reflection.
- No TODO comments in the code.
