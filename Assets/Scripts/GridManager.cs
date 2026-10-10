using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Size")]
    [Min(1)] public int width = 8;
    [Min(1)] public int height = 8;
    public float cellSize = 1f;

    [Header("Visuals")]
    public GameObject tilePrefab;
    public Material tileDefaultMaterial;
    public Material tileHighlightMaterial;

    readonly Dictionary<Vector2Int, Renderer> tiles = new();
    readonly Dictionary<Vector2Int, GameObject> cubes = new();

    Vector2Int? selected;

    // Bottom-left cell position, so the grid is centered on this object
    Vector3 Origin => transform.position
        - new Vector3((width - 1) * cellSize * 0.5f, 0f, (height - 1) * cellSize * 0.5f);

    void Awake() => Generate();

    void Generate()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = new Vector2Int(x, y);
                Vector3 pos = CellToWorld(cell) + Vector3.down * 0.05f;
                var tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(cellSize * 0.95f, 0.1f, cellSize * 0.95f);
                tile.name = $"Tile {x},{y}";

                var rend = tile.GetComponent<Renderer>();
                rend.sharedMaterial = tileDefaultMaterial;
                tiles.Add(cell, rend);
            }
    }

    // ---------- Coordinate helpers ----------
    public bool InBounds(Vector2Int c) => c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;

    public Vector3 CellToWorld(Vector2Int c) =>
        Origin + new Vector3(c.x * cellSize, 0f, c.y * cellSize);

    Vector3 CubeWorldPos(Vector2Int c) => CellToWorld(c) + Vector3.up * cellSize * 0.4f;

    public bool TryGetCell(Ray ray, out Vector2Int cell)
    {
        var plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float dist))
        {
            Vector3 local = ray.GetPoint(dist) - Origin;
            cell = new Vector2Int(Mathf.RoundToInt(local.x / cellSize),
                                  Mathf.RoundToInt(local.z / cellSize));
            return InBounds(cell);
        }
        cell = default;
        return false;
    }

    // ---------- Selection ----------
    public Vector2Int? Selected => selected;

    public void Select(Vector2Int cell)
    {
        if (selected.HasValue) tiles[selected.Value].sharedMaterial = tileDefaultMaterial;
        selected = cell;
        tiles[cell].sharedMaterial = tileHighlightMaterial;
    }

    // ---------- Spawning ----------
    public bool TrySpawn(Vector2Int cell, GameObject prefab, CubeTypeSO type)
    {
        if (!InBounds(cell) || cubes.ContainsKey(cell)) return false;

        var go = Instantiate(prefab, CubeWorldPos(cell), Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * cellSize * 0.8f;
        go.GetComponentInChildren<Renderer>().sharedMaterial = type.material;
        cubes.Add(cell, go);
        return true;
    }

    // ---------- Movement ----------
    public void MoveAll(Vector2Int dir)
    {
        // Process cubes closest to the movement edge first so cubes
        // in a line can "follow" each other without overlapping.
        var cells = new List<Vector2Int>(cubes.Keys);
        cells.Sort((a, b) => Vector2.Dot(b, dir).CompareTo(Vector2.Dot(a, dir)));

        foreach (var from in cells)
        {
            Vector2Int to = from + dir;
            if (!InBounds(to) || cubes.ContainsKey(to)) continue; // blocked: this cube stays

            var go = cubes[from];
            cubes.Remove(from);
            cubes[to] = go;
            go.transform.position = CubeWorldPos(to);
        }
    }
}