# Purrdicted FPS Controller

PurrNet Purrdiction FPS Character Controller for general purpose. Uses Input System, Cinemachine and Built-In Character Controller.

**Note:** I will update this according to my game's purposes. But it will stay general.

## How to Install

Go to package manager and add package via git: `https://github.com/Warwlock/Purrdicted-FPS-Controller.git`

## How to use

Install **`Example Prefab Setup`** from samples.

## What is currently included
* Input System (Uses InputActionReference variable)
* Cinemachine with priority change (local player has priority of 10 and others has -1)
* Moving, jumping and sprinting at the moment.
* Basic animation controller within `UpdateView` function.
* Mixamo example art assets and animations inside `Samples`. (Only idle and walking animations, no IK)

## Important Things to Consider

### Camera Setup
* The camera is placed inside the visual component of the character for smooth movement. However, rotating the camera also rotates the character, which can cause an infinite rotation feedback loop. 
* **Fix:** Ensure **`Reference Frame`** is set to **`World`** inside the **`Cinemachine Pan Tilt`** component.

### Animations & Network
* Animations are **not networked**. Clients will see slightly different keyframes on their screens, but the overall animation states (running, idle) will look identical because they are driven by the predicted movement data.

### Prefab Setup & Inputs
* **Predicted Prefabs:** Ensure your Player Prefab is inside a folder referenced by the `Predicted Prefabs` asset. If not, PurrNet will fail to spawn it.
* **Input Asset:** The Example Prefab uses a `Player Input` component referencing `PurrdictedCharacterInputs.inputaction`. If your game uses a project-wide input asset, you can remove this component—just remember to update the `Input Action References` inside the `Player Movement` script.


## 🛠️ Advanced Integration (The Coordinator Pattern)

### Using IMovement

If you want to get data from movement script use `GetComponent<IMovement>`.

```csharp
public class PlayerAnimationController : MonoBehaviour
{
    private IMovement movement;
    private Animator anim;

    private void Awake()
    {
        movement = GetComponent<IMovement>(); // Or assign from inspector
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (movement == null) return;

        // Drive the animator based on the interface properties
        anim.SetFloat("Speed", movement.MovementDirectionSpeed.magnitude);
        anim.SetBool("IsGrounded", movement.IsGrounded);
        anim.SetBool("IsJumped", movement.IsJumped);
        
        // For a 2D Blend Tree (MoveX, MoveZ)
        anim.SetFloat("MoveX", movement.MovementDirectionSpeed.x);
        anim.SetFloat("MoveZ", movement.MovementDirectionSpeed.y);
    }
}
```


### Bridge/Coordinator

To keep this package clean, `PlayerMovement` does not know about inventories, health, or interactables.

If you want to disable movement when the player interacts with an object (from the Interaction System package), create a **Bridge/Coordinator** script in your Main Game Project:

**Note:** Only call `Simulate` functions within `Simulate` loop. Otherwise unwanted behaviour can happen.

```csharp
using UnityEngine;
using Warwlock.FPSController;
using Warwlock.Interaction;

// Put this on your Player Prefab
public class PlayerSystemsCoordinator : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteractor interactor;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        interactor = GetComponent<PlayerInteractor>();
    }

    private void OnEnable()
    {
        // Listen to the interaction package
        interactor.OnInteractSimulate += HandleInteractionSimulate;
    }

    private void OnDisable()
    {
        interactor.OnInteractSimulate -= HandleInteractionSimulate;
    }

    private void HandleInteractionSimulate(InteractionInfo info)
    {
        // Tell the movement package to stop
        movement.SetMovementEnabledSimulate(false);
        
        // Restore movement after 1 second
        Invoke(nameof(RestoreMovement), 1.0f);
    }

    private void RestoreMovement() => movement.SetMovementEnabledSimulate(true);
}
```

