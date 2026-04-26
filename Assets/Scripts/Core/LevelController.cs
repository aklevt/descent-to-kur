using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { get; private set; }
        public bool IsLevelLoaded { get; private set; } = false;

        [Header("Level Settings")] [SerializeField]
        private List<GameObject> roomPrefabs;

        [SerializeField] private PlayerMovement player;

        private int currentRoomIndex = 0;
        private GameObject currentRoomInstance;

        private readonly List<EnemyBase> activeEnemies = new();
        private bool isGameOver;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log("Инициализация первого уровня");
            var playerHealth = player.GetComponent<Health>();
            playerHealth.OnDeath += HandlePlayerDeath;

            if (roomPrefabs != null && roomPrefabs.Count > 0)
            {
                LoadRoom(roomPrefabs[currentRoomIndex]);
            }
        }

        public void LoadRoom(GameObject roomPrefab)
        {
            if (roomPrefab == null) return;

            if (GridHighlighter.Instance != null)
                GridHighlighter.Instance.Clear();

            if (player != null)
                GridManager.Instance.UnregisterEntity(player.CurrentCell);

            if (currentRoomInstance != null)
                Destroy(currentRoomInstance);

            IsLevelLoaded = false;
            isGameOver = false;
            activeEnemies.Clear();
            TurnManager.Instance.ResetEnemies();

            currentRoomInstance = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            var data = currentRoomInstance.GetComponent<RoomData>();

            GridManager.Instance.UpdateObstacles(data.obstacleTilemap);

            if (GridHighlighter.Instance != null)
            {
                GridHighlighter.Instance.UpdateTilemaps(data.selectionTilemap, data.effectTilemap);
            }

            if (player != null && data.playerSpawnPoint != null)
            {
                var spawnCell = GridManager.Instance.WorldToCell(data.playerSpawnPoint.position);
                player.TeleportToCell(spawnCell);

                CameraFollow.Instance?.ResetFocus();
            }

            var enemiesInRoom = data.GetEnemiesInRoom();
            foreach (var enemy in enemiesInRoom)
            {
                activeEnemies.Add(enemy);
                enemy.GetComponent<Health>().OnDeath += HandleEnemyDeath;
            }

            IsLevelLoaded = true;
            TurnManager.Instance.BeginLevel();

            if (AbilityController.Instance != null)
                AbilityController.Instance.SelectAbilityByIndex(0);
        }

        private void HandleEnemyDeath(GameObject enemyObject)
        {
            var enemy = enemyObject.GetComponent<EnemyBase>();
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }

            if (activeEnemies.Count == 0 && !isGameOver)
            {
                WinGame();
            }
        }

        private void HandlePlayerDeath(GameObject playerObj)
        {
            if (!isGameOver)
            {
                LoseGame();
            }
        }

        private void WinGame()
        {
            isGameOver = true;
            Debug.Log("<color=green>ПОБЕДА</color>");
            OnRoomCleared();
        }

        private void OnRoomCleared()
        {
            Debug.Log("Цель уровня выполнена, переход к следующей комнате");

            StartCoroutine(ShowVictoryAndLoadNext());
        }

        private IEnumerator ShowVictoryAndLoadNext()
        {
            yield return new WaitForSeconds(1.5f);

            if (TransitionScreenManager.Instance != null)
            {
                yield return TransitionScreenManager.Instance.ShowVictoryScreen(
                    title: "Комната пройдена"
                );
            }

            currentRoomIndex++;

            if (currentRoomIndex < roomPrefabs.Count)
            {
                yield return TransitionScreenManager.Instance.FadeToBlack(() =>
                {
                    LoadRoom(roomPrefabs[currentRoomIndex]);
                });

                yield return TransitionScreenManager.Instance.FadeFromBlack();
            }
            else
            {
                Debug.Log("<color=cyan>Все комнаты пройдены</color>");
            }
        }

        private void LoseGame()
        {
            isGameOver = true;
            Debug.Log("<color=red>ПОРАЖЕНИЕ</color>");
            AbilityController.Instance.DisableAllOverlaysAfterDeath();
            TurnManager.Instance.StopAllCoroutines();

            if (CameraFollow.Instance != null)
            {
                CameraFollow.Instance.ResetToPlayer();
            }

            StartCoroutine(ShowDefeatAndRestart());
        }

        private IEnumerator ShowDefeatAndRestart()
        {
            yield return new WaitForSeconds(1.5f);

            if (TransitionScreenManager.Instance != null)
            {
                yield return TransitionScreenManager.Instance.ShowDefeatScreen(
                    title: "ПОРАЖЕНИЕ!"
                );
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}