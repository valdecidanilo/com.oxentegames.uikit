# Oxente UI Kit

Oxente UI Kit is a collection of reusable, production-ready controls and
transitions for Unity UGUI.

## Controls

- `CustomButton`: button with color tint, sprite swap, animation presets,
  child graphic transitions, opacity control, and inverted TextMeshPro colors.
- `RangeSlider`: integer range selector with two handles, minimum/maximum span,
  grouped dragging, and Unity/C# events.
- `SlideSwitch`: accessible toggle-style switch with a dependency-free,
  configurable animation.
- Optional TextMeshPro presenters for range values and switch labels.

## Requirements

- Unity 6000.0.23f1 or newer
- Unity UI (`com.unity.ugui`), installed automatically

Oxente UI Kit has no runtime dependency on DOTween or application-specific
services.

## Installation

In Unity, open **Window > Package Management > Package Manager**, select
**Install package from Git URL**, and enter:

```text
https://github.com/valdecidanilo/com.oxentegames.custombutton.git
```

To follow the latest development version:

```text
https://github.com/valdecidanilo/com.oxentegames.custombutton.git#latest
```

For reproducible builds, prefer a release tag such as `#v2.0.0`.

## Creating controls

Use the Unity hierarchy menu:

- **GameObject > UI (Canvas) > Oxente UI > Custom Button**
- **GameObject > UI (Canvas) > Oxente UI > Range Slider**
- **GameObject > UI (Canvas) > Oxente UI > Slide Switch**

## Script references

```csharp
using OxenteGames.UI;
using UnityEngine;

public sealed class FightView : MonoBehaviour
{
    [SerializeField] private CustomButton fightButton;
    [SerializeField] private RangeSlider betRange;
    [SerializeField] private SlideSwitch autoPlaySwitch;
}
```

### RangeSlider

```csharp
betRange.Configure(0, 999, minimumSpan: 10, maximumSpan: 200);
betRange.SetValues(100, 250);
betRange.ValueChanged += (low, high) => Debug.Log($"{low}..{high}");
```

The control preserves serialized values when entering Play Mode. Call
`Center()` explicitly when a centered range is desired.

### SlideSwitch

```csharp
autoPlaySwitch.SetOn(true, animate: true, notify: true);
autoPlaySwitch.OnValueChanged += value => Debug.Log($"Auto play: {value}");
```

Animation uses an `AnimationCurve` and Unity coroutines. It supports scaled or
unscaled time and is stopped safely when the component is disabled.

## Migration from Custom Button 1.x

Version 2 is a major package/API migration:

- Package ID: `com.notask.custom-button` to `com.oxentegames.uikit`
- Namespace: `CustomButton` or `OxenteGames.CustomButton` to `OxenteGames.UI`
- Runtime assembly: `CustomButton` to `OxenteGames.UI.Runtime`
- Editor assembly: `CustomButton.Editor` to `OxenteGames.UI.Editor`
- Type: `CustomButtonBase` to `CustomButton`

`CustomButtonClass` remains as an obsolete, hidden compatibility component so
existing scenes and prefabs can retain their MonoScript reference.

Sample content can be imported from the package details in Package Manager.
