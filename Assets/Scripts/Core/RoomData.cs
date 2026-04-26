using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RoomData : MonoBehaviour
{
    [Header("Links")] public Tilemap obstacleTilemap;
    public Tilemap selectionTilemap;
    public Tilemap effectTilemap;
    public Transform playerSpawnPoint;

    /// <summary>
    /// Находит всех врагов внутри комнаты в момент вызова
    /// </summary>
    public List<EnemyBase> GetEnemiesInRoom()
    {
        return new List<EnemyBase>(GetComponentsInChildren<EnemyBase>());
    }
}