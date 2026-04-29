using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using Entities;

/// <summary>
/// Управляет инициализацией, отслеживанием врагов, проверкой целей в пределах комнаты
/// Привязан к префабу комнаты
/// </summary>
public class RoomController : MonoBehaviour
{
    [Header("Room Setup")]
    public Tilemap obstacleTilemap;
    public Tilemap selectionTilemap;
    public Tilemap effectTilemap;
    public Transform playerSpawnPoint;

    [Header("Objective")]
    [SerializeField] private RoomObjectiveType objectiveType = RoomObjectiveType.KillAllEnemies;
    [SerializeField] private string victoryMessage = "Комната пройдена";

    public RoomObjectiveType ObjectiveType => objectiveType;
    public string VictoryMessage => victoryMessage;

    /// <summary>
    /// Вызывается, когда все цели комнаты выполнены
    /// </summary>
    public event Action OnRoomCleared;

    private List<EnemyBase> enemiesInRoom = new();
    private bool isCleared = false;

    private void Awake()
    {
        enemiesInRoom = new List<EnemyBase>(GetComponentsInChildren<EnemyBase>());
    }
    
    public void Initialize()
    {
        isCleared = false;

        if (GridManager.Instance != null)
        {
            GridManager.Instance.UpdateObstacles(obstacleTilemap);
        }

        if (GridHighlighter.Instance != null)
        {
            GridHighlighter.Instance.UpdateTilemaps(selectionTilemap, effectTilemap);
        }

        foreach (var enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    health.OnDeath += HandleEnemyDeath;
                }
            }
        }

        Debug.Log($"<color=cyan>[RoomController]</color> Комната инициализирована. Врагов: {enemiesInRoom.Count}");
    }

    /// <summary>
    /// Очистить ресурсы комнаты перед уничтожением
    /// </summary>
    public void Cleanup()
    {
        foreach (var enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    health.OnDeath -= HandleEnemyDeath;
                }
            }
        }

        if (GridHighlighter.Instance != null)
        {
            GridHighlighter.Instance.Clear();
        }
    }

    private void HandleEnemyDeath(GameObject enemyObj)
    {
        CheckObjective();
    }

    /// <summary>
    /// Проверить, выполнены ли цели комнаты
    /// </summary>
    private void CheckObjective()
    {
        if (isCleared) return;

        switch (objectiveType)
        {
            case RoomObjectiveType.KillAllEnemies:
                if (AreAllEnemiesDead())
                {
                    CompleteRoom();
                }
                break;

            case RoomObjectiveType.BossFight:
                // Заглушка
                if (AreAllEnemiesDead())
                {
                    CompleteRoom();
                }
                break;
        }
    }

    private bool AreAllEnemiesDead()
    {
        foreach (var enemy in enemiesInRoom)
        {
            if (enemy != null && enemy.Stats != null && !enemy.Stats.IsDead)
            {
                return false;
            }
        }
        return enemiesInRoom.Count > 0;
    }

    private void CompleteRoom()
    {
        isCleared = true;
        Debug.Log($"<color=green>[RoomController]</color> Цели комнаты выполнены");
        OnRoomCleared?.Invoke();
    }

    /// <summary>
    /// Получить точку спауна игрока
    /// </summary>
    public Vector3Int GetPlayerSpawnCell()
    {
        if (playerSpawnPoint == null)
        {
            Debug.LogWarning("[RoomController] Точка спауна игрока не задана");
            return Vector3Int.zero;
        }

        return GridManager.Instance.WorldToCell(playerSpawnPoint.position);
    }
}

/// <summary>
/// Тип цели комнаты
/// </summary>
public enum RoomObjectiveType
{
    KillAllEnemies,      
    BossFight
}

// public class RoomController : MonoBehaviour
// {
//     [Header("Links")] public Tilemap obstacleTilemap;
//     public Tilemap selectionTilemap;
//     public Tilemap effectTilemap;
//     public Transform playerSpawnPoint;
//
//     /// <summary>
//     /// Находит всех врагов внутри комнаты в момент вызова
//     /// </summary>
//     public List<EnemyBase> GetEnemiesInRoom()
//     {
//         return new List<EnemyBase>(GetComponentsInChildren<EnemyBase>());
//     }
// }