using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour 
{
    private IEnumerator Start() 
    {
        yield return null;
        Debug.Log("Инициализация первого уровня");
        
        TurnManager.Instance.BeginLevel();
    }
}