# Unity Cloud Build Automation — PROG2006

This repository is configured for the Unity version recommended by the assessment.

## Build target

- Repository: `pure-alone/Dewy-Water-Journey-Unity`
- Branch: `main`
- Unity editor: **2022.3.62f2**
- Platform: **WebGL**
- Target name: `dewy-water-journey-webgl-2022`
- Project subfolder: leave blank
- Development build: OFF
- Auto-build: optional; OFF is simplest for assessment submission

## Scene configuration

The repository already registers:

`Assets/Scenes/Main.unity`

in `ProjectSettings/EditorBuildSettings.asset`.

Leave the Unity Cloud Scene List blank so Build Automation uses the project's EditorBuildSettings. If the UI requires a scene entry, use:

`Scenes/Main.unity`

because Unity Cloud's scene field is relative to the Assets folder.

## WebGL settings applied by the project

`Assets/Editor/DewyBuild.cs` applies these settings before every WebGL build:

- Canvas/player size: **450 × 900**
- WebGL template: `PROJECT:Dewy`
- Compression: **Gzip**
- Decompression Fallback: **enabled**
- Main scene: `Assets/Scenes/Main.unity`

No custom pre-build or post-build method needs to be entered in Unity Cloud.

## Source control

Connect GitHub repository:

`pure-alone/Dewy-Water-Journey-Unity`

and build branch:

`main`

The repository is public. Unity Build Automation only needs repository read access unless you enable webhook-based automatic builds.

## itch.io artifact

After a successful WebGL build:

1. Download the build artifact.
2. Extract it if Unity Cloud returns an archive.
3. Zip the WebGL output contents so `index.html` is at the ZIP root.
4. Upload the ZIP to itch.io as an **HTML** project.
5. Enable browser play.
6. Use a 450 × 900 viewport or responsive/fullscreen scaling.
