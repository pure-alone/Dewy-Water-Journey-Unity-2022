# Dewy's Water Journey — Unity WebGL

Native Unity UGUI conversion of the supplied **Dewy's Water Journey** HTML prototype for PROG2006 Assessment 2.

## Preserved experience

- Home + 6 interactive story scenes + Credits in one native Unity scene.
- Portrait reference resolution: **450 × 900**.
- Original story copy and student credit: **Zhihe Zhang**.
- Scene 1: drag the sun upward to unlock **Evaporation**.
- Scene 2: drag all four droplets into the cloud for **Condensation**.
- Scene 3: tap the rain cloud four times for **Precipitation**.
- Scene 4: guide Dewy through ordered **Infiltration / Groundwater** checkpoints.
- Scene 5: guide Dewy through ordered **Runoff & Collection** checkpoints.
- Scene 6: guide Dewy through the river route back to the ocean and reveal the ending card.
- Next stays locked until the current scene interaction is complete.
- Sound on/off is persisted through `PlayerPrefs`.
- The main branch contains all **18 original MP3 files** under `Assets/Resources/Audio/`, unchanged.

## Audio assets

The `main` branch contains all **18 original MP3 files** under `Assets/Resources/Audio/`. They are committed byte-for-byte with no codec validation or transcoding. `AUDIO_SHA256.txt` records the SHA-256 checksum for every audio file.

## Unity version

The project is pinned to **Unity 2022.3.62f2 LTS**, the version strongly recommended for PROG2006 Assessment 2, in `ProjectSettings/ProjectVersion.txt`.

Compatibility changes from the original Unity 6 project are intentionally limited to editor/package/runtime API compatibility:
- UGUI is pinned to `com.unity.ugui 1.0.0`.
- Runtime UI uses Unity 2022's built-in `Arial.ttf`.
- Unity 6 object lookup calls were replaced with Unity 2022-compatible `FindObjectOfType` calls.
- WebGL, story flow, visual polish, audio, interactions and 450 × 900 mobile layout are preserved.

## Open locally

1. Clone this repository.
2. In Unity Hub choose **Add project from disk** and select the repository folder.
3. Open it with Unity **2022.3.62f2** and ensure **WebGL Build Support** is installed.
4. Open `Assets/Scenes/Main.unity`.
5. Press Play.

The single scene contains a `DewyBootstrap` component. The complete Canvas, EventSystem, page flow and UGUI visuals are created natively at runtime. The original Unity 6 state is preserved on the `unity6-original-backup` branch.

## Build WebGL locally

Use **Dewy > Build WebGL for itch.io** in the Unity Editor. Output is written to:

`Builds/WebGL/`

The build helper applies:

- Web player size: 450 × 900
- Custom template: `PROJECT:Dewy`
- Gzip compression with decompression fallback
- Main scene: `Assets/Scenes/Main.unity`

## Unity Cloud / Build Automation

1. In Unity Cloud, connect the GitHub repository `pure-alone/Dewy-Water-Journey-Unity`.
2. Create a **Build Automation** target for **WebGL**.
3. Use branch **main**.
4. Select **Unity 2022.3.62f2 LTS**; the project is pinned to that exact editor revision.
5. Trigger the build and download the WebGL artifact when it completes.

`Assets/Editor/DewyBuild.cs` includes a pre-build hook that reapplies the 450 × 900 WebGL/template settings during cloud builds.

## Deploy to itch.io

1. Extract the downloaded Unity Cloud WebGL artifact if needed.
2. Zip the **contents** of the WebGL output folder so `index.html` is at the ZIP root.
3. In itch.io create/edit the project and select **Kind of project = HTML**.
4. Upload the ZIP and mark it playable in the browser.
5. Use an embed viewport of **450 × 900**, or allow fullscreen/mobile scaling.
6. Test sound toggle, all drag/tap completion gates, Credits and Restart Journey.

## Key project files

- `Assets/Scenes/Main.unity` — single Unity scene.
- `Assets/Scripts/DewyBootstrap.cs` — scene bootstrap.
- `Assets/Scripts/DewyApp.cs` — Canvas, page state, navigation, progress, locked Next, toast.
- `Assets/Scripts/DewyPages.cs` — all Home/Scene 1–6/Credits visuals and interactions.
- `Assets/Scripts/DewyUI.cs` — reusable UGUI primitives and procedural rounded/circle sprites.
- `Assets/Scripts/DewyAudio.cs` — BGM/SFX mapping and persisted sound preference.
- `Assets/Scripts/DewyDragHandler.cs` — pointer drag callbacks.
- `Assets/Scripts/DewyContent.cs` — original story and Credits copy.
- `Assets/WebGLTemplates/Dewy/` — 450 × 900 WebGL shell.
- `Assets/Editor/DewyBuild.cs` — local/cloud WebGL settings.
- `Assets/Resources/Audio/` — original MP3 assets, copied byte-for-byte.
- `tests/test_project_structure.py` — structural parity/build-readiness checks.

## Validation available without Unity

```bash
python -m unittest discover -s tests -v
```

These tests validate project structure, story copy, interaction coverage, audio inventory and byte identity, 450 × 900 configuration, scene registration and WebGL configuration. Actual C# compilation and WebGL player generation require the Unity Editor or Unity Build Automation.
