using System.Collections;
using System.Collections.Generic;
using Sprites;
using UnityEngine;
using UnityEngine.SceneManagement; // Для перезагрузки сцены

public class LevelController : MonoBehaviour 
{
    private readonly List<EnemyBase> activeEnemies = new();
    private bool isGameOver;

    private IEnumerator Start() 
    {
        yield return null;
        Debug.Log("Инициализация первого уровня");

        var playerHealth = PlayerMovement.Instance.GetComponent<Health>();
        playerHealth.OnDeath += HandlePlayerDeath;

        var enemiesOnScene = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (var enemy in enemiesOnScene)
        {
            activeEnemies.Add(enemy);
            
            enemy.GetComponent<Health>().OnDeath += HandleEnemyDeath;
        }

        TurnManager.Instance.BeginLevel();
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
    }

    private void LoseGame()
    {
        isGameOver = true;
        Debug.Log("<color=red>ПОРАЖЕНИЕ</color>");
        AbilityController.Instance.DisableAllOverlaysAfterDeath();
        
        Invoke(nameof(RestartLevel), 2f);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}