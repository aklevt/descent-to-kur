using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private Tilemap obstaclesTilemap;
    
    private readonly Dictionary<Vector3Int, GameObject> entitiesOnGrid = new();

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
        var result = new HashSet<Vector3Int>();
        var queue = new Queue<(Vector3Int pos, int steps)>();
        queue.Enqueue((startPos, 0));

        var directions = new[] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        while (queue.Count > 0)
        {
            var (pos, steps) = queue.Dequeue();
            if (steps >= range) 
                continue;

            foreach (var dir in directions)
            {
                var next = pos + dir;
                if (result.Contains(next) || !IsCellWalkable(next, currentEntity)) 
                    continue;
                result.Add(next);
                queue.Enqueue((next, steps + 1));
            }
        }
        return result.ToList();
    }
    
    /// <summary>
    /// Возвращает сущность, находящаяся на указанной клетке, если она там есть
    /// </summary>
    /// <returns>GameObject сущности или null</returns>
    public GameObject GetEntityAt(Vector3Int cellPos)
    {
        return entitiesOnGrid.GetValueOrDefault(cellPos);
    }
    
    public void UnregisterEntity(Vector3Int pos)
    {
        entitiesOnGrid.Remove(pos);
    }
    
    /// <summary>
    /// Возвращает соседние клетки, в которых нет стен
    /// ❗ переписать через BFS ❗
    /// /// </summary>
    public List<Vector3Int> GetAttackableCellsInRadius(Vector3Int center, int maxRange, int minRange = 1)
    {
        var cells = new List<Vector3Int>();

        for (var x = -maxRange; x <= maxRange; x++)
        {
            for (var y = -maxRange; y <= maxRange; y++)
            {
                var distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance > maxRange || distance < minRange) continue;
                
                var cellPos = new Vector3Int(center.x + x, center.y + y, center.z);
                
                if (!obstaclesTilemap.HasTile(cellPos))
                {
                    cells.Add(cellPos);
                }
            }
        }
        return cells;
        
        // Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
        //
        // return directions
        //     .Select(dir => center + dir)
        //     .Where(pos => !obstaclesTilemap.HasTile(pos))
        //     .ToList();
    }

    public Vector3Int WorldToCell(Vector3 worldPos) => obstaclesTilemap.WorldToCell(worldPos);

    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => obstaclesTilemap.GetCellCenterWorld(cellPos);
}