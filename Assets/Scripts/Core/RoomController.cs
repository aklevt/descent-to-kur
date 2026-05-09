using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UI;
using UI.Dialogue;

/// <summary>
/// Управляет инициализацией, отслеживанием врагов, проверкой целей в пределах комнаты
/// Привязан к префабу комнаты
/// </summary>
public class RoomController : MonoBehaviour
{
    [Header("Room Setup")] public Tilemap obstacleTilemap;
    public Tilemap selectionTilemap;
    public Tilemap effectTilemap;
    public Transform playerSpawnPoint;

    [Header("Objective")] [SerializeField] private RoomObjectiveType objectiveType = RoomObjectiveType.KillAllEnemies;
    [SerializeField] private string victoryMessage = "Комната пройдена";

    public RoomObjectiveType ObjectiveType => objectiveType;
    public string VictoryMessage => victoryMessage;

    /// <summary>
    /// Вызывается, когда все цели комнаты выполнены
    /// </summary>
    public event Action OnRoomCleared;

    private List<EnemyBase> enemiesInRoom = new();
    private List<RoomDialogueTrigger> dialogueTriggers = new();
    private bool isCleared = false;

    private void Awake()
    {
        enemiesInRoom = new List<EnemyBase>(GetComponentsInChildren<EnemyBase>());
        dialogueTriggers = new List<RoomDialogueTrigger>(GetComponentsInChildren<RoomDialogueTrigger>());
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

        if (CameraFollow.Instance != null && obstacleTilemap != null)
        {
            CameraFollow.Instance.SetCameraBounds(obstacleTilemap.localBounds);
        }
        
        if (FogSystemManager.Instance != null)
        {
            FogSystemManager.Instance.OnRoomChanged();
        }

        // Инициализация врагов
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

        // Инициализация диалогов

        foreach (var trigger in dialogueTriggers)
        {
            if (trigger != null)
            {
                trigger.Initialize();
            }
        }

        Debug.Log($"<color=cyan>[RoomController]</color> Комната инициализирована. Врагов: {enemiesInRoom.Count}");
        Debug.Log($"<color=cyan>[RoomController]</color> Диалогов: {dialogueTriggers.Count}");

        // TriggerDialoguesOfType(UI.DialogueTriggerType.OnRoomEnter);
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

        foreach (var trigger in dialogueTriggers)
        {
            if (trigger != null)
            {
                trigger.Cleanup();
            }
        }


        if (GridHighlighter.Instance != null)
        {
            GridHighlighter.Instance.Clear();
        }

        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.ClearBounds();
        }
    }
    
    /// <summary>
    /// Запустить все диалоги указанного типа последовательно
    /// </summary>
    public IEnumerator TriggerDialoguesOfTypeSequential(UI.DialogueTriggerType type)
    {
        var dialoguesOfType = dialogueTriggers.FindAll(t => t != null && t.TriggerType == type);
    
        Debug.Log($"<color=yellow>[RoomController]</color> Найдено диалогов типа {type}: {dialoguesOfType.Count}");
    
        foreach (var trigger in dialoguesOfType)
        {
            Debug.Log($"<color=yellow>[RoomController]</color> Запускаем диалог: {trigger.DialogueData?.name}");
            trigger.TriggerDialogue();
        
            yield return new WaitUntil(() => 
                DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive);
        }
    
        Debug.Log($"<color=yellow>[RoomController]</color> Все диалоги типа {type} завершены");

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