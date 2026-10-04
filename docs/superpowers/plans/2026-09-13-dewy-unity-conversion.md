# Dewy's Water Journey Unity Conversion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a native Unity UGUI WebGL version of the supplied Dewy's Water Journey HTML prototype with 450×900 portrait layout, equivalent interactions, and unchanged MP3 assets.

**Architecture:** One `Main.unity` scene boots a runtime-built UGUI application. `DewyApp` handles page state and navigation, `DewyPages` builds page visuals and interactions, `DewyUI` supplies procedural UI primitives, `DewyAudio` maps BGM/SFX and persists sound preference, and `DewyBuild` configures local/cloud WebGL builds.

**Tech Stack:** Unity 6000.3.15f1, C#, Unity UGUI, WebGL, Python unittest structural validation.

**Spec:** `docs/superpowers/specs/2026-09-13-dewy-unity-conversion-design.md`

## Global Constraints

- Reference resolution is exactly 450 × 900.
- Use one scene: `Assets/Scenes/Main.unity`.
- Preserve Home + six story scenes + Credits and all source story copy.
- Preserve the six original interaction completion gates and locked-Next behavior.
- Copy all 18 MP3 files byte-for-byte; do not transcode or validate codecs.
- Target Unity 6000.3.15f1 / Unity 6.3 LTS.
- WebGL output must be suitable for Unity Build Automation and itch.io.

---

### Task 1: Define project-level acceptance tests

**Files:**
- Create: `tests/test_project_structure.py`

**Interfaces:**
- Consumes: source prototype at `/mnt/data/dewy_src` during local verification.
- Produces: executable acceptance checks for every required project artifact.

- [ ] **Step 1:** Write tests that require Unity project structure, 450×900 settings, scene bootstrap, page copy, interaction tokens, WebGL template, and 18 unchanged MP3 files.
- [ ] **Step 2:** Run `python -m unittest discover -s tests -v` and confirm failure because Unity project files do not exist yet.
- [ ] **Step 3:** Keep tests unchanged while implementation is added.

### Task 2: Create Unity project shell and WebGL configuration

**Files:**
- Create: `Packages/manifest.json`
- Create: `ProjectSettings/ProjectVersion.txt`
- Create: `ProjectSettings/EditorBuildSettings.asset`
- Create: `ProjectSettings/ProjectSettings.asset`
- Create: `Assets/Scenes/Main.unity`
- Create: `Assets/Scenes/Main.unity.meta`
- Create: `Assets/Scripts/DewyBootstrap.cs`
- Create: `Assets/Scripts/DewyBootstrap.cs.meta`
- Create: `Assets/Editor/DewyBuild.cs`
- Create: `Assets/WebGLTemplates/Dewy/index.html`
- Create: `Assets/WebGLTemplates/Dewy/TemplateData/style.css`

**Interfaces:**
- Produces: a registered Main scene and a cloud/local WebGL build configuration fixed to 450×900.

- [ ] **Step 1:** Add the minimal Unity package/project settings and scene registration.
- [ ] **Step 2:** Add the scene bootstrap object referencing `DewyBootstrap` by GUID.
- [ ] **Step 3:** Add WebGL template and editor/pre-build configuration.
- [ ] **Step 4:** Run acceptance tests and leave only application/content/audio tests failing.

### Task 3: Implement shared UGUI, audio, navigation, and content

**Files:**
- Create: `Assets/Scripts/DewyUI.cs`
- Create: `Assets/Scripts/DewyAudio.cs`
- Create: `Assets/Scripts/DewyDragHandler.cs`
- Create: `Assets/Scripts/DewyContent.cs`
- Create: `Assets/Scripts/DewyApp.cs`

**Interfaces:**
- Produces: page state/navigation API, procedural UI factory, sound preference, source-copy constants, drag callbacks.

- [ ] **Step 1:** Implement runtime Canvas/EventSystem and shared shell/topbar/story/nav/toast components.
- [ ] **Step 2:** Implement PlayerPrefs-backed audio mapping matching the original JavaScript names and volumes.
- [ ] **Step 3:** Implement content constants containing every page's original brand/kicker/title/body/instruction/learning label.
- [ ] **Step 4:** Run acceptance tests and confirm shared-content checks pass.

### Task 4: Implement all page visuals and interaction gates

**Files:**
- Create: `Assets/Scripts/DewyPages.cs`

**Interfaces:**
- Consumes: `DewyApp`, `DewyUI`, `DewyAudio`, `DewyDragHandler`, `DewyContent`.
- Produces: Home, Scene 1–6, Credits runtime layouts and the exact completion conditions.

- [ ] **Step 1:** Implement Home and Credits.
- [ ] **Step 2:** Implement Scene 1 sun drag completion.
- [ ] **Step 3:** Implement Scene 2 four-droplet cloud collection.
- [ ] **Step 4:** Implement Scene 3 four cloud taps and forest healing feedback.
- [ ] **Step 5:** Implement Scene 4 three ordered groundwater checkpoints.
- [ ] **Step 6:** Implement Scene 5 four ordered stream checkpoints and growing tributaries.
- [ ] **Step 7:** Implement Scene 6 four ordered river checkpoints and ending card.
- [ ] **Step 8:** Run acceptance tests and confirm all interaction/content checks pass.

### Task 5: Copy source audio byte-for-byte and document build/deployment

**Files:**
- Create: `Assets/Resources/Audio/*.mp3` (18 files)
- Modify: `README.md`
- Create: `.gitignore`

**Interfaces:**
- Produces: unchanged runtime audio assets and user-facing Unity Cloud Build/itch.io instructions.

- [ ] **Step 1:** Copy `/mnt/data/dewy_src/audio/*.mp3` directly to `Assets/Resources/Audio/` without any processing.
- [ ] **Step 2:** Document local Unity opening, Unity Build Automation setup, WebGL artifact handling, and itch.io upload.
- [ ] **Step 3:** Run `python -m unittest discover -s tests -v` and require all tests to pass, including SHA-256 audio identity.
- [ ] **Step 4:** Inspect repository tree for generated/cache artifacts and keep only source/project files.
