# [1.1.0](https://github.com/valdecidanilo/com.oxentegames.uikit/compare/v1.0.0...v1.1.0) (2026-07-25)


### Features

* Rebrand to Oxente UI Kit and add controls ([1b34710](https://github.com/valdecidanilo/com.oxentegames.uikit/commit/1b34710bc2fdeca208c7e11f1aa298d00e088dcc))

# 1.0.0 (2026-07-25)


### Bug Fixes

* namespace and reference ([523e5cb](https://github.com/valdecidanilo/com.oxentegames.custombutton/commit/523e5cb9529332111262e13924ee3f5746c5f428))
* remove project unity ([0625a1c](https://github.com/valdecidanilo/com.oxentegames.custombutton/commit/0625a1c0ca3095b834a323e103e5eb1b4b5dd63b))


### Features

* added custom button scripts ([43fc3db](https://github.com/valdecidanilo/com.oxentegames.custombutton/commit/43fc3db4a11de7b1838a4e645c77305bc7a275c5))
* initial project ([2acb0b1](https://github.com/valdecidanilo/com.oxentegames.custombutton/commit/2acb0b186d1535a1aa7bfc65b4164536679db438))
* Restructure to package-only UPM layout ([943977e](https://github.com/valdecidanilo/com.oxentegames.custombutton/commit/943977e601a0f0f44fafdd1cc86fd06c9dc87a6c))
* update new custombutton ([701545c](https://github.com/valdecidanilo/com.oxentegames.custombutton/commit/701545c0516079f369e4716666d834a3ff20f644))

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
