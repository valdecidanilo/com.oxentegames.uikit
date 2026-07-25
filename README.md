# Custom Button

Custom Unity UI button with color tint, sprite swap, animation presets, child
graphic transitions, and TextMeshPro color inversion.

## Requirements

- Unity 6000.0.23f1 or newer
- Unity UI (`com.unity.ugui`), installed automatically as a package dependency

## Installation

In Unity, open **Window > Package Management > Package Manager**, select
**Install package from Git URL**, and enter:

```text
https://github.com/valdecidanilo/com.notask.custombutton.git
```

To follow the development branch explicitly:

```text
https://github.com/valdecidanilo/com.notask.custombutton.git#development
```

For reproducible builds, prefer a release tag such as `#v1.2.1`.

## Usage

Create a button from **GameObject > UI > Custom Button - TextMeshPro** or add
the `CustomButtonClass` component to a UI object with an `Image`.

The Inspector provides:

- Color Tint, Sprite Swap, and Animation transition tabs
- Separate child color and opacity controls
- A list of child `Graphic` components affected by transitions
- Automatic inverted colors for child TextMeshPro labels
- Normal, highlighted, pressed, selected, and disabled states

Sample content can be imported from the package details in Package Manager.
