using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    #region Core

    public static GridManager Instance { get; private set; }

    [SerializeField] private Tilemap obstaclesTilemap;
    private readonly Dictionary<Vector3Int, GameObject> entitiesOnGrid = new();
    private GridPathfinder pathfinder;

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            pathfinder = new GridPathfinder(this);
        }
        else
            Destroy(gameObject);
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

    #endregion

    #region Entity Management

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

    public void UnregisterEntity(Vector3Int pos)
    {
        entitiesOnGrid.Remove(pos);
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

    #endregion

    #region Cell Validation

    /// <summary>
    /// Проверяет, может ли клетка быть пройдена с учетом указанных опций
    /// Базовый метод для всех алгоритмов поиска
    /// </summary>
    /// <param name="cellPos">Координаты проверяемой клетки</param>
    /// <param name="options">Опции проверки (учитывать ли сущности, какую сущность игнорировать)</param>
    /// <returns>true если клетка проходима, false если заблокирована</returns>
    public bool IsCellPassable(Vector3Int cellPos, CellCheckOptions options)
    {
        if (obstaclesTilemap.HasTile(cellPos)) return false;

        if (!options.checkEntities) return true;

        if (entitiesOnGrid.TryGetValue(cellPos, out var entityInCell))
        {
            if (entityInCell == null)
            {
                entitiesOnGrid.Remove(cellPos);
                return true;
            }

            if (entityInCell != options.currentEntity)
                return false;

            var health = entityInCell.GetComponent<Health>();
            if (health != null && health.IsDead)
            {
                return true;
            }
        }

        return true;
    }

    #endregion

    #region Search API

    /// <summary>
    /// Ищет все доступные клетки в радиусе с настраиваемыми параметрами
    /// Универсальный метод для движения и атак
    /// </summary>
    /// <param name="center">Центральная клетка</param>
    /// <param name="maxRange">Максимальный радиус</param>
    /// <param name="minRange">Минимальный радиус</param>
    /// <param name="options">Опции поиска (алгоритм, учет сущностей)</param>
    /// <returns>Список доступных клеток</returns>
    public List<Vector3Int> GetCellsInRange(Vector3Int center, int maxRange, int minRange, CellCheckOptions options)
    {
        return pathfinder.GetCellsInRange(center, maxRange, minRange, options);
    }

    /// <summary>
    /// Вычисляет кратчайшее расстояние между клетками через проходимые пути (BFS)
    /// </summary>
    /// <param name="start">Начальная клетка</param>
    /// <param name="target">Целевая клетка</param>
    /// <param name="currentEntity">Сущность для которой считается путь (может проходить через свою клетку)</param>
    /// <returns>Расстояние в клетках или int.MaxValue, если путь не найден</returns>
    public int GetPathDistance(Vector3Int start, Vector3Int target, GameObject currentEntity = null)
    {
        return pathfinder.GetPathDistance(start, target, currentEntity);
    }

    public bool IsCellWalkable(Vector3Int cellPos, GameObject currentEntity = null)
        => IsCellPassable(cellPos, CellCheckOptions.ForMovement(currentEntity));

    public List<Vector3Int> GetWalkableCellsInRange(Vector3Int startPos, int range, GameObject currentEntity = null)
        => GetCellsInRange(startPos, range, 1, CellCheckOptions.ForMovement(currentEntity));

    public List<Vector3Int> GetAttackableCellsInRadius(Vector3Int center, int maxRange, int minRange = 1)
        => GetCellsInRange(center, maxRange, minRange, CellCheckOptions.ForAttack());

    #endregion

    #region Coordinate calculations

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        if (obstaclesTilemap == null)
            return Vector3Int.zero;

        return obstaclesTilemap.WorldToCell(worldPos);
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPos) => obstaclesTilemap.GetCellCenterWorld(cellPos);

    #endregion
}
