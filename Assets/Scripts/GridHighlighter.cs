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
    [SerializeField] private TileBase defaultEffectTile;

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

    public void HighlightCells(List<Vector3Int> cells, Color? color = null, TileBase customTile = null)
    {
        if (selectionTilemap == null) return;
        
        selectionTilemap.ClearAllTiles();
        var baseColor = color ?? highlightColor;
        var finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a);
        
        var tileToSet = customTile != null ? customTile : highlightTile;
        
        foreach (var cell in cells)
        {
            SetTile(selectionTilemap, cell, tileToSet, finalColor);
        }
    }
    
    public void HighlightEffect(List<Vector3Int> cells, Color? color = null, TileBase customTile = null)    {
        if (effectTilemap == null) return;
        
        effectTilemap.ClearAllTiles();
        var baseColor = color ?? highlightColor;
        var finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.99f);
        
        var tileToSet = customTile != null ? customTile : defaultEffectTile;
        
        foreach (var cell in cells)
        {
            SetTile(effectTilemap, cell, tileToSet, finalColor);
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

    private void SetTile(Tilemap tilemap, Vector3Int cell, TileBase tile, Color color)
    {       
        tilemap.SetTile(cell, tile);
        tilemap.SetTileFlags(cell, TileFlags.None);
        tilemap.SetColor(cell, color);
    }
}