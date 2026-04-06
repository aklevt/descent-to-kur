using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour 
{
    private IEnumerator Start() 
    {
        yield return null;
        Debug.Log("Инициализация первого уровня");

        var enemiesOnScene = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (var enemy in enemiesOnScene)
        {
            TurnManager.Instance.RegisterEnemy(enemy);
        }

        TurnManager.Instance.BeginLevel();
        Debug.Log($"<color=cyan>[LevelController]</color> Зарегистрировано врагов: {enemiesOnScene.Length}");
    }
}