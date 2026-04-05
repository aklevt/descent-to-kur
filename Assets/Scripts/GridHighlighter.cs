using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridHighlighter : MonoBehaviour
{
    public static GridHighlighter Instance { get; private set; }

    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private Color highlightColor = new Color(1, 1, 1, 0.8f);

    private void Awake()
    {
        Instance = this;
    }

    public void HighlightCells(List<Vector3Int> cells)
    {
        Clear();
        foreach (var cell in cells)
        {
            highlightTilemap.SetTile(cell, highlightTile);
            highlightTilemap.SetTileFlags(cell, TileFlags.None);
            highlightTilemap.SetColor(cell, highlightColor);
        }
    }

    public void Clear() => highlightTilemap.ClearAllTiles();
}