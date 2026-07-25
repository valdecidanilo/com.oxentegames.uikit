# Changelog

All notable changes to this package are documented in this file.

## [Unreleased]

## [2.0.0] - 2026-07-25

- Rebranded the package as Oxente UI Kit.
- Changed the package ID to `com.oxentegames.uikit`.
- Moved the public API to the `OxenteGames.UI` namespace.
- Renamed the runtime and editor assemblies to `OxenteGames.UI.Runtime` and
  `OxenteGames.UI.Editor`.
- Renamed `CustomButtonBase` to `CustomButton`.
- Kept `CustomButtonClass` as an obsolete serialized-asset compatibility
  component.
- Added the reusable `RangeSlider` control.
- Added the reusable `SlideSwitch` control.
- Removed the hard DOTween dependency from `SlideSwitch`.
- Removed the game/server-specific `RangeSliderServerBinder` from the package.
- Added optional TextMeshPro label presenters.
- Separated Runtime and Editor code into directional assemblies.
- Preserved child graphic color, opacity, and inverted TextMeshPro transitions.

## [1.2.1] - 2026-07-25

- Converted the repository to a package-only UPM layout.
- Restored child graphic color and opacity transitions.
- Restored automatic inverted TextMeshPro colors.
