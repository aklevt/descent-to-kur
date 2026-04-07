using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private Tilemap obstaclesTilemap;
    
    private Dictionary<Vector3Int, GameObject> entitiesOnGrid = new();

    public void MoveEntity(Vector3Int from, Vector3Int to, GameObject entity)
    {
        if (entitiesOnGrid.ContainsKey(from) && entitiesOnGrid[from] == entity)
        {
            entitiesOnGrid.Remove(from);
        }
        entitiesOnGrid[to] = entity;
    }
    
    public void RegisterFixedEntity(Vector3Int pos, GameObject entity)
    {
        entitiesOnGrid[pos] = entity;
    }

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }

    public bool IsCellWalkable(Vector3Int cellPos, GameObject currentEntity = null)    {
        if (obstaclesTilemap.HasTile(cellPos)) return false;

        if (entitiesOnGrid.TryGetValue(cellPos, out var entityInCell))
        {
            if (entityInCell != currentEntity) 
                return false;
        }

        return true;
    }
    
    public List<Vector3Int> GetWalkableTilesInRange(Vector3Int startPos, int range, GameObject currentEntity = null)
    {
        var walkable = new List<Vector3Int>();
    
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        foreach (var dir in directions)
        {
            var checkingPos = startPos + dir;
            if (IsCellWalkable(checkingPos, currentEntity))
            {
                walkable.Add(checkingPos);
            }
        }
        return walkable;
    }
    
    /// <summary>
    /// Возвращает объект, находящийся на указанной клетке, если он там есть
    /// </summary>
    /// <returns>GameObject сущности или null</returns>
    public GameObject GetEntityAt(Vector3Int cellPos)
    {
        if (entitiesOnGrid.TryGetValue(cellPos, out var entity))
        {
            return entity;
        }
        return null;
    }
    
    public void UnregisterEntity(Vector3Int pos)
    {
        entitiesOnGrid.Remove(pos);
    }
    
    /// <summary>
    /// Возвращает соседние клетки, в которых нет стен
    /// /// </summary>
    public List<Vector3Int> GetAttackableCells(Vector3Int center)
    {
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        return directions
            .Select(dir => center + dir)
            .Where(pos => !obstaclesTilemap.HasTile(pos))
            .ToList();
    }

    public Vector3Int WorldToCell(Vector3 worldPos) => obstaclesTilemap.WorldToCell(worldPos);

    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => obstaclesTilemap.GetCellCenterWorld(cellPos);
}