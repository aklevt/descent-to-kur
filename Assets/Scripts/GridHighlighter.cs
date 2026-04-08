using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridHighlighter : MonoBehaviour
{
    public static GridHighlighter Instance { get; private set; }

    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private Color highlightColor = new(1, 1, 1, 0.8f);

    private void Awake()
    {
        Instance = this;
    }

    public void HighlightCells(List<Vector3Int> cells, Color? color = null)
    {
        Clear();
        var baseColor = color ?? highlightColor;
        var finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
        foreach (var cell in cells)
        {
            highlightTilemap.SetTile(cell, highlightTile);
            highlightTilemap.SetTileFlags(cell, TileFlags.None);
            highlightTilemap.SetColor(cell, finalColor);
        }
    }

    public void Clear() => highlightTilemap.ClearAllTiles();
}