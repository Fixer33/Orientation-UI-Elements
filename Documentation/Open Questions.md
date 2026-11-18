# Orientation UI Elements - Open Questions

## Behaviour and correctness
1. **Runtime applies snapshots that were never saved.** In `OrientationElementBase.Update`, `_cachedVertical ??= new TT()` creates a default snapshot, and `UpdatePlaying` then writes it to the element without checking `_hasVerticalCache` / `_hasHorizontalCache`. For `OrientationImage` that means a null sprite and a transparent color; for `OrientationTextBasic` the record fields are null/zero (this could blank the text unless `_ignoreText` is set). Is it intended that every orientation must be saved, or should an unsaved orientation be skipped? (The flags are only used in the editor Load path.)
2. **Orientation detection.** `Screen.width > Screen.height` means landscape, otherwise portrait; a square screen is portrait. Intended? `Update` also runs every frame and compares every property of every element (no event-based approach); performance with many elements is unknown.
3. **Load without resolution check.** The "Set game view resolution" button checks that the size exists in the Game view size list, but the "Load" button does not; `GameViewUtils.SetSize` would receive -1 if the size is missing. Oversight?
4. **Game view group fixed to Standalone.** `SceneToolbar` and `GameViewUtils.AddAndSelectCustomSize` always use `GameViewSizeGroupType.Standalone`, even if the active build target uses another group (e.g. Android). Intended?
5. **Scope of Save/Load.** The events reach every orientation component enabled in the editor (including scene objects), though the toolbar only appears in prefab mode with a Canvas. Is the prefab-only design deliberate?
6. **Property coverage.** E.g. `OrientationImage` does not cover `Image.type`, fill settings or preserve aspect; `OrientationTextBasic` does not cover font asset, color or wrapping. Is the property set final or a first pass?

## Packaging and metadata
8. **No declared dependencies.** The runtime assembly references `Unity.TextMeshPro` and uses uGUI, but `package.json` declares nothing. Intentionally left to the host project's built-in packages?
10. **Debug menu items.** Seven `Test/...` menu items in `GameViewUtils` ship with the package. Leftovers, or wanted?
11. **Unused style.** `SceneToolbar.uss` has a `#RecordingBtn` / `.isRecording` rule set not used by any code. Remnant of a planned recording feature?
12. **`overrideReferences: true`** in the runtime asmdef with empty `precompiledReferences`. Deliberate?

## Missing material
13. **No samples or tests,** and the README has no usage instructions beyond installation. Are any planned?
14. **Terminology.** README says "album" orientation (and package.json "album"), the code uses `Landscape`. Wording only?

## Answered
- **Version:** the correct version is 1.0.0-alpha; `package.json` was updated to match the CHANGELOG.
- **Conditional-compilation symbol in `Editor/Utilities.cs`:** author decided it is neither documented nor removed.
