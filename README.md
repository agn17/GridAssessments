# Grid Mechanics \& Data-Driven Logic: Technical Assessment



Submission for the Magic Orbit Studios Jr. Developer Assignment.

**Unity version:** 6000.0.84f1
**Render pipeline:** Universal 3D (URP)
**Third-party assets / packages:** None.



## How to Run



1. Open the project in Unity.
2. Open `Assets/Scenes/Main.unity`.
3. Click inside the Game view and press Play.

To change the grid size, select the `Grid` object in the Hierarchy and edit **Width**, **Height** and **Cell Size** on the `GridManager` component, then press Play.

## 

## Controls



|Input|Action|
|-|-|
|Left mouse button|Select a grid cell (highlighted tile)|
|Mouse wheel|Cycle through cube types (current type shown on screen)|
|Space|Spawn a cube of the current type on the selected cell|
|W / A / S / D or Arrow keys|Move every cube one cell in that direction|

## 

## Project Structure



```
Assets/
  Materials/          Tile and cube materials
  Prefabs/            Tile and GridCube prefabs
  ScriptableObjects/  Cube type assets (Red, Blue, Green)
  Scenes/             Main scene
  Scripts/
    CubeTypeSO.cs             ScriptableObject defining a cube type (name + material)
    GridManager.cs            Grid data, bounds checks, spawning, selection, movement
    GridInputController.cs    Reads player input and calls into GridManager
```

## 

## Design Overview



* **Separation of logic and input.** `GridManager` owns all grid state and rules and knows nothing about the keyboard or mouse. `GridInputController` only reads input and forwards it. This keeps the logic easy to test and change.
* **Grid state.** Occupied cells are stored in a `Dictionary<Vector2Int, GameObject>`. A dictionary cannot hold two entries for the same key, so the "one cube per cell" rule is enforced by checking `ContainsKey` before spawning or moving.
* **Data-driven cube types.** Cube types are `CubeTypeSO` ScriptableObject assets. Adding a new type means creating a new asset and adding it to the list on `GameController`. No code changes are needed.
* **Cell picking.** The mouse ray is intersected with a mathematical plane at the grid height and converted to a cell coordinate by dividing by cell size. No colliders are required.
* **Isometric view.** An orthographic 3D camera is rotated to (35., 45, 0).

## 

## Assumptions



* Only one cube can occupy a cell. Pressing Space on an occupied cell does nothing.
* Pressing Space with no cell selected does nothing.
* Cubes cannot push, swap with, or stack on other cubes.
* On a move input, each cube is handled independently. If its target cell is outside the grid, or is occupied by a cube that could not move, that cube stays in place and the other cubes still move. This matches the requirement to ignore an invalid input for that cube only.
* Cubes are processed starting from the ones closest to the movement direction, so a line of cubes can shift forward together without overlapping.
* One key press moves cubes exactly one cell (holding a key does not repeat).
* Movement keys follow the grid axes (W = +Z, S = -Z, D = +X, A = -X). Because the camera is rotated 45 degrees, these directions appear diagonal on screen.
* The grid is generated once when the scene starts and is centered on the `Grid` object.

## 

## Known Limitations



* Changing grid size requires restarting Play mode. There is no live resize or Edit-mode preview.
* Cubes move instantly with no animation.
* Uses the legacy `UnityEngine.Input` API (Active Input Handling is set to "Both").
* The current cube type is shown with a basic `OnGUI` label rather than proper UI.
* No way to remove cubes or reset the board.
* Movement directions are relative to grid axes, not screen directions.

## 

## What I Would Improve With More Time



* Migrate to the new Input System with rebindable actions.
* Remap movement keys so they match screen directions under the isometric camera.
* A custom editor script or gizmos to preview the grid in Edit mode.
* Proper UI (Canvas) showing the selected cube type and a hover highlight.
* Remove / clear cubes, and undo support.
* A cube type icon and per-type properties, such as movement speed or size, on the ScriptableObject.

