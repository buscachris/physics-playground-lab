# Verification

This file records the current evidence boundary for the Physics Playground Lab portfolio prototype.

## Previously recorded evidence (not rerun for this export)

- Unity version: `2022.3.22f1`
- `ProjectBuilder.BuildProject`: passed; scene marker `PHYSICS_PLAYGROUND_SCENE_CREATED`
- `ProjectBuilder.BuildStandalone`: passed; marker `PHYSICS_PLAYGROUND_BUILD_RESULT:Succeeded`
- Standalone output: `Builds/Linux/PhysicsPlaygroundLab.x86_64` (ignored by Git)
- MIT license present with Christian Busca attribution
- Standalone player configured for a 1024×768 window

## Not yet verified

- Interactive gameplay capture in the standalone player
- Automated play-mode coverage for camera, wall jump, and force systems
- WebGL build
- Unity Profiler baseline

## Reproduce

Set `UNITY_EDITOR` to your Unity 2022.3.22f1 executable, then run from the project root. An active Unity license is required.

```bash
"$UNITY_EDITOR" \
  -batchmode -nographics \
  -projectPath "$PWD" \
  -executeMethod ProjectBuilder.BuildProject -quit

"$UNITY_EDITOR" \
  -batchmode -nographics \
  -projectPath "$PWD" \
  -executeMethod ProjectBuilder.BuildStandalone -quit
```

## Source export review — 2026-11-11

Assets (including .meta files), Packages, ProjectSettings and documentation were selected. Generated caches, diagnostics, player binaries and prior Git history are excluded. Static JSON, meta-pair and scene GUID checks passed. Unity compilation, builds and gameplay were not rerun for this export.
