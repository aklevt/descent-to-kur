using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridHighlighter : MonoBehaviour
{
    public static GridHighlighter Instance { get; private set; }

    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Tilemap effectTilemap;
    [SerializeField] private Color highlightColor = new(1, 1, 1, 0.8f);
    [SerializeField] private TileBase highlightTile;

    private void Awake()
    {
        Instance = this;
    }
    
    public void UpdateTilemaps(Tilemap selection, Tilemap effect)
    {
        if (selectionTilemap != null) selectionTilemap.ClearAllTiles();
        if (effectTilemap != null) effectTilemap.ClearAllTiles();

        selectionTilemap = selection;
        effectTilemap = effect;
    }

    public void HighlightCells(List<Vector3Int> cells, Color? color = null)
    {
        if (selectionTilemap == null) return;
        
        selectionTilemap.ClearAllTiles();
        var baseColor = color ?? highlightColor;
        var finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
        foreach (var cell in cells)
        {
            SetTile(selectionTilemap, cell, finalColor);
        }
    }
    
    public void HighlightEffect(List<Vector3Int> cells, Color? color = null)
    {
        if (effectTilemap == null) return;
        
        effectTilemap.ClearAllTiles();
        var baseColor = color ?? highlightColor;
        var finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
        foreach (var cell in cells)
        {
            SetTile(effectTilemap, cell, finalColor);
        }
    }
    
    public void ClearEffect() => effectTilemap?.ClearAllTiles();

    public void Clear()
    {
        if (selectionTilemap != null)
            selectionTilemap.ClearAllTiles();
        
        if (effectTilemap != null)
            effectTilemap.ClearAllTiles();
    }

    private void SetTile(Tilemap tilemap, Vector3Int cell, Color color)
    {
        tilemap.SetTile(cell, highlightTile);
        tilemap.SetTileFlags(cell, TileFlags.None);
        tilemap.SetColor(cell, color);
    }
}