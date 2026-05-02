using System.Collections;
using System.Collections.Generic;
using Entities;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// Управляет списком комнат и переходами между ними
    /// </summary>
    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { get; private set; }

        [Header("Level Progression")]
        [SerializeField] private List<GameObject> roomPrefabs;

        [Header("Player")]
        [SerializeField] private PlayerMovement player;

        private int currentRoomIndex = 0;
        private RoomController currentRoom;
        private bool isGameOver;

        public bool IsLevelLoaded => currentRoom != null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (player != null)
            {
                var playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.OnDeath += HandlePlayerDeath;
                }
            }

            if (roomPrefabs != null && roomPrefabs.Count > 0)
            {
                LoadRoomByIndex(currentRoomIndex);
            }
            else
            {
                Debug.LogError("[LevelController] Нет комнат для загрузки");
            }
        }

        /// <summary>
        /// Загрузить комнату по индексу
        /// </summary>
        private void LoadRoomByIndex(int index)
        {
            if (index >= roomPrefabs.Count)
            {
                Debug.Log("<color=cyan>[LevelController]</color> Все комнаты пройдены!");
                return;
            }

            UnloadCurrentRoom();

            isGameOver = false;

            var roomInstance = Instantiate(roomPrefabs[index], Vector3.zero, Quaternion.identity);
            currentRoom = roomInstance.GetComponent<RoomController>();

            if (currentRoom == null)
            {
                Debug.LogError($"[LevelController] Префаб комнаты {roomPrefabs[index].name} не содержит RoomController");
                return;
            }

            currentRoom.OnRoomCleared += HandleRoomCleared;

            currentRoom.Initialize();

            SpawnPlayer();

            TurnManager.Instance.ResetEnemies();
            TurnManager.Instance.BeginLevel();

            if (AbilityController.Instance != null)
            {
                AbilityController.Instance.UnblockInput();
                AbilityController.Instance.SelectAbilityByIndex(0);
            }

            Debug.Log($"<color=green>[LevelController]</color> Комната {index + 1}/{roomPrefabs.Count} загружена");
        }

        /// <summary>
        /// Выгрузить текущую комнату
        /// </summary>
        private void UnloadCurrentRoom()
        {
            if (currentRoom != null)
            {
                currentRoom.OnRoomCleared -= HandleRoomCleared;
                currentRoom.Cleanup();
                Destroy(currentRoom.gameObject);
                currentRoom = null;
            }
        }

        /// <summary>
        /// Спаун игрока в текущей комнате
        /// </summary>
        private void SpawnPlayer()
        {
            if (player == null || currentRoom == null) return;

            GridManager.Instance.UnregisterEntity(player.CurrentCell);
            

            if (currentRoom.playerSpawnPoint != null)
            {
                player.transform.position = currentRoom.playerSpawnPoint.position;
            }
            else
            {
                Debug.LogError("[LevelController] playerSpawnPoint не назначен");
            }

            player.InitializeOnGrid();

            CameraFollow.Instance?.ResetFocus();
        }

        /// <summary>
        /// Обработка выполнения целей комнаты
        /// </summary>
        private void HandleRoomCleared()
        {
            if (isGameOver) return;
            isGameOver = true;
            
            AbilityController.Instance?.BlockInput();
            
            Debug.Log("<color=green>[LevelController]</color> Комната пройдена!");
            StartCoroutine(TransitionToNextRoom());
        }

        /// <summary>
        /// Переход к следующей комнате
        /// </summary>
        private IEnumerator TransitionToNextRoom()
        {
            yield return new WaitForSeconds(1.5f);

            if (TransitionScreenManager.Instance != null && currentRoom != null)
            {
                yield return TransitionScreenManager.Instance.ShowVictoryScreen(currentRoom.VictoryMessage);
            }

            currentRoomIndex++;

            if (currentRoomIndex < roomPrefabs.Count)
            {
                if (TransitionScreenManager.Instance != null)
                {
                    yield return TransitionScreenManager.Instance.FadeToBlack(() =>
                    {
                        LoadRoomByIndex(currentRoomIndex);
                    });

                    yield return TransitionScreenManager.Instance.FadeFromBlack();
                }
                else
                {
                    LoadRoomByIndex(currentRoomIndex);
                }
            }
            else
            {
                Debug.Log("<color=cyan>[LevelController]</color> ВСЕ УРОВНИ ПРОЙДЕНЫ!");
                // TODO: показать финальный экран
            }
        }

        /// <summary>
        /// Обработка смерти игрока
        /// </summary>
        private void HandlePlayerDeath(GameObject playerObj)
        {
            if (isGameOver) return;

            isGameOver = true;
            Debug.Log("<color=red>[LevelController]</color> Игрок погиб");

            if (AbilityController.Instance != null)
            {
                AbilityController.Instance.DisableAllOverlaysAfterDeath();
            }

            TurnManager.Instance.StopAllCoroutines();

            CameraFollow.Instance?.ResetToPlayer();

            StartCoroutine(ShowDefeatAndRestart());
        }

        /// <summary>
        /// Показать экран поражения и перезапустить сцену
        /// </summary>
        private IEnumerator ShowDefeatAndRestart()
        {
            yield return new WaitForSeconds(1.5f);

            if (TransitionScreenManager.Instance != null)
            {
                yield return TransitionScreenManager.Instance.ShowDefeatScreen("ПОРАЖЕНИЕ!");
            }

            // Перезагрузить сцену
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}