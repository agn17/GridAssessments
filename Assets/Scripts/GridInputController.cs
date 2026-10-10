using System.Collections.Generic;
using UnityEngine;

public class GridInputController : MonoBehaviour
{
    [SerializeField] GridManager grid;
    [SerializeField] Camera cam;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] List<CubeTypeSO> cubeTypes = new();

    int typeIndex;

    void Update()
    {
        // Left click selects a cell
        if (Input.GetMouseButtonDown(0) &&
            grid.TryGetCell(cam.ScreenPointToRay(Input.mousePosition), out var cell))
        {
            grid.Select(cell);
        }

        // Mouse wheel cycles cube types
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && cubeTypes.Count > 0)
        {
            int step = scroll > 0 ? 1 : -1;
            typeIndex = (typeIndex + step + cubeTypes.Count) % cubeTypes.Count;
        }

        // Space spawns at the selected cell
        if (Input.GetKeyDown(KeyCode.Space) && grid.Selected.HasValue && cubeTypes.Count > 0)
            grid.TrySpawn(grid.Selected.Value, cubePrefab, cubeTypes[typeIndex]);

        // WASD / arrows move all cubes
        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;
        if (dir != Vector2Int.zero) grid.MoveAll(dir);
    }

    // Quick on-screen indicator of the current cube type
    void OnGUI()
    {
        if (cubeTypes.Count == 0) return;
        GUI.Label(new Rect(10, 10, 300, 25), $"Cube type: {cubeTypes[typeIndex].displayName}");
    }
}