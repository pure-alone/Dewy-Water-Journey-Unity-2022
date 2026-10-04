# Dewy's Water Journey Unity Conversion Design

## Goal
Convert the supplied HTML/CSS/JavaScript prototype into a native Unity UGUI WebGL project while preserving the 450×900 portrait composition, all page copy, navigation, interaction gates, visual feedback, audio mapping, credits, and restart flow.

## Architecture
The project uses one Unity scene, `Assets/Scenes/Main.unity`. A bootstrap MonoBehaviour creates a `DewyApp` controller at runtime. The controller owns the page state (Home, Scene 1–6, Credits), recreates the UGUI hierarchy for the active page, and keeps navigation inside the single scene. `DewyUI` provides reusable UI primitives and procedural sprites so the CSS-drawn prototype can be represented without external art dependencies. `DewyAudio` owns one looping BGM source plus one SFX source and persists the sound preference through PlayerPrefs.

## Visual parity
The Canvas uses Scale With Screen Size at a reference resolution of 450×900 with matchWidthOrHeight 0.5. Layout values are derived from the source prototype's mobile presentation. The color system preserves the source palette: ink `#11394D`, muted text `#5F7280`, sky blues, ocean blues, sun yellow `#FFC94F`, vegetation greens, and earth brown. Home, ocean, cloud, forest, soil, mountain-stream, river, and credits visuals are recreated using nested UGUI Images and procedural rounded/circular sprites.

## Page flow and interactions
Home starts the journey and can open Credits. Scene 1 requires dragging the sun above the completion threshold. Scene 2 requires dragging all four droplets into the cloud. Scene 3 requires four taps on the rain cloud. Scene 4 requires dragging Dewy through three ordered groundwater checkpoints. Scene 5 requires four ordered stream checkpoints. Scene 6 requires four ordered river checkpoints and reveals the ending card. Next remains locked until the active scene is complete. Back is always available where present. Credits supports Restart Journey and Home.

## Audio
All 18 source MP3 files are copied byte-for-byte into `Assets/Resources/Audio/`. No audio encoding, transcoding, validation, or recompression is performed. Page BGM and SFX names match the original JavaScript mappings. Sound on/off persists under PlayerPrefs key `dewySound`.

## WebGL / Unity Cloud Build
The project targets Unity 6000.3.15f1. `DewyBuild.cs` configures WebGL, registers `Main.unity`, sets the WebGL canvas size to 450×900, selects the custom `Dewy` WebGL template, enables gzip compression with decompression fallback, and provides a local build menu item. A pre-build hook reapplies the same settings for Unity Build Automation.

## Verification
Python structural tests verify required project files, exact 450×900 configuration, scene registration, page/story copy, all six interaction gates, the 18-file audio inventory and source-vs-destination SHA-256 identity, WebGL template dimensions, and bootstrap scene/script linkage. Unity compilation itself requires Unity Editor/Unity Build Automation and cannot be executed inside the current container.
