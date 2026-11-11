# Physics Playground Lab — Unity/C# Portfolio Prototype

A separate portfolio project based on the physics playground concepts in Casey Hardman’s *Game Programming with Unity and C#*, Chapter 33, “Physics Playground Design and Outline.”

## Current vertical slice

- First-person and third-person camera modes with mouse look and scroll zoom.
- WASD movement, gravity, grounded jumping, and wall-jump detection.
- Mouse-aimed telekinesis: left-click pulls and right-click pushes nearby Rigidbodies.
- A moving platform that carries the player while grounded.
- A linked Rigidbody swing assembled with ConfigurableJoints.
- A continuous force field and impulse jump pad.
- Live UI for camera mode, telekinesis target state, and controls.

## Run it

Open the project in Unity **2022.3.22f1** and open `Assets/Scenes/Main.unity`. The standalone build is configured for a 1024×768 window so it can be reviewed and recorded without taking over the desktop.

Click the Game view to capture the mouse. Use `WASD` to move, `SPACE` to jump or wall jump, `V` to switch camera modes, and the mouse wheel to adjust third-person distance. Aim at the blue crate or barrel; left-click pulls and right-click pushes. Press `ESC` to release the cursor.

## Portfolio evidence

| Role requirement | Demonstrated by |
| --- | --- |
| Unity/C# | Runtime scene construction, MonoBehaviour components, reusable physics systems |
| 3D interaction | CharacterController movement, camera modes, raycast targeting |
| Physics | Rigidbody forces, triggers, joints, moving platforms, gravity, and impulses |
| Maintainability | Focused player, camera, telekinesis, field, pad, and platform components |
| UI | Canvas instructions, mode readout, crosshair, and target feedback |

## Source attribution

Planning concepts are informed by Casey Hardman, *Game Programming with Unity and C#*, Chapter 33, “Physics Playground Design and Outline” (2020), DOI: https://doi.org/10.1007/978-1-4842-5656-5_33. This project is an original portfolio adaptation and does not copy chapter code or assets.

## Verification status

This source snapshot was checked for JSON validity, Unity asset/meta pairs and scene GUID references. Unity compilation, standalone builds and gameplay were not rerun during export preparation. Historical build results and remaining checks are recorded in [VERIFICATION.md](VERIFICATION.md).

Interactive gameplay and automated play-mode coverage remain pending. Runtime materials use `Shader.Find("Standard")`; shader retention in a fresh standalone build has not been verified. Treat this as a development prototype.

## Next steps

1. Add automated tests for camera mode transitions, wall jumps, and force application.
2. Add a dedicated layer setup for Player, ForceField, and Unmovable objects.
3. Add profiler evidence for Rigidbody counts and fixed-update cost.
4. Add a WebGL build and replace primitives with original or licensed assets.
