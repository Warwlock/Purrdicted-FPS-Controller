# Purrdicted FPS Controller

PurrNet Purrdiction FPS Character Controller for general purpose. Uses Input System, Cinemachine and Built-In Character Controller.
**Note:** I will update this according to my game's purposes. But it will stay general.

## How to Install

Go to package manager and add package via git: `https://github.com/Warwlock/Purrdicted-FPS-Controller.git`

## How to use

Install `Example Prefab Setup` from samples.

## What is currently included
* Input System (Uses InputActionReference variable)
* Cinemachine with priority change (local player has priority of 10 and others has -1)
* Moving, jumping and sprinting at the moment.
* Basic animation controller within `UpdateView` function.
* Mixamo example art assets and animations inside `Samples`. (Only idle and walking animations, no IK)

## Important Things to Consider

### Camera
* Camera is inside the visual component of character, so it will move smoothly with character. But rotating camera also rotates character and this cause infinite feedback, causing endlessly rotate. To fix it we set **`Reference Frame`** to **`World`** inside **`Cinemachine Pan Tilt`** component.

### Animations
* Animation is not networked so clients will see different keyframes at their screen but the animation will look same.

### Prefab Setup and Inputs
* Be sure you have your `Example Prefab` or `Your Own Prefab` inside a folder that is referenced by `Predicted Prefabs` asset. Prediction manager asks for one and if you don't provide it, it will not spawn your prefab. And like I said, put your prefabs inside the folder of `Predicted Prefabs` asset is referencing.

* Example Prefab has `Player Input` component for enabling `PurrdictedCharacterInputs.inputaction` asset. If you are using project-wide input action asset, then you can remove this component. And don't forget to change `Input Action References` inside the `Player Movement` component.