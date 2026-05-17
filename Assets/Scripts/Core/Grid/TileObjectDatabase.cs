using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileObjectDatabase", menuName = "Grid/Tile Entity Database")]
public class TileObjectDatabase : ScriptableObject
{
    public List<TileMapping> mappings = new();

    [System.Serializable]
    public struct TileMapping
    {
        public TileBase tile;
        public GameObject entityPrefab;
    }

    public GameObject GetPrefabForTile(TileBase tile)
    {
        var mapping = mappings.Find(m => m.tile == tile);
        if (mapping.tile == null)
        {
            Debug.LogWarning($"[TileObjectDatabase]");
            return null;
        }
        return mapping.entityPrefab;
    }
}