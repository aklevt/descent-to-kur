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

    public bool IsCellWalkable(Vector3Int cellPos, GameObject currentEntity = null)
    {
        if (obstaclesTilemap.HasTile(cellPos)) return false;

        if (entitiesOnGrid.TryGetValue(cellPos, out var entityInCell))
        {
            if (entityInCell == null)
            {
                entitiesOnGrid.Remove(cellPos);
                return true;
            }

            if (entityInCell != currentEntity)
                return false;

            var health = entityInCell.GetComponent<Health>();
            if (health != null && health.IsDead)
            {
                return true;
            }
        }

        return true;
    }

    /// <summary>
    /// Возвращает кратчайшее расстояние между двумя клетками с учётом препятствий (BFS)
    /// </summary>
    /// <param name="start">Начальная клетка</param>
    /// <param name="target">Целевая клетка</param>
    /// <param name="currentEntity">Сущность, для которой считаем путь (может проходить через себя)</param>
    /// <returns>Расстояние в клетках или int.MaxValue если путь не найден</returns>
    public int GetPathDistance(Vector3Int start, Vector3Int target, GameObject currentEntity = null)
    {
        if (start == target) return 0;

        var queue = new Queue<(Vector3Int pos, int dist)>();
        var visited = new HashSet<Vector3Int> { start };

        queue.Enqueue((start, 0));

        var directions = new[] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            foreach (var dir in directions)
            {
                var next = current + dir;

                if (next == target) return dist + 1;

                if (visited.Contains(next) || !IsCellWalkable(next, currentEntity))
                    continue;

                visited.Add(next);
                queue.Enqueue((next, dist + 1));
            }
        }

        return int.MaxValue;
    }

    public List<Vector3Int> GetWalkableTilesInRange(Vector3Int startPos, int range, GameObject currentEntity = null)
    {
        if (range <= 0) return new List<Vector3Int>();

        var result = new HashSet<Vector3Int>();
        var visited = new HashSet<Vector3Int> { startPos };
        var queue = new Queue<(Vector3Int pos, int steps)>();

        queue.Enqueue((startPos, 0));

        var directions = new[] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        while (queue.Count > 0)
        {
            var (pos, steps) = queue.Dequeue();

            if (steps > 0 && IsCellWalkable(pos, currentEntity))
            {
                result.Add(pos);
            }

            if (steps >= range)
                continue;

            foreach (var dir in directions)
            {
                var next = pos + dir;

                if (visited.Contains(next))
                    continue;

                if (!IsCellWalkable(next, currentEntity))
                    continue;

                visited.Add(next);
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
        if (entitiesOnGrid.TryGetValue(cellPos, out var entity))
        {
            if (entity == null)
            {
                entitiesOnGrid.Remove(cellPos);
                return null;
            }

            var health = entity.GetComponent<Health>();
            if (health != null && health.IsDead)
            {
                return null;
            }

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

    /// <summary>
    /// Обновляет ссылку на тайлмап препятствий и очищает данные о сущностях на сетке (для перехода в новую комнату)
    /// </summary>
    /// <param name="newMap">Тайлпам стен из префаба новой комнаты</param>
    public void UpdateObstacles(Tilemap newMap)
    {
        obstaclesTilemap = newMap;
        entitiesOnGrid.Clear();
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        if (obstaclesTilemap == null)
            return Vector3Int.zero;

        return obstaclesTilemap.WorldToCell(worldPos);
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => obstaclesTilemap.GetCellCenterWorld(cellPos);
}